
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ECommons.EzIpcManager;

namespace Ocelot.IPC;

#pragma warning disable CS8618
public class VNavmesh : IPCProvider
{
    public VNavmesh() : base("vnavmesh") { }

    [EzIPC("Nav.IsReady")]
    public Func<bool> IsNavmeshReady;

    [EzIPC("Nav.BuildProgress")]
    public Func<float> GetBuildProgress;

    [EzIPC("Nav.Reload")]
    public Action ReloadNavmesh;

    [EzIPC("Nav.Rebuild")]
    public Action RebuildNavmesh;

    [EzIPC("Nav.Pathfind")]
    public Func<Vector3, Vector3, bool, Task<List<Vector3>>> Pathfind;

    [EzIPC("Nav.PathfindCancelable")]
    public Func<Vector3, Vector3, bool, CancellationToken, List<Vector3>> PathfindCancelable;

    [EzIPC("Nav.PathfindCancelAll")]
    public Action CancelAllPathfinds;

    [EzIPC("Nav.PathfindInProgress")]
    public Func<bool> IsPathfinding;

    [EzIPC("Nav.PathfindNumQueued")]
    public Func<int> NumQueuedPathfindRequests;

    [EzIPC("Nav.IsAutoLoad")]
    public Func<bool> IsAutoLoadEnabled;

    [EzIPC("Nav.SetAutoLoad")]
    public Action<bool> SetAutoLoad;

    [EzIPC("Nav.BuildBitmap")]
    public Func<Vector3, string, float, bool> BuildBitmap;

    [EzIPC("Nav.BuildBitmapBounded")]
    public Func<Vector3, string, float, Vector3, Vector3, bool> BuildBitmapBounded;

    [EzIPC("Query.Mesh.NearestPoint")]
    public Func<Vector3, float, float, Vector3?> FindNearestPointOnMesh;

    [EzIPC("Query.Mesh.PointOnFloor")]
    public Func<Vector3, bool, float, Vector3?> FindPointOnFloor;

    [EzIPC("Path.MoveTo")]
    public Action<List<Vector3>, bool> MoveToPath;

    [EzIPC("Path.MoveTo")]
    public Action<List<Vector3>, bool> FollowPath;

    [EzIPC("Path.Stop")]
    public Action StopPath;

    [EzIPC("Path.IsRunning")]
    public Func<bool> IsRunning;

    [EzIPC("Path.NumWaypoints")]
    public Func<int> GetNumWaypoints;

    [EzIPC("Path.ListWaypoints")]
    public Func<List<Vector3>> GetWaypoints;

    [EzIPC("Path.GetMovementAllowed")]
    public Func<bool> IsMovementAllowed;

    [EzIPC("Path.SetMovementAllowed")]
    public Action<bool> SetMovementAllowed;

    [EzIPC("Path.GetAlignCamera")]
    public Func<bool> GetAlignCamera;

    [EzIPC("Path.SetAlignCamera")]
    public Action<bool> SetAlignCamera;

    [EzIPC("Path.GetTolerance")]
    public Func<float> GetTolerance;

    [EzIPC("Path.SetTolerance")]
    public Action<float> SetTolerance;

    [EzIPC("SimpleMove.PathfindAndMoveTo")]
    public Func<Vector3, bool, bool> PathfindAndMoveTo;

    [EzIPC("SimpleMove.PathfindInProgress")]
    public Func<bool> IsSimpleMoveInProgress;

    [EzIPC("Window.IsOpen")]
    public Func<bool> IsMainWindowOpen;

    [EzIPC("Window.SetOpen")]
    public Action<bool> SetMainWindowOpen;

    [EzIPC("DTR.IsShown")]
    public Func<bool> IsDtrShown;

    [EzIPC("DTR.SetShown")]
    public Action<bool> SetDtrShown;


    [EzIPC("Path.Stop")]
    public readonly Action Stop;

    public async Task WaitToStop()
    {
        while (true)
        {
            if (IsRunning == null)
                break;

            if (!IsRunning())
                break;

            await Task.Delay(50);
        }
    }

    public async Task WaitToStart()
    {
        while (true)
        {
            if (IsRunning == null)
                break;

            if (IsRunning())
                break;

            await Task.Delay(50);
        }
    }

    public class Path
    {
        public List<Vector3> Waypoints { get; init; } = [];

        public float Length => Distance();

        public float Distance()
        {
            if (Waypoints.Count < 2)
                return 0f;

            float totalDistance = 0f;
            for (int i = 0; i < Waypoints.Count - 1; i++)
            {
                totalDistance += Vector3.Distance(Waypoints[i], Waypoints[i + 1]);
            }

            return totalDistance;
        }
    }

    public async Task<Path> CreatePathAsync(Vector3 from, Vector3 to, bool fly)
    {
        return new Path { Waypoints = await Pathfind(from, to, fly) };
    }
}
