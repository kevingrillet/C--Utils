using System;

namespace CSharp_Utils.Csv.Entities;

public sealed class CsvCell : IEquatable<CsvCell>
{
    public int ColIndex { get; set; }
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    public string Value { get; set; }

    public bool Equals(CsvCell other)
    {
        if (other == null) return false;

        return ColIndex == other.ColIndex && Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as CsvCell);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ColIndex, Value);
    }

    public override string ToString()
    {
        return $"ColIndex: {ColIndex}, Value: '{Value}'";
    }
}
