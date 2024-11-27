using CSharp_Utils.Csv;
using NUnit.Framework;
using System;

namespace CSharp_Utils.Tests.Csv;

[TestFixture, NonParallelizable]
internal class VanillaCsvReaderHelperTest
{
    [TestCase("Csv/Ressources/personnes_import_10k.csv")]
    [TestCase("Csv/Ressources/personnes_import_100k.csv")]
    public void MeasureExecutionTime(string filePath)
    {
        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var (headers, rows) = VanillaCsvReaderHelper.ReadCsv(filePath, ';');
        stopwatch.Stop();

        // Print results
        Console.WriteLine($"Execution Time Without CsvHelper: {stopwatch.Elapsed.TotalSeconds} seconds");
        Console.WriteLine($"Headers Read: {headers.Count}");
        Console.WriteLine($"Rows Read: {rows.Count}");

        Assert.Pass();
    }
}
