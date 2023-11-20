namespace Settings
{
    public class FileSettingsSaver : ISettingsSaver
    {
        private readonly string _filename;

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
