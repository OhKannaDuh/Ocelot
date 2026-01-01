using Dalamud.Plugin.Services;
using Lumina.Excel;

namespace Ocelot.Services.Data;

public class ExcelSubrowDataRepository<TSubrow> : ISubrowDataRepository<TSubrow>
    where TSubrow : struct, IExcelSubrow<TSubrow>
{
    public SubrowExcelSheet<TSubrow> Sheet { get; }

    public ExcelSubrowDataRepository(IDataManager data)
    {
        Sheet = data.GameData.GetSubrowExcelSheet<TSubrow>() ??
                throw new InvalidOperationException($"Unable to get subrow excel sheet for {typeof(TSubrow).FullName ?? "Unknown"}");
    }

    public IEnumerable<uint> RowIds
    {
        get => Sheet.Select(rc => rc.RowId);
    }

    public IEnumerable<SubrowAccessor> Keys
    {
        get
        {
            foreach (var row in Sheet)
            {
                for (ushort i = 0; i < row.Count; i++)
                {
                    yield return new SubrowAccessor(row.RowId, i);
                }
            }
        }
    }

    public bool HasRow(uint rowId)
    {
        return Sheet.HasRow(rowId);
    }

    public bool HasSubrow(uint rowId, ushort subrowId)
    {
        return Sheet.HasSubrow(rowId, subrowId);
    }

    public bool HasSubrow(SubrowAccessor key)
    {
        return Sheet.HasSubrow(key.RowId, key.SubId);
    }

    public SubrowCollection<TSubrow> GetRow(uint rowId)
    {
        return Sheet.GetRow(rowId);
    }

    public bool TryGetRow(uint rowId, out SubrowCollection<TSubrow> row)
    {
        return Sheet.TryGetRow(rowId, out row);
    }

    public TSubrow Get(SubrowAccessor key)
    {
        return Sheet.GetSubrow(key.RowId, key.SubId);
    }

    public bool TryGet(SubrowAccessor key, out TSubrow subrow)
    {
        return Sheet.TryGetSubrow(key.RowId, key.SubId, out subrow);
    }

    public IEnumerable<SubrowCollection<TSubrow>> Rows
    {
        get => Sheet;
    }

    public IEnumerable<TSubrow> Subrows
    {
        get => Sheet.Flatten();
    }
}
