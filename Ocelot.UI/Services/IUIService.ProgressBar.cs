using System.Numerics;

namespace Ocelot.UI.Services;

public partial interface IUIService
{
    void ProgressBar(float progress, Vector2 size, string? text = null);

    void ProgressBar(float progress, string? text = null);
}
