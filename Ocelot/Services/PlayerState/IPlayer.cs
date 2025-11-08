using System.Numerics;
using Lumina.Excel.Sheets;

namespace Ocelot.Services.PlayerState;

public interface IPlayer
{
    int GetLevel();

    ClassJob? GetClassJob();

    int GetLevel(ClassJob job);

    Vector3 GetPosition();

    bool IsMounting();

    bool IsMounted();

    bool IsCasting();

    bool IsBetweenAreas();

    bool IsInteracting();

    float GetRange();

    bool IsMelee();

    bool IsTank();

    bool IsMeleeDps();

    bool IsHealer();

    bool IsCaster();
}
