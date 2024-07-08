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
        /// <summary>
        /// Format général (0).
        /// Exemple : 1234.5678
        /// </summary>
        General = 0,

        /// <summary>
        /// Format décimal (1).
        /// Exemple : 1234.57
        /// </summary>
        Decimal = 1,

        /// <summary>
        /// Format décimal avec 2 chiffres après la virgule (2).
        /// Exemple : 1234.57
        /// </summary>
        Decimal2 = 2,

        /// <summary>
        /// Format en milliers (3).
        /// Exemple : 1,234
        /// </summary>
        Thousands = 3,

        /// <summary>
        /// Format en milliers avec 2 chiffres après la virgule (4).
        /// Exemple : 1,234.57
        /// </summary>
        Thousands2 = 4,

        /// <summary>
        /// Format pourcentage (9).
        /// Exemple : 123457%
        /// </summary>
        Percentage = 9,

        /// <summary>
        /// Format pourcentage avec 2 chiffres après la virgule (10).
        /// Exemple : 123456.78%
        /// </summary>
        Percentage2 = 10,

        /// <summary>
        /// Format scientifique (11).
        /// Exemple : 1.23E+03
        /// </summary>
        Scientific = 11,

        /// <summary>
        /// Format fraction (12).
        /// Exemple : 1234 1/2
        /// </summary>
        Fraction1 = 12,

        /// <summary>
        /// Format fraction (13).
        /// Exemple : 1234 2/3
        /// </summary>
        Fraction2 = 13,

        /// <summary>
        /// Format date courte (14).
        /// Exemple : 14-Mar
        /// </summary>
        DateShort = 14,

        /// <summary>
        /// Format date longue (15).
        /// Exemple : 14-Mars
        /// </summary>
        DateLong = 15,

        /// <summary>
        /// Format date et heure longue (16).
        /// Exemple : 14-Mar-01 01:30 PM
        /// </summary>
        DateTimeLong = 16,

        /// <summary>
        /// Format heure 12 heures (17).
        /// Exemple : 1:30 PM
        /// </summary>
        Time12Hour = 17,

        /// <summary>
        /// Format heure 12 heures avec secondes (18).
        /// Exemple : 1:30:55 PM
        /// </summary>
        Time12HourSeconds = 18,

        /// <summary>
        /// Format heure 24 heures (19).
        /// Exemple : 13:30
        /// </summary>
        Time24Hour = 19,

        /// <summary>
        /// Format heure 24 heures avec secondes (20).
        /// Exemple : 13:30:55
        /// </summary>
        Time24HourSeconds = 20,

        /// <summary>
        /// Format date et heure (21).
        /// Exemple : 14-Mar-01 13:30
        /// </summary>
        DateTime = 21,

        /// <summary>
        /// Format date et heure avec secondes (22).
        /// Exemple : 14-Mar-01 13:30:55
        /// </summary>
        DateTimeSeconds = 22,

        /// <summary>
        /// Format milliers (37).
        /// Exemple : (1,234)
        /// </summary>
        Thousands3 = 37,

        /// <summary>
        /// Format milliers avec négatif (38).
        /// Exemple : (1,234)
        /// </summary>
        Thousands3Negative = 38,

        /// <summary>
        /// Format milliers avec négatif en rouge (39).
        /// Exemple : (1,234)
        /// </summary>
        Thousands3RedNegative = 39,

        /// <summary>
        /// Format milliers avec négatif en rouge entre parenthèses (40).
        /// Exemple : (1,234)
        /// </summary>
        Thousands3RedNegativeParentheses = 40,

        /// <summary>
        /// Format comptabilité (44).
        /// Exemple : (1,234)
        /// </summary>
        Accounting = 44,

        /// <summary>
        /// Format minutes et secondes (45).
        /// Exemple : 13:30
        /// </summary>
        TimeMinutesSeconds = 45,

        /// <summary>
        /// Format minutes et secondes avec dixièmes de seconde (46).
        /// Exemple : 13:30.5
        /// </summary>
        TimeMinutesSecondsTenths = 46,

        /// <summary>
        /// Format minutes et secondes avec centièmes de seconde (47).
        /// Exemple : 13:30.55
        /// </summary>
        TimeMinutesSecondsHundredths = 47,

        /// <summary>
        /// Format scientifique (48).
        /// Exemple : 1.23E+03
        /// </summary>
        Scientific2 = 48,

        /// <summary>
        /// Format texte (49).
        /// Exemple : 1234
        /// </summary>
        Text = 49
    }

    /// <summary>
    /// Cette classe fournit des méthodes pour lire les cellules à partir de fichiers Excel (xlsx) en utilisant OpenXML.
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
            List<CellValue> cellValues = [];

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
                    int maxColumnIndex = row.Descendants<Cell>().Select(c => GetColumnIndexFromCellReference(c.CellReference)).Max();

                    for (int colIndex = 0; colIndex <= maxColumnIndex; colIndex++)
                    {
                        Cell cell = row.Elements<Cell>().FirstOrDefault(c => GetColumnIndexFromCellReference(c.CellReference) == colIndex);
                        cell ??= new Cell() { CellReference = GetCellReferenceFromIndexes(row.RowIndex, colIndex) };
                        cellValues.Add(GetCellValue(cell, stringTablePart, cellFormats));
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
        /// <returns>Un objet CellValue représentant la valeur de la cellule formatée.</returns>
        private static CellValue DetermineCustomNumberFormatType(string cellReference, string cellInnerText, uint? numberFormatId, string dataType)
        {
            // Implémentez ici la logique pour déterminer le type attendu pour les formats numériques personnalisés
            // Par exemple, vous pouvez mapper certains IDs de format à des types spécifiques

            if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double customValue))
            {
                if (IsWholeNumber(customValue))
                {
                    return new CellValue(cellReference, typeof(int), (int)customValue, cellInnerText, numberFormatId, dataType);
                }
                else
                {
                    return new CellValue(cellReference, typeof(double), customValue, cellInnerText, numberFormatId, dataType);
                }
            }

            return new CellValue(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, dataType);
        }

        /// <summary>
        /// Formatte la valeur d'une cellule en fonction de son format numérique.
        /// </summary>
        /// <param name="cellReference">La référence de la cellule.</param>
        /// <param name="cellInnerText">Le texte interne de la cellule.</param>
        /// <param name="numberFormat">Le format numérique de la cellule.</param>
        /// <param name="numberFormatId">L'ID du format numérique de la cellule.</param>
        /// <param name="dataType">Le type de données de la cellule.</param>
        /// <returns>Un objet CellValue représentant la valeur de la cellule formatée.</returns>
        private static CellValue FormatCellValue(string cellReference, string cellInnerText, NumberFormat numberFormat, uint? numberFormatId, string dataType)
        {
            switch (numberFormat)
            {
                case NumberFormat.General:
                    return new CellValue(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, dataType);

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
                        if (IsWholeNumber(numberValue))
                        {
                            return new CellValue(cellReference, typeof(int), (int)numberValue, cellInnerText, numberFormatId, dataType);
                        }
                        else
                        {
                            return new CellValue(cellReference, typeof(double), numberValue, cellInnerText, numberFormatId, dataType);
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
                        return new CellValue(cellReference, typeof(DateTime), dateValue, cellInnerText, numberFormatId, dataType);
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
                        return new CellValue(cellReference, typeof(TimeSpan), timeValue, cellInnerText, numberFormatId, dataType);
                    }
                    break;

                case NumberFormat.Fraction1:
                case NumberFormat.Fraction2:
                    if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double fractionValue))
                    {
                        return new CellValue(cellReference, typeof(double), fractionValue, cellInnerText, numberFormatId, dataType);
                    }
                    break;

                case NumberFormat.Text:
                    return new CellValue(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, dataType);

                default:
                    return new CellValue(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, dataType);
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

            if (cell.DataType.Value == CellValues.SharedString)
            {
                if (stringTablePart != null && int.TryParse(cellInnerText, out int index))
                {
                    return new CellValue(cellReference, typeof(string), stringTablePart.SharedStringTable.ElementAt(index).InnerText, cellInnerText, null, cell.DataType?.InnerText);
                }
            }
            else if (cell.DataType.Value == CellValues.Boolean)
            {
                return new CellValue(cellReference, typeof(bool), cellInnerText == "1", cellInnerText, null, cell.DataType?.InnerText);
            }
            else if (cell.DataType.Value == CellValues.Date)
            {
                if (DateTime.TryParse(cellInnerText, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                {
                    return new CellValue(cellReference, typeof(DateTime), dateValue.Date, cellInnerText, null, cell.DataType?.InnerText);
                }
            }
            else if (cell.DataType.Value == CellValues.Number)
            {
                if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numberValue))
                {
                    if (IsWholeNumber(numberValue))
                    {
                        return new CellValue(cellReference, typeof(int), (int)numberValue, cellInnerText, null, cell.DataType?.InnerText);
                    }
                    else
                    {
                        return new CellValue(cellReference, typeof(double), numberValue, cellInnerText, null, cell.DataType?.InnerText);
                    }
                }
            }
            else if (cell.DataType.Value == CellValues.Error)
            {
                return new CellValue(cellReference, typeof(string), "ERROR: " + cellInnerText, cellInnerText, null, cell.DataType?.InnerText);
            }

            return new CellValue(cellReference, typeof(string), cellInnerText, cellInnerText, null, cell.DataType?.InnerText);
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
                    if (numberFormatId >= 164)
                    {
                        return DetermineCustomNumberFormatType(cellReference, cellInnerText, numberFormatId, cell.DataType?.InnerText);
                    }
                    return FormatCellValue(cellReference, cellInnerText, numberFormat, numberFormatId, cell.DataType?.InnerText);
                }
            }

            if (double.TryParse(cellInnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
            {
                if (IsWholeNumber(numericValue))
                {
                    return new CellValue(cellReference, typeof(int), (int)numericValue, cellInnerText, numberFormatId, cell.DataType?.InnerText);
                }
                else
                {
                    return new CellValue(cellReference, typeof(double), numericValue, cellInnerText, numberFormatId, cell.DataType?.InnerText);
                }
            }

            if (stringTablePart != null && cell.CellValue != null && int.TryParse(cell.CellValue.Text, out int sharedStringIndex))
            {
                return new CellValue(cellReference, typeof(string), stringTablePart.SharedStringTable.ElementAt(sharedStringIndex).InnerText, cellInnerText, numberFormatId, cell.DataType?.InnerText);
            }

            return new CellValue(cellReference, typeof(string), cellInnerText, cellInnerText, numberFormatId, cell.DataType?.InnerText);
        }

        /// <summary>
        /// Récupère l'index de la colonne à partir de la référence de la cellule.
        /// </summary>
        /// <param name="cellReference">La référence de la cellule.</param>
        /// <returns>L'index de la colonne.</returns>
        private static int GetColumnIndexFromCellReference(string cellReference)
        {
            int columnIndex = 0;
            foreach (var ch in cellReference.Where(ch => char.IsLetter(ch)))
            {
                columnIndex = (columnIndex * 26) + (ch - 'A' + 1);
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
                columnName = $"{(char)(remainder + 'A')}{columnName}";
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
        public string DataType { get; set; }
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
        public CellValue(string cellReference, Type valueType, object value, string innerText = null, uint? numberFormat = null, string dataType = null)
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
