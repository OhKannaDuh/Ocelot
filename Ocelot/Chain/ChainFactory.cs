using System;
using Dalamud.Game.ClientState.Keys;
using ECommons.Automation.NeoTaskManager;

namespace Ocelot.Chain;

public abstract class ChainFactory
{
    protected abstract Chain Create(Chain chain);

    public Func<Chain> Factory() => () => Create(Chain.Create(GetType().Name));

    public virtual TaskManagerConfiguration? Config() => null;
}