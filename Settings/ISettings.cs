namespace Settings
{
    /// <summary>
    /// Interface for classes that save and load serialized classes.
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Resets all settings values to the default values.
        /// </summary>
        void Reset();

        /// <summary>
        /// Reloads the settings from the last point of save.
        /// </summary>
        /// <returns><see langword="false"/> if unable to retrieve serialization from SettingsSaver; else <see langword="true"/>.</returns>
        bool Reload();

        /// <summary>
        /// Checks to see if any settings values have been changed since the last point of save.
        /// </summary>
        /// <returns><see langword="false"/> if all values are the same; else <see langword="true"/>.</returns>
        /// <remarks>Collections are compared by their element values, not the collection reference directly.</remarks>
        bool CheckIsDirty();

        /// <summary>
        /// Saves the current settings values.
        /// </summary>
        /// <param name="forceSave">Saves the settings, without preforming an IsDirtyCheck.</param>
        /// <returns><see langword="false"/> if there was an error while saving; else <see langword="true"/>.</returns>
        /// <remarks>If an IsDirtyCheck determines there are no changes to save, <c>true</c> will be returned without saving.</remarks>
        bool Save(bool forceSave);
    }
}
