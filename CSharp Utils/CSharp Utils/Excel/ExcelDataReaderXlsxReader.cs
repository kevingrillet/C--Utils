using CSharp_Utils.Excel.Entities;
using CSharp_Utils.Excel.Utils;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CSharp_Utils.Excel;

public static class ExcelDataReaderXlsxReader
{
    /// <summary>
    /// Lit un fichier Excel et retourne les en-têtes et les lignes sous forme de tuple.
    /// </summary>
    /// <param name="filePath">Chemin du fichier Excel.</param>
    /// <param name="includeDebugInfo">Indique si des informations de débogage doivent être incluses.</param>
    /// <returns>
    /// Un tuple contenant :
    /// <list type="bullet">
    /// <item>Une liste des en-têtes.</item>
    /// <item>Une liste des lignes.</item>
    /// </list>
    /// </returns>
    public static (List<string> headers, List<ExcelRow> rows) ReadExcelFile(string filePath, bool includeDebugInfo = false)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            ConfigureDataTable = (_) => new ExcelDataTableConfiguration
            {
                UseHeaderRow = false
            }
        });

        var table = dataSet.Tables[0];

        var headers = table.Rows[0].ItemArray
            .Select(cell => cell?.ToString() ?? string.Empty)
            .ToList();

        var rows = table.Rows.Cast<DataRow>()
            .Skip(1)
            .Select((row, rowIndex) => new ExcelRow
            {
                RowIndex = rowIndex + 2,
                Columns = row.ItemArray.Select((cellValue, colIndex) =>
                {
                    if (includeDebugInfo)
                    {
                        var cellReference = ExcelUtils.GetCellReferenceFromIndexes(rowIndex + 2, colIndex);
                        return new ExcelCellDebug(
                            cellReference,
                            cellValue?.GetType() != typeof(DBNull) ? cellValue?.GetType() : null,
                            cellValue?.GetType() != typeof(DBNull) ? cellValue : null,
                            innerText: cellValue?.ToString(),
                            numberFormat: null,
                            colIndex: colIndex
                        );
                    }

                    return new ExcelCell(
                        colIndex,
                        cellValue?.GetType() != typeof(DBNull) ? cellValue?.GetType() : null,
                        cellValue?.GetType() != typeof(DBNull) ? cellValue : null
                    );
                })
                .Where(c => !c.IsEmpty)
                .ToList()
            })
            .Where(row => !row.IsEmpty)
            .ToList();

        return (headers, rows);
    }
}
