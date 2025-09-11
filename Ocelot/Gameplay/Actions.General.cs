using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Gameplay;

public partial class Actions
{
    public static readonly Action AutoAttack = new(ActionType.GeneralAction, 1);

    public static readonly Action Jump = new(ActionType.GeneralAction, 2);

    public static readonly Action LimitBreak = new(ActionType.GeneralAction, 3);

    public static readonly Action Sprint = new(ActionType.GeneralAction, 4);

    public static readonly Action Desynthesis = new(ActionType.GeneralAction, 5);

    public static readonly Action Repair = new(ActionType.GeneralAction, 6);

    public static readonly Action Teleport = new(ActionType.GeneralAction, 7);

    public static readonly Action Return = new(ActionType.GeneralAction, 8);

    public static readonly Action MountRoulette = new(ActionType.GeneralAction, 9);

    public static readonly Action MinionRoulette = new(ActionType.GeneralAction, 10);

    public static readonly Action MateriaMelding = new(ActionType.GeneralAction, 12);

    public static readonly Action AdvancedMateriaMelding = new(ActionType.GeneralAction, 13);

    public static readonly Action MateriaExtraction = new(ActionType.GeneralAction, 14);

    public static readonly Action Dye = new(ActionType.GeneralAction, 15);

    public static readonly Action TargetForward = new(ActionType.GeneralAction, 16);

    public static readonly Action TargetBack = new(ActionType.GeneralAction, 17);

    public static readonly Action SetDown = new(ActionType.GeneralAction, 18);

    public static readonly Action Decipher = new(ActionType.GeneralAction, 19);

    public static readonly Action Dig = new(ActionType.GeneralAction, 20);

    public static readonly Action AetherialReduction = new(ActionType.GeneralAction, 21);

    public static readonly Action CastGlamour = new(ActionType.GeneralAction, 22);

    public static readonly Action Dismount = new(ActionType.GeneralAction, 23);

    public static readonly Action FlyingMountRoulette = new(ActionType.GeneralAction, 24);

    public static readonly Action GlamourPlate = new(ActionType.GeneralAction, 25);

    public static readonly Action DutyActionI = new(ActionType.GeneralAction, 26);

    public static readonly Action DutyActionII = new(ActionType.GeneralAction, 27);

    public static readonly Action PutAway = new(ActionType.GeneralAction, 28);

    public static readonly Action SortPetHotbar = new(ActionType.GeneralAction, 29);

    public static readonly Action PhantomActionI = new(ActionType.GeneralAction, 31);

    public static readonly Action PhantomActionII = new(ActionType.GeneralAction, 32);

    public static readonly Action PhantomActionIII = new(ActionType.GeneralAction, 33);

    public static readonly Action PhantomActionIV = new(ActionType.GeneralAction, 34);

    public static readonly Action PhantomActionV = new(ActionType.GeneralAction, 35);

    public static readonly Action AttackTarget = new(ActionType.GeneralAction, 36);

    public static readonly Action BindTarget = new(ActionType.GeneralAction, 37);

    public static readonly Action IgnoreTarget = new(ActionType.GeneralAction, 38);

    public static readonly Action SquareTarget = new(ActionType.GeneralAction, 39);

    public static readonly Action CircleTarget = new(ActionType.GeneralAction, 40);

    public static readonly Action PlusTarget = new(ActionType.GeneralAction, 41);

    public static readonly Action TriangleTarget = new(ActionType.GeneralAction, 42);
}
