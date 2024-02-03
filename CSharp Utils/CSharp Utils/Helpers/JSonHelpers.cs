using System;
using System.IO;
using System.Text.Json;

namespace CSharp_Utils.Helpers
{
    /// <summary>
    /// Helper class for loading and saving JSON.
    /// </summary>
    /// <typeparam name="TType">The type of object to load or save.</typeparam>
    public class JsonHelpers<TType> where TType : class, new()
    {
        /// <summary>
        /// Deserializes JSON data from a file into an object of type TType.
        /// </summary>
        /// <param name="pathConfig">The path to the config file.</param>
        /// <param name="jsonSerializerOptions">The JSON serializer options.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when pathConfig is null or empty.</exception>
        public TType Load(string pathConfig, JsonSerializerOptions jsonSerializerOptions = null)
        {
            if (string.IsNullOrWhiteSpace(pathConfig))
            {
                throw new ArgumentNullException(nameof(pathConfig));
            }

            if (!File.Exists(pathConfig))
            {
                throw new FileNotFoundException("Config file not found.", pathConfig);
            }

            string json = File.ReadAllText(pathConfig);
            return JsonSerializer.Deserialize<TType>(json, jsonSerializerOptions);
        }

        /// <summary>
        /// Serializes the config object into JSON data and saves it to a file.
        /// </summary>
        /// <param name="pathConfig">The path to the config file.</param>
        /// <param name="config">The object to save.</param>
        /// <param name="jsonSerializerOptions">The JSON serializer options.</param>
        /// <exception cref="ArgumentNullException">Thrown when pathConfig or config is null.</exception>
        public void Save(string pathConfig, TType config, JsonSerializerOptions jsonSerializerOptions = null)
        {
            ArgumentNullException.ThrowIfNull(config);

            string json = JsonSerializer.Serialize(config, jsonSerializerOptions);
            SaveToFile(pathConfig, json);
        }

        /// <summary>
        /// Saves the JSON data to a file.
        /// </summary>
        /// <param name="pathConfig">The path to the config file.</param>
        /// <param name="json">The JSON string to save.</param>
        /// <exception cref="ArgumentNullException">Thrown when pathConfig or json is null or empty.</exception>
        public virtual void SaveToFile(string pathConfig, string json)
        {
            if (string.IsNullOrWhiteSpace(pathConfig))
            {
                throw new ArgumentNullException(nameof(pathConfig));
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            File.WriteAllText(pathConfig, json);
        }
    }
}
