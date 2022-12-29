using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        ///     Settings used by the save system's json serializer.
        /// </summary>
        public static JsonSerializerSettings SaveSystemSerializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new SaveSystemSerializationBinder()
        };

        private static SaveFileCache cachedSaveFile = null;

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

            // Get save file if it exists, explicitly ignore non-existent files here
            var saveFile = LoadFromFile(path, settings.location, true);

            // If file doesn't exist, create a new save file data object, otherwise deserialize the data
            if (saveFile == null)
                saveFile = new SaveFile(settings.compress, settings.format);

            // Set the value in the save file
            saveFile.Set(key, value);

            SaveToFile(saveFile, path, settings);
        }

        /// <summary>
        ///     Loads the value at the given key from the file at the given path.
        /// </summary>
        /// <param name="key">The key to load from.</param>
        /// <param name="path">
        ///     The local file path ("SaveFiles/SaveFile.sav") to load from. Only use a full path if using Location.AbsolutePath.
        /// </param>
        /// <param name="location">The location to load from. Defaults to PersistentDataPath.</param>
        /// <typeparam name="T">The type of the value being loaded.</typeparam>
        /// <returns>Value loaded from the save file.</returns>
        public static T Load<T>(string key, string path, Location location = Location.PersistentDataPath)
        {
            var saveFile = LoadFromFile(path, location);

            return saveFile.Get<T>(key);
        }

        /// <summary>
        ///     Moves a file from its original location to a new one.
        /// </summary>
        /// <param name="originalPath">The original filepath.</param>
        /// <param name="originalLocation">The original location used to store the file.</param>
        /// <param name="newPath">The new path to save to.</param>
        /// <param name="settings">Optional settings for where and how to save the file that's being moved.</param>
        public static void MoveFile(string originalPath, Location originalLocation, string newPath,
            SaveSystemSettings settings = null)
        {
            var saveFile = LoadFromFile(originalPath, originalLocation);
            Delete(originalPath, originalLocation);
            SaveToFile(saveFile, newPath, settings ?? DefaultSettings);
        }

        /// <summary>
        ///     Deletes the given key from the file at the given path.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        /// <param name="path">The path to the file.</param>
        /// <param name="location">The location of the file.</param>
        public static void Delete(string key, string path, Location location = Location.PersistentDataPath)
        {
            var saveFile = LoadFromFile(path, location);

            if (saveFile.ContainsKey(key))
            {
                saveFile.Remove(key);

                SaveToFile(saveFile, path, new SaveSystemSettings(location, saveFile.Format, saveFile.Compressed));
            }
            else
            {
                Logging.LogWarning($"Unable to delete key '{key}' at path: '{GetPath(path, location)}'");
            }
        }

        /// <summary>
        ///     Deletes a file at the given path.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="location">The location of the file.</param>
        public static void Delete(string path, Location location = Location.PersistentDataPath)
        {
            var filePath = GetPath(path, location);
            Logging.Log($"Deleting file at filePath '{filePath}' with values path '{path}' and location '{location}'");
            cachedSaveFile = null; // Ensure cached file is cleared when deleting any save file.
            if (location == Location.PlayerPrefs)
            {
                if (PlayerPrefs.HasKey(filePath))
                    PlayerPrefs.DeleteKey(filePath);
                else
                    Logging.LogWarning($"Unable to delete file in PlayerPrefs at path: '{filePath}'");

                return;
            }

            if (File.Exists(filePath))
                File.Delete(filePath);
            else
                Logging.LogWarning($"Attempted to delete non-existent file at path: '{filePath}'");
        }

        /// <summary>
        /// Saves a save file to a file at the given path.
        /// </summary>
        /// <param name="saveFile">The save file to save.</param>
        /// <param name="path">The path to save to.</param>
        /// <param name="settings">The settings to apply.</param>
        private static void SaveToFile(SaveFile saveFile, string path, SaveSystemSettings settings)
        {
            var data = SerializeSaveFile(saveFile, settings.format, settings.compress);
            string filePath = GetPath(path, settings.location);
            Logging.Log($"Saving to file at filePath '{filePath}' with values path '{path}' and location '{settings.location}'");
            if(settings.location == Location.PlayerPrefs)
                PlayerPrefs.SetString(filePath, Convert.ToBase64String(data));
            else
                WriteFile(filePath, data);
        }

        /// <summary>
        ///     Loads a save file from the given path and location.
        /// </summary>
        /// <param name="path">The path of the file to load.</param>
        /// <param name="location">The location of the file to load. Defaults to PersistentDataPath.</param>
        /// <param name="ignoreNonExistent">Whether to ignore non-existent files. Defaults to false.</param>
        /// <returns>The save file stored at the given path and location.</returns>
        private static SaveFile LoadFromFile(string path, Location location = Location.PersistentDataPath,
            bool ignoreNonExistent = false)
        {
            var loadPath = GetPath(path, location);
            Logging.Log($"Loading from filepath: {loadPath}");
            if (cachedSaveFile != null && cachedSaveFile.FilePath == loadPath)
            {
                Logging.Log("Loading from cached file.");
                return cachedSaveFile.SaveFile;
            }
            
            if (location == Location.PlayerPrefs && PlayerPrefs.HasKey(loadPath))
            {
                var ppData = Convert.FromBase64String(PlayerPrefs.GetString(loadPath));
                var ppSaveFile = DeserializeSaveFile(ppData);
                return ppSaveFile;
            }

            if (!File.Exists(loadPath))
            {
                if (ignoreNonExistent)
                    return null;
                Logging.LogError($"File does not exist at : {loadPath}");
            }

            var data = ReadFile(loadPath);

            var saveFile = DeserializeSaveFile(data);

            cachedSaveFile ??= new SaveFileCache();
            cachedSaveFile.SaveFile = saveFile;
            cachedSaveFile.FilePath = loadPath;

            return saveFile;
        }

        /// <summary>
        ///     Writes bytes to the given path.
        /// </summary>
        /// <param name="path">The path to save to.</param>
        /// <param name="value">The bytes to save.</param>
        private static void WriteFile(string path, byte[] value)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            File.WriteAllBytes(path, value);
        }

        /// <summary>
        ///     Reads bytes from the given path if it exists.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <returns>The bytes of the file at the path. Null if the file does not exist.</returns>
        private static byte[] ReadFile(string path)
        {
            if (File.Exists(path))
                return File.ReadAllBytes(path);
            return null;
        }

        /// <summary>
        ///     Constructs a path based on the given path and location.
        /// </summary>
        /// <param name="path">The local filepath.</param>
        /// <param name="location">The location to of the filepath. Defaults to PersistentDataPath.</param>
        /// <returns>Constructed filepath based on the location.</returns>
        private static string GetPath(string path, Location location = Location.PersistentDataPath)
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
                    loadPath = path;
                    break;
            }

            return loadPath;
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
            saveFile.SetSettings(compress, format);

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
        public static string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj,
                SaveSystemSerializerSettings);
        }

        /// <summary>
        ///     Deserializes the given JSON string to the given type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <returns>The object deserialized from the given JSON.</returns>
        public static T DeserializeFromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json,
                SaveSystemSerializerSettings);
        }

        /// <summary>
        ///     Serializes the given object to binary.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>Byte array generated by the given object.</returns>
        public static byte[] SerializeToBinary(object obj)
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
        public static T DeserializeFromBinary<T>(byte[] bytes)
        {
            using MemoryStream stream = new(bytes);
            using BinaryReader reader = new(stream);
            return DeserializeFromJson<T>(reader.ReadString());
        }

        /// <summary>
        ///     Serialization Binder used by the save system.
        /// </summary>
        private class SaveSystemSerializationBinder : DefaultSerializationBinder
        {
            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                base.BindToName(serializedType, out assemblyName, out typeName);
            }

            public override Type BindToType(string assemblyName, string typeName)
            {
                return base.BindToType(assemblyName, typeName);
            }
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
                using var dataStream = new MemoryStream();
                using var compressionStream = new GZipStream(dataStream, CompressionMode.Compress);
                compressionStream.Write(data, 0, data.Length);
                compressionStream.Close();
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
        /// Cache containing the most recent save file. Allows loading multiple keys from one file without having to re-read the entire file.
        /// </summary>
        private class SaveFileCache
        {
            public SaveFile SaveFile { get; set; }
            public string FilePath { get; set; }
        }

        /// <summary>
        ///     Class that contains data saved into a file, index by key.
        /// </summary>
        [Serializable]
        private class SaveFile
        {
            /// <summary>
            ///     Whether the SaveFile was saved as compressed.
            /// </summary>
            [SerializeField] private bool compressed;

            /// <summary>
            ///     The format the SaveFile was saved into.
            /// </summary>
            [SerializeField] private Format format;

            /// <summary>
            ///     Dictionary containing all of the SaveFile's data.
            /// </summary>
            [JsonProperty] private Dictionary<string, object> data = new();

            /// <summary>
            ///     Default Constructor.
            ///     Initializes settings to default values.
            /// </summary>
            public SaveFile()
            {
                SetSettings(DefaultSettings.compress, DefaultSettings.format);
            }

            /// <summary>
            ///     Parameterized constructor. Allows setting save settings.
            /// </summary>
            /// <param name="compressed">Whether the file is saved as compressed.</param>
            /// <param name="format">The format the file is saved into.</param>
            public SaveFile(bool compressed, Format format)
            {
                SetSettings(compressed, format);
            }

            /// <summary>
            ///     Accessor to whether the SaveFile was saved as compressed.
            /// </summary>
            public bool Compressed => compressed;

            /// <summary>
            ///     Accessor to the format the SaveFile was saved into.
            /// </summary>
            public Format Format => format;

            /// <summary>
            ///     Adds a new key with the given value if it does not exist.
            /// </summary>
            /// <param name="key">The new key.</param>
            /// <param name="value">The value to set.</param>
            public void Add(string key, object value)
            {
                if (!ContainsKey(key))
                    data.Add(key, value);
                else
                    Logging.Log($"Attempted to add existing key '{key}' to SaveFile. No data has changed.");
            }

            /// <summary>
            ///     Removes the key from the save file if it exists.
            /// </summary>
            /// <param name="key">The key to remove.</param>
            public void Remove(string key)
            {
                if (ContainsKey(key))
                    data.Remove(key);
                else
                    Logging.Log($"Attempted to remove non-existent key '{key}' from SaveFile. No data has changed.");
            }

            /// <summary>
            ///     Sets the key to the given value if it exists. Adds a new key if it does not exist.
            /// </summary>
            /// <param name="key">The key to set.</param>
            /// <param name="value">The value to set to.</param>
            public void Set(string key, object value)
            {
                if (ContainsKey(key))
                {
                    data[key] = value;
                }
                else
                {
                    Add(key, value);
                    Logging.Log(
                        $"Attempted to set non-existent key '{key}' in SaveFile. Key has been added with given value.");
                }
            }

            /// <summary>
            ///     Gets the value from the key if it exists.
            /// </summary>
            /// <param name="key">The key to get data from.</param>
            /// <returns>The value at the key. Null if key does not exist.</returns>
            public object Get(string key)
            {
                if (!data.TryGetValue(key, out var value))
                    Logging.LogError(
                        $"Attempted to get non-existent key '{key}' from SaveFile. Null has been returned.");

                return value;
            }

            /// <summary>
            ///     Gets the value from the key if it exists.
            /// </summary>
            /// <param name="key">The key to get data from.</param>
            /// <typeparam name="T">The type of data to get.</typeparam>
            /// <returns>The casted value at the key. Null if the key does not exist or if the value doesn't match the given type.</returns>
            public T Get<T>(string key)
            {
                var value = Get(key);
                if (value is T castedValue)
                    return castedValue;
                Logging.LogWarning(
                    $"Attempted to get key '{key}' from SaveFile as type '{typeof(T)}' but the value is of type '{value.GetType()}'. Default has been returned.");
                return default;
            }

            /// <summary>
            ///     Clears all data in the save file.
            /// </summary>
            public void Clear()
            {
                data.Clear();
            }

            /// <summary>
            ///     Whether the given key exists in the save file.
            /// </summary>
            /// <param name="key">Key to check for.</param>
            /// <returns>True if the key exists.</returns>
            public bool ContainsKey(string key)
            {
                return data.ContainsKey(key);
            }

            /// <summary>
            ///     Sets the save settings.
            /// </summary>
            /// <param name="compress">Whether the file is compressed.</param>
            /// <param name="format">The format saved in.</param>
            public void SetSettings(bool compress, Format format)
            {
                compressed = compress;
                this.format = format;
            }
        }
    }
}