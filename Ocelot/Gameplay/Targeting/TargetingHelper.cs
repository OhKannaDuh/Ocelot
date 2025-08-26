using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;

namespace Ocelot.Gameplay.Targeting;

public static class TargetingHelper
{
    private static bool Initialized = false;

    public static float MeleeDistance { get; private set; } = 3.5f;

    public static float RangedDistance { get; private set; } = 25f;

    public static IEnumerable<IBattleNpc> Npcs { get; private set; } = [];

    public static IEnumerable<IBattleNpc> Enemies
    {
        get => Npcs.Where(o => o.IsHostile());
    }

    public static IEnumerable<IBattleNpc> Players
    {
        get => Npcs.Where(o => o.ObjectKind == ObjectKind.Player);
    }

    internal static void Initialize(OcelotPlugin plugin)
    {
        if (Initialized)
        {
            return;
        }

        Logger.Info("[TargetingHelper] Initializing");
        Initialized = true;

        Svc.Framework.Update += Update;
    }

    internal static void TearDown()
    {
        if (!Initialized)
        {
            return;
        }

        Svc.Framework.Update -= Update;
    }

    private static void Update(IFramework _)
    {
        Npcs = Svc.Objects.OfType<IBattleNpc>()
            .Where(o => o is
            {
                IsDead: false,
                IsTargetable: true,
            })
            .OrderBy(Player.DistanceTo);
    }

    public static void Clear()
    {
        Npcs = [];
    }
}
