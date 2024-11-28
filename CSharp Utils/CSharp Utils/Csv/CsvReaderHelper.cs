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
    public static (List<string> Headers, List<CsvRow> Cells) ReadCsv(string filePath, char delimiter = ',')
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
                new CsvRow
                {
                    Columns = row.Values.Select((value, colIndex) =>
                        new CsvCell
                        {
                            ColIndex = colIndex + 1,
                            Value = value?.ToString()
                        })
                        .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                        .ToList(),
                    RowIndex = row.RowIndex + 1
                })
            .Where(r => !r.IsEmpty)
            .ToList();

        return (headers, rows);
    }
}
