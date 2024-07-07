using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CSharp_Utils.Helpers
{
    /// <summary>
    /// Enumération représentant les formats numériques des cellules.
    /// </summary>
    public enum NumberFormat
    {
        General = 0,
        Decimal = 1,
        Decimal2 = 2,
        Thousands = 3,
        Thousands2 = 4,
        Percentage = 9,
        Percentage2 = 10,
        Scientific = 11,
        Fraction1 = 12,
        Fraction2 = 13,
        DateShort = 14,
        DateLong = 15,
        DateTimeLong = 16,
        Time12Hour = 17,
        Time12HourSeconds = 18,
        Time24Hour = 19,
        Time24HourSeconds = 20,
        DateTime = 21,
        DateTimeSeconds = 22,
        Thousands3 = 37,
        Thousands3Negative = 38,
        Thousands3RedNegative = 39,
        Thousands3RedNegativeParentheses = 40,
        TimeMinutesSeconds = 45,
        TimeMinutesSecondsTenths = 46,
        TimeMinutesSecondsHundredths = 47,
        Scientific2 = 48,
        Text = 49
    }

    /// <summary>
    /// Cette classe fournit des méthodes pour lire les cellules à partir de fichiers Excel (xlsx) en utilisant OpenXML.
    /// Auteur : OpenAI ChatGPT
    /// </summary>
    public static class XlsxOpenXmlReader
    {
        /// <summary>
        /// Lit les cellules d'un fichier Excel (xlsx) et renvoie une liste de CellValue.
        /// </summary>
        /// <param name="filePath">Le chemin du fichier Excel.</param>
        /// <param name="sheetName">Le nom de la feuille à lire. Si non spécifié, la première feuille sera utilisée.</param>
        /// <returns>Une liste de CellValue contenant les valeurs des cellules.</returns>
        public static List<CellValue> ReadCells(string filePath, string sheetName = null)
        {
            List<CellValue> cellValues = new List<CellValue>();

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
                    // Obtenez l'index de colonne maximal dans la ligne
                    int maxColumnIndex = row.Descendants<Cell>().Select(c => GetColumnIndexFromCellReference(c.CellReference)).Max();

                    for (int colIndex = 0; colIndex <= maxColumnIndex; colIndex++)
                    {
                        Cell cell = row.Elements<Cell>().FirstOrDefault(c => GetColumnIndexFromCellReference(c.CellReference) == colIndex);
                        if (cell == null)
                        {
                            // Créez une cellule vide si elle n'existe pas
                            cell = new Cell() { CellReference = GetCellReferenceFromIndexes(row.RowIndex, colIndex) };
                        }
                        cellValues.Add(GetCellValue(cell, stringTablePart, cellFormats));
                    }
                }
            }

            return cellValues;
        }

        /// <summary>
        /// Formatte la valeur d'une cellule en fonction de son format numérique.
        /// </summary>
        /// <param name="cellReference">La référence de la cellule.</param>
        /// <param name="cellInnerText">Le texte interne de la cellule.</param>
        /// <param name="numberFormat">Le format numérique de la cellule.</param>
        /// <param name="numberFormatId">L'ID du format numérique de la cellule.</param>
        /// <param name="dataType">Le type de données de la cellule.</param>
        /// <returns>Un objet CellValue représentant la valeur de la cellule.</returns>
        private static CellValue FormatCellValue(string cellReference, string cellInnerText, NumberFormat numberFormat, uint? numberFormatId, CellValues? dataType)
        {
            if (numberFormat == NumberFormat.DateShort || numberFormat == NumberFormat.DateTime)
            {
                if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double oaDate))
                {
                    DateTime dateValue = DateTime.FromOADate(oaDate);
                    if (numberFormat == NumberFormat.DateShort)
                    {
                        return new CellValue(cellReference, typeof(DateTime), dateValue.Date, cellInnerText, numberFormatId, dataType);
                    }
                    else
                    {
                        return new CellValue(cellReference, typeof(DateTime), dateValue, cellInnerText, numberFormatId, dataType);
                    }
                }
            }

            if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numberValue))
            {
                // Vérifiez si le nombre peut être converti en int
                if (IsWholeNumber(numberValue))
                {
                    return new CellValue(cellReference, typeof(int), (int)numberValue, cellInnerText, numberFormatId, dataType);
                }
                else
                {
                    return new CellValue(cellReference, typeof(double), numberValue, cellInnerText, numberFormatId, dataType);
                }
            }

            return new CellValue(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, dataType);
        }

        /// <summary>
        /// Récupère la référence de la cellule à partir des index de la ligne et de la colonne.
        /// </summary>
        /// <param name="rowIndex">L'index de la ligne.</param>
        /// <param name="columnIndex">L'index de la colonne.</param>
        /// <returns>La référence de la cellule.</returns>
        private static string GetCellReferenceFromIndexes(uint rowIndex, int columnIndex)
        {
            string columnName = GetColumnNameFromIndex(columnIndex);
            return columnName + rowIndex.ToString();
        }

        /// <summary>
        /// Récupère la valeur d'une cellule en fonction de son type de données ou de son format.
        /// </summary>
        /// <param name="cell">La cellule dont la valeur doit être récupérée.</param>
        /// <param name="stringTablePart">La partie tableau des chaînes partagées.</param>
        /// <param name="cellFormats">Les formats des cellules.</param>
        /// <returns>Un objet CellValue représentant la valeur de la cellule.</returns>
        private static CellValue GetCellValue(Cell cell, SharedStringTablePart stringTablePart, CellFormats cellFormats)
        {
            if (cell == null)
            {
                return new CellValue(null, typeof(string), null);
            }

            if (cell.DataType != null)
            {
                return GetCellValueByDataType(cell, stringTablePart);
            }
            else
            {
                return GetCellValueByFormat(cell, cellFormats, stringTablePart);
            }
        }

        /// <summary>
        /// Récupère la valeur d'une cellule en fonction de son type de données.
        /// </summary>
        /// <param name="cell">La cellule dont la valeur doit être récupérée.</param>
        /// <param name="stringTablePart">La partie tableau des chaînes partagées.</param>
        /// <returns>Un objet CellValue représentant la valeur de la cellule.</returns>
        private static CellValue GetCellValueByDataType(Cell cell, SharedStringTablePart stringTablePart)
        {
            string cellInnerText = cell.InnerText;
            string cellReference = cell.CellReference;
            CellValues? dataType = cell.DataType?.Value;

            if (cell.DataType.Value == CellValues.SharedString)
            {
                if (stringTablePart != null && int.TryParse(cellInnerText, out int index))
                {
                    return new CellValue(cellReference, typeof(string), stringTablePart.SharedStringTable.ElementAt(index).InnerText, cellInnerText, null, dataType);
                }
            }
            else if (cell.DataType.Value == CellValues.Boolean)
            {
                return new CellValue(cellReference, typeof(bool), cellInnerText == "1", cellInnerText, null, dataType);
            }
            else if (cell.DataType.Value == CellValues.Date)
            {
                if (DateTime.TryParse(cellInnerText, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                {
                    return new CellValue(cellReference, typeof(DateTime), dateValue.Date, cellInnerText, null, dataType);
                }
            }
            else if (cell.DataType.Value == CellValues.Number)
            {
                if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numberValue))
                {
                    // Vérifiez si le nombre peut être converti en int
                    if (IsWholeNumber(numberValue))
                    {
                        return new CellValue(cellReference, typeof(int), (int)numberValue, cellInnerText, null, dataType);
                    }
                    else
                    {
                        return new CellValue(cellReference, typeof(double), numberValue, cellInnerText, null, dataType);
                    }
                }
            }
            else if (cell.DataType.Value == CellValues.Error)
            {
                return new CellValue(cellReference, typeof(string), "ERROR: " + cellInnerText, cellInnerText, null, dataType);
            }

            return new CellValue(cellReference, typeof(string), cellInnerText, cellInnerText, null, dataType);
        }

        /// <summary>
        /// Récupère la valeur d'une cellule en fonction de son format.
        /// </summary>
        /// <param name="cell">La cellule dont la valeur doit être récupérée.</param>
        /// <param name="cellFormats">Les formats des cellules.</param>
        /// <param name="stringTablePart">La partie tableau des chaînes partagées.</param>
        /// <returns>Un objet CellValue représentant la valeur de la cellule.</returns>
        private static CellValue GetCellValueByFormat(Cell cell, CellFormats cellFormats, SharedStringTablePart stringTablePart)
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
                    return FormatCellValue(cellReference, cellInnerText, numberFormat, numberFormatId, cell.DataType?.Value);
                }
            }

            // Vérifiez si la cellule est une valeur numérique même si DataType est null
            if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
            {
                // Vérifiez si le nombre peut être converti en int
                if (IsWholeNumber(numericValue))
                {
                    return new CellValue(cellReference, typeof(int), (int)numericValue, cellInnerText, numberFormatId, cell.DataType?.Value);
                }
                else
                {
                    return new CellValue(cellReference, typeof(double), numericValue, cellInnerText, numberFormatId, cell.DataType?.Value);
                }
            }

            // Gestion spécifique pour les chaînes partagées lorsque DataType est null
            if (stringTablePart != null && cell.CellValue != null && int.TryParse(cell.CellValue.Text, out int sharedStringIndex))
            {
                return new CellValue(cellReference, typeof(string), stringTablePart.SharedStringTable.ElementAt(sharedStringIndex).InnerText, cellInnerText, numberFormatId, cell.DataType?.Value);
            }

            return new CellValue(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, cell.DataType?.Value);
        }

        /// <summary>
        /// Récupère l'index de la colonne à partir de la référence de la cellule.
        /// </summary>
        /// <param name="cellReference">La référence de la cellule.</param>
        /// <returns>L'index de la colonne.</returns>
        private static int GetColumnIndexFromCellReference(string cellReference)
        {
            int columnIndex = 0;
            foreach (char ch in cellReference)
            {
                if (char.IsLetter(ch))
                {
                    columnIndex = (columnIndex * 26) + (ch - 'A' + 1);
                }
            }
            return columnIndex - 1;
        }

        /// <summary>
        /// Récupère le nom de la colonne à partir de l'index de la colonne.
        /// </summary>
        /// <param name="columnIndex">L'index de la colonne.</param>
        /// <returns>Le nom de la colonne.</returns>
        private static string GetColumnNameFromIndex(int columnIndex)
        {
            string columnName = "";
            while (columnIndex >= 0)
            {
                int remainder = columnIndex % 26;
                columnName = (char)(remainder + 'A') + columnName;
                columnIndex = (columnIndex / 26) - 1;
            }
            return columnName;
        }

        /// <summary>
        /// Vérifie si un nombre est un nombre entier.
        /// </summary>
        /// <param name="number">Le nombre à vérifier.</param>
        /// <returns>Vrai si le nombre est un nombre entier, sinon faux.</returns>
        private static bool IsWholeNumber(double number)
        {
            return Math.Abs(number % 1) < double.Epsilon * 100;
        }
    }

    /// <summary>
    /// Classe représentant la valeur d'une cellule.
    /// </summary>
    public class CellValue
    {
        public string CellReference { get; set; }
        public CellValues? DataType { get; set; }
        public string InnerText { get; set; }
        public uint? NumberFormat { get; set; }
        public object Value { get; set; }
        public Type ValueType { get; set; }

        /// <summary>
        /// Constructeur de la classe CellValue.
        /// </summary>
        /// <param name="cellReference">La référence de la cellule.</param>
        /// <param name="valueType">Le type de la valeur.</param>
        /// <param name="value">La valeur de la cellule.</param>
        /// <param name="innerText">Le texte interne de la cellule.</param>
        /// <param name="numberFormat">Le format numérique de la cellule.</param>
        /// <param name="dataType">Le type de données de la cellule.</param>
        public CellValue(string cellReference, Type valueType, object value, string innerText = null, uint? numberFormat = null, CellValues? dataType = null)
        {
            CellReference = cellReference;
            ValueType = valueType;
            Value = value;
            InnerText = innerText;
            NumberFormat = numberFormat;
            DataType = dataType;
        }

        /// <summary>
        /// Retourne une chaîne représentant l'objet CellValue.
        /// </summary>
        /// <returns>Une chaîne représentant l'objet CellValue.</returns>
        public override string ToString()
        {
            return $"{CellReference}: {Value?.ToString() ?? string.Empty} ({ValueType.Name}), InnerText: {InnerText}, NumberFormat: {NumberFormat}, DataType: {DataType}";
        }
    }
}
