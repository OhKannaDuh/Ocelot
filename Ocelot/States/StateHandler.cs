using System;
using Ocelot.Modules;

namespace Ocelot.States;

public abstract class StateHandler<T, M>
    where T : struct, Enum
    where M : IModule
{
    public abstract T? Handle(M module);

    public virtual void OnEnter(M module)
    {
    }

    public virtual void OnExit(M module)
    {
    }
}
