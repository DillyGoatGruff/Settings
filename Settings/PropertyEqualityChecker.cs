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

		public void Reload()
		{

		}

		public void UpdateSavedValues(T settings)
		{
			base.UpdateSavedValues(settings!);
		}

		public bool CheckIsDirty(T settings)
		{
			return base.CheckIsDirty(settings!);
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
		/// <param name="settings">The object to reset back to default values.</param>
		public void Reset(object settings)
		{
			for (int i = 0; i < _savedPropertyInfoValues.Length; i++)
			{
				PropertyInfoValue defaultPropertyInfoValue = _defaultPropertyInfoValues[i];
				defaultPropertyInfoValue.PropertyInfo.SetValue(settings, defaultPropertyInfoValue.Value);
			}

			for (int i = 0; i < _subClassPropertyEqualityCheckers.Length; i++)
			{
				PropertyEqualityChecker propertyEqualityChecker = _subClassPropertyEqualityCheckers[i];
				object? value = propertyEqualityChecker.GetPropertyValue(settings);
				propertyEqualityChecker.Reset(value);
			}
		}



		public void UpdateSavedValues(object settings)
		{
			for (int i = 0; i < _savedPropertyInfoValues.Length; i++)
			{
				object? value = _savedPropertyInfoValues[i].PropertyInfo.GetValue(settings);
				_savedPropertyInfoValues[i] = _savedPropertyInfoValues[i] with { Value = value };
			}

			for (int i = 0; i < _subClassPropertyEqualityCheckers.Length; i++)
			{
				PropertyEqualityChecker propertyEqualityChecker = _subClassPropertyEqualityCheckers[i];
				object? value = propertyEqualityChecker.GetPropertyValue(settings);
				propertyEqualityChecker.UpdateSavedValues(value);
			}
		}

		public bool CheckIsDirty(object? settings)
		{
			for (int i = 0; i < _savedPropertyInfoValues.Length; i++)
			{
				PropertyInfoValue savedPropertyInfoValue = _savedPropertyInfoValues[i];
				object? value = savedPropertyInfoValue.PropertyInfo.GetValue(settings);
				if (!value?.Equals(savedPropertyInfoValue.Value) ?? savedPropertyInfoValue.Value is not null)
				{
					return false;
				}
			}

			for (int i = 0; i < _subClassPropertyEqualityCheckers.Length; i++)
			{
				PropertyEqualityChecker propertyEqualityChecker = _subClassPropertyEqualityCheckers[i];
				object? savedValue = propertyEqualityChecker.GetPropertyValue(settings);
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
