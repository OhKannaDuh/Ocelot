using System.Numerics;
using Ocelot.Graphics;
using Ocelot.Services.OverlayRenderer;

namespace Ocelot.Pictomancy.Services.OverlayRenderer;

public class OverlayRendererService(IPictomancyProvider pictomancy) : IOverlayRendererService
{
    public void StrokeLine(Vector3 start, Vector3 end, Color color, float thickness = 3)
    {
        if (!pictomancy.HasDrawList)
        {
            return;
        }

        pictomancy.GetDrawList().AddLine(start, end, 1f / 1000f, color.ToRgba(), thickness);
    }

    public void StrokeCircle(Vector3 center, float radius, Color color, uint segments = 0, float thickness = 3)
    {
        if (!pictomancy.HasDrawList)
        {
            return;
        }

        pictomancy.GetDrawList().AddCircle(center, radius, color.ToRgba(), segments, thickness);
    }

    public void FillCircle(Vector3 center, float radius, Color color, uint segments = 0)
    {
        if (!pictomancy.HasDrawList)
        {
            return;
        }

        pictomancy.GetDrawList().AddCircleFilled(center, radius, color.ToRgba(), segments);
    }
}
