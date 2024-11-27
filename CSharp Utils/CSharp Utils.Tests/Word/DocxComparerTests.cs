using NUnit.Framework;
using CSharp_Utils.Word;

namespace CSharp_Utils.Tests.Word;

[TestFixture, NonParallelizable]
internal class DocxComparerTests : DocxComparer
{
    private DocxComparer _docxComparer;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _docxComparer = new DocxComparer();
    }

    [TestCase("document1", "document2", true)]
    [TestCase("document1", "document3", false)]
    [TestCase("document", "document2", false)]
    [TestCase("", "document2", false)]
    [TestCase(null, "document2", false)]
    [TestCase("document1", "document", false)]
    [TestCase("document1", "", false)]
    [TestCase("document1", null, false)]
    public void TestCompare(string document1, string document2, bool expected)
    {
        Assert.That(_docxComparer.DocumentsAreEqual($@"Word/Ressources/{document1}.docx", $@"Word/Ressources/{document2}.docx"), Is.EqualTo(expected));
    }
}
