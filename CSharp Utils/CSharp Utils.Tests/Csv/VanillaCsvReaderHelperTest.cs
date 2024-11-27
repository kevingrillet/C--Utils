using CSharp_Utils.Csv;
using NUnit.Framework;
using System;

namespace CSharp_Utils.Tests.Csv;

[TestFixture, NonParallelizable]
internal class VanillaCsvReaderHelperTest
{
    [Test]
    public void MeasureExecutionTime()
    {
        string filePath = "Csv/Ressources/personnes_import.csv";

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var (headers, data) = VanillaCsvReaderHelper.ReadCsv(filePath, ';');
        stopwatch.Stop();

        // Print results
        Console.WriteLine($"Execution Time Without CsvHelper: {stopwatch.Elapsed.TotalSeconds} seconds");
        Console.WriteLine($"Headers Read: {headers.Count}");
        Console.WriteLine($"Rows Read: {data.Count}");

        Assert.Pass();
    }
}
