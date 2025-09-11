using ECommons.ExcelServices;
using ECommons.GameHelpers;
using Ocelot.Chain;
using Ocelot.Extensions;

namespace Ocelot.Gameplay;

public static class TankHelper
{
    public static readonly Paladin Paladin = new();

    public static readonly Warrior Warrior = new();

    public static readonly DarkKnight DarkKnight = new();

    public static readonly Gunbreaker Gunbreaker = new();

    public static bool IsTank {
        get => Current != null;
    }

    public static ITank? Current {
        get => Player.Job.GetData().RowId switch {
            1 or 19 => Paladin,
            3 or 21 => Warrior,
            32 => DarkKnight,
            37 => Gunbreaker,
            _ => null,
        };
    }
}

public interface ITank
{
    bool HasStanceOn();

    ChainRunner TurnStanceOn();

    ChainRunner TurnStanceOff();
}

public abstract class BaseTank(uint statusId) : ITank
{
    public bool HasStanceOn()
    {
        return Player.Status.HasStatus(statusId);
    }

    public abstract ChainRunner TurnStanceOn();

    public abstract ChainRunner TurnStanceOff();
}

public class Paladin() : BaseTank(79)
{
    public override ChainRunner TurnStanceOn()
    {
        return Actions.Paladin.IronWill.GetChainRunner();
    }

    public override ChainRunner TurnStanceOff()
    {
        return Actions.Paladin.ReleaseIronWill.GetChainRunner();
    }
}

public class Warrior() : BaseTank(91)
{
    public override ChainRunner TurnStanceOn()
    {
        return Actions.Warrior.Defiance.GetChainRunner();
    }

    public override ChainRunner TurnStanceOff()
    {
        return Actions.Warrior.ReleaseDefiance.GetChainRunner();
    }
}

public class DarkKnight() : BaseTank(743)
{
    public override ChainRunner TurnStanceOn()
    {
        return Actions.DarkKnight.Grit.GetChainRunner();
    }

    public override ChainRunner TurnStanceOff()
    {
        return Actions.DarkKnight.ReleaseGrit.GetChainRunner();
    }
}

public class Gunbreaker() : BaseTank(1833)
{
    public override ChainRunner TurnStanceOn()
    {
        return Actions.Gunbreaker.RoyalGuard.GetChainRunner();
    }

    public override ChainRunner TurnStanceOff()
    {
        return Actions.Gunbreaker.ReleaseRoyalGuard.GetChainRunner();
    }
}
