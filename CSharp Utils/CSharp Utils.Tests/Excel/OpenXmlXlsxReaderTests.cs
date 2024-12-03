using CSharp_Utils.Excel;
using CSharp_Utils.Excel.Entities;
using CSharp_Utils.Excel.EqualityComparers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp_Utils.Tests.Excel;

[TestFixture, NonParallelizable]
internal class OpenXmlXlsxReaderTests
{
    private List<ExcelRow> expectedDebugRows;
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

        expectedDebugRows = [
            new ExcelRow {
                RowIndex = 1,
                Columns =[
                    new ExcelCellDebug("B2", typeof(string), "Lorem"),
                    new ExcelCellDebug("C2", typeof(double), (double)1),
                    new ExcelCellDebug("D2", typeof(double), (double)123456789),
                    new ExcelCellDebug("E2", typeof(string), "123456789.123456789"),
                    new ExcelCellDebug("F2", typeof(double), 123456789.123456),
                    new ExcelCellDebug("G2", typeof(double), 123456789.123456),
                    new ExcelCellDebug("H2", typeof(DateTime), DateTime.FromOADate(2)),
                    new ExcelCellDebug("I2", typeof(DateTime), DateTime.FromOADate(2.5)),
                    new ExcelCellDebug("J2", typeof(TimeSpan), TimeSpan.FromHours(12)),
                    new ExcelCellDebug("K2", typeof(double), 0.5),
                    new ExcelCellDebug("L2", typeof(double), 0.5),
                    new ExcelCellDebug("M2", typeof(double), 123456789.123456),
                    new ExcelCellDebug("N2", typeof(string), "Lorem"),
                ]
            }];

        expectedRows = [
            new ExcelRow{
                Columns= expectedDebugRows[0].Columns.Select(c => new ExcelCell(c.ColIndex, c.Type, c.Value)).ToList(),
                RowIndex = 1
            }];
    }

    [TestCase("Excel/Ressources/personnes_import_10k.xlsx", 12, 10000)]
    [TestCase("Excel/Ressources/personnes_import_100k.xlsx", 12, 100000)]
    public void MeasureExecutionTime(string filePath, int nbHeaders, int nbRows)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var (headers, rows) = OpenXmlXlsxReader.ReadExcelFile(filePath);
        stopwatch.Stop();

        Console.WriteLine($"Execution time with CsvHelper: {stopwatch.Elapsed.TotalSeconds} seconds");
        Assert.Multiple(() =>
        {
            Assert.That(headers, Has.Count.EqualTo(nbHeaders), $"Headers read: {headers.Count}");
            Assert.That(rows, Has.Count.EqualTo(nbRows), $"Rows read: {rows.Count}");
        });
    }

    [TestCase("Excel/Ressources/XlsxReader.xlsx")]
    public void Test(string path)
    {
        var (headers, rows) = OpenXmlXlsxReader.ReadExcelFile(path);
        var (_, debugRows) = OpenXmlXlsxReader.ReadExcelFile(path, true);

        Assert.Multiple(() =>
        {
            Assert.That(headers, Has.Count.EqualTo(expectedHeaders.Count));
            for (int i = 0; i < headers.Count; i++)
            {
                Assert.That(headers[i], Is.EqualTo(expectedHeaders[i]), $"{headers[i]} != {expectedHeaders[i]}");
            }

            Assert.That(rows, Has.Count.EqualTo(expectedRows.Count));
            var excelCellComparer = new ExcelCellEqualityComparer();
            Assert.That(rows[0].Columns, Has.Count.EqualTo(expectedRows[0].Columns.Count),
                $"More: {string.Join(",", rows[0].Columns.Where(c => !expectedRows[0].Columns.Exists(cc => cc.ColIndex == c.ColIndex)))}\n" +
                $"Missing: {string.Join(",", expectedRows[0].Columns.Where(c => !rows[0].Columns.Exists(cc => cc.ColIndex == c.ColIndex)))}");
            for (int i = 0; i < rows[0].Columns.Count; i++)
            {
                var col = rows[0].Columns[i];
                var expectedCol = expectedRows[0].Columns.Find(c => c.ColIndex == col.ColIndex);
                Assert.That(col, Is.EqualTo(expectedCol).Using<ExcelCell>(excelCellComparer), col.ToString());
            }

            Assert.That(debugRows, Has.Count.EqualTo(expectedDebugRows.Count));
            var debugExcelCellComparer = new ExcelCellDebugEqualityComparer();
            Assert.That(debugRows[0].Columns, Has.Count.EqualTo(expectedDebugRows[0].Columns.Count),
                $"More: {string.Join(",", debugRows[0].Columns.Where(c => !expectedDebugRows[0].Columns.Exists(cc => cc.ColIndex == c.ColIndex)))}\n" +
                $"Missing: {string.Join(",", expectedDebugRows[0].Columns.Where(c => !debugRows[0].Columns.Exists(cc => cc.ColIndex == c.ColIndex)))}");
            for (int i = 0; i < debugRows[0].Columns.Count; i++)
            {
                var col = debugRows[0].Columns[i];
                var expectedCol = expectedDebugRows[0].Columns.Find(c => c.ColIndex == col.ColIndex);
                Assert.That(col, Is.EqualTo(expectedCol).Using<ExcelCellDebug>(debugExcelCellComparer), col.ToString());
            }
        });
    }
}
