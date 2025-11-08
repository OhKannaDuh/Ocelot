using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Dalamud.Configuration;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Ocelot.Config;
using Ocelot.Config.Fields;
using Ocelot.Extensions;
using Ocelot.Services.Translation;
using Ocelot.Windows;

namespace Ocelot.Services.Commands;

public class ConfigCommand(
    IDalamudPluginInterface plugin,
    IConfigWindow window,
    IChatGui chat,
    IPluginLog logger,
    ITranslator translator
) : OcelotCommand(translator), IConfigCommand
{
    public override string Command { get; } = plugin.InternalName.ToKebabCase().WithSuffix("-config");

    public override List<string> Aliases { get; } =
    [
        plugin.InternalName.ToKebabCase().WithSuffix("-cfg"),
        plugin.InternalName.ToKebabCase().WithSuffix("-c"),
    ];

    public override string HelpTranslationKey { get; } = $"commands.{plugin.InternalName.ToKebabCase().WithSuffix("-config")}.help";

    private readonly Dictionary<string, Accessor> index = new(StringComparer.OrdinalIgnoreCase);

    private readonly HashSet<object> roots = new(ReferenceEqualityComparer.Instance);

    public void Expose<T>(T root) where T : IPluginConfiguration
    {
        if (!roots.Add(root))
        {
            return;
        }

        BuildIndex(root, root, "", new Stack<PropertyInfo>(),
            new HashSet<object>(ReferenceEqualityComparer.Instance));

        logger.Info($"Config: added root '{root.GetType().Name}' with {index.Count} total keys.");
    }

    public override void Execute(CommandContext ctx)
    {
        if (ctx.Args.Length == 0)
        {
            window.Toggle();
            return;
        }

        switch (ctx.Args[0].ToLowerInvariant())
        {
            case "list":
                DoList();
                break;

            case "get":
                if (ctx.Args.Length < 2)
                {
                    chat.PrintError("Usage: get <key>");
                    return;
                }

                DoGet(ctx.Args[1]);
                break;

            case "set":
                if (ctx.Args.Length < 3)
                {
                    chat.PrintError("Usage: set <key> <value>");
                    return;
                }

                DoSet(ctx.Args[1], string.Join(" ", ctx.Args.Skip(2)));
                break;

            default:
                chat.PrintError("Unknown subcommand. Try: list, get <key>, set <key> <value>");
                break;
        }
    }

    private void BuildIndex(object obj, object root, string prefix, Stack<PropertyInfo> path, HashSet<object> visited)
    {
        if (!visited.Add(obj))
        {
            return;
        }

        var t = obj.GetType();

        foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!p.CanRead)
            {
                continue;
            }

            if (p.GetIndexParameters().Length > 0)
            {
                continue;
            }

            if (p.GetCustomAttribute<ConfigHiddenAttribute>() != null)
            {
                continue;
            }

            if (p.GetCustomAttribute<BrowsableAttribute>()?.Browsable == false)
            {
                continue;
            }

            if (p.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            {
                continue;
            }

            var leafType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

            if (leafType.GetCustomAttribute<ConfigHiddenAttribute>() != null)
            {
                continue;
            }

            if (leafType.GetCustomAttribute<BrowsableAttribute>()?.Browsable == false)
            {
                continue;
            }


            if (typeof(IEnumerable).IsAssignableFrom(leafType) && leafType != typeof(string))
            {
                continue;
            }


            var segment = p.Name.EndsWith("Config") ? p.Name.Replace("Config", "").ToSnakeCase() : p.Name.ToSnakeCase();
            var fullPrefix = string.IsNullOrEmpty(prefix) ? segment : $"{prefix}.{segment}";

            logger.Info(fullPrefix);

            if (IsLeaf(leafType))
            {
                var chain = path.Reverse().Append(p).ToArray();
                index[fullPrefix] = new Accessor(root, chain);
            }
            else
            {
                var child = p.GetValue(obj);
                if (child != null)
                {
                    path.Push(p);
                    BuildIndex(child, root, fullPrefix, path, visited);
                    path.Pop();
                }
            }
        }
    }

    private static bool IsLeaf(Type t)
    {
        if (t.IsEnum)
        {
            return true;
        }

        if (t == typeof(string))
        {
            return true;
        }

        var supported = new[]
        {
            typeof(bool),
            typeof(byte), typeof(sbyte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal),
        };

        return supported.Contains(t);
    }

    private void DoList()
    {
        if (index.Count == 0)
        {
            chat.Print("No configuration is exposed.");
            return;
        }

        chat.Print($"Config keys ({index.Count}):");
        foreach (var k in index.Keys.OrderBy(s => s))
        {
            chat.Print($" - {k}");
        }
    }

    private void DoGet(string rawKey)
    {
        if (!TryResolveKey(rawKey, out var acc))
        {
            chat.PrintError($"Unknown key: {rawKey}");
            SuggestClosest(rawKey);
            return;
        }

        var v = acc.GetValue();
        chat.Print($"{acc.DisplayPath} = {v}");
    }

    private void DoSet(string rawKey, string rawValue)
    {
        if (!TryResolveKey(rawKey, out var acc))
        {
            chat.PrintError($"Unknown key: {rawKey}");
            SuggestClosest(rawKey);
            return;
        }

        try
        {
            var leafProp = acc.Leaf;
            var converted = ConvertForProperty(leafProp, rawValue);
            var error = ValidateAgainstAttributes(leafProp, converted);
            if (error is not null)
            {
                chat.PrintError(error);
                return;
            }

            acc.SetValue(converted);
            chat.Print($"Set {acc.DisplayPath} = {acc.GetValue()}");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to set config");
            chat.PrintError($"Failed to set {rawKey}: {ex.Message}");
        }
    }

    private bool TryResolveKey(string input, out Accessor acc)
    {
        if (index.TryGetValue(input, out acc!))
        {
            return true;
        }

        var candidates = index.Where(kvp => kvp.Key.EndsWith(input, StringComparison.OrdinalIgnoreCase)).ToList();
        if (candidates.Count == 1)
        {
            acc = candidates[0].Value;
            return true;
        }

        acc = default!;
        return false;
    }

    private void SuggestClosest(string input)
    {
        var nearest = index.Keys
            .Select(k => (k, d: Levenshtein(k, input)))
            .OrderBy(x => x.d)
            .Take(3)
            .Select(x => x.k)
            .ToArray();

        if (nearest.Length > 0)
        {
            chat.Print($"Did you mean: {string.Join(", ", nearest)} ?");
        }
    }

    private static object ConvertForProperty(PropertyInfo p, string value)
    {
        var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

        if (t == typeof(string))
        {
            return value;
        }

        if (t == typeof(bool))
        {
            if (TryParseBool(value, out var b))
            {
                return b;
            }

            throw new ArgumentException("Expected boolean (true/false/yes/no/on/off/1/0).");
        }

        if (t.IsEnum)
        {
            if (Enum.TryParse(t, value, true, out var e))
            {
                return e;
            }

            var names = string.Join(", ", Enum.GetNames(t));
            throw new ArgumentException($"Expected one of: {names}");
        }

        if (t == typeof(int) && int.TryParse(value, out var i))
        {
            return i;
        }

        if (t == typeof(long) && long.TryParse(value, out var l))
        {
            return l;
        }

        if (t == typeof(float) && float.TryParse(value, out var f))
        {
            return f;
        }

        if (t == typeof(double) && double.TryParse(value, out var d))
        {
            return d;
        }

        if (t == typeof(decimal) && decimal.TryParse(value, out var m))
        {
            return m;
        }

        return Convert.ChangeType(value, t);
    }

    private static string? ValidateAgainstAttributes(PropertyInfo p, object value)
    {
        if (p.GetCustomAttribute<CheckboxAttribute>() is not null)
        {
            if ((Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType) != typeof(bool))
            {
                return $"[{nameof(CheckboxAttribute)}] requires a bool.";
            }
        }

        var intRange = p.GetCustomAttribute<IntRangeAttribute>();
        if (intRange is not null && value is int i)
        {
            var min = TryGetInt(intRange, "Min");
            var max = TryGetInt(intRange, "Max");
            if (min.HasValue && i < min.Value)
            {
                return $"{p.Name.ToSnakeCase()} must be ≥ {min.Value}.";
            }

            if (max.HasValue && i > max.Value)
            {
                return $"{p.Name.ToSnakeCase()} must be ≤ {max.Value}.";
            }
        }

        return null;

        static int? TryGetInt(object obj, string name)
        {
            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi?.PropertyType == typeof(int))
            {
                return (int?)pi.GetValue(obj)!;
            }

            var fi = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.Instance);
            if (fi?.FieldType == typeof(int))
            {
                return (int?)fi.GetValue(obj)!;
            }

            return null;
        }
    }

    private readonly struct Accessor(object root, PropertyInfo[] chain)
    {
        public PropertyInfo Leaf
        {
            get => chain[^1];
        }

        public string DisplayPath
        {
            get => string.Join(".", chain.Select(s => s.Name.ToSnakeCase()));
        }

        public object? GetValue()
        {
            var cur = root;
            for (var i = 0; i < chain.Length - 1; i++)
            {
                cur = chain[i].GetValue(cur!);
            }

            return chain[^1].GetValue(cur!);
        }

        public void SetValue(object value)
        {
            var cur = root;
            for (var i = 0; i < chain.Length - 1; i++)
            {
                cur = chain[i].GetValue(cur!);
            }

            chain[^1].SetValue(cur!, value);
        }
    }

    private static bool TryParseBool(string s, out bool b)
    {
        switch (s.Trim().ToLowerInvariant())
        {
            case "true":
            case "t":
            case "yes":
            case "y":
            case "on":
            case "1":
                b = true;
                return true;
            case "false":
            case "f":
            case "no":
            case "n":
            case "off":
            case "0":
                b = false;
                return true;
            default:
                b = false;
                return false;
        }
    }

    private static int Levenshtein(string a, string b)
    {
        var dp = new int[a.Length + 1, b.Length + 1];
        for (var i = 0; i <= a.Length; i++)
        {
            dp[i, 0] = i;
        }

        for (var j = 0; j <= b.Length; j++)
        {
            dp[0, j] = j;
        }

        for (var i = 1; i <= a.Length; i++)
        for (var j = 1; j <= b.Length; j++)
        {
            var cost = a[i - 1] == b[j - 1] ? 0 : 1;
            dp[i, j] = Math.Min(Math.Min(
                    dp[i - 1, j] + 1,
                    dp[i, j - 1] + 1),
                dp[i - 1, j - 1] + cost
            );
        }

        return dp[a.Length, b.Length];
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public readonly static ReferenceEqualityComparer Instance = new();

        bool IEqualityComparer<object>.Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
