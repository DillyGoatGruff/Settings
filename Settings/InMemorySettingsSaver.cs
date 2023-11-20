namespace Settings
{
    public class InMemorySettingsSaver : ISettingsSaver
    {
        private string _serialization = "";

        public string GetSavedSerialization()
        {
            return _serialization;
        }

        public bool SaveSerialization(string serialization)
        {
            _serialization = serialization;
            return true;
        }
    }
}
