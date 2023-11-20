namespace Settings
{
    public interface ISettingsSaver
    {
        /// <summary>
        /// Gets the serialized settings.
        /// </summary>
        /// <returns>The serialization of the settings. If unable to access the saved serialization, then <c>null</c>.</returns>
        string? GetSavedSerialization();

        bool SaveSerialization(string serialization);
    }
}
