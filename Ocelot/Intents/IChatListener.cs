using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace Ocelot.Intents;

[Intent]
public interface IChatListener
{
    void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled);
}
