using CSharp_Utils.Excel;
using CSharp_Utils.Excel.Entities;
using CSharp_Utils.Excel.EqualityComparers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp_Utils.Tests.Excel;

[TestFixture, NonParallelizable]
internal class ExcelDataReaderXlsxReaderTests
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
                Columns =[
                    new ExcelCellDebug("A2", null, string.Empty, "", null, null) {ColIndex = 0},
                    new ExcelCellDebug("B2", typeof(string), "Lorem", "Lorem", null, "String") {ColIndex = 1},
                    new ExcelCellDebug("C2", typeof(int), 1, "1", null, null) {ColIndex = 2},
                    new ExcelCellDebug("D2", typeof(int), 123456789, "123456789", null, null) {ColIndex = 3},
                    new ExcelCellDebug("E2", typeof(string), "123456789.123456789", "123456789.123456789", null, "String") {ColIndex = 4},
                    new ExcelCellDebug("F2", typeof(double), 123456789.123456, "123456789.123456", null, "Double") {ColIndex = 5},
                    new ExcelCellDebug("G2", typeof(double), 123456789.123456, "123456789.123456", null, "Double") {ColIndex = 6},
                    new ExcelCellDebug("H2", typeof(DateTime), DateTime.FromOADate(1), "", null, "DateTime") {ColIndex = 7},
                    new ExcelCellDebug("I2", typeof(DateTime), DateTime.FromOADate(1), "", null, "DateTime") {ColIndex = 8},
                    new ExcelCellDebug("J2", typeof(TimeSpan), TimeSpan.FromDays(12), "", null, null) {ColIndex = 9},
                    new ExcelCellDebug("K2", typeof(double), 0.5, "0.5", null, "Double") {ColIndex = 10},
                    new ExcelCellDebug("L2", typeof(double), 0.5, "0.5", null, "Double") {ColIndex = 11},
                    new ExcelCellDebug("M2", typeof(double), 123456789.123456, "123456789.123456", null, "Double") {ColIndex = 12},
                    new ExcelCellDebug("N2", typeof(string), "Lorem", "Lorem", null, "String") {ColIndex = 13},
                ],
                RowIndex = 1
            }
            ];

        expectedRows = [
            new ExcelRow{
                Columns= expectedDebugRows[0].Columns.Select(c => new ExcelCell(c.ColIndex, c.Type, c.Value)).ToList(),
                RowIndex = 1
            }
            ];
    }

    [TestCase("Excel/Ressources/personnes_import_10k.xlsx", 12, 10000)]
    [TestCase("Excel/Ressources/personnes_import_100k.xlsx", 12, 100000)]
    public void MeasureExecutionTime(string filePath, int nbHeaders, int nbRows)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var (headers, rows) = ExcelDataReaderXlsxReader.ReadExcelFile(filePath);
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
        var (headers, rows) = ExcelDataReaderXlsxReader.ReadExcelFile(path);
        var (_, debugRows) = ExcelDataReaderXlsxReader.ReadExcelFile(path, true);

        Assert.Multiple(() =>
        {
            Assert.That(headers, Has.Count.EqualTo(expectedHeaders.Count));
            for (int i = 0; i < headers.Count; i++)
            {
                Assert.That(headers[i], Is.EqualTo(expectedHeaders[i]), $"{headers[i]} != {expectedHeaders[i]}");
            }

            Assert.That(rows, Has.Count.EqualTo(expectedRows.Count));
            var excelCellComparer = new ExcelCellEqualityComparer();
            Assert.That(rows[0].Columns, Has.Count.EqualTo(expectedRows[0].Columns.Count));
            for (int i = 0; i < rows[0].Columns.Count; i++)
            {
                Assert.That(rows[0].Columns[i], Is.EqualTo(expectedRows[0].Columns[i]).Using<ExcelCell>(excelCellComparer), rows[0].Columns[i].ToString());
            }

            Assert.That(debugRows, Has.Count.EqualTo(expectedDebugRows.Count));
            var debugExcelCellComparer = new ExcelCellDebugEqualityComparer();
            Assert.That(debugRows[0].Columns, Has.Count.EqualTo(expectedDebugRows[0].Columns.Count));
            for (int i = 0; i < debugRows[0].Columns.Count; i++)
            {
                Assert.That(debugRows[0].Columns[i], Is.EqualTo(expectedDebugRows[0].Columns[i]).Using<ExcelCellDebug>(debugExcelCellComparer), debugRows[0].Columns[i].ToString());
            }
        });
    }
}
