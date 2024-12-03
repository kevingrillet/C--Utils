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
    /// <param name="includeNumberFormat">Indique si le NumberFormat doit être inclus (debug).</param>
    /// <returns>
    /// Un tuple contenant :
    /// <list type="bullet">
    /// <item>Une liste des en-têtes.</item>
    /// <item>Une liste des lignes.</item>
    /// </list>
    /// </returns>
    public static (List<string> Headers, List<ExcelRow> Rows) ReadExcelFile(string filePath, bool includeDebugInfo = false, bool includeNumberFormat = false)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Le fichier spécifié est introuvable.", filePath);

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1);
        if (worksheet == null || !worksheet.RowsUsed().Any())
            throw new InvalidOperationException("La feuille de calcul est vide ou introuvable.");

        var firstRow = worksheet.FirstRowUsed();
        if (firstRow == null || !firstRow.CellsUsed().Any())
            throw new InvalidOperationException("Aucun en-tête trouvé dans la première ligne.");

        var headers = firstRow.Cells().Select(cell => cell.GetValue<string>()).ToList();

        var rows = worksheet.RowsUsed()
            .Skip(1)
            .Where(row => row.CellsUsed().Any())
            .Select(dataRow => new ExcelRow
            {
                RowIndex = dataRow.RowNumber(),
                Columns = dataRow.Cells().Select(cell =>
                {
                    if (includeDebugInfo)
                    {
                        return new ExcelCellDebug(
                            cellReference: cell.Address.ToString(),
                            valueType: cell.DataType.ToSystemType(),
                            value: cell.ToObject(),
                            innerText: cell.GetString(),
                            numberFormat: includeNumberFormat ? (uint?)cell.Style.NumberFormat.NumberFormatId : null,
                            colIndex: cell.Address.ColumnNumber - 1
                        );
                    }
                    return new ExcelCell
                    {
                        ColIndex = cell.Address.ColumnNumber - 1,
                        Type = cell.DataType.ToSystemType(),
                        Value = cell.ToObject()
                    };
                })
                .Where(c => !c.IsEmpty)
                .ToList()
            })
            .Where(r => !r.IsEmpty)
            .ToList();

        return (headers, rows);
    }

    private static object ToObject(this IXLCell cell)
    {
        return cell.DataType switch
        {
            XLDataType.Blank => null,
            XLDataType.Boolean => cell.Value.GetBoolean(),
            XLDataType.Number => cell.Value.GetNumber(),
            XLDataType.Text => cell.Value.GetText(),
            XLDataType.Error => cell.Value.GetError(),
            XLDataType.DateTime => cell.Value.GetDateTime(),
            XLDataType.TimeSpan => cell.Value.GetTimeSpan(),
            _ => cell.GetValue<object>()
        };
    }

    /// <summary>
    /// Extension pour convertir les types de données ClosedXML en types .NET.
    /// </summary>
    private static Type ToSystemType(this XLDataType dataType)
    {
        return dataType switch
        {
            XLDataType.Boolean => typeof(bool),
            XLDataType.DateTime => typeof(DateTime),
            XLDataType.Number => typeof(double),
            XLDataType.Text => typeof(string),
            XLDataType.TimeSpan => typeof(TimeSpan),
            _ => typeof(object),
        };
    }
}
