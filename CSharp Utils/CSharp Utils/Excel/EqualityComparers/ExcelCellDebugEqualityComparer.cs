using CSharp_Utils.Excel.Entities;
using System.Collections.Generic;
using System;

namespace CSharp_Utils.Excel.EqualityComparers;

public class ExcelCellDebugEqualityComparer : EqualityComparer<ExcelCellDebug>
{
    public override bool Equals(ExcelCellDebug x, ExcelCellDebug y)
    {
        if (x == null || y == null)
            return x == y;

        return x.ColIndex == y.ColIndex &&
               x.Type == y.Type &&
               Equals(x.Value, y.Value) &&
               x.CellReference == y.CellReference &&
               x.DataType == y.DataType &&
               x.InnerText == y.InnerText &&
               x.NumberFormat == y.NumberFormat;
    }

    public override int GetHashCode(ExcelCellDebug obj)
    {
        if (obj == null)
            return 0;

        return HashCode.Combine(
            obj.ColIndex,
            obj.Type,
            obj.Value,
            obj.CellReference,
            obj.DataType,
            obj.InnerText,
            obj.NumberFormat
        );
    }
}
