using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.DalamudServices.Legacy;
using Lumina.Excel.Sheets;
using Ocelot.Services.Data;

namespace Ocelot.Services.ClientState;

public class Client(
    IClientState clientState,
    IDataRepository<TerritoryType> territories,
    IDataRepository<Map> maps,
    IObjectTable objects,
    IPlayerState playerState
) : IClient
{
    public ClientLanguage ClientLanguage
    {
        get => clientState.ClientLanguage;
    }

    public ushort CurrentTerritoryId
    {
        get => (ushort)clientState.TerritoryType;
    }

    public TerritoryType? CurrentTerritory
    {
        get => CurrentTerritoryId == 0 ? null : territories.Get(CurrentTerritoryId);
    }

    public uint CurrentMapId
    {
        get => clientState.MapId;
    }

    public Map? CurrentMap
    {
        get => CurrentMapId == 0 ? null : maps.Get(CurrentMapId);
    }

    public IPlayerCharacter? Player
    {
        get => objects.LocalPlayer;
    }

    public ulong LocalContentId
    {
        get => playerState.ContentId;
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
