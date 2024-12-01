using System;
using System.Linq;

namespace CSharp_Utils.Excel.Utils;

public static class ExcelUtils
{
    /// <summary>
    /// Récupère la référence de la cellule à partir des index de la ligne et de la colonne.
    /// </summary>
    /// <param name="rowIndex">L'index de la ligne.</param>
    /// <param name="columnIndex">L'index de la colonne.</param>
    /// <returns>La référence de la cellule.</returns>
    public static string GetCellReferenceFromIndexes(uint rowIndex, int columnIndex)
    {
        string columnName = GetColumnNameFromIndex(columnIndex);
        return columnName + rowIndex.ToString();
    }

    /// <summary>
    /// Récupère la référence de la cellule à partir des index de la ligne et de la colonne.
    /// </summary>
    /// <param name="rowIndex">L'index de la ligne.</param>
    /// <param name="columnIndex">L'index de la colonne.</param>
    /// <returns>La référence de la cellule.</returns>
    public static string GetCellReferenceFromIndexes(int rowIndex, int columnIndex)
    {
        string columnName = GetColumnNameFromIndex(columnIndex);
        return columnName + rowIndex.ToString();
    }

    /// <summary>
    /// Récupère l'index de la colonne à partir de la référence de la cellule.
    /// </summary>
    /// <param name="cellReference">La référence de la cellule.</param>
    /// <returns>L'index de la colonne.</returns>
    public static int GetColumnIndexFromCellReference(string cellReference)
    {
        int columnIndex = 0;
        foreach (var ch in cellReference.Where(ch => char.IsLetter(ch)))
        {
            columnIndex = columnIndex * 26 + ch - 'A' + 1;
        }

        return columnIndex - 1;
    }

    /// <summary>
    /// Récupère le nom de la colonne à partir de l'index de la colonne.
    /// </summary>
    /// <param name="columnIndex">L'index de la colonne.</param>
    /// <returns>Le nom de la colonne.</returns>
    public static string GetColumnNameFromIndex(int columnIndex)
    {
        string columnName = "";
        while (columnIndex >= 0)
        {
            int remainder = columnIndex % 26;
            columnName = $"{(char)(remainder + 'A')}{columnName}";
            columnIndex = columnIndex / 26 - 1;
        }
        return columnName;
    }

    /// <summary>
    /// Vérifie si un nombre est un nombre entier.
    /// </summary>
    /// <param name="number">Le nombre à vérifier.</param>
    /// <returns>Vrai si le nombre est un nombre entier, sinon faux.</returns>
    public static bool IsWholeNumber(double number)
    {
        return Math.Abs(number % 1) < double.Epsilon * 100;
    }
}
