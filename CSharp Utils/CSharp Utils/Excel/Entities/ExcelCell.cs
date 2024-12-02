using System;

namespace CSharp_Utils.Excel.Entities;

public class ExcelCell
{
    public int ColIndex { get; set; }
    public bool IsEmpty => !(Value is null || (Type == typeof(string) && !string.IsNullOrWhiteSpace((string)Value)));
    public Type Type { get; set; }
    public object Value { get; set; }

    public ExcelCell()
    {
    }

    public ExcelCell(int colIndex, Type type, object value)
    {
        ColIndex = colIndex;
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return $"ColIndex: {ColIndex}, Type: {Type}, Value: '{Value}'";
    }
}
