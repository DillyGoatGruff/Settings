using System.Reflection;
using System.Text.Json;

namespace Settings
{
    /// <summary>
    /// The base class for the settings class to inherit from.
    /// </summary>
    /// <typeparam name="T">The settings class to save and load.</typeparam>
    public abstract class SettingsBase<T> : ISettings where T : SettingsBase<T>
    {
        public static readonly string c_defaultFilename;

        static readonly JsonSerializerOptions m_jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true
        };

        #region Fields

        private readonly ISettingsSaver _settingsSaver;
        private readonly PropertyEqualityChecker<T> _propertyEqualityChecker;

        #endregion


        #region Properties



        #endregion


        #region Constructors

        static SettingsBase()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath)!;
            string configFilename = Path.Combine(assemblyDirectory!, $"{AppDomain.CurrentDomain.FriendlyName}.cfg");
            c_defaultFilename = configFilename;
        }

        /// <summary>
        /// Base implementation of <see cref="ISettings"/>.
        /// </summary>
        /// <remarks>A <see cref="FileSettingsSaver"/> is used to save/load settings. File is saved with a .cfg extension in same directory as executable.</remarks>
        protected SettingsBase() : this(new FileSettingsSaver(c_defaultFilename)) { }

        /// <summary>
        /// Base implementation of <see cref="ISettings"/>.
        /// </summary>
        /// <param name="filename">The name, including the path, of the file used to save and load serialized data.</param>
        /// <remarks>A <see cref="FileSettingsSaver"/> is used as the <see cref="ISettingsSaver"/>.</remarks>
        protected SettingsBase(string filename) : this(new FileSettingsSaver(filename)) { }

        /// <summary>
        /// Base implementation of <see cref="ISettings"/>.
        /// </summary>
        /// <param name="settingsSaver">Saves and loads settings.</param>
        /// <remarks>Settings will be loaded using <paramref name="settingsSaver"/> automatically.</remarks>
        protected SettingsBase(ISettingsSaver settingsSaver) : this(settingsSaver, true) { }

        /// <summary>
        /// Base implementation of <see cref="ISettings"/>.
        /// </summary>
        /// <param name="settingsSaver">Saves and loads settings.</param>
        /// <param name="loadSettings">If <see langword="false"/>, settings remain as default values until <see cref="Reload"/> is called.</param>
        protected SettingsBase(ISettingsSaver settingsSaver, bool loadSettings)
        {
            T currentSettings = (T)this;
            currentSettings.InitializeDefaultValues();
            _propertyEqualityChecker = new PropertyEqualityChecker<T>(currentSettings);
            _settingsSaver = settingsSaver;

            //If the reload returns false, the settings have not been serialized and saved before and should be saved to disk.
            if (loadSettings && Reload())
                Save(true);
        }

        #endregion


        #region Private Methods

        private static T CreateDefaultSettings()
        {
            //return new T();
#if NET5_0_OR_GREATER
            T defaultSettings = (T)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(T));
#else
            T defaultSettings = return (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
#endif
            defaultSettings.InitializeDefaultValues();
            return defaultSettings;
        }

        private string Serialize()
        {
            return JsonSerializer.Serialize<T>((T)this, m_jsonSerializerOptions);
        }

        private T? Deserialize(string serialization)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serialization))
                    return CreateDefaultSettings();
                T defaultSettings = CreateDefaultSettings();
                JsonSerializerExt.PopulateObject<T>(serialization, defaultSettings);
                return defaultSettings;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        #endregion


        #region Public Methods

        /// <inheritdoc/>
        public void Reset()
        {
            _propertyEqualityChecker.Reset();
        }

        /// <inheritdoc/>
        public bool Reload()
        {
            string? serialization = _settingsSaver.GetSavedSerialization();
            T savedSettings;
            if (serialization is not null)
            {
                savedSettings = Deserialize(serialization) ?? CreateDefaultSettings();
                _propertyEqualityChecker.Reload(savedSettings);

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool CheckIsDirty()
        {
            return _propertyEqualityChecker.CheckIsDirty();
        }

        /// <inheritdoc/>
        public bool Save(bool forceSave = false)
        {
            if (forceSave || CheckIsDirty())
            {
                string serialization = Serialize();
                bool isSuccessful = _settingsSaver.SaveSerialization(serialization);

                if (isSuccessful)
                {
                    _propertyEqualityChecker.UpdateSavedValues((T)this);
                    return true;
                }
                return false;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Method called to set default values for properties.
        /// </summary>
        /// <remarks>Default values have to be set either directly on the properties/backing field or in this method - any value set in the constructor will not be used as the default value.</remarks>
        protected abstract void InitializeDefaultValues();
    }
}
