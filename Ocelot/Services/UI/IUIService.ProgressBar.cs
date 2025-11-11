using System.Numerics;

namespace Ocelot.Services.UI;

public partial interface IUIService
{
    void ProgressBar(float progress, Vector2 size, string? text = null);

    void ProgressBar(float progress, string? text = null);
}
