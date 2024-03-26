namespace Settings
{
    /// <summary>
    /// Interface defining methods needed to save and load strings containing serialized data.
    /// </summary>
    public interface ISettingsSaver
    {
        /// <summary>
        /// Gets the serialized settings.
        /// </summary>
        /// <returns>The serialization of the settings. If unable to access the saved serialization, then <see langword="null"/>.</returns>
        string? GetSavedSerialization();

        /// <summary>
        /// Saves the string.
        /// </summary>
        /// <param name="serialization">The serialized data.</param>
        /// <returns><see langword="true"/> if successfully saved; else, <see langword="false"/>.</returns>
        bool SaveSerialization(string serialization);
    }
}
