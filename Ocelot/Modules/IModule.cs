using System;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;

namespace Ocelot.Modules;

public interface IModule : IDisposable
{
    bool enabled { get; }

    void Initialize();

    // Functions
    void Tick(IFramework framework);
    void Draw();
    bool DrawMainUi();
    void DrawConfigUi();

    // Events
    void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled);
    void OnTerritoryChanged(ushort id);
}
