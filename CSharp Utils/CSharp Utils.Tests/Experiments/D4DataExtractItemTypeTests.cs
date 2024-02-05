using CSharp_Utils.Helpers;
using CSharp_Utils.Tests.Entities.D4Companion;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CSharp_Utils.Tests.Experiments
{
    [TestFixture]
    internal class D4DataExtractItemTypeTests
    {
        protected bool Output { get; set; } = false;
        private List<D4ItemType> ItemTypes { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ItemTypes = [];
        }

        [Test]
        public void Test_0_Extract()
        {
            foreach (string fileName in Directory.GetFiles("Ressources/d4data/ItemType", "*.json"))
            {
                if (File.Exists(fileName))
                {
                    ItemTypes.Add(new D4ItemType(JsonHelpers<D4DataItemType>.Load(fileName)));
                }
            }

            if (Output) JsonHelpers<List<D4ItemType>>.Save("Ressources/d4data/ItemTypes.json", ItemTypes, new JsonSerializerOptions() { WriteIndented = true });
            Assert.That(ItemTypes, Is.Not.Empty);
        }
    }
}
