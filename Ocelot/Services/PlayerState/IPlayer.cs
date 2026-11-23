using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace Ocelot.Services.PlayerState;

public interface IPlayer
{
    IPlayerState State { get; }

    IPlayerCharacter? PlayerCharacter { get; }

    StatusList? StatusList => PlayerCharacter?.StatusList;

    Vector3 Position { get; }

    ICondition Conditions { get; }

    ClassJob? GetClassJob();

    Vector3 GetPosition();

    bool IsMounted();

    bool IsCasting();

    bool IsBetweenAreas();

    float GetAttackRange();

    bool IsMelee();

    bool IsTank();

    bool IsMeleeDps();
}
