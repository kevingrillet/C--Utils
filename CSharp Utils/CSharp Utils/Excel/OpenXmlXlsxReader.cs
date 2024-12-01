using CSharp_Utils.Excel.Entities;
using CSharp_Utils.Excel.Enums;
using CSharp_Utils.Excel.Utils;
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
    /// Lit les cellules d'un fichier Excel (xlsx) et renvoie une liste de ExcelCellDebug.
    /// </summary>
    /// <param name="filePath">Le chemin du fichier Excel.</param>
    /// <param name="sheetName">Le nom de la feuille à lire. Si non spécifié, la première feuille sera utilisée.</param>
    /// <returns>Une liste de ExcelCellDebug contenant les valeurs des cellules.</returns>
    public static List<ExcelCellDebug> ReadCells(string filePath, string sheetName = null)
    {
        List<ExcelCellDebug> cellValues = [];

        using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false))
        {
            WorkbookPart workbookPart = document.WorkbookPart;
            Sheet sheet;

            if (string.IsNullOrEmpty(sheetName))
            {
                sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault();
            }
            else
            {
                sheet = workbookPart.Workbook.Descendants<Sheet>()
                    .FirstOrDefault(s => s.Name == sheetName);
            }

            if (sheet == null)
            {
                throw new ArgumentException("Feuille non trouvée.");
            }

            WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
            SharedStringTablePart stringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            CellFormats cellFormats = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats;

            foreach (Row row in worksheetPart.Worksheet.Descendants<Row>())
            {
                int maxColumnIndex = row.Descendants<Cell>().Select(c => ExcelUtils.GetColumnIndexFromCellReference(c.CellReference)).Max();

                for (int colIndex = 0; colIndex <= maxColumnIndex; colIndex++)
                {
                    Cell cell = row.Elements<Cell>().FirstOrDefault(c => ExcelUtils.GetColumnIndexFromCellReference(c.CellReference) == colIndex);
                    cell ??= new Cell() { CellReference = ExcelUtils.GetCellReferenceFromIndexes(row.RowIndex, colIndex) };
                    cellValues.Add(GetValue(cell, stringTablePart, cellFormats));
                }
            }
        }

        return cellValues;
    }

    /// <summary>
    /// Détermine le type de données attendu pour un format numérique personnalisé.
    /// </summary>
    /// <param name="cellReference">La référence de la cellule.</param>
    /// <param name="cellInnerText">Le texte interne de la cellule.</param>
    /// <param name="numberFormatId">L'ID du format numérique de la cellule.</param>
    /// <param name="dataType">Le type de données de la cellule.</param>
    /// <returns>Un objet ExcelCellDebug représentant la valeur de la cellule formatée.</returns>
    private static ExcelCellDebug DetermineCustomNumberFormatType(string cellReference, string cellInnerText, uint? numberFormatId, string dataType)
    {
        // Implémentez ici la logique pour déterminer le type attendu pour les formats numériques personnalisés
        // Par exemple, vous pouvez mapper certains IDs de format à des types spécifiques

        if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double customValue))
        {
            if (ExcelUtils.IsWholeNumber(customValue))
            {
                return new ExcelCellDebug(cellReference, typeof(int), (int)customValue, cellInnerText, numberFormatId, dataType);
            }
            else
            {
                return new ExcelCellDebug(cellReference, typeof(double), customValue, cellInnerText, numberFormatId, dataType);
            }
        }

        return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, dataType);
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
    private static ExcelCellDebug FormatCellValue(string cellReference, string cellInnerText, NumberFormat numberFormat, uint? numberFormatId, string dataType)
    {
        switch (numberFormat)
        {
            case NumberFormat.General:
                return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, dataType);

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
                    if (ExcelUtils.IsWholeNumber(numberValue))
                    {
                        return new ExcelCellDebug(cellReference, typeof(int), (int)numberValue, cellInnerText, numberFormatId, dataType);
                    }
                    else
                    {
                        return new ExcelCellDebug(cellReference, typeof(double), numberValue, cellInnerText, numberFormatId, dataType);
                    }
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
                    return new ExcelCellDebug(cellReference, typeof(DateTime), dateValue, cellInnerText, numberFormatId, dataType);
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
                    return new ExcelCellDebug(cellReference, typeof(TimeSpan), timeValue, cellInnerText, numberFormatId, dataType);
                }
                break;

            case NumberFormat.Fraction1:
            case NumberFormat.Fraction2:
                if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double fractionValue))
                {
                    return new ExcelCellDebug(cellReference, typeof(double), fractionValue, cellInnerText, numberFormatId, dataType);
                }
                break;

            case NumberFormat.Text:
                return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, dataType);

            default:
                return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, dataType);
        }
        return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, dataType);
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
                return new ExcelCellDebug(cellReference, typeof(string), stringTablePart.SharedStringTable.ElementAt(index).InnerText, cellInnerText, null, cell.DataType?.InnerText);
            }
        }
        else if (cell.DataType.Value == CellValues.Boolean)
        {
            return new ExcelCellDebug(cellReference, typeof(bool), cellInnerText == "1", cellInnerText, null, cell.DataType?.InnerText);
        }
        else if (cell.DataType.Value == CellValues.Date)
        {
            if (DateTime.TryParse(cellInnerText, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
            {
                return new ExcelCellDebug(cellReference, typeof(DateTime), dateValue.Date, cellInnerText, null, cell.DataType?.InnerText);
            }
        }
        else if (cell.DataType.Value == CellValues.Number)
        {
            if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numberValue))
            {
                if (ExcelUtils.IsWholeNumber(numberValue))
                {
                    return new ExcelCellDebug(cellReference, typeof(int), (int)numberValue, cellInnerText, null, cell.DataType?.InnerText);
                }
                else
                {
                    return new ExcelCellDebug(cellReference, typeof(double), numberValue, cellInnerText, null, cell.DataType?.InnerText);
                }
            }
        }
        else if (cell.DataType.Value == CellValues.Error)
        {
            return new ExcelCellDebug(cellReference, typeof(string), "ERROR: " + cellInnerText, cellInnerText, null, cell.DataType?.InnerText);
        }

        return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, null, cell.DataType?.InnerText);
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
                    return DetermineCustomNumberFormatType(cellReference, cellInnerText, numberFormatId, cell.DataType?.InnerText);
                }
                return FormatCellValue(cellReference, cellInnerText, numberFormat, numberFormatId, cell.DataType?.InnerText);
            }
        }

        if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
        {
            if (ExcelUtils.IsWholeNumber(numericValue))
            {
                return new ExcelCellDebug(cellReference, typeof(int), (int)numericValue, cellInnerText, numberFormatId, cell.DataType?.InnerText);
            }
            else
            {
                return new ExcelCellDebug(cellReference, typeof(double), numericValue, cellInnerText, numberFormatId, cell.DataType?.InnerText);
            }
        }

        if (stringTablePart != null && cell.CellValue != null && int.TryParse(cell.CellValue.Text, out int sharedStringIndex))
        {
            return new ExcelCellDebug(cellReference, typeof(string), stringTablePart.SharedStringTable.ElementAt(sharedStringIndex).InnerText, cellInnerText, numberFormatId, cell.DataType?.InnerText);
        }

        return new ExcelCellDebug(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, cell.DataType?.InnerText);
    }
}
