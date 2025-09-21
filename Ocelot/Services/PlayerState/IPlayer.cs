using System.Numerics;
using Lumina.Excel.Sheets;

namespace Ocelot.Services.PlayerState;

public interface IPlayer
{
    int GetLevel();

    ClassJob? GetClassJob();
    
    int GetLevel(ClassJob job);

    Vector3 GetPosition();
}
