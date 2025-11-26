using System.Numerics;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace Ocelot.Services.PlayerState;

public interface IPlayer
{
    IPlayerState State { get; }

    Vector3 Position { get; }

    ICondition Conditions { get; }

    int GetLevel();

    ClassJob? GetClassJob();

    int GetLevel(ClassJob job);

    Vector3 GetPosition();

    bool IsMounting();

    bool IsMounted();

    bool IsCasting();

    bool IsBetweenAreas();

    bool IsInteracting();

    float GetAttackRange();

    bool IsMelee();

    bool IsTank();

    bool IsMeleeDps();

    bool IsHealer();

    bool IsCaster();

    bool CanFly();
}
