using CSharp_Utils.Helpers;
using NUnit.Framework;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace CSharp_Utils.Tests.Helpers
{
    public class Config
    {
        public Color ColorCS { get; set; }
        public Color ColorHEX { get; set; }
        public Color ColorHTML { get; set; }
        public Color ColorJson { get; set; }
        public Color ColorRGBA { get; set; }
    }

    public class RgbaColorConverterTests : RgbaColorConverter
    {
        private JsonSerializerOptions _serializeOptions;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _serializeOptions = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
            };
            _serializeOptions.Converters.Add(new RgbaColorConverter());
        }

        [TestCase("Red", "rgba(255,0,0,1)")]
        [TestCase("Lime", "rgba(0,255,0,1)")]
        [TestCase("Blue", "rgba(0,0,255,1)")]
        public void Test_ColorToString(string color, string rgba)
        {
            Assert.Multiple(() =>
            {
                Assert.That(FormatRgbaString(Color.FromName(color)), Is.EqualTo(rgba));
                Assert.That(ParseString(rgba).ToArgb(), Is.EqualTo(Color.FromName(color).ToArgb()));
            });
        }

        [Test]
        public void Test_JsonLoad()
        {
            string path = "Ressources/colors.json";
            Config result = JsonSerializer.Deserialize<Config>(File.ReadAllText(path), _serializeOptions);

            Assert.Multiple(() =>
            {
                Assert.That(result.ColorCS.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(result.ColorHEX.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(result.ColorHTML.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(result.ColorJson.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(result.ColorRGBA.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            });
        }
    }
}
