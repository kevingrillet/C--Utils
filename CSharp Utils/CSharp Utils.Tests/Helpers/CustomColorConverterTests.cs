using CSharp_Utils.Helpers;
using NUnit.Framework;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace CSharp_Utils.Tests.Helpers
{
    public class Config
    {
        public Color? Color { get; set; }
        public Color? ColorCS { get; set; }
        public Color? ColorHTML { get; set; }
        public Color? ColorJson { get; set; }
        public Color? ColorRGB { get; set; }
        public Color? ColorRGBA { get; set; }
    }

    public class CustomColorConverterTests : CustomColorConverter
    {
        private JsonSerializerOptions _serializeOptions;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _serializeOptions = new JsonSerializerOptions();
            _serializeOptions.Converters.Add(new CustomColorConverter());
        }

        [SetUp]
        public void SetUp()
        {
            Mode = CustomColorConverterMode.RGBA;
        }

        [TestCase("Red")]
        [TestCase("Lime")]
        [TestCase("Blue")]
        public void Test_ColorToString(string color)
        {
            Assert.That(ParseString(color).ToArgb(), Is.EqualTo(Color.FromName(color).ToArgb()));
        }

        [TestCase("Red", "#F00", "#FF0000")]
        [TestCase("Red", null, "#FF0000")]
        [TestCase("Lime", "#0F0", "#00FF00")]
        [TestCase("Lime", null, "#00FF00")]
        [TestCase("Blue", "#00F", "#0000FF")]
        [TestCase("Blue", null, "#0000FF")]
        public void Test_ColorToStringHtml(string color, string shtml, string html)
        {
            Assert.Multiple(() =>
            {
                Assert.That(FormatHtmlString(Color.FromName(color)), Is.EqualTo(html));
                Assert.That(ParseString(shtml ?? html).ToArgb(), Is.EqualTo(Color.FromName(color).ToArgb()));
            });
        }

        [TestCase("Red", "255:0:0")]
        [TestCase("Lime", "0:255:0")]
        [TestCase("Blue", "0:0:255")]
        public void Test_ColorToStringRgb(string color, string rgb)
        {
            Assert.Multiple(() =>
            {
                Assert.That(FormatRgbString(Color.FromName(color)), Is.EqualTo(rgb));
                Assert.That(ParseString(rgb).ToArgb(), Is.EqualTo(Color.FromName(color).ToArgb()));
            });
        }

        [TestCase("Red", "rgba(255,0,0,1)")]
        [TestCase("Lime", "rgba(0,255,0,1)")]
        [TestCase("Blue", "rgba(0,0,255,1)")]
        public void Test_ColorToStringRGBA(string color, string rgba)
        {
            Assert.Multiple(() =>
            {
                Assert.That(FormatRgbaString(Color.FromName(color)), Is.EqualTo(rgba));
                Assert.That(ParseString(rgba).ToArgb(), Is.EqualTo(Color.FromName(color).ToArgb()));
            });
        }

        [Test]
        public void Test_InvalidColorToString()
        {
            Assert.Throws<JsonException>(() => ParseString("Lorem"));
        }

        [Test]
        public void Test_JsonLoad()
        {
            string path = "Ressources/colors.json";
            Config result = JsonSerializer.Deserialize<Config>(File.ReadAllText(path), _serializeOptions);

            Assert.Multiple(() =>
            {
                Assert.That(result.Color, Is.Null);
                Assert.That(result.ColorCS.Value.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(result.ColorHTML.Value.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(result.ColorJson.Value.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(result.ColorRGB.Value.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(result.ColorRGBA.Value.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            });
        }

        [Test]
        public void Test_JsonSave()
        {
            string path = "Ressources/colors_save.json";
            Config expected = new() { Color = Color.Red };
            File.WriteAllText(path, JsonSerializer.Serialize(expected, _serializeOptions));
            Config result = JsonSerializer.Deserialize<Config>(File.ReadAllText(path), _serializeOptions);
            File.Delete(path);

            Assert.Multiple(() =>
            {
                Assert.That(result.Color.Value.ToArgb(), Is.EqualTo(expected.Color.Value.ToArgb()));
                Assert.That(result.ColorCS, Is.Null);
                Assert.That(result.ColorHTML, Is.Null);
                Assert.That(result.ColorJson, Is.Null);
                Assert.That(result.ColorRGB, Is.Null);
                Assert.That(result.ColorRGBA, Is.Null);
            });
        }

        [TestCase(CustomColorConverterMode.Object, "{\"R\":255,\"G\":0,\"B\":0,\"A\":255,\"IsKnownColor\":true,\"IsEmpty\":false,\"IsNamedColor\":true,\"IsSystemColor\":false,\"Name\":\"Red\"}")]
        [TestCase(CustomColorConverterMode.HTML, "\"#FF0000\"")]
        [TestCase(CustomColorConverterMode.RGB, "\"255:0:0\"")]
        [TestCase(CustomColorConverterMode.RGBA, "\"rgba(255,0,0,1)\"")]
        public void Test_OverrideMode(CustomColorConverterMode mode, string expected)
        {
            Mode = mode;
            Assert.That(FormatString(Color.FromName("Red")), Is.EqualTo(expected));
        }
    }
}
