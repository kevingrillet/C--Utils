using CSharp_Utils.Excel.Entities;
using CSharp_Utils.Excel.Enums;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CSharp_Utils.Excel;

/// <summary>
/// Cette classe fournit des méthodes pour lire les cellules à partir de fichiers Excel (xlsx) en utilisant OpenXML.
/// </summary>
public static class OpenXmlXlsxReader
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
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Le chemin du fichier est invalide.", nameof(filePath));

        var headers = new List<string>();
        var rows = new List<ExcelRow>();

        using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false))
        {
            WorkbookPart workbookPart = document.WorkbookPart;
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault()
                ?? throw new ArgumentException("Feuille non trouvée.");
            WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
            SharedStringTablePart stringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            CellFormats cellFormats = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats;

            var rowsData = worksheetPart.Worksheet.Descendants<Row>().ToList();
            if (rowsData.Count > 0)
            {
                headers = rowsData[0].Descendants<Cell>()
                    .Select(cell => GetExcelCell(cell, stringTablePart, cellFormats, includeDebugInfo, includeNumberFormat).Value.ToString())
                    .ToList();

                rows = rowsData
                    .Skip(1)
                    .Select(row => new ExcelRow
                    {
                        RowIndex = (int)row.RowIndex.Value,
                        Columns = row.Descendants<Cell>().Select(cell => GetExcelCell(cell, stringTablePart, cellFormats, includeDebugInfo, includeNumberFormat)).ToList()
                    })
                    .Where(r => !r.IsEmpty)
                    .ToList();
            }
        }

        return (headers, rows);
    }

    /// <summary>
    /// Détermine le type de données attendu pour un format numérique personnalisé.
    /// </summary>
    /// <param name="cellReference">La référence de la cellule.</param>
    /// <param name="cellInnerText">Le texte interne de la cellule.</param>
    /// <param name="numberFormatId">L'ID du format numérique de la cellule.</param>
    /// <param name="dataType">Le type de données de la cellule.</param>
    /// <returns>Un objet ExcelCellDebug représentant la valeur de la cellule formatée.</returns>
    private static ExcelCellDebug DetermineCustomNumberFormatType(string cellReference, string cellInnerText, uint? numberFormatId)
    {
        // Implémentez ici la logique pour déterminer le type attendu pour les formats numériques personnalisés
        // Par exemple, vous pouvez mapper certains IDs de format à des types spécifiques

        if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double customValue))
        {
            return new ExcelCellDebug(cellReference, typeof(double), customValue, cellInnerText, numberFormatId);
        }

        return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId);
    }

    /// <summary>
    /// Formatte la valeur d'une cellule en fonction de son format numérique.
    /// </summary>
    /// <param name="cellReference">La référence de la cellule.</param>
    /// <param name="cellInnerText">Le texte interne de la cellule.</param>
    /// <param name="numberFormat">Le format numérique de la cellule.</param>
    /// <param name="numberFormatId">L'ID du format numérique de la cellule.</param>
    /// <param name="dataType">Le type de données de la cellule.</param>
    /// <returns>Un objet ExcelCellDebug représentant la valeur de la cellule formatée.</returns>
    private static ExcelCellDebug FormatCellValue(string cellReference, string cellInnerText, NumberFormat numberFormat, uint? numberFormatId)
    {
        switch (numberFormat)
        {
            case NumberFormat.General:
                return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId);

            case NumberFormat.Decimal:
            case NumberFormat.Decimal2:
            case NumberFormat.Thousands:
            case NumberFormat.Thousands2:
            case NumberFormat.Percentage:
            case NumberFormat.Percentage2:
            case NumberFormat.Scientific:
            case NumberFormat.Scientific2:
            case NumberFormat.Accounting:
                if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numberValue))
                {
                    return new ExcelCellDebug(cellReference, typeof(double), numberValue, cellInnerText, numberFormatId);
                }
                break;

            case NumberFormat.DateShort:
            case NumberFormat.DateLong:
            case NumberFormat.DateTime:
            case NumberFormat.DateTimeLong:
            case NumberFormat.DateTimeSeconds:
                if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double oaDate))
                {
                    DateTime dateValue = DateTime.FromOADate(oaDate);
                    return new ExcelCellDebug(cellReference, typeof(DateTime), dateValue, cellInnerText, numberFormatId);
                }
                break;

            case NumberFormat.Time12Hour:
            case NumberFormat.Time12HourSeconds:
            case NumberFormat.Time24Hour:
            case NumberFormat.Time24HourSeconds:
            case NumberFormat.TimeMinutesSeconds:
            case NumberFormat.TimeMinutesSecondsTenths:
            case NumberFormat.TimeMinutesSecondsHundredths:
                if (TimeSpan.TryParse(cellInnerText, CultureInfo.InvariantCulture, out TimeSpan timeValue))
                {
                    return new ExcelCellDebug(cellReference, typeof(TimeSpan), timeValue, cellInnerText, numberFormatId);
                }
                break;

            case NumberFormat.Fraction1:
            case NumberFormat.Fraction2:
                if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double fractionValue))
                {
                    return new ExcelCellDebug(cellReference, typeof(double), fractionValue, cellInnerText, numberFormatId);
                }
                break;

            case NumberFormat.Text:
                return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId);

            default:
                return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId);
        }
        return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId);
    }

    private static ExcelCell GetExcelCell(Cell cell, SharedStringTablePart stringTablePart, CellFormats cellFormats, bool includeDebugInfo, bool includeNumberFormat)
    {
        var excelCell = GetValue(cell, stringTablePart, cellFormats);
        if (includeDebugInfo)
        {
            if (!includeNumberFormat)
            {
                excelCell.NumberFormat = null;
            }
            return excelCell;
        }
        return new ExcelCell(excelCell.ColIndex, excelCell.Type, excelCell.Value);
    }

    /// <summary>
    /// Récupère la valeur d'une cellule en fonction de son type de données ou de son format.
    /// </summary>
    /// <param name="cell">La cellule dont la valeur doit être récupérée.</param>
    /// <param name="stringTablePart">La partie tableau des chaînes partagées.</param>
    /// <param name="cellFormats">Les formats des cellules.</param>
    /// <returns>Un objet ExcelCellDebug représentant la valeur de la cellule.</returns>
    private static ExcelCellDebug GetValue(Cell cell, SharedStringTablePart stringTablePart, CellFormats cellFormats)
    {
        if (cell == null)
        {
            return new ExcelCellDebug(null, typeof(string), null);
        }

        if (cell.DataType != null)
        {
            return GetValueByDataType(cell, stringTablePart);
        }
        else
        {
            return GetValueByFormat(cell, cellFormats, stringTablePart);
        }
    }

    /// <summary>
    /// Récupère la valeur d'une cellule en fonction de son type de données.
    /// </summary>
    /// <param name="cell">La cellule dont la valeur doit être récupérée.</param>
    /// <param name="stringTablePart">La partie tableau des chaînes partagées.</param>
    /// <returns>Un objet ExcelCellDebug représentant la valeur de la cellule.</returns>
    private static ExcelCellDebug GetValueByDataType(Cell cell, SharedStringTablePart stringTablePart)
    {
        string cellInnerText = cell.InnerText;
        string cellReference = cell.CellReference;

        if (cell.DataType.Value == CellValues.SharedString)
        {
            if (stringTablePart != null && int.TryParse(cellInnerText, out int index))
            {
                return new ExcelCellDebug(cellReference, typeof(string), stringTablePart.SharedStringTable.ElementAt(index).InnerText, cellInnerText, null);
            }
        }
        else if (cell.DataType.Value == CellValues.Boolean)
        {
            return new ExcelCellDebug(cellReference, typeof(bool), cellInnerText == "1", cellInnerText, null);
        }
        else if (cell.DataType.Value == CellValues.Date)
        {
            if (DateTime.TryParse(cellInnerText, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
            {
                return new ExcelCellDebug(cellReference, typeof(DateTime), dateValue.Date, cellInnerText, null);
            }
        }
        else if (cell.DataType.Value == CellValues.Number)
        {
            if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numberValue))
            {
                return new ExcelCellDebug(cellReference, typeof(double), numberValue, cellInnerText, null);
            }
        }
        else if (cell.DataType.Value == CellValues.Error)
        {
            return new ExcelCellDebug(cellReference, typeof(string), "ERROR: " + cellInnerText, cellInnerText, null);
        }

        return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, null);
    }

    /// <summary>
    /// Récupère la valeur d'une cellule en fonction de son format.
    /// </summary>
    /// <param name="cell">La cellule dont la valeur doit être récupérée.</param>
    /// <param name="cellFormats">Les formats des cellules.</param>
    /// <param name="stringTablePart">La partie tableau des chaînes partagées.</param>
    /// <returns>Un objet ExcelCellDebug représentant la valeur de la cellule.</returns>
    private static ExcelCellDebug GetValueByFormat(Cell cell, CellFormats cellFormats, SharedStringTablePart stringTablePart)
    {
        string cellInnerText = cell.InnerText;
        string cellReference = cell.CellReference;
        uint? numberFormatId = null;

        if (cell.StyleIndex != null)
        {
            uint styleIndex = cell.StyleIndex.Value;
            CellFormat cellFormat = (CellFormat)cellFormats.ElementAt((int)styleIndex);

            if (cellFormat != null && cellFormat.NumberFormatId != null)
            {
                numberFormatId = cellFormat.NumberFormatId.Value;
                NumberFormat numberFormat = (NumberFormat)cellFormat.NumberFormatId.Value;
                if (numberFormatId >= 164)
                {
                    return DetermineCustomNumberFormatType(cellReference, cellInnerText, numberFormatId);
                }
                return FormatCellValue(cellReference, cellInnerText, numberFormat, numberFormatId);
            }
        }

        if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
        {
            return new ExcelCellDebug(cellReference, typeof(double), numericValue, cellInnerText, numberFormatId);
        }

        if (stringTablePart != null && cell.CellValue != null && int.TryParse(cell.CellValue.Text, out int sharedStringIndex))
        {
            return new ExcelCellDebug(cellReference, typeof(string), stringTablePart.SharedStringTable.ElementAt(sharedStringIndex).InnerText, cellInnerText, numberFormatId);
        }

        return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId);
    }
}
