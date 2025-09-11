using Ocelot.Windows;

namespace Ocelot.Intents;

[Intent]
public interface IMainRenderable
{
    bool RenderMainUi(RenderContext ctx);
}
