using CSharp_Utils.Excel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp_Utils.Excel.EqualityComparers;

public class ExcelRowEqualityComparer : EqualityComparer<ExcelRow>
{
    private readonly IEqualityComparer<ExcelCell> _cellComparer;

    public ExcelRowEqualityComparer(IEqualityComparer<ExcelCell> cellComparer)
    {
        _cellComparer = cellComparer;
    }

    public override bool Equals(ExcelRow x, ExcelRow y)
    {
        if (x == null || y == null)
            return x == y;

        return x.RowIndex == y.RowIndex &&
               x.Columns.SequenceEqual(y.Columns, _cellComparer);
    }

    public override int GetHashCode(ExcelRow obj)
    {
        if (obj == null)
            return 0;

        return HashCode.Combine(obj.RowIndex, obj.Columns);
    }
}
