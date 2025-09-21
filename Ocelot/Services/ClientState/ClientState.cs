using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.Sheets;
using Ocelot.Services.Data;
using IDalamudState = Dalamud.Plugin.Services.IClientState;

namespace Ocelot.Services.ClientState;

public class ClientState(
    IDalamudState clientState,
    IDataRepository<TerritoryType> territories,
    IDataRepository<Map> maps
) : IClientState
{
    public ClientLanguage ClientLanguage
    {
        get => clientState.ClientLanguage;
    }

    public ushort CurrentTerritoryId
    {
        get => clientState.TerritoryType;
    }

    public TerritoryType CurrentTerritory
    {
        get => territories.Get(clientState.TerritoryType);
    }

    public uint CurrentMapId
    {
        get => clientState.MapId;
    }

    public Map CurrentMap
    {
        get => maps.Get(clientState.MapId);
    }

    public IPlayerCharacter? Player
    {
        get => clientState.LocalPlayer;
    }

    public ulong LocalContentId
    {
        get => clientState.LocalContentId;
    }

    public bool IsLoggedIn
    {
        get => clientState.IsLoggedIn;
    }

    public bool IsPvP
    {
        get => clientState.IsPvP;
    }

    public bool IsPvPExcludingDen
    {
        get => clientState.IsPvPExcludingDen;
    }

    public bool IsGPosing
    {
        get => clientState.IsGPosing;
    }

    public bool IsClientIdle(out ConditionFlag blockingFlag)
    {
        return clientState.IsClientIdle(out blockingFlag);
    }

    public bool IsClientIdle()
    {
        return clientState.IsClientIdle();
    }

    public bool IsPlayerAvailable()
    {
        return Player != null;
    }
}
