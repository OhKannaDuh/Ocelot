using System.Numerics;
using Ocelot.Graphics;

namespace Ocelot.Services.OverlayRenderer;

public interface IOverlayRenderer
{
    void StrokeLine(Vector3 start, Vector3 end, Color color, float thickness = 3f);

    void StrokeCircle(Vector3 center, float radius, Color color, uint segments = 0, float thickness = 3f);

    void FillCircle(Vector3 center, float radius, Color color, uint segments = 0);
}
