using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using ImGuiNET;

namespace Ocelot.Modules;

public class ModuleManager
{
    private readonly List<IModule> modules = new();

    private readonly Dictionary<IModule, int> configOrders = new();

    private readonly Dictionary<IModule, int> mainOrders = new();

    private List<IModule> enabled => modules.Where(m => m.enabled).ToList();

    public void Add(Module<OcelotPlugin, IOcelotConfig> module) => modules.Add(module);

    public void AutoRegister(OcelotPlugin plugin, IOcelotConfig config)
    {
        var moduleTypes = Registry
            .GetTypesWithAttributeData<OcelotModuleAttribute>()
            .Where(t => typeof(IModule).IsAssignableFrom(t.type));

        foreach (var (type, attr) in moduleTypes)
        {
            Logger.Info($"Registering module: {type.FullName}");
            var moduleInstance = (IModule)Activator.CreateInstance(type, plugin, config)!;
            modules.Add(moduleInstance);
            configOrders[moduleInstance] = attr.configOrder;
            mainOrders[moduleInstance] = attr.mainOrder;
        }
    }

    private IEnumerable<IModule> GetModulesByMainOrder() =>
        enabled.OrderBy(m => mainOrders.TryGetValue(m, out var order) ? order : int.MaxValue);

    private IEnumerable<IModule> GetModulesByConfigOrder() =>
        modules.OrderBy(m => configOrders.TryGetValue(m, out var order) ? order : int.MaxValue);

    public void Initialize(OcelotPlugin plugin, IOcelotConfig config) => enabled.ForEach(m => m.Initialize());

    public void Tick(IFramework framework) => enabled.ForEach(m => m.Tick(framework));

    public void Draw() => enabled.ForEach(m => m.Draw());

    public void DrawMainUi()
    {
        var modules = GetModulesByMainOrder();
        foreach (var module in modules)
        {
            OcelotUI.Region($"OcelotMain##{module.GetType().FullName}", () =>
            {
                if (module.DrawMainUi())
                {
                    OcelotUI.VSpace();
                    if (module != modules.Last())
                    {
                        OcelotUI.Separator();
                    }
                }
            });
        }
    }
    public void DrawConfigUi()
    {
        var modules = GetModulesByConfigOrder();
        foreach (var module in modules)
        {
            module.DrawConfigUi();
            OcelotUI.VSpace();
            if (module != modules.Last())
            {
                OcelotUI.Separator();
            }
        }
    }


    public void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled)
        => enabled.ForEach(m => m.OnChatMessage(type, timestamp, sender, message, isHandled));

    public void OnTerritoryChanged(ushort id) => enabled.ForEach(m => m.OnTerritoryChanged(id));

    public T? GetModule<T>() where T : class, IModule => modules.OfType<T>().FirstOrDefault();

    public bool TryGetModule<T>(out T? module) where T : class, IModule
    {
        module = GetModule<T>();
        return module != null;
    }

    public void Dispose() => modules.ForEach(m => m.Dispose());
}
