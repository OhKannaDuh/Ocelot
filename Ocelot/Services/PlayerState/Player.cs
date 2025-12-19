using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Ocelot.Services.ClientState;
using DalamudPlayerState = FFXIVClientStructs.FFXIV.Client.Game.UI.PlayerState;

namespace Ocelot.Services.PlayerState;

public class Player(
    IPlayerState state,
    IObjectTable objects,
    ICondition condition
) : IPlayer
{
    private const float MeleeRange = 3.5f;

    private const float RangedRange = 25f;

    private bool IsAvailable
    {
        get => PlayerCharacter != null;
    }

    public IPlayerState State { get; } = state;

    public Vector3 Position
    {
        get => PlayerCharacter?.Position ?? Vector3.Zero;
    }

    public ICondition Conditions { get; } = condition;


    public IPlayerCharacter? PlayerCharacter
    {
        get => objects.LocalPlayer;
    }

    public int GetLevel()
    {
        return PlayerCharacter?.Level ?? 0;
    }

    public ClassJob? GetClassJob()
    {
        var classJob = PlayerCharacter?.ClassJob;

        return classJob?.Value;
    }

    public unsafe int GetLevel(ClassJob job)
    {
        var state = DalamudPlayerState.Instance();
        return state == null ? 0 : state->ClassJobLevels[job.ExpArrayIndex];
    }

    public Vector3 GetPosition()
    {
        return IsAvailable ? PlayerCharacter!.Position : Vector3.Zero;
    }

    public bool IsMounting()
    {
        return Conditions[ConditionFlag.Mounting] || Conditions[ConditionFlag.Mounting71];
    }

    public bool IsMounted()
    {
        return Conditions[ConditionFlag.Mounted];
    }

    public bool IsCasting()
    {
        return Conditions[ConditionFlag.Casting] || Conditions[ConditionFlag.Casting87];
    }

    public bool IsBetweenAreas()
    {
        return Conditions[ConditionFlag.BetweenAreas] || Conditions[ConditionFlag.BetweenAreas51];
    }

    public bool IsInteracting()
    {
        return Conditions.Any(ConditionFlag.OccupiedInEvent, ConditionFlag.OccupiedInQuestEvent, ConditionFlag.OccupiedInCutSceneEvent);
    }

    public float GetAttackRange()
    {
        return IsMelee() ? MeleeRange : RangedRange;
    }


    public bool IsMelee()
    {
        // 0 = crafter/gatherer, 1 = tank, 2 = melee
        return GetClassJob()?.Role <= 2;
    }

    public bool IsTank()
    {
        return GetClassJob()?.Role == 1;
    }

    public bool IsMeleeDps()
    {
        return GetClassJob()?.Role == 2;
    }

    public bool IsHealer()
    {
        return GetClassJob()?.Role == 4;
    }

    public bool IsCaster()
    {
        return GetClassJob()?.RowId is 7 or 25 or 26 or 27 or 35 or 42;
    }

    public bool CanFly()
    {
        if (!IsAvailable)
        {
            return false;
        }

        unsafe
        {
            var state = DalamudPlayerState.Instance();
            return state != null && state->CanFly;
        }
    }
}
