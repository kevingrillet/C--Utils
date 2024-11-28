using CSharp_Utils.Csv.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharp_Utils.Csv;

public static class VanillaCsvReaderHelper
{
    public static (List<string> Headers, List<CsvRow> Cells) ReadCsv(string filePath, char delimiter = ',')
    {
        var lines = File.ReadAllLines(filePath);

        var headers = lines[0]
            .Split(delimiter)
            .ToList();

        var rows = lines
            .Skip(1)
            .Select((line, rowIndex) =>
                new CsvRow
                {
                    Columns = line.Split(delimiter)
                        .Select((value, colIndex) =>
                            new CsvCell
                            {
                                ColIndex = colIndex,
                                Value = value
                            })
                        .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                        .ToList(),
                    RowIndex = rowIndex + 1
                }
            )
            .Where(r => !r.IsEmpty)
            .ToList();

        return (headers, rows);
    }
}
