using System.Collections.Generic;

namespace Ocelot.Config.Handlers;

public interface IListProvider<T>
    where T : notnull
{
    IEnumerable<T> GetItems();

    bool Filter(T item);

    string GetLabel(T item);
}
