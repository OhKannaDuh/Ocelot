using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameHelpers;
using Ocelot.Commands;
using Ocelot.IPC;
using Ocelot.Modules;
using Pictomancy;

namespace Ocelot.Windows;

public class RenderContext(OcelotPlugin plugin, IModule? module = null)
{
    public readonly OcelotPlugin Plugin = plugin;

    public IModule? Module { get; private set; } = module;

    public readonly PctDrawList Pictomancy = PictoService.GetDrawList();

    public OcelotConfig Config
    {
        get => Plugin.OcelotConfig;
    }

    public ModuleManager Modules
    {
        get => Plugin.Modules;
    }

    public WindowManager Windows
    {
        get => Plugin.Windows;
    }

    public CommandManager Commands
    {
        get => Plugin.Commands;
    }

    public IPCManager IPC
    {
        get => Plugin.IPC;
    }

    public RenderContext ForModule(IModule module)
    {
        return new RenderContext(Plugin, module);
    }

    public bool IsForModule<T>(out T module) where T : class, IModule
    {
        if (Module is T typed)
        {
            module = typed;
            return true;
        }

        module = null!;
        return false;
    }

    public void DrawLine(Vector3 start, Vector3 end, Vector4 color, float thickness = 3f)
    {
        Pictomancy.AddLine(start, end, 1f / 1000f, ImGui.GetColorU32(color), thickness);
    }

    public void DrawLine(Vector3 end, Vector4 color, float thickness = 3f)
    {
        DrawLine(Player.Position, end, color, thickness);
    }

    public enum DrawMode
    {
        Outline,

        Filled,
    }

    public void DrawCircle(Vector3 position, float radius, Vector4 color, DrawMode mode = DrawMode.Outline)
    {
        if (mode == DrawMode.Outline)
        {
            Pictomancy.AddCircle(position, radius, ImGui.GetColorU32(color));
        }

        if (mode == DrawMode.Filled)
        {
            Pictomancy.AddCircleFilled(position, radius, ImGui.GetColorU32(color));
        }
    }

    public void DrawCircle(IGameObject obj, Vector4 color, DrawMode mode = DrawMode.Outline)
    {
        if (mode == DrawMode.Outline)
        {
            Pictomancy.AddCircle(obj.Position, obj.HitboxRadius, ImGui.GetColorU32(color));
        }

        if (mode == DrawMode.Filled)
        {
            Pictomancy.AddCircleFilled(obj.Position, obj.HitboxRadius, ImGui.GetColorU32(color));
        }
    }

    public void DrawSquare(Vector3 position, float halfExtent, Vector4 color, float rotation = 0f, DrawMode mode = DrawMode.Outline)
    {
        // Define corners relative to center (no rotation)
        var corners = new Vector3[]
        {
            new(-halfExtent, 0, -halfExtent),
            new(halfExtent, 0, -halfExtent),
            new(halfExtent, 0, halfExtent),
            new(-halfExtent, 0, halfExtent),
        };

        // Precompute sin/cos for rotation
        var cos = MathF.Cos(rotation);
        var sin = MathF.Sin(rotation);

        // Rotate corners around Y-axis (up)
        for (var i = 0; i < corners.Length; i++)
        {
            var x = corners[i].X;
            var z = corners[i].Z;

            var rotatedX = x * cos - z * sin;
            var rotatedZ = x * sin + z * cos;

            corners[i] = new Vector3(position.X + rotatedX, position.Y, position.Z + rotatedZ);
        }

        if (mode == DrawMode.Outline)
        {
            Pictomancy.AddQuad(corners[0], corners[1], corners[2], corners[3], ImGui.GetColorU32(color));
        }
        else if (mode == DrawMode.Filled)
        {
            Pictomancy.AddQuadFilled(corners[0], corners[1], corners[2], corners[3], ImGui.GetColorU32(color));
        }
    }

    public void DrawApproachCone(Vector3 start, Vector3 end, float radius, float angleDegrees, Vector4 color, DrawMode mode = DrawMode.Outline)
    {
        var direction = start - end;
        if (direction == Vector3.Zero)
        {
            return;
        }

        direction = Vector3.Normalize(direction);

        var angleRadians = MathF.PI * angleDegrees / 180f;
        var worldAngle = MathF.Atan2(direction.Z, direction.X);
        var rotation = worldAngle - MathF.PI / 2f;

        var halfAngle = angleRadians / 2f;
        var minAngle = rotation - halfAngle;
        var maxAngle = rotation + halfAngle;

        if (mode == DrawMode.Outline)
        {
            Pictomancy.AddFan(end, 0f, radius, minAngle, maxAngle, ImGui.GetColorU32(color), 0);
        }

        if (mode == DrawMode.Filled)
        {
            Pictomancy.AddConeFilled(end, radius, rotation, angleRadians, ImGui.GetColorU32(color));
        }
    }

    public void DrawApproachConeFromPlayer(Vector3 end, float radius, float angleDegrees, Vector4 color, DrawMode mode = DrawMode.Outline)
    {
        DrawApproachCone(Player.Position, end, radius, angleDegrees, color, mode);
    }
}
