using NUnit.Framework;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using System.IO;

namespace CSharp_Utils.Tests.Word;

[TestFixture, Parallelizable]
internal class SyncfusionDocxComparerTests
{
    [TestCase("document1", "document2", false)]
    [TestCase("document1", "document3", true)]
    public void TestCompare(string document1, string document2, bool expected)
    {
        var doc1 = new FileStream(Path.GetFullPath($@"Word/Ressources/{document1}.docx"), FileMode.Open);
        var word1 = new WordDocument(doc1, FormatType.Docx);
        doc1.Close();
        var doc2 = new FileStream(Path.GetFullPath($@"Word/Ressources/{document2}.docx"), FileMode.Open);
        var word2 = new WordDocument(doc2, FormatType.Docx);
        doc2.Close();

        word1.TrackChanges = true;
        word1.Compare(word2);

        Assert.That(word1.HasChanges, Is.EqualTo(expected));
    }
}
