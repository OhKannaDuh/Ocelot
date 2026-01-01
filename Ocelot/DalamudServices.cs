using System.Diagnostics.CodeAnalysis;
using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace Ocelot;
#nullable disable

public sealed class DalamudServices
{
    [PluginService] public IAddonEventManager AddonEventManager { get; private set; }

    [PluginService] public IAddonLifecycle AddonLifecycle { get; private set; }

    [PluginService] public IAetheryteList AetheryteList { get; private set; }

    [PluginService] public IBuddyList BuddyList { get; private set; }

    [PluginService] public IChatGui ChatGui { get; private set; }

    [PluginService] public IClientState ClientState { get; private set; }

    [PluginService] public ICommandManager CommandManager { get; private set; }

    [PluginService] public ICondition Condition { get; private set; }

    [PluginService] public IContextMenu ContextMenu { get; private set; }

    [PluginService] public IDataManager DataManager { get; private set; }

    [PluginService] public IDtrBar DtrBar { get; private set; }

    [PluginService] public IDutyState DutyState { get; private set; }

    [PluginService] public IFateTable FateTable { get; private set; }

    [PluginService] public IFlyTextGui FlyTextGui { get; private set; }

    [PluginService] public IFramework Framework { get; private set; }

    [PluginService] public IGameConfig GameConfig { get; private set; }

    [PluginService] public IGameGui GameGui { get; private set; }

    [PluginService] public IGameInteropProvider GameInteropProvider { get; private set; }

    [PluginService] public IGameInventory GameInventory { get; private set; }

    [PluginService] public IGameLifecycle GameLifecycle { get; private set; }

    [PluginService] public IGamepadState GamepadState { get; private set; }

    [PluginService] public IJobGauges JobGauges { get; private set; }

    [PluginService] public IKeyState KeyState { get; private set; }

    [PluginService] public IMarketBoard MarketBoard { get; private set; }

    [PluginService] public INamePlateGui NamePlateGui { get; private set; }

    [PluginService] public INotificationManager NotificationManager { get; private set; }

    [PluginService] public IObjectTable ObjectTable { get; private set; }

    [PluginService] public IPartyFinderGui PartyFinderGui { get; private set; }

    [PluginService] public IPartyList PartyList { get; private set; }

    [PluginService] public IPlayerState PlayerState { get; private set; }

    [PluginService] public IPluginLog PluginLog { get; private set; }

    [PluginService]
    [Experimental("Dalamud001")]
    public IReliableFileStorage ReliableFileStorage { get; private set; }

    [PluginService] public ISeStringEvaluator SeStringEvaluator { get; private set; }

    [PluginService] public ISigScanner SigScanner { get; private set; }

    [PluginService] public ITargetManager TargetManager { get; private set; }

    [PluginService] public ITextureProvider TextureProvider { get; private set; }

    [PluginService] public ITextureReadbackProvider TextureReadbackProvider { get; private set; }

    [PluginService] public ITextureSubstitutionProvider TextureSubstitutionProvider { get; private set; }

    [PluginService] public ITitleScreenMenu TitleScreenMenu { get; private set; }

    [PluginService] public IToastGui ToastGui { get; private set; }

    [PluginService]
    [Experimental("Dalamud001")]
    public IUnlockState UnlockState { get; private set; }
}
