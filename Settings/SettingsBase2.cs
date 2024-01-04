using System.Reflection;
using System.Text.Json;

namespace Settings
{
	public abstract class SettingsBase2<T> : ISettings where T : SettingsBase2<T>, new()
	{

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

		[Obsolete("This constructor should only be called from a default constructor within the inherited class. " +
			"This constructors should not be called directly.")]
		protected SettingsBase2()
		{
			_propertyEqualityChecker = default!;
			_settingsSaver = default!;
		}

		protected SettingsBase2(ISettingsSaver settingsSaver)
		{
			T currentSettings = (T)this;
			_propertyEqualityChecker = new PropertyEqualityChecker<T>(currentSettings);
			_settingsSaver = settingsSaver;
		}

		#endregion


		#region Private Methods

		private static T CreateDefaultSettings()
		{
			return new T();
		}

		private string Serialize()
		{
			return JsonSerializer.Serialize<T>((T)this, m_jsonSerializerOptions);
		}

		private T? Deserialize(string serialization)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(serialization)) return CreateDefaultSettings();
				return JsonSerializer.Deserialize<T>(serialization, m_jsonSerializerOptions);
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
	}
}
