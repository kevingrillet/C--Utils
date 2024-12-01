using CSharp_Utils.Excel.Entities;
using System;
using System.Collections.Generic;

namespace CSharp_Utils.Excel.EqualityComparers;

public class ExcelCellEqualityComparer : EqualityComparer<ExcelCell>
{
    public override bool Equals(ExcelCell x, ExcelCell y)
    {
        if (x == null || y == null)
            return x == y;

        return x.ColIndex == y.ColIndex &&
               x.Type == y.Type &&
               Equals(x.Value, y.Value);
    }

    public override int GetHashCode(ExcelCell obj)
    {
        if (obj == null)
            return 0;

        return HashCode.Combine(obj.ColIndex, obj.Type, obj.Value);
    }
}
