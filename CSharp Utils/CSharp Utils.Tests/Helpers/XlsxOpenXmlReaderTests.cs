using CSharp_Utils.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CSharp_Utils.Tests.Helpers
{
    [TestFixture, Parallelizable]
    internal class XlsxOpenXmlReaderTests
    {
        private List<CellValue> expectedCells;

        [OneTimeSetUp]
        public void AA_OneTimeSetUp()
        {
            expectedCells =
            [
                // Row 1 (Headers)
                new CellValue("A1", typeof(string), "Empty"),
                new CellValue("B1", typeof(string), "General Text"),
                new CellValue("C1", typeof(string), "General Number"),
                new CellValue("D1", typeof(string), "Number"),
                new CellValue("E1", typeof(string), "Long Number"),
                new CellValue("F1", typeof(string), "Currency"),
                new CellValue("G1", typeof(string), "Compatibility"),
                new CellValue("H1", typeof(string), "DateShort"),
                new CellValue("I1", typeof(string), "DateTime"),
                new CellValue("J1", typeof(string), "Time"),
                new CellValue("K1", typeof(string), "Percentage"),
                new CellValue("L1", typeof(string), "Fraction"),
                new CellValue("M1", typeof(string), "Scientific"),
                new CellValue("N1", typeof(string), "Text"),

                // Row 2
                new CellValue("A2", typeof(string), string.Empty),
                new CellValue("B2", typeof(string), "Lorem"),
                new CellValue("C2", typeof(int), 1),
                new CellValue("D2", typeof(int), 123456789),
                new CellValue("E2", typeof(double), 123456789.123456789),
                new CellValue("F2", typeof(double), 123456789.123456),
                new CellValue("G2", typeof(double), 123456789.123456),
                new CellValue("H2", typeof(DateTime), DateTime.FromOADate(1)),
                new CellValue("I2", typeof(DateTime), DateTime.FromOADate(1)),
                new CellValue("J2", typeof(TimeSpan), TimeSpan.FromDays(12)),
                new CellValue("K2", typeof(double), 0.5),
                new CellValue("L2", typeof(double), 0.5),
                new CellValue("M2", typeof(double), 123456789.123456),
                new CellValue("N2", typeof(string), "Lorem"),
            ];
        }

        [TestCase("Ressources/TestXlsxOpenXmlReader.xlsx")]
        public void Test(string path)
        {
            var cells = XlsxOpenXmlReader.ReadCells(path);

            Assert.Multiple(() =>
            {
                Assert.That(cells, Has.Count.EqualTo(expectedCells.Count));
                for (int i = 0; i < expectedCells.Count; i++)
                {
                    Assert.That(cells[i].CellReference, Is.EqualTo(expectedCells[i].CellReference), $@"CellReference of {i} -> {cells[i]}");
                    Assert.That(cells[i].Value, Is.EqualTo(expectedCells[i].Value), $@"Value of {i} -> {cells[i]}");
                    Assert.That(cells[i].ValueType, Is.EqualTo(expectedCells[i].ValueType), $@"ValueType of {i} -> {cells[i]}");
                }
            });
        }
    }
}
