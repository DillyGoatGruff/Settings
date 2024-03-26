namespace Settings
{
    /// <summary>
    /// An implementation of <see cref="ISettingsSaver"/> that stores the serialization in a file.
    /// </summary>
    public class FileSettingsSaver : ISettingsSaver
    {
        private readonly string _filename;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">The full filename that the serialized data is saved in and loaded from.</param>
        /// <remarks>If the file does not already exist, it will be created, along with any directories that are needed.</remarks>
        public FileSettingsSaver(string filename)
        {
            _filename = filename;
        }

        /// <inheritdoc/>
        public string? GetSavedSerialization()
        {
            if (File.Exists(_filename))
            {
                using TextReader tr = new StreamReader(_filename);
                return tr.ReadToEnd();
            }

            return null;
        }

        /// <inheritdoc/>
        public bool SaveSerialization(string serialization)
        {
            FileInfo fileInfo = new FileInfo(_filename);
            if (!fileInfo.Directory!.Exists)
            {
                fileInfo.Directory.Create();
            }

            using TextWriter tw = new StreamWriter(_filename);
            tw.Write(serialization);

            return true;
        }
    }
}
