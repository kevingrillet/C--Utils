using System.Collections.Generic;
using System.Text.Json;

namespace CSharp_Utils.Csv.Entities;

public class CsvRow
{
    public List<CsvCell> Columns { get; set; }
    public bool IsEmpty => !Columns.Exists(c => !c.IsEmpty);
    public int RowIndex { get; set; }
    public string SerializedColumns => JsonSerializer.Serialize(Columns);

    public CsvRow()
    {
        Columns = [];
    }

    public override string ToString()
    {
        return $"RowIndex: {RowIndex}, Columns: [{SerializedColumns}]";
    }
}
