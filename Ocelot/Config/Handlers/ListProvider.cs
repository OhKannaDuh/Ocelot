using System.Collections.Generic;

namespace Ocelot.Config.Handlers;

public abstract class ListProvider<T> : IListProvider<T>
    where T : notnull
{
    public abstract IEnumerable<T> GetItems();

    public virtual bool Filter(T item)
    {
        return true;
    }

    public virtual string GetLabel(T item)
    {
        return item.ToString() ?? "";
    }
}
