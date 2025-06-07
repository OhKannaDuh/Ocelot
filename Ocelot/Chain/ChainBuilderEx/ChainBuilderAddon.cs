using System;
using ECommons.Automation;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Ocelot.Chain.ChainBuilderEx;

public static class ChainBuilderAddon
{
    public static ChainBuilder WaitForAddonReady(this ChainBuilder builder, string addonName)
        => builder
            .Debug($"Waiting for addon to be ready {addonName}")
            .WaitUntil(() =>
            {
                unsafe
                {
                    var addonPtr = Svc.GameGui.GetAddonByName(addonName, 1);
                    if (addonPtr != IntPtr.Zero)
                    {
                        var addon = (AtkUnitBase*)addonPtr;
                        if (addon->IsReady)
                        {
                            return true;
                        }

                    }
                }

                return false;
            });

    public static ChainBuilder AddonCallback(this ChainBuilder builder, string addonName, bool updateState = true, params object[] callbackValues)
        => builder
            .Debug($"Firing callback on addon {addonName} updateState: {updateState} values: {string.Join(", ", callbackValues)}")
            .WaitUntil(() =>
            {
                unsafe
                {
                    var addonPtr = Svc.GameGui.GetAddonByName(addonName, 1);
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
            });
}
