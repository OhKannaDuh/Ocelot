using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;

namespace Ocelot.Prowler;

public class GameObjectTargetProvider(Func<IGameObject?> getObject, float threshold) : ITargetProvider
{
    private Vector3 lastPosition = getObject()?.Position ?? Vector3.NaN;

    public Vector3 GetCurrentPosition(ProwlContext context)
    {
        var obj = getObject();
        if (obj == null)
        {
            return lastPosition;
        }

        lastPosition = obj.Position;
        return obj.Position;
    }
}
