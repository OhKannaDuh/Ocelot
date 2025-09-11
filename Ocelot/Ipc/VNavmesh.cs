using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ECommons.EzIpcManager;
using ECommons.Reflection;

namespace Ocelot.Ipc;

[OcelotIpc("vnavmesh")]
public static class VNavmesh
{
    public static bool IsLoaded()
    {
        return DalamudReflector.TryGetDalamudPlugin("vnavmesh", out _, false, true);
    }

    [EzIPC("Nav.IsReady")]
    public static Func<bool> IsNavmeshReady = null!;

    [EzIPC("Nav.BuildProgress")]
    public static Func<float> GetBuildProgress = null!;

    [EzIPC("Nav.Reload")]
    public static Action ReloadNavmesh = null!;

    [EzIPC("Nav.Rebuild")]
    public static Action RebuildNavmesh = null!;

    [EzIPC("Nav.Pathfind")]
    public static Func<Vector3, Vector3, bool, Task<List<Vector3>>> Pathfind = null!;

    [EzIPC("Nav.PathfindCancelable")]
    public static Func<Vector3, Vector3, bool, CancellationToken, List<Vector3>> PathfindCancelable = null!;

    [EzIPC("Nav.PathfindCancelAll")]
    public static Action CancelAllPathfinds = null!;

    [EzIPC("Nav.PathfindInProgress")]
    public static Func<bool> IsPathfinding = null!;

    [EzIPC("Nav.PathfindNumQueued")]
    public static Func<int> NumQueuedPathfindRequests = null!;

    [EzIPC("Nav.IsAutoLoad")]
    public static Func<bool> IsAutoLoadEnabled = null!;

    [EzIPC("Nav.SetAutoLoad")]
    public static Action<bool> SetAutoLoad = null!;

    [EzIPC("Nav.BuildBitmap")]
    public static Func<Vector3, string, float, bool> BuildBitmap = null!;

    [EzIPC("Nav.BuildBitmapBounded")]
    public static Func<Vector3, string, float, Vector3, Vector3, bool> BuildBitmapBounded = null!;

    [EzIPC("Query.Mesh.NearestPoint")]
    public static Func<Vector3, float, float, Vector3?> FindNearestPointOnMesh = null!;

    [EzIPC("Query.Mesh.PointOnFloor")]
    public static Func<Vector3, bool, float, Vector3?> FindPointOnFloor = null!;

    [EzIPC("Path.MoveTo")]
    public static Action<List<Vector3>, bool> MoveTo = null!;

    [EzIPC("Path.MoveTo")]
    public static Action<List<Vector3>, bool> FollowPath = null!;

    [EzIPC("Path.Stop")]
    public static Action StopPath = null!;

    [EzIPC("Path.IsRunning")]
    public static Func<bool> IsRunning = null!;

    [EzIPC("Path.NumWaypoints")]
    public static Func<int> GetNumWaypoints = null!;

    [EzIPC("Path.ListWaypoints")]
    public static Func<List<Vector3>> GetWaypoints = null!;

    [EzIPC("Path.GetMovementAllowed")]
    public static Func<bool> IsMovementAllowed = null!;

    [EzIPC("Path.SetMovementAllowed")]
    public static Action<bool> SetMovementAllowed = null!;

    [EzIPC("Path.GetAlignCamera")]
    public static Func<bool> GetAlignCamera = null!;

    [EzIPC("Path.SetAlignCamera")]
    public static Action<bool> SetAlignCamera = null!;

    [EzIPC("Path.GetTolerance")]
    public static Func<float> GetTolerance = null!;

    [EzIPC("Path.SetTolerance")]
    public static Action<float> SetTolerance = null!;

    [EzIPC("SimpleMove.PathfindAndMoveTo")]
    public static Func<Vector3, bool, bool> PathfindAndMoveTo = null!;

    [EzIPC("SimpleMove.PathfindInProgress")]
    public static Func<bool> IsSimpleMoveInProgress = null!;

    [EzIPC("Window.IsOpen")]
    public static Func<bool> IsMainWindowOpen = null!;

    [EzIPC("Window.SetOpen")]
    public static Action<bool> SetMainWindowOpen = null!;

    [EzIPC("DTR.IsShown")]
    public static Func<bool> IsDtrShown = null!;

    [EzIPC("DTR.SetShown")]
    public static Action<bool> SetDtrShown = null!;


    [EzIPC("Path.Stop")]
    public static Action Stop = null!;
}
