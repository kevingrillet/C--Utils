using System.Drawing;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace CSharp_Utils.Helpers
{
    public enum CustomColorConverterMode
    {
        Object = 0,
        RGBA = 1,
        RGB = 2,
        HTML = 3,
    }

    /// <summary>
    /// Represents a custom JSON converter for the Color class that converts RGBA color values to and from JSON strings.
    /// </summary>
    /// <remarks>
    /// This converter supports three different formats for representing RGBA colors in JSON:
    /// <list type="bullet">
    /// <item>Native object structure.</item>
    /// <item>"rgba(255,0,0,1)": Represents an RGBA color using the red, green, blue, and alpha components as integers ranging from 0 to 255.</item>
    /// <item>"255:0:0": Represents an RGBA color using the red, green, and blue components as integers ranging from 0 to 255, with an alpha value of 255 (fully opaque).</item>
    /// <item>"#FF0000": Represents an RGBA color using a hexadecimal string in the format "#RRGGBB", where RR, GG, and BB are two-digit hexadecimal values representing the red, green, and blue components respectively.</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// The following example demonstrates how to use the CustomColorConverter to serialize and deserialize Color objects to and from JSON:
    /// <code>
    /// // Create a JsonSerializerOptions object and configure it to use the CustomColorConverter
    /// var options = new JsonSerializerOptions();
    /// options.Converters.Add(new CustomColorConverter());
    ///
    /// // Serialize a Color object to JSON
    /// Color color = Color.FromArgb(255, 0, 0);
    /// string json = JsonSerializer.Serialize(color, options);
    ///
    /// // Deserialize the JSON back to a Color object
    /// Color deserializedColor = JsonSerializer.Deserialize<![CDATA[<Color>]]>(json, options);
    /// </code>
    /// </example>
    public class CustomColorConverter : JsonConverter<Color>
    {
        private static readonly char[] separator = [' ', '#', '(', ')', ',', ':'];

        public virtual CustomColorConverterMode Mode { get; set; } = CustomColorConverterMode.RGBA;

        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.StartObject => ParseJsonDoc(JsonDocument.ParseValue(ref reader)),
                JsonTokenType.String => ParseString(reader.GetString() ?? string.Empty),
                _ => throw new JsonException("Unable to parse color."),
            };
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteRawValue(FormatString(value));
        }

        protected virtual string FormatHtmlString(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        protected virtual string FormatObjectString(Color color)
        {
            return JsonSerializer.Serialize(color);
        }

        protected virtual string FormatRgbaString(Color color)
        {
            return $"rgba({color.R},{color.G},{color.B},{color.A / 255f})";
        }

        protected virtual string FormatRgbString(Color color)
        {
            return $"{color.R}:{color.G}:{color.B}";
        }

        protected virtual string FormatString(Color color)
        {
            return Mode switch
            {
                CustomColorConverterMode.HTML => string.Format("\"{0}\"", FormatHtmlString(color)),
                CustomColorConverterMode.Object => FormatObjectString(color),
                CustomColorConverterMode.RGB => string.Format("\"{0}\"", FormatRgbString(color)),
                CustomColorConverterMode.RGBA => string.Format("\"{0}\"", FormatRgbaString(color)),
                _ => null,
            };
        }

        protected virtual Color ParseJsonDoc(JsonDocument doc)
        {
            var root = doc.RootElement;

            int r = root.TryGetProperty("R", out var rValue) ? rValue.GetInt32() : 0;
            int g = root.TryGetProperty("G", out var gValue) ? gValue.GetInt32() : 0;
            int b = root.TryGetProperty("B", out var bValue) ? bValue.GetInt32() : 0;
            int a = root.TryGetProperty("A", out var aValue) ? aValue.GetInt32() : 0;

            return Color.FromArgb(a, r, g, b);
        }

        protected virtual Color ParseString(string str)
        {
            string[] components = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            // "rgba(255,0,0,1)"
            if (components.Length == 5 && components[0].Equals("rgba", StringComparison.OrdinalIgnoreCase))
            {
                int r = int.Parse(components[1]);
                int g = int.Parse(components[2]);
                int b = int.Parse(components[3]);
                float a = float.Parse(components[4], CultureInfo.InvariantCulture);

                return Color.FromArgb((int)(a * 255), r, g, b);
            }
            // "255:0:0"
            else if (components.Length == 3)
            {
                int r = int.Parse(components[0]);
                int g = int.Parse(components[1]);
                int b = int.Parse(components[2]);

                return Color.FromArgb(255, r, g, b);
            }
            // #F00 or #FF0000
            else if (components.Length == 1 && str[0] == '#' && (components[0].Length == 3 || components[0].Length == 6))
            {
                return ColorTranslator.FromHtml(str);
            }

            try
            {
                // Red
                return ColorTranslator.FromHtml(str);
            }
            catch
            {
                throw new JsonException("Invalid RGBA string format");
            }
        }
    }
}
