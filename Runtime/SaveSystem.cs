using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    ///     Class that allows easy access to saving and loading from various file locations and formats.
    /// </summary>
    public static class SaveSystem
    {
        /// <summary>
        ///     The format to save the file in.
        /// </summary>
        public enum Format
        {
            JSON,
            Binary
        }

        /// <summary>
        ///     The location to save the file to.
        /// </summary>
        public enum Location
        {
            PersistentDataPath,
            StreamingAssetsPath,
            AbsolutePath,
            PlayerPrefs,
            Resources
        }

        /// <summary>
        ///     The default save settings. Saves an uncompressed json file to the persistent data path.
        /// </summary>
        public static SaveSystemSettings DefaultSettings = new()
        {
            location = Location.PersistentDataPath,
            format = Format.JSON,
            compress = false
        };

        /// <summary>
        ///     Saves the given object to the key at the given path.
        /// </summary>
        /// <param name="key">The value key to save to.</param>
        /// <param name="value">The value to save.</param>
        /// <param name="path">
        ///     The local file path ("SavedFiles/SaveFile.sav") to save to. Only use a full path if using
        ///     Location.AbsolutePath.
        /// </param>
        /// <param name="settings">The optional settings to save the file with.</param>
        public static void Save(string key, object value, string path, SaveSystemSettings settings = null)
        {
            // Get default settings if no settings are provided
            settings ??= DefaultSettings;

            SaveFile saveFile;

            // Get save file if it exists
            var data = ReadFile(path);

            // If file doesn't exist, create a new save file data object, otherwise deserialize the data
            if (data == null)
                saveFile = new SaveFile();
            else
                saveFile = DeserializeSaveFile(data);

            // Set the value in the save file
            saveFile.Set(key, value);

            data = SerializeSaveFile(saveFile, settings.format, settings.compress);

            // Save to location based on settings
            switch (settings.location)
            {
                case Location.PersistentDataPath:
                    WriteFile(Path.Combine(Application.persistentDataPath, path), data);
                    break;
                case Location.StreamingAssetsPath:
                    WriteFile(Path.Combine(Application.streamingAssetsPath, path), data);
                    break;
                case Location.Resources:
                    // TODO: Figure out if resources should be supported, will probably need to use AssetDatabase
                    //WriteFile(Path.Combine(Application.dataPath, "Resources", path), data);
                    break;
                case Location.AbsolutePath:
                    WriteFile(path, data);
                    break;
                case Location.PlayerPrefs:
                    PlayerPrefs.SetString(path, Convert.ToBase64String(data));
                    break;
            }
        }

        /// <summary>
        ///     Loads the value at the given key from the file at the given path.
        /// </summary>
        /// <param name="key">The key to load from.</param>
        /// <param name="path">
        ///     The local file path ("SaveFiles/SaveFile.sav") to load from. Only use a full path if using Location.AbsolutePath.
        /// </param>
        /// <param name="location">The location to load from.</param>
        /// <typeparam name="T">The type of the value being loaded.</typeparam>
        /// <returns>Value loaded from the save file.</returns>
        public static T Load<T>(string key, string path, Location location)
        {
            var loadPath = path;
            switch (location)
            {
                case Location.PersistentDataPath:
                    loadPath = Path.Combine(Application.persistentDataPath, path);
                    break;
                case Location.StreamingAssetsPath:
                    loadPath = Path.Combine(Application.streamingAssetsPath, path);
                    break;
                case Location.AbsolutePath:
                    loadPath = path;
                    break;
                case Location.PlayerPrefs:
                    // Special handling for player prefs
                    if (PlayerPrefs.HasKey(path))
                    {
                        var ppData = Convert.FromBase64String(PlayerPrefs.GetString(path));
                        var ppSaveFile = DeserializeSaveFile(ppData);
                        return ppSaveFile.Get<T>(key);
                    }

                    break;
            }

            if (!File.Exists(loadPath)) Debug.LogError($"File does not exist at : {loadPath}");

            var data = ReadFile(loadPath);

            var saveFile = DeserializeSaveFile(data);

            return saveFile.Get<T>(key);
        }

        /// <summary>
        ///     Writes bytes to the given path.
        /// </summary>
        /// <param name="path">The path to save to.</param>
        /// <param name="value">The bytes to save.</param>
        private static void WriteFile(string path, byte[] value)
        {
            File.WriteAllBytes(path, value);
        }

        /// <summary>
        ///     Reads bytes from the given path if it exists.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <returns>The bytes of the file at the path. Null if the file does not exist.</returns>
        private static byte[] ReadFile(string path)
        {
            return !File.Exists(path) ? null : File.ReadAllBytes(path);
        }

        /// <summary>
        ///     Serializes the given save file object with the given format and compresssion.
        /// </summary>
        /// <param name="saveFile">The save file data object to serialize.</param>
        /// <param name="format">The format to serialize to.</param>
        /// <param name="compress">Whether to compress the data.</param>
        /// <returns>An array of serialize bytes.</returns>
        private static byte[] SerializeSaveFile(SaveFile saveFile, Format format, bool compress)
        {
            byte[] data = null;
            // Serialize the save file based on the settings
            if (format == Format.JSON)
                data = Encoding.UTF8.GetBytes(SerializeToJson(saveFile));
            else if (format == Format.Binary) data = SerializeToBinary(saveFile);

            // Compress the data if needed
            if (compress)
                data = Compression.Compress(data);

            return data;
        }

        /// <summary>
        ///     Deserializes a save file data object from the given bytes.
        /// </summary>
        /// <param name="data">The bytes to deserialize.</param>
        /// <returns>The deserialize save file data object.</returns>
        private static SaveFile DeserializeSaveFile(byte[] data)
        {
            if (Compression.IsCompressed(data))
                data = Compression.Decompress(data);

            // check if data is serialized to binary or json
            if (data[0] == '{')
            {
                var json = Encoding.UTF8.GetString(data);
                return DeserializeFromJson<SaveFile>(json);
            }

            return DeserializeFromBinary<SaveFile>(data);
        }

        /// <summary>
        ///     Serializes the given object to JSON.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>JSON string generated by the given object.</returns>
        private static string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        ///     Deserializes the given JSON string to the given type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <returns>The object deserialized from the given JSON.</returns>
        private static T DeserializeFromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        ///     Serializes the given object to binary.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>Byte array generated by the given object.</returns>
        private static byte[] SerializeToBinary(object obj)
        {
            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream);
            writer.Write(SerializeToJson(obj));
            return stream.ToArray();
        }

        /// <summary>
        ///     Deserializes the given byte array to the given type.
        /// </summary>
        /// <param name="bytes">The bytes to deserialize.</param>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <returns>The object deserialized from the given byte array.</returns>
        private static T DeserializeFromBinary<T>(byte[] bytes)
        {
            using MemoryStream stream = new(bytes);
            using BinaryReader reader = new(stream);
            return DeserializeFromJson<T>(reader.ReadString());
        }

        /// <summary>
        ///     Inner class that handles compression.
        /// </summary>
        public static class Compression
        {
            /// <summary>
            ///     Compresses an array of bytes using GZip.
            /// </summary>
            /// <param name="data">An array of bytes to compress.</param>
            /// <returns>A compressed array of bytes.</returns>
            public static byte[] Compress(byte[] data)
            {
                using var dataStream = new MemoryStream(data);
                using var compressionStream = new GZipStream(dataStream, CompressionMode.Compress);
                compressionStream.Write(data, 0, data.Length);
                // TODO: Determine if compression stream needs to be disposed of/closed manually
                return dataStream.ToArray();
            }

            /// <summary>
            ///     Decompresses an array of bytes using GZip.
            /// </summary>
            /// <param name="data">An array of bytes to decompress.</param>
            /// <returns>A decompressed array of bytes.</returns>
            public static byte[] Decompress(byte[] data)
            {
                using var inputDataStream = new MemoryStream(data);
                using var outputDataStream = new MemoryStream();
                using var compressionStream = new GZipStream(inputDataStream, CompressionMode.Decompress);

                compressionStream.CopyTo(outputDataStream);
                // TODO: Determine if compression stream needs to be disposed of/closed manually
                return outputDataStream.ToArray();
            }

            /// <summary>
            ///     Checks if a byte array is compressed.
            /// </summary>
            /// <param name="data">An array of bytes to check.</param>
            /// <returns>True if the data is compressed.</returns>
            public static bool IsCompressed(byte[] data)
            {
                return data[0] == 31 && data[1] == 139;
            }
        }

        /// <summary>
        ///     Class that contains data saved into a file, index by key.
        /// </summary>
        [Serializable]
        private class SaveFile
        {
            [SerializeField] private Dictionary<string, object> data = new();

            public void Add(string key, object value)
            {
                if (!ContainsKey(key))
                    data.Add(key, value);
            }

            public void Remove(string key)
            {
                if (ContainsKey(key))
                    data.Remove(key);
            }

            public void Set(string key, object value)
            {
                if (ContainsKey(key))
                    data[key] = value;
                else
                    Add(key, value);
            }

            public object Get(string key)
            {
                data.TryGetValue(key, out var value);
                return value;
            }

            public T Get<T>(string key)
            {
                return (T)Get(key);
            }

            public void Clear()
            {
                data.Clear();
            }

            public bool ContainsKey(string key)
            {
                return data.ContainsKey(key);
            }
        }
    }
}