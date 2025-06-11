using System;

namespace Ocelot.Config.Handlers;

public abstract class EnumProvider<T> : IEnumProvider<T>
    where T : Enum
{
    public virtual bool Filter(T item) => true;

    public virtual string GetLabel(T item) => item.ToString();
}
