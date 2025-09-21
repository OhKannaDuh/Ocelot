using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.Sheets;
using Ocelot.Services.ClientState;

using DalamudPlayerState = FFXIVClientStructs.FFXIV.Client.Game.UI.PlayerState;

namespace Ocelot.Services.PlayerState;

public class PlayerState(IClientState client) : IPlayerState
{
    private IPlayerCharacter? Player
    {
        get => client.Player;
    }
    
    private bool IsAvailable
    {
        get => client.IsPlayerAvailable();
    }
    
    public int GetLevel()
    {
        return client.Player?.Level ?? 0;
    }

    public ClassJob? GetClassJob()
    {
        var classJob = client.Player?.ClassJob;
        
        return classJob?.Value;
    }

    public unsafe int GetLevel(ClassJob job)
    {
        var state = DalamudPlayerState.Instance();
        return state == null ? 0 : state->ClassJobLevels[job.ExpArrayIndex];
    }

    public Vector3 GetPosition()
    {
        return IsAvailable ? Player!.Position : Vector3.Zero;
    }
}
