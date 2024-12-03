using System.Collections.Generic;
using System.Text.Json;

namespace CSharp_Utils.Excel.Entities;

public class ExcelRow
{
    public List<ExcelCell> Columns { get; set; }
    public bool IsEmpty => !Columns.Exists(c => !c.IsEmpty);
    public int RowIndex { get; set; }
    public string SerializedColumns => JsonSerializer.Serialize(Columns);

    public ExcelRow()
    {
        Columns = [];
    }

    public override string ToString()
    {
        return $"RowIndex: {RowIndex}, Columns: [{SerializedColumns}]";
    }
}
