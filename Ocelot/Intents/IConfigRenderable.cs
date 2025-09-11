using Ocelot.Windows;

namespace Ocelot.Intents;

[Intent]
public interface IConfigRenderable
{
    bool RenderConfigUi(RenderContext ctx);
}
