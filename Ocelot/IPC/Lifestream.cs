using System;
using System.Collections.Generic;
using System.Numerics;
using ECommons.EzIpcManager;

namespace Ocelot.IPC;

#pragma warning disable CS8618
public class Lifestream : IPCSubscriber
{
    public Lifestream() : base("Lifestream")
    {
    }

    [EzIPC] public readonly Action<string> ExecuteCommand;

    // [EzIPC]
    // public readonly Func<string, string, string, string, bool, bool, AddressBookEntryTuple> BuildAddressBookEntry;

    // [EzIPC]
    // public readonly Func<AddressBookEntryTuple, bool> IsHere;

    // [EzIPC]
    // public readonly Func<AddressBookEntryTuple, bool> IsQuickTravelAvailable;

    // [EzIPC]
    // public readonly Action<AddressBookEntryTuple> GoToHousingAddress;

    [EzIPC] public readonly Func<bool> IsBusy;

    [EzIPC] public readonly Action Abort;

    [EzIPC] public readonly Func<string, bool> CanVisitSameDC;

    [EzIPC] public readonly Func<string, bool> CanVisitCrossDC;

    [EzIPC] public readonly Action<string, bool, string, bool, int?, bool?, bool?> TPAndChangeWorld;

    [EzIPC] public readonly Func<uint, int?> GetWorldChangeAetheryteByTerritoryType;

    [EzIPC] public readonly Func<string, bool> ChangeWorld;

    [EzIPC] public readonly Func<uint, bool> ChangeWorldById;

    [EzIPC] public readonly Func<string, bool> AethernetTeleport;

    [EzIPC] public readonly Func<uint, bool> AethernetTeleportByPlaceNameId;

    [EzIPC] public readonly Func<uint, bool> AethernetTeleportById;

    [EzIPC] public readonly Func<uint, bool> HousingAethernetTeleportById;

    [EzIPC] public readonly Func<bool> AethernetTeleportToFirmament;

    [EzIPC] public readonly Func<uint> GetActiveAetheryte;

    [EzIPC] public readonly Func<uint> GetActiveResidentialAetheryte;

    [EzIPC] public readonly Func<uint, byte, bool> Teleport;

    [EzIPC] public readonly Func<bool> TeleportToFC;

    [EzIPC] public readonly Func<bool> TeleportToHome;

    [EzIPC] public readonly Func<bool> TeleportToApartment;

    // [EzIPC]
    // public readonly Func<ulong, (HousePathData Private, HousePathData FC)> GetHousePathData;

    // [EzIPC]
    // public readonly Func<ResidentialAetheryteKind, uint> GetResidentialTerritory;

    [EzIPC] public readonly Func<uint, int, Vector3?> GetPlotEntrance;

    // [EzIPC]
    // public readonly Action<TaskPropertyShortcut.PropertyType, HouseEnterMode?> EnqueuePropertyShortcut;

    [EzIPC] public readonly Action<bool> EnterApartment;

    [EzIPC] public readonly Action<int?> EnqueueInnShortcut;

    [EzIPC] public readonly Action<int?> EnqueueLocalInnShortcut;

    // [EzIPC]
    // public readonly Func<(ResidentialAetheryteKind Kind, int Ward, int Plot)?> GetCurrentPlotInfo;

    [EzIPC] public readonly Func<bool> CanChangeInstance;

    [EzIPC] public readonly Func<int> GetNumberOfInstances;

    [EzIPC] public readonly Action<int> ChangeInstance;

    [EzIPC] public readonly Func<int> GetCurrentInstance;

    [EzIPC] public readonly Func<bool?> HasApartment;

    [EzIPC] public readonly Func<bool?> HasPrivateHouse;

    [EzIPC] public readonly Func<bool?> HasFreeCompanyHouse;

    [EzIPC] public readonly Action<List<Vector3>> Move;

    [EzIPC] public readonly Func<bool> CanMoveToWorkshop;

    [EzIPC] public readonly Action MoveToWorkshop;

    [EzIPC] public readonly Func<uint> GetRealTerritoryType;


    [EzIPCEvent] public readonly Action OnHouseEnterError;
}
