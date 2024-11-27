using CSharp_Utils.Csv;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace CSharp_Utils.Tests.Csv;

[TestFixture, NonParallelizable]
internal class CsvTest
{
    private const string SampleCsvContent =
        "Col1,Col2,Col3\n" +
        "Value1,Value2,Value3\n" +
        "Value4,,Value6\n" +
        ",,\n";

    [Test]
    public void CsvReaderHelperWithCsvHelper_ShouldReadCorrectly()
    {
        // Arrange
        string filePath = CreateTestCsvFile();

        // Act
        var (headers, cells) = CsvReaderHelper.ReadCsv(filePath);

        Assert.Multiple(() =>
        {
            // Assert - Headers
            Assert.That(headers, Is.EqualTo(new List<string> { "Col1", "Col2", "Col3" }), "Headers mismatch");

            // Assert - Cells count
            Assert.That(cells, Has.Count.EqualTo(5), "Incorrect number of cells read");

            // Assert - Specific cells
            var expectedCells = new List<(int Row, int Col, string Value)>
            {
                (1, 0, "Value1"), (1, 1, "Value2"), (1, 2, "Value3"),
                (2, 0, "Value4"), (2, 2, "Value6")
            };

            foreach (var (row, col, value) in expectedCells)
            {
                Assert.That(cells.Exists(c => c.RowIndex == row && c.ColIndex == col && c.Value == value),
                    Is.True, $"Missing cell at Row {row}, Col {col} with value '{value}'");
            }
        });
    }

    [Test]
    public void CsvReaderHelperWithoutCsvHelper_ShouldReadCorrectly()
    {
        // Arrange
        string filePath = CreateTestCsvFile();

        // Act
        var (headers, cells) = VanillaCsvReaderHelper.ReadCsv(filePath);

        Assert.Multiple(() =>
        {
            // Assert - Headers
            Assert.That(headers, Is.EqualTo(new List<string> { "Col1", "Col2", "Col3" }), "Headers mismatch");

            // Assert - Cells count
            Assert.That(cells, Has.Count.EqualTo(5), "Incorrect number of cells read");

            // Assert - Specific cells
            var expectedCells = new List<(int Row, int Col, string Value)>
            {
                (1, 0, "Value1"), (1, 1, "Value2"), (1, 2, "Value3"),
                (2, 0, "Value4"), (2, 2, "Value6")
            };

            foreach (var (row, col, value) in expectedCells)
            {
                Assert.That(cells.Exists(c => c.RowIndex == row && c.ColIndex == col && c.Value == value),
                    Is.True, $"Missing cell at Row {row}, Col {col} with value '{value}'");
            }
        });
    }

    private static string CreateTestCsvFile()
    {
        string filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, SampleCsvContent);
        return filePath;
    }
}
