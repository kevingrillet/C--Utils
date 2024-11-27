using CSharp_Utils.Csv.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CSharp_Utils.Csv;

public static class CsvReaderHelper
{
    public static (List<string> Headers, List<List<CsvCell>> Cells) ReadCsv(string filePath, char delimiter = ',')
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = delimiter.ToString(),
            BadDataFound = null
        });

        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord.ToList();

        var rows = csv.GetRecords<dynamic>() // Lire les lignes en tant qu'objets dynamiques
            .Select((record, rowIndex) => new
            {
                RowIndex = rowIndex + 1,
                Values = ((IDictionary<string, object>)record).Values.ToArray()
            })
            .Select(row =>
                row.Values.Select((value, colIndex) => new CsvCell
                {
                    RowIndex = row.RowIndex,
                    ColIndex = colIndex,
                    Value = value?.ToString()
                })
                .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                .ToList())
            .Where(r => r.Count != 0)
            .ToList();

        return (headers, rows);
    }
}
