using CSharp_Utils.Excel.Entities;
using CSharp_Utils.Excel;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using CSharp_Utils.Excel.EqualityComparers;

namespace CSharp_Utils.Tests.Excel;

internal class ExcelDataReaderXlsxReaderTests
{
    private List<string> expectedHeaders;
    private List<ExcelRow> expectedRows;

    [OneTimeSetUp]
    public void AA_OneTimeSetUp()
    {
        expectedHeaders = [
            "Empty",
            "General Text",
            "General Number",
            "Number",
            "Long Number",
            "Currency",
            "Accounting",
            "DateShort",
            "DateTime",
            "Time",
            "Percentage",
            "Fraction",
            "Scientific",
            "Text"];

        expectedRows = [
            new ExcelRow {
                Columns =[
                    new ExcelCellDebug("A2", null, string.Empty),
                    new ExcelCellDebug("B2", typeof(string), "Lorem"),
                    new ExcelCellDebug("C2", typeof(int), 1),
                    new ExcelCellDebug("D2", typeof(int), 123456789),
                    new ExcelCellDebug("E2", typeof(string), "123456789.123456789"),
                    new ExcelCellDebug("F2", typeof(double), 123456789.123456),
                    new ExcelCellDebug("G2", typeof(double), 123456789.123456),
                    new ExcelCellDebug("H2", typeof(DateTime), DateTime.FromOADate(1)),
                    new ExcelCellDebug("I2", typeof(DateTime), DateTime.FromOADate(1)),
                    new ExcelCellDebug("J2", typeof(TimeSpan), TimeSpan.FromDays(12)),
                    new ExcelCellDebug("K2", typeof(double), 0.5),
                    new ExcelCellDebug("L2", typeof(double), 0.5),
                    new ExcelCellDebug("M2", typeof(double), 123456789.123456),
                    new ExcelCellDebug("N2", typeof(string), "Lorem"),
                ],
                RowIndex = 1
            }];
    }

    [TestCase("Excel/Ressources/TestXlsxOpenXmlReader.xlsx")]
    public void Test(string path)
    {
        var (headers, rows) = ExcelDataReaderXlsxReader.ReadExcelFile(path, true);

        Assert.Multiple(() =>
        {
            Assert.That(headers, Has.Count.EqualTo(expectedHeaders.Count));
            for (int i = 0; i < headers.Count; i++)
            {
                Assert.That(headers[i], Is.EqualTo(expectedHeaders[i]), $"{headers[i]} != {expectedHeaders[i]}");
            }

            Assert.That(rows, Has.Count.EqualTo(expectedRows.Count));

            var comparer = new ExcelCellDebugEqualityComparer();
            Assert.That(rows[0].Columns, Has.Count.EqualTo(expectedRows[0].Columns.Count));
            for (int i = 0; i < rows[0].Columns.Count; i++)
            {
                Assert.That(rows[0].Columns[i], Is.EqualTo(expectedRows[0].Columns[i]).Using<ExcelCellDebug>(comparer), rows[0].Columns[i].ToString());
            }
        });
    }
}
