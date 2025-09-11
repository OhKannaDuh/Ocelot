using Ocelot.Windows;

namespace Ocelot.Intents;

[Intent]
public interface IRenderable
{
    void Render(RenderContext context);
}
