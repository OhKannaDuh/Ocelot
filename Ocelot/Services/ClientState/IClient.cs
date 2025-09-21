using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.Sheets;

namespace Ocelot.Services.ClientState;

public interface IClient
{
    ClientLanguage ClientLanguage { get; }
    
    ushort CurrentTerritoryId { get; }
    
    TerritoryType? CurrentTerritory { get; }
    
    uint CurrentMapId { get; }
    
    Map? CurrentMap { get; }
    
    IPlayerCharacter? Player { get; }
    
    ulong LocalContentId { get; }
    
    bool IsLoggedIn { get; }
    
    bool IsPvP { get; }
    
    bool IsPvPExcludingDen { get; }
    
    bool IsGPosing { get; }

    bool IsClientIdle(out ConditionFlag blockingFlag);

    bool IsClientIdle();

    bool IsPlayerAvailable();
}
