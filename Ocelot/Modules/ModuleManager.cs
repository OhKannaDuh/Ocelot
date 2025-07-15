using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Ocelot.Windows;

namespace Ocelot.Modules;

public class ModuleManager
{
    private readonly List<IModule> modules = new();

    private readonly Dictionary<IModule, int> configOrders = new();

    private readonly Dictionary<IModule, int> mainOrders = new();

    private List<IModule> ToUpdate
    {
        get => modules.Where(m => m.ShouldUpdate).ToList();
    }

    private List<IModule> ToRender
    {
        get => modules.Where(m => m.ShouldRender).ToList();
    }

    private List<IModule> ToInitialize
    {
        get => modules.Where(m => m.ShouldInitialize).ToList();
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
        return ToRender.OrderBy(m => mainOrders.GetValueOrDefault(m, int.MaxValue));
    }

    public IEnumerable<IModule> GetModulesByConfigOrder()
    {
        return modules.OrderBy(m => configOrders.GetValueOrDefault(m, int.MaxValue));
    }

    public void PreInitialize()
    {
        modules.ForEach(m => m.Config?.SetOwner(m));
        ToInitialize.ForEach(m => m.PreInitialize());
    }

    public void Initialize()
    {
        ToInitialize.ForEach(m => m.Initialize());
    }

    public void PostInitialize()
    {
        ToInitialize.ForEach(m => m.PostInitialize());
    }

    public void PreUpdate(UpdateContext context)
    {
        ToUpdate.ForEach(m => m.PreUpdate(context));
    }

    public void Update(UpdateContext context)
    {
        ToUpdate.ForEach(m => m.Update(context));
    }

    public void PostUpdate(UpdateContext context)
    {
        ToUpdate.ForEach(m => m.PostUpdate(context));
    }

    public void Render(RenderContext context)
    {
        ToRender.ForEach(m => m.Render(context));
    }

    public void RenderMainUi(RenderContext context)
    {
        var orderedModules = GetModulesByMainOrder().ToList();
        foreach (var module in orderedModules)
        {
            OcelotUI.Region($"OcelotMain##{module.GetType().FullName}", () =>
            {
                if (module.RenderMainUi(context))
                {
                    OcelotUI.VSpace();
                    if (module != orderedModules.Last())
                    {
                        OcelotUI.Separator();
                    }
                }
            });
        }
    }

    public void RenderConfigUi(RenderContext context)
    {
        var orderedModules = GetModulesByConfigOrder().ToList();
        foreach (var module in orderedModules)
        {
            module.RenderConfigUi(context);
            OcelotUI.VSpace();
            if (module != orderedModules.Last())
            {
                OcelotUI.Separator();
            }
        }
    }


    public void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled)
    {
        ToUpdate.ForEach(m => m.OnChatMessage(type, timestamp, sender, message, isHandled));
    }

    public void OnTerritoryChanged(ushort id)
    {
        ToUpdate.ForEach(m => m.OnTerritoryChanged(id));
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
