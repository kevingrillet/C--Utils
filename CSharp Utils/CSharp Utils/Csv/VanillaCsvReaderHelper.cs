using CSharp_Utils.Csv.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharp_Utils.Csv;

public static class VanillaCsvReaderHelper
{
    public static (List<string> Headers, List<CsvCell> Cells) ReadCsv(string filePath, char delimiter = ',')
    {
        var lines = File.ReadAllLines(filePath);

        var headers = lines[0]
            .Split(delimiter)
            .ToList();

        var cells = lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line) && Array.Exists(line.Split(delimiter), value => !string.IsNullOrWhiteSpace(value)))
            .SelectMany((line, rowIndex) => line.Split(delimiter)
            .Select((value, colIndex) => new CsvCell
            {
                RowIndex = rowIndex + 1,
                ColIndex = colIndex,
                Value = value
            }))
            .Where(c => !string.IsNullOrWhiteSpace(c.Value))
            .ToList();

        return (headers, cells);
    }
}
