using System;
using System.IO;
using Newtonsoft.Json;

namespace CSharp_Utils.Helpers
{
    /// <summary>
    /// Helper class for loading and saving JSON.
    /// </summary>
    /// <typeparam name="TType">The type of object to load or save.</typeparam>
    public static class NewtonsoftJsonHelpers<TType> where TType : class, new()
    {
        /// <summary>
        /// Deserializes JSON data from a file into an object of type TType.
        /// </summary>
        /// <param name="pathConfig">The path to the config file.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when pathConfig is null or empty.</exception>
        public static TType Load(string pathConfig)
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
            return JsonConvert.DeserializeObject<TType>(json);
        }

        /// <summary>
        /// Serializes the config object into JSON data and saves it to a file.
        /// </summary>
        /// <param name="pathConfig">The path to the config file.</param>
        /// <param name="config">The object to save.</param>
        /// <exception cref="ArgumentNullException">Thrown when pathConfig or config is null.</exception>
        public static void Save(string pathConfig, TType config)
        {
            ArgumentNullException.ThrowIfNull(config);

            string json = JsonConvert.SerializeObject(config);
            SaveToFile(pathConfig, json);
        }

        /// <summary>
        /// Saves the JSON data to a file.
        /// </summary>
        /// <param name="pathConfig">The path to the config file.</param>
        /// <param name="json">The JSON string to save.</param>
        /// <exception cref="ArgumentNullException">Thrown when pathConfig or json is null or empty.</exception>
        public static void SaveToFile(string pathConfig, string json)
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
