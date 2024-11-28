using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CSharp_Utils.Csv.Entities;

public class CsvRow
{
    public List<CsvCell> Columns { get; set; }
    public bool IsEmpty => !Columns.Exists(c => !string.IsNullOrWhiteSpace(c.Value));
    public int RowIndex { get; set; }
    public string SerializedColumns => JsonSerializer.Serialize(Columns);

    public CsvRow()
    {
        Columns = [];
    }
}
