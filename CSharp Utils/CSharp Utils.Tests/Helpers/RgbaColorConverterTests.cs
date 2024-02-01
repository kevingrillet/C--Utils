using CSharp_Utils.Helpers;
using NUnit.Framework;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace CSharp_Utils.Tests.Helpers
{
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
        public void Test_RGBColorToString(string color, string rgba)
        {
            Assert.Multiple(() =>
            {
                Assert.That(FormatRgbaString(Color.FromName(color)), Is.EqualTo(rgba));
                Assert.That(ParseRgbaString(rgba).ToArgb(), Is.EqualTo(Color.FromName(color).ToArgb()));
            });
        }

        [Test]
        public void Test_LoadJson()
        {
            string path = "Ressources/config_rgba.json";
            Config result = JsonSerializer.Deserialize<Config>(File.ReadAllText(path), _serializeOptions);

            Assert.Multiple(() =>
            {
                Assert.That(result.ColorJson.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(result.ColorOther.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(result.ColorRGBA.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            });
        }
    }

    public class Config
    {
        public Color ColorJson { get; set; }
        public Color ColorOther { get; set; }
        public Color ColorRGBA { get; set; }
    }
}