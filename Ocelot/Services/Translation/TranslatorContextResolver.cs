using System.Reflection;
using Dalamud.Interface.Windowing;
using Ocelot.Extensions;
using Ocelot.Services.Commands;
using Ocelot.States.Flow;
using Ocelot.States.Score;

namespace Ocelot.Services.Translation;

public class TranslatorContextResolver(TranslatorContextResolverOptions options) : ITranslatorContextResolver
{
    public string ResolveScope(Type context)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
        var prop = context.GetProperty("TranslationScope", flags);
        if (prop != null && prop.PropertyType == typeof(string) && prop.GetValue(null) is string value)
        {
            return value;
        }

        var field = context.GetField("TranslationScope", flags);
        if (field is not null && field.FieldType == typeof(string) && field.GetValue(null) is string fieldValue)
        {
            return fieldValue;
        }

        var full = context.FullName ?? context.Name;
        foreach (var (pattern, replacer) in options.Replacements)
        {
            if (pattern.IsMatch(full))
            {
                return replacer(context);
            }
        }

        if (context.IsSubclassOf(typeof(Window)))
        {
            var name = (context.Name.EndsWith("Window") ? context.Name.Replace("Window", string.Empty) : context.Name).ToSnakeCase();

            return $"windows.{name}";
        }

        if (typeof(IOcelotCommand).IsAssignableFrom(context))
        {
            var name = (context.Name.EndsWith("Command") ? context.Name.Replace("Command", string.Empty) : context.Name).ToSnakeCase();

            return $"commands.{name}";
        }

        if (context.IsGenericType && context.GetGenericTypeDefinition() == typeof(FlowStateMachine<>))
        {
            var stateType = context.GetGenericArguments()[0];
            var stateName = stateType.Name.Replace("State", "").ToSnakeCase();

            return $"state_machines.{stateName}";
        }

        if (context.IsGenericType && context.GetGenericTypeDefinition() == typeof(ScoreStateMachine<,>))
        {
            var stateType = context.GetGenericArguments()[0];
            var stateName = stateType.Name.Replace("State", "").ToSnakeCase();

            return $"state_machines.{stateName}";
        }

        return Fallback(context);
    }

    private string Fallback(Type context)
    {
        var full = context.FullName ?? context.Name;

        if (full.StartsWith(options.Namespace))
        {
            full = full.Substring(options.Namespace.Length + 1);
        }

        return full.ToSnakeCase();
    }
}
