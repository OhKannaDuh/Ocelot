using System;
using ECommons.Automation;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Ocelot.Chain.ChainEx;

public static class ChainAddon
{
    private static unsafe TaskManagerTask AddonCallback(string addonName, bool updateState = true, params object[] callbackValues)
    {
        return new TaskManagerTask(() =>
        {
            if (EzThrottler.Throttle($"ChainAddon.AddonCallback({addonName}, {updateState}, {string.Join(", ", callbackValues)})"))
            {
                var addonPtr = Svc.GameGui.GetAddonByName(addonName);
                if (addonPtr != IntPtr.Zero)
                {
                    var addon = (AtkUnitBase*)addonPtr;
                    if (addon->IsReady)
                    {
                        Callback.Fire(addon, updateState, callbackValues);
                        return true;
                    }
                }
            }

            return false;
        }, new TaskManagerConfiguration { TimeLimitMS = 3000 });
    }

    public static Chain AddonCallback(this Chain chain, string addonName, bool updateState = true, params object[] callbackValues)
    {
        return chain
            .Debug($"Waiting for addon callback to fire {addonName} {updateState} {string.Join(", ", callbackValues)}")
            .Then(AddonCallback(addonName, updateState, callbackValues));
    }
}
