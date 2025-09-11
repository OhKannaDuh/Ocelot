using System;
using System.Collections.Generic;
using System.Numerics;
using ECommons.EzIpcManager;

namespace Ocelot.Ipc;

[OcelotIpc("Lifestream")]
public static class Lifestream
{
    [EzIPC]
    public static Action<string> ExecuteCommand = null!;

    // [EzIPC] public readonly Func<string, string, string, string, bool, bool, AddressBookEntryTuple> BuildAddressBookEntry = null!;

    // [EzIPC] public readonly Func<AddressBookEntryTuple, bool> IsHere = null!;

    // [EzIPC] public readonly Func<AddressBookEntryTuple, bool> IsQuickTravelAvailable = null!;

    // [EzIPC] public readonly Action<AddressBookEntryTuple> GoToHousingAddress = null!;

    [EzIPC]
    public static Func<bool> IsBusy = null!;

    [EzIPC]
    public static Action Abort = null!;

    [EzIPC]
    public static Func<string, bool> CanVisitSameDC = null!;

    [EzIPC]
    public static Func<string, bool> CanVisitCrossDC = null!;

    [EzIPC]
    public static Action<string, bool, string, bool, int?, bool?, bool?> TPAndChangeWorld = null!;

    [EzIPC]
    public static Func<uint, int?> GetWorldChangeAetheryteByTerritoryType = null!;

    [EzIPC]
    public static Func<string, bool> ChangeWorld = null!;

    [EzIPC]
    public static Func<uint, bool> ChangeWorldById = null!;

    [EzIPC]
    public static Func<string, bool> AethernetTeleport = null!;

    [EzIPC]
    public static Func<uint, bool> AethernetTeleportByPlaceNameId = null!;

    [EzIPC]
    public static Func<uint, bool> AethernetTeleportById = null!;

    [EzIPC]
    public static Func<uint, bool> HousingAethernetTeleportById = null!;

    [EzIPC]
    public static Func<bool> AethernetTeleportToFirmament = null!;

    [EzIPC]
    public static Func<uint> GetActiveAetheryte = null!;

    [EzIPC]
    public static Func<uint> GetActiveCustomAetheryte = null!;

    [EzIPC]
    public static Func<uint> GetActiveResidentialAetheryte = null!;

    [EzIPC]
    public static Func<uint, byte, bool> Teleport = null!;

    [EzIPC]
    public static Func<bool> TeleportToFC = null!;

    [EzIPC]
    public static Func<bool> TeleportToHome = null!;

    [EzIPC]
    public static Func<bool> TeleportToApartment = null!;

    // [EzIPC] public readonly Func<ulong, (HousePathData Private, HousePathData FC)> GetHousePathData = null!;

    // [EzIPC] public readonly Func<ResidentialAetheryteKind, uint> GetResidentialTerritory = null!;

    [EzIPC]
    public static Func<uint, int, Vector3?> GetPlotEntrance = null!;

    // [EzIPC] public readonly Action<TaskPropertyShortcut.PropertyType, HouseEnterMode?> EnqueuePropertyShortcut = null!;

    [EzIPC]
    public static Action<bool> EnterApartment = null!;

    [EzIPC]
    public static Action<int?> EnqueueInnShortcut = null!;

    [EzIPC]
    public static Action<int?> EnqueueLocalInnShortcut = null!;

    // [EzIPC] public readonly Func<(ResidentialAetheryteKind Kind, int Ward, int Plot)?> GetCurrentPlotInfo = null!;

    [EzIPC]
    public static Func<bool> CanChangeInstance = null!;

    [EzIPC]
    public static Func<int> GetNumberOfInstances = null!;

    [EzIPC]
    public static Action<int> ChangeInstance = null!;

    [EzIPC]
    public static Func<int> GetCurrentInstance = null!;

    [EzIPC]
    public static Func<bool?> HasApartment = null!;

    [EzIPC]
    public static Func<bool?> HasPrivateHouse = null!;

    [EzIPC]
    public static Func<bool?> HasFreeCompanyHouse = null!;

    [EzIPC]
    public static Action<List<Vector3>> Move = null!;

    [EzIPC]
    public static Func<bool> CanMoveToWorkshop = null!;

    [EzIPC]
    public static Action MoveToWorkshop = null!;

    [EzIPC]
    public static Func<uint> GetRealTerritoryType = null!;

    [EzIPCEvent]
    public static Action OnHouseEnterError = null!;
}
