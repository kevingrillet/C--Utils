using System.Drawing;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSharp_Utils.Helpers
{
    /// <summary>
    /// Represents a JSON converter for converting between Color objects and RGBA color strings.
    /// </summary>
    /// <remarks>
    /// This converter is used to serialize and deserialize Color objects to and from RGBA color strings in JSON.
    /// It provides methods for reading and writing Color objects from and to JSON using the Utf8JsonReader and Utf8JsonWriter classes.
    /// The converter supports both object and string representations of colors.
    /// </remarks>
    /// <example>
    /// The following example demonstrates how to use the RgbaColorConverter to serialize and deserialize Color objects to and from JSON:
    /// <code>
    /// // Create a JsonSerializerOptions object and configure it to use the RgbaColorConverter
    /// var options = new JsonSerializerOptions();
    /// options.Converters.Add(new RgbaColorConverter());
    ///
    /// // Serialize a Color object to JSON
    /// Color color = Color.FromArgb(255, 0, 0, 255);
    /// string json = JsonSerializer.Serialize(color, options);
    ///
    /// // Deserialize a Color object from JSON
    /// Color deserializedColor = JsonSerializer.Deserialize<![CDATA[<Color>]]>(json, options);
    /// </code>
    /// </example>
    public class RgbaColorConverter : JsonConverter<Color>
    {
        private static readonly char[] separator = [' ', '#', '(', ')', ',', ':'];

        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    var doc = JsonDocument.ParseValue(ref reader);
                    return ParseJsonDoc(doc);

                case JsonTokenType.String:
                    string str = reader.GetString() ?? string.Empty;
                    return ParseString(str);
            }
            throw new JsonException("Unable to parse color.");
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            string rgbaString = FormatRgbaString(value);
            writer.WriteStringValue(rgbaString);
        }

        protected virtual string FormatRgbaString(Color color)
        {
            return $"rgba({color.R},{color.G},{color.B},{color.A / 255f})";
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
                float a = float.Parse(components[4]);

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
            // #FF0000
            else if (components.Length == 1 && str[0] == '#')
            {
                return ColorTranslator.FromHtml(str);
            }

            throw new JsonException("Invalid RGBA string format");
        }
    }
}
