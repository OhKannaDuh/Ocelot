using Lumina.Excel;

namespace Ocelot.Services.Data;

public readonly record struct SubrowAccessor(uint RowId, ushort SubId);

public interface ISubrowDataRepository<TSubrow>
    where TSubrow : struct, IExcelSubrow<TSubrow>
{
    SubrowExcelSheet<TSubrow> Sheet { get; }

    IEnumerable<uint> RowIds { get; }

    IEnumerable<SubrowAccessor> Keys { get; }

    bool HasRow(uint rowId);

    bool HasSubrow(uint rowId, ushort subrowId);

    bool HasSubrow(SubrowAccessor key);

    SubrowCollection<TSubrow> GetRow(uint rowId);

    bool TryGetRow(uint rowId, out SubrowCollection<TSubrow> row);

    TSubrow Get(SubrowAccessor key);

    TSubrow Get(uint rowId, ushort subrowId)
    {
        return Get(new SubrowAccessor(rowId, subrowId));
    }

    bool TryGet(SubrowAccessor key, out TSubrow subrow);

    bool TryGet(uint rowId, ushort subrowId, out TSubrow subrow)
    {
        return TryGet(new SubrowAccessor(rowId, subrowId), out subrow);
    }

    IEnumerable<SubrowCollection<TSubrow>> Rows { get; }

    IEnumerable<TSubrow> Subrows { get; }
}
