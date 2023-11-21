using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Settings
{
	internal class PropertyEqualityChecker<T> : PropertyEqualityChecker where T : new()
	{
		private readonly T _settings;


		public PropertyEqualityChecker(T settings) : base(default!, settings!, Activator.CreateInstance(typeof(T))!)
		{
			_settings = settings;
		}

		public void Reset()
		{
			base.Reset(_settings!);
		}

		public void Reload(T settings)
		{
			base.Reload(_settings, settings);
		}

		public void UpdateSavedValues(T settings)
		{
			base.UpdateSavedValues(settings!);
		}

		public bool CheckIsDirty()
		{
			return base.CheckIsDirty(_settings!);
		}
	}


	internal class PropertyEqualityChecker
	{

		record struct PropertyInfoValue(PropertyInfo PropertyInfo, object? Value);

		#region Fields

		private readonly PropertyInfo _parentPropertyInfo;
		private readonly PropertyInfoValue[] _defaultPropertyInfoValues;
		private readonly PropertyInfoValue[] _savedPropertyInfoValues;
		private readonly PropertyEqualityChecker[] _subClassPropertyEqualityCheckers;

		#endregion

		#region Properties

		#endregion

		#region Constructor

		public PropertyEqualityChecker(PropertyInfo parentPropertyInfo, object obj, object defaultObj)
		{
			_parentPropertyInfo = parentPropertyInfo;

			PropertyInfo[] tempPropertyInfos = obj.GetType()
			.GetProperties()
			.Where(x => x.CanWrite) // Only need to monitor properties that have setters
			.ToArray();

			List<PropertyEqualityChecker> propertyCheckers = new List<PropertyEqualityChecker>(tempPropertyInfos.Length);
			List<PropertyInfoValue> defaultPropertyInfoValues = new List<PropertyInfoValue>(tempPropertyInfos.Length);
			List<PropertyInfoValue> savedPropertyInfoValues = new List<PropertyInfoValue>(tempPropertyInfos.Length);

			for (int i = 0; i < tempPropertyInfos.Length; i++)
			{
				object? savedPropertyValue = tempPropertyInfos[i].GetValue(obj);
				object? defaultPropertyValue = tempPropertyInfos[i].GetValue(defaultObj);
				if (tempPropertyInfos[i].PropertyType.IsClass && tempPropertyInfos[i].PropertyType != typeof(string))
				{
					propertyCheckers.Add(new PropertyEqualityChecker(tempPropertyInfos[i], savedPropertyValue, defaultPropertyValue));
				}
				else
				{
					defaultPropertyInfoValues.Add(new PropertyInfoValue(tempPropertyInfos[i], defaultPropertyValue));
					savedPropertyInfoValues.Add(new PropertyInfoValue(tempPropertyInfos[i], savedPropertyInfoValues));
				}
			}

			if (defaultPropertyInfoValues.Count != 0)
			{
				_defaultPropertyInfoValues = defaultPropertyInfoValues.ToArray();
				_savedPropertyInfoValues = savedPropertyInfoValues.ToArray();
			}
			else
			{
				_defaultPropertyInfoValues = Array.Empty<PropertyInfoValue>();
				_savedPropertyInfoValues = Array.Empty<PropertyInfoValue>();
			}

			if (propertyCheckers.Count != 0) _subClassPropertyEqualityCheckers = propertyCheckers.ToArray();
			else _subClassPropertyEqualityCheckers = Array.Empty<PropertyEqualityChecker>();
		}

		#endregion

		#region Private Methods

		private object? GetPropertyValue(object parent)
		{
			return _parentPropertyInfo.GetValue(parent);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Resets all settings values to the default values.
		/// </summary>
		/// <param name="currentSettings">The object to reset back to default values.</param>
		public void Reset(object currentSettings)
		{
			for (int i = 0; i < _savedPropertyInfoValues.Length; i++)
			{
				PropertyInfoValue defaultPropertyInfoValue = _defaultPropertyInfoValues[i];
				defaultPropertyInfoValue.PropertyInfo.SetValue(currentSettings, defaultPropertyInfoValue.Value);
			}

			for (int i = 0; i < _subClassPropertyEqualityCheckers.Length; i++)
			{
				PropertyEqualityChecker propertyEqualityChecker = _subClassPropertyEqualityCheckers[i];
				object? value = propertyEqualityChecker.GetPropertyValue(currentSettings);
				propertyEqualityChecker.Reset(value);
			}
		}

		/// <summary>
		/// Updates <paramref name="currentSettings"/> and the saved values to those within <paramref name="newSettings"/>.
		/// </summary>
		/// <param name="currentSettings">The current settings to update.</param>
		/// <param name="newSettings">The new settings to update values to.</param>
		public void Reload(object currentSettings, object newSettings)
		{
			for (int i = 0; i < _savedPropertyInfoValues.Length; i++)
			{
				PropertyInfo propertyInfo = _savedPropertyInfoValues[i].PropertyInfo;
				object? value = propertyInfo.GetValue(newSettings);

				propertyInfo.SetValue(currentSettings, value);
				_savedPropertyInfoValues[i] = _savedPropertyInfoValues[i] with { Value = value };
			}

			for (int i = 0; i < _subClassPropertyEqualityCheckers.Length; i++)
			{
				PropertyEqualityChecker propertyEqualityChecker = _subClassPropertyEqualityCheckers[i];
				object? currentValue = propertyEqualityChecker.GetPropertyValue(currentSettings);
				object? newValue = propertyEqualityChecker.GetPropertyValue(newSettings);
				propertyEqualityChecker.Reload(currentValue, newValue);
			}
		}

		public void UpdateSavedValues(object currentSettings)
		{
			for (int i = 0; i < _savedPropertyInfoValues.Length; i++)
			{
				object? value = _savedPropertyInfoValues[i].PropertyInfo.GetValue(currentSettings);
				_savedPropertyInfoValues[i] = _savedPropertyInfoValues[i] with { Value = value };
			}

			for (int i = 0; i < _subClassPropertyEqualityCheckers.Length; i++)
			{
				PropertyEqualityChecker propertyEqualityChecker = _subClassPropertyEqualityCheckers[i];
				object? value = propertyEqualityChecker.GetPropertyValue(currentSettings);
				propertyEqualityChecker.UpdateSavedValues(value);
			}
		}

		public bool CheckIsDirty(object? currentSettings)
		{
			for (int i = 0; i < _savedPropertyInfoValues.Length; i++)
			{
				PropertyInfoValue savedPropertyInfoValue = _savedPropertyInfoValues[i];
				object? value = savedPropertyInfoValue.PropertyInfo.GetValue(currentSettings);
				if (!value?.Equals(savedPropertyInfoValue.Value) ?? savedPropertyInfoValue.Value is not null)
				{
					return false;
				}
			}

			for (int i = 0; i < _subClassPropertyEqualityCheckers.Length; i++)
			{
				PropertyEqualityChecker propertyEqualityChecker = _subClassPropertyEqualityCheckers[i];
				object? savedValue = propertyEqualityChecker.GetPropertyValue(currentSettings);
				if (propertyEqualityChecker.CheckIsDirty(savedValue))
				{
					return true;
				}
			}

			return false;
		}

		#endregion
	}
}
