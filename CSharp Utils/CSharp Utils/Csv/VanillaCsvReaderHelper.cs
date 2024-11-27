using CSharp_Utils.Csv.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharp_Utils.Csv;

public static class VanillaCsvReaderHelper
{
    public static (List<string> Headers, List<List<CsvCell>> Cells) ReadCsv(string filePath, char delimiter = ',')
    {
        var lines = File.ReadAllLines(filePath);

        var headers = lines[0]
            .Split(delimiter)
            .ToList();

        var rows = lines
            .Skip(1)
            .Select((line, rowIndex) => line.Split(delimiter)
                .Select((value, colIndex) => new CsvCell
                {
                    RowIndex = rowIndex + 1,
                    ColIndex = colIndex,
                    Value = value
                })
                .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                .ToList())
            .Where(r => r.Count != 0)
            .ToList();

        return (headers, rows);
    }
}
