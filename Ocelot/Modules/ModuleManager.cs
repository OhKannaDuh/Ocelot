using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;

namespace Ocelot.Modules;

public class ModuleManager
{
    private readonly List<IModule> modules = new();

    private readonly Dictionary<IModule, int> configOrders = new();

    private readonly Dictionary<IModule, int> mainOrders = new();

    private List<IModule> tick
    {
        get => modules.Where(m => m.tick).ToList();
    }

    private List<IModule> render
    {
        get => modules.Where(m => m.render).ToList();
    }

    public void Add(Module<OcelotPlugin, IOcelotConfig> module)
    {
        modules.Add(module);
    }

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
            if (attr != null)
            {
                configOrders[moduleInstance] = attr.configOrder;
                mainOrders[moduleInstance] = attr.mainOrder;
            }
        }
    }

    public IEnumerable<IModule> GetModulesByMainOrder()
    {
        return render.OrderBy(m => mainOrders.GetValueOrDefault(m, int.MaxValue));
    }

    public IEnumerable<IModule> GetModulesByConfigOrder()
    {
        return modules.OrderBy(m => configOrders.GetValueOrDefault(m, int.MaxValue));
    }

    public void PreInitialize()
    {
        modules.ForEach(m => m.config?.SetOwner(m));
        tick.ForEach(m => m.PreInitialize());
    }

    public void Initialize()
    {
        tick.ForEach(m => m.Initialize());
    }

    public void PostInitialize()
    {
        tick.ForEach(m => m.PostInitialize());
    }

    public void Tick(IFramework framework)
    {
        tick.ForEach(m => m.Tick(framework));
    }

    public void Draw()
    {
        render.ForEach(m => m.Draw());
    }

    public void DrawMainUi()
    {
        var modules = GetModulesByMainOrder().ToList();
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
        var modules = GetModulesByConfigOrder().ToList();
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
    {
        tick.ForEach(m => m.OnChatMessage(type, timestamp, sender, message, isHandled));
    }

    public void OnTerritoryChanged(ushort id)
    {
        tick.ForEach(m => m.OnTerritoryChanged(id));
    }

    public T GetModule<T>() where T : class, IModule
    {
        var module = modules.OfType<T>().FirstOrDefault();
        if (module == null)
        {
            throw new UnableToLoadModuleException($"Module of type {typeof(T).Name} was not found.");
        }

        return module;
    }

    public bool TryGetModule<T>(out T? module) where T : class, IModule
    {
        try
        {
            module = GetModule<T>();
            return true;
        }
        catch (UnableToLoadModuleException ex)
        {
            Logger.Error(ex.Message);
            module = null;
            return false;
        }
    }

    public void Dispose()
    {
        modules.ForEach(m => m.Dispose());
    }
}
