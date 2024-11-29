using CSharp_Utils.Csv;
using CSharp_Utils.Csv.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSharp_Utils.Tests.Csv;

[TestFixture, NonParallelizable]
internal class VanillaCsvReaderHelperTest
{
    private const string SampleCsv =
        "Col1;Col2;Col3\n" +
        "Value1;Value2;Value3\n" +
        "Value4;;Value6\n" +
        ";;\n";

    [TestCase("Csv/Ressources/personnes_import_10k.csv", 12, 10000)]
    [TestCase("Csv/Ressources/personnes_import_100k.csv", 12, 100000)]
    public void MeasureExecutionTime(string filePath, int nbHeaders, int nbRows)
    {
        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var (headers, rows) = VanillaCsvReaderHelper.ReadCsv(filePath, ';');
        stopwatch.Stop();

        // Print results
        Console.WriteLine($"Execution time without CsvHelper: {stopwatch.Elapsed.TotalSeconds} seconds");
        Assert.Multiple(() =>
        {
            Assert.That(headers, Has.Count.EqualTo(nbHeaders), $"Headers Read: {headers.Count}");
            Assert.That(rows, Has.Count.EqualTo(nbRows), $"Rows Read: {rows.Count}");
        });
    }

    [Test]
    public void ReadCsv_ShouldReturnHeadersAndRows()
    {
        // Arrange
        string filePath = WriteTempCsv(SampleCsv);

        // Act
        var (headers, rows) = VanillaCsvReaderHelper.ReadCsv(filePath, ';');

        Assert.Multiple(() =>
        {
            // Assert - Headers
            Assert.That(headers, Is.EqualTo(new List<string> { "Col1", "Col2", "Col3" }));

            // Assert - Rows
            Assert.That(rows, Has.Count.EqualTo(2), string.Join(", ", rows));

            Assert.That(rows[0].Columns, Has.Count.EqualTo(3));
            Assert.That(rows[1].Columns, Has.Count.EqualTo(2));

            Assert.That(rows[0].Columns[0], Is.EqualTo(new CsvCell { ColIndex = 0, Value = "Value1" }), rows[0].Columns[0].ToString());
            Assert.That(rows[1].Columns[1], Is.EqualTo(new CsvCell { ColIndex = 2, Value = "Value6" }), rows[1].Columns[1].ToString());
        });
    }

    private static string WriteTempCsv(string content)
    {
        string filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, content);
        return filePath;
    }
}
