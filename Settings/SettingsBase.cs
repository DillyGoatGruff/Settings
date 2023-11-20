using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Settings
{

	public abstract class SettingsBase<T> : ISettings where T : SettingsBase<T>, new()
	{

		record struct PropertyInfoValue(int PropertyInfoIndex, object? Value);

		static readonly JsonSerializerOptions m_jsonSerializerOptions = new JsonSerializerOptions()
		{
			WriteIndented = true,
			IncludeFields = true
		};

		private static readonly PropertyInfo[] m_propertyInfo;
		private static readonly PropertyInfoValue[] m_defaultPropertyValues;

		#region Fields

		private readonly ISettingsSaver _settingsSaver;
		private readonly T _currentSettings;
		private readonly PropertyInfoValue[] _savedSettingsPropertyInfoValues;
		private readonly PropertyInfoValue[] _propertyInfoValuesBuffer;

		#endregion


		#region Properties



		#endregion


		#region Constructors

		static SettingsBase()
		{
			PropertyInfo[] GetPropertyInfo(Type t)
			{
				return
					t.GetProperties()
					.Where(x => x.CanWrite) // Only need to monitor properties that have setters
					.ToArray();
			}

			PropertyInfoValue[] GetDefaultPropertyInfoValues(Type t, PropertyInfo[] propertyInfo)
			{
				PropertyInfoValue[] defaultPropertyInfoValues = new PropertyInfoValue[m_propertyInfo.Length];
				var temp = Activator.CreateInstance(t);
				for (int i = 0; i < defaultPropertyInfoValues.Length; i++)
				{
					object? value = propertyInfo[i].GetValue(temp);
					defaultPropertyInfoValues[i] = new PropertyInfoValue(i, value);
				}
				return defaultPropertyInfoValues;
			}

			m_propertyInfo = GetPropertyInfo(typeof(T));
			m_defaultPropertyValues = GetDefaultPropertyInfoValues(typeof(T), m_propertyInfo);
		}



		[Obsolete("This constructor should only be called from a default constructor within the inherited class. " +
			"This constructors should not be called directly.")]
		protected SettingsBase()
		{
			_settingsSaver = default!;
			_currentSettings = default!;
			_savedSettingsPropertyInfoValues = default!;
			_propertyInfoValuesBuffer = default!;
		}

		protected SettingsBase(ISettingsSaver settingsSaver)
		{
			_currentSettings = (T)this;
			_settingsSaver = settingsSaver;

			_propertyInfoValuesBuffer = new PropertyInfoValue[m_propertyInfo.Length];
			_savedSettingsPropertyInfoValues = new PropertyInfoValue[m_propertyInfo.Length];
			T savedSettings = CreateDefaultSettings();
			for (int i = 0; i < m_propertyInfo.Length; i++)
			{
				PropertyInfo propertyInfo = m_propertyInfo[i];
				object? value = propertyInfo.GetValue(savedSettings);
				_savedSettingsPropertyInfoValues[i] = new PropertyInfoValue(i, value);
				_propertyInfoValuesBuffer[i] = new PropertyInfoValue(i, value);
			}

			Reload();
		}

		#endregion


		#region Private Methods

		private static T CreateDefaultSettings()
		{
			return new T();
		}

		private string Serialize()
		{
			return JsonSerializer.Serialize<T>(_currentSettings, m_jsonSerializerOptions);
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

		private Span<PropertyInfoValue> GetDirtyProperties(Span<PropertyInfoValue> propertyInfosToCheck)
		{
			int dirtyPropertyIndex = 0;
			for (int i = 0; i < propertyInfosToCheck.Length; i++)
			{
				PropertyInfo propertyInfo = m_propertyInfo[propertyInfosToCheck[i].PropertyInfoIndex];
				object? currentValue = propertyInfo.GetValue(_currentSettings);
				if (!currentValue?.Equals(_savedSettingsPropertyInfoValues[i].Value) ?? false)
				{
					propertyInfosToCheck[i] = propertyInfosToCheck[i] with { Value = _savedSettingsPropertyInfoValues[i].Value };
					dirtyPropertyIndex++;
				}
			}

			return propertyInfosToCheck.Slice(0, dirtyPropertyIndex);
		}

		private void UpdateSavedSettingsPropertyInfoValues(T settings)
		{
			for (int i = 0; i < m_propertyInfo.Length; i++)
			{
				object? value = m_propertyInfo[i].GetValue(settings);
				_savedSettingsPropertyInfoValues[i] = _savedSettingsPropertyInfoValues[i] with { Value = value };
			}
		}

		private void SetProperties(ReadOnlySpan<PropertyInfoValue> propertyValues)
		{
			for (int i = 0; i < propertyValues.Length; i++)
			{
				PropertyInfoValue piValue = propertyValues[i];
				m_propertyInfo[piValue.PropertyInfoIndex].SetValue(_currentSettings, piValue.Value);
			}
		}

		#endregion


		#region Public Methods

		/// <inheritdoc/>
		public void Reset()
		{
			SetProperties(m_defaultPropertyValues);
		}

		/// <inheritdoc/>
		public bool Reload()
		{
			string? serialization = _settingsSaver.GetSavedSerialization();
			T savedSettings;
			if (serialization is not null)
			{
				savedSettings = Deserialize(serialization) ?? CreateDefaultSettings();
				UpdateSavedSettingsPropertyInfoValues(savedSettings);

				Span<PropertyInfoValue> dirtyProperties = GetDirtyProperties(_propertyInfoValuesBuffer);
				SetProperties(dirtyProperties);

				return true;
			}

			return false;
		}

		/// <inheritdoc/>
		public bool CheckIsDirty()
		{
			for (int i = 0; i < m_propertyInfo.Length; i++)
			{
				object? currentValue = m_propertyInfo[i].GetValue(_currentSettings);
				object? savedValued = _savedSettingsPropertyInfoValues[i].Value;

				if (!currentValue?.Equals(savedValued) ?? savedValued is not null)
					return true;
			}

			return false;
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
					UpdateSavedSettingsPropertyInfoValues(_currentSettings);
					return true;
				}
				return false;
			}

			return true;
		}

		#endregion
	}
}
