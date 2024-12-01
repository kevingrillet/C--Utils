using CSharp_Utils.Excel;
using CSharp_Utils.Excel.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CSharp_Utils.Tests.Excel;

[TestFixture, NonParallelizable]
internal class OpenXmlXlsxReaderTests
{
    private List<ExcelCellDebug> expectedCells;

    [OneTimeSetUp]
    public void AA_OneTimeSetUp()
    {
        expectedCells =
        [
            // Row 1 (Headers)
            new ExcelCellDebug("A1", typeof(string), "Empty"),
            new ExcelCellDebug("B1", typeof(string), "General Text"),
            new ExcelCellDebug("C1", typeof(string), "General Number"),
            new ExcelCellDebug("D1", typeof(string), "Number"),
            new ExcelCellDebug("E1", typeof(string), "Long Number"),
            new ExcelCellDebug("F1", typeof(string), "Currency"),
            new ExcelCellDebug("G1", typeof(string), "Accounting"),
            new ExcelCellDebug("H1", typeof(string), "DateShort"),
            new ExcelCellDebug("I1", typeof(string), "DateTime"),
            new ExcelCellDebug("J1", typeof(string), "Time"),
            new ExcelCellDebug("K1", typeof(string), "Percentage"),
            new ExcelCellDebug("L1", typeof(string), "Fraction"),
            new ExcelCellDebug("M1", typeof(string), "Scientific"),
            new ExcelCellDebug("N1", typeof(string), "Text"),

            // Row 2
            new ExcelCellDebug("A2", typeof(string), string.Empty),
            new ExcelCellDebug("B2", typeof(string), "Lorem"),
            new ExcelCellDebug("C2", typeof(int), 1),
            new ExcelCellDebug("D2", typeof(int), 123456789),
            new ExcelCellDebug("E2", typeof(string), "123456789.123456789"), // Double trop long géré comme string par Excel??
            new ExcelCellDebug("F2", typeof(double), 123456789.123456),
            new ExcelCellDebug("G2", typeof(double), 123456789.123456),
            new ExcelCellDebug("H2", typeof(DateTime), DateTime.FromOADate(1)),
            new ExcelCellDebug("I2", typeof(DateTime), DateTime.FromOADate(1)), // Custom format?
            new ExcelCellDebug("J2", typeof(TimeSpan), TimeSpan.FromDays(12)), // Custom format?
            new ExcelCellDebug("K2", typeof(double), 0.5),
            new ExcelCellDebug("L2", typeof(double), 0.5),
            new ExcelCellDebug("M2", typeof(double), 123456789.123456),
            new ExcelCellDebug("N2", typeof(string), "Lorem"),
        ];
    }

    [TestCase("Excel/Ressources/personnes_import_10k.xlsx", 12, 10000)]
    [TestCase("Excel/Ressources/personnes_import_100k.xlsx", 12, 100000)]
    public void MeasureExecutionTime(string filePath, int nbHeaders, int nbRows)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var cells = OpenXmlXlsxReader.ReadCells(filePath);
        stopwatch.Stop();

        Console.WriteLine($"Execution time with CsvHelper: {stopwatch.Elapsed.TotalSeconds} seconds");
        Assert.Multiple(() =>
        {
            Assert.That(cells, Has.Count.EqualTo(nbHeaders * (nbRows + 1)), $"Cells read: {cells.Count}");
        });
    }

    [TestCase("Excel/Ressources/XlsxReader.xlsx")]
    public void Test(string path)
    {
        var cells = OpenXmlXlsxReader.ReadCells(path);

        Assert.Multiple(() =>
        {
            Assert.That(cells, Has.Count.EqualTo(expectedCells.Count));
            for (int i = 0; i < expectedCells.Count; i++)
            {
                Assert.That(cells[i].CellReference, Is.EqualTo(expectedCells[i].CellReference), $@"CellReference of {i} -> {cells[i]}");
                Assert.That(cells[i].Value, Is.EqualTo(expectedCells[i].Value), $@"Value of {i} -> {cells[i]}");
                Assert.That(cells[i].Type, Is.EqualTo(expectedCells[i].Type), $@"Type of {i} -> {cells[i]}");
            }
        });
    }
}
