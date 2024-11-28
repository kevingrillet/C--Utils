using CSharp_Utils.Csv;
using NUnit.Framework;
using System;

namespace CSharp_Utils.Tests.Csv;

[TestFixture, NonParallelizable]
internal class CsvReaderHelperTest
{
    [TestCase("Csv/Ressources/personnes_import_10k.csv", 12, 10000)]
    [TestCase("Csv/Ressources/personnes_import_100k.csv", 12, 100000)]
    public void MeasureExecutionTime(string filePath, int nbHeaders, int nbRows)
    {
        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var (headers, rows) = CsvReaderHelper.ReadCsv(filePath, ';');
        stopwatch.Stop();

        // Print results
        Console.WriteLine($"Execution time with CsvHelper: {stopwatch.Elapsed.TotalSeconds} seconds");
        Assert.Multiple(() =>
        {
            Assert.That(headers, Has.Count.EqualTo(nbHeaders), $"Headers Read: {headers.Count}");
            Assert.That(rows, Has.Count.EqualTo(nbRows), $"Rows Read: {rows.Count}");
        });
    }
}
