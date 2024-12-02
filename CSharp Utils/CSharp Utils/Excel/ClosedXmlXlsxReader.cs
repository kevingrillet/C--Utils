using ClosedXML.Excel;
using CSharp_Utils.Excel.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CSharp_Utils.Excel;

public static class ClosedXmlXlsxReader
{
    /// <summary>
    /// Lit un fichier Excel et retourne les en-têtes et les lignes sous forme de tuple, avec une option pour inclure des informations de débogage.
    /// </summary>
    /// <param name="filePath">Chemin du fichier Excel.</param>
    /// <param name="includeDebugInfo">Indique si des informations de débogage doivent être incluses.</param>
    /// <returns>
    /// Un tuple contenant :
    /// - Une liste des en-têtes (List<string>).
    /// - Une liste des lignes (List<ExcelRow> ou List<ExcelRowDebug> selon le mode).
    /// </returns>
    public static (List<string> Headers, List<ExcelRow> Rows) ReadExcelFile(string filePath, bool? includeDebugInfo = false)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Le fichier spécifié est introuvable.", filePath);

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1);

        var firstRow = worksheet.FirstRowUsed();
        var headers = firstRow.Cells().Select(cell => cell.GetValue<string>()).ToList();

        var rows = worksheet.RowsUsed()
            .Skip(1)
            .Select(dataRow => new ExcelRow
            {
                RowIndex = dataRow.RowNumber(),
                Columns = dataRow.Cells().Select(cell =>
                {
                    if (includeDebugInfo == true)
                    {
                        return new ExcelCellDebug(
                            cellReference: cell.Address.ToString(),
                            valueType: cell.DataType.ToSystemType(),
                            value: cell.Value,
                            innerText: cell.GetString(),
                            numberFormat: null, // (uint?)cell.Style.NumberFormat.NumberFormatId,
                            dataType: cell.DataType.ToString()
                        )
                        { ColIndex = cell.Address.ColumnNumber };
                    }
                    return new ExcelCell
                    {
                        ColIndex = cell.Address.ColumnNumber,
                        Type = cell.DataType.ToSystemType(),
                        Value = cell.Value
                    };
                }).ToList()
            })
            //.Where(row => !row.IsEmpty)
            .ToList();

        return (headers, rows);
    }

    /// <summary>
    /// Extension pour convertir les types de données ClosedXML en types .NET.
    /// </summary>
    private static Type ToSystemType(this XLDataType dataType)
    {
        return dataType switch
        {
            XLDataType.Text => typeof(string),
            XLDataType.Number => typeof(double),
            XLDataType.Boolean => typeof(bool),
            XLDataType.DateTime => typeof(DateTime),
            _ => typeof(object),
        };
    }
}
