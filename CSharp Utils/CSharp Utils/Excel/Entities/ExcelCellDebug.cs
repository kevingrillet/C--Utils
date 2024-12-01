using System;

namespace CSharp_Utils.Excel.Entities;

public class ExcelCellDebug : ExcelCell
{
    public string CellReference { get; set; }
    public string DataType { get; set; }
    public string InnerText { get; set; }
    public uint? NumberFormat { get; set; }

    /// <summary>
    /// Constructeur de la classe ExcelCellDebug.
    /// </summary>
    /// <param name="cellReference">La référence de la cellule.</param>
    /// <param name="valueType">Le type de la valeur.</param>
    /// <param name="value">La valeur de la cellule.</param>
    /// <param name="innerText">Le texte interne de la cellule.</param>
    /// <param name="numberFormat">Le format numérique de la cellule.</param>
    /// <param name="dataType">Le type de données de la cellule.</param>
    public ExcelCellDebug(string cellReference, Type valueType, object value, string innerText = null, uint? numberFormat = null, string dataType = null)
    {
        CellReference = cellReference;
        Type = valueType;
        Value = value;
        InnerText = innerText;
        NumberFormat = numberFormat;
        DataType = dataType;
    }

    /// <summary>
    /// Retourne une chaîne représentant l'objet ExcelCellDebug.
    /// </summary>
    /// <returns>Une chaîne représentant l'objet ExcelCellDebug.</returns>
    public override string ToString()
    {
        return $"{CellReference} ({ColIndex}): {Value?.ToString() ?? string.Empty} ({Type.Name}), InnerText: {InnerText}, NumberFormat: {NumberFormat}, DataType: {DataType}";
    }
}
