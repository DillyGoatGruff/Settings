using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Settings
{
    internal class PropertyEqualityChecker<T> : PropertyEqualityChecker
    {
        private readonly T _settings;


        public PropertyEqualityChecker(T settings) : base(default!, typeof(T), settings!, settings!)
        {
            _settings = settings;
        }

        public void Reset()
        {
            base.Reset(_settings!);
        }

        public void Reload(T settings)
        {
            base.Reload(_settings!, settings!);
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
        private readonly Type _type;
        private readonly PropertyInfoValue[] _defaultPropertyInfoValues;
        private readonly PropertyInfoValue[] _savedPropertyInfoValues;
        private readonly PropertyEqualityChecker[] _subClassPropertyEqualityCheckers;
        private readonly bool _isParentDefaultPropertyNull;
        private bool _isParentSavedPropertyNull;

        #endregion

        #region Properties

        #endregion

        #region Constructor

        public PropertyEqualityChecker(PropertyInfo parentPropertyInfo, Type type, object obj, object defaultObj)
        {
            _parentPropertyInfo = parentPropertyInfo;
            _type = type;
            _isParentDefaultPropertyNull = defaultObj is null;
            _isParentSavedPropertyNull = obj is null;

            PropertyInfo[] tempPropertyInfos = type
            .GetProperties()
            .Where(x => x.CanWrite) // Only need to monitor properties that have setters
            .ToArray();

            List<PropertyEqualityChecker> propertyCheckers = new List<PropertyEqualityChecker>(tempPropertyInfos.Length);
            List<PropertyInfoValue> defaultPropertyInfoValues = new List<PropertyInfoValue>(tempPropertyInfos.Length);
            List<PropertyInfoValue> savedPropertyInfoValues = new List<PropertyInfoValue>(tempPropertyInfos.Length);

            for (int i = 0; i < tempPropertyInfos.Length; i++)
            {
                //TODO: upon creation, saved values should always be identical to the default values so this should be able to reuse same value as default.
                object? defaultPropertyValue = (obj is not null) ? tempPropertyInfos[i].GetValue(defaultObj) : null;
                object? savedPropertyValue = (obj is not null) ? tempPropertyInfos[i].GetValue(obj) : null;

                if (tempPropertyInfos[i].PropertyType.IsClass && tempPropertyInfos[i].PropertyType != typeof(string))
                {
                    propertyCheckers.Add(new PropertyEqualityChecker(tempPropertyInfos[i], tempPropertyInfos[i].PropertyType, savedPropertyValue, defaultPropertyValue));
                }
                else
                {
                    defaultPropertyInfoValues.Add(new PropertyInfoValue(tempPropertyInfos[i], defaultPropertyValue));
                    savedPropertyInfoValues.Add(new PropertyInfoValue(tempPropertyInfos[i], savedPropertyValue));
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

        private void SetPropertyValue(object parent, object? value)
        {
            _parentPropertyInfo.SetValue(parent, value);
        }

        /// <summary>
        /// Creates an instance of the type the property equality checker monitors.
        /// </summary>
        /// <returns>The instance of <see cref="_type"/></returns>
        private object CreateInstance()
        {
            //Requires the type to have a default constructor
            object obj =  Activator.CreateInstance(_type)!;
            return obj;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets all settings values to the default values.
        /// </summary>
        /// <param name="currentSettings">The object to reset back to default values.</param>
        internal void Reset(object currentSettings)
        {
            for (int i = 0; i < _savedPropertyInfoValues.Length; i++)
            {
                PropertyInfoValue defaultPropertyInfoValue = _defaultPropertyInfoValues[i];
                defaultPropertyInfoValue.PropertyInfo.SetValue(currentSettings, defaultPropertyInfoValue.Value);
            }

            for (int i = 0; i < _subClassPropertyEqualityCheckers.Length; i++)
            {
                PropertyEqualityChecker propertyEqualityChecker = _subClassPropertyEqualityCheckers[i];
                if (propertyEqualityChecker._isParentDefaultPropertyNull)
                    propertyEqualityChecker.SetPropertyValue(currentSettings, null);
                else
                {
                    object? value = propertyEqualityChecker.GetPropertyValue(currentSettings);

                    if(value is null)
                    {
                        //Current value is null but default property is NOT null so need to instantiate a new instance so the default values can be set.
                        value = propertyEqualityChecker.CreateInstance();
                        propertyEqualityChecker.SetPropertyValue(currentSettings, value);
                    }
                    propertyEqualityChecker.Reset(value);
                }
            }
        }

        /// <summary>
        /// Updates <paramref name="currentSettings"/> and the saved values to those within <paramref name="newSettings"/>.
        /// </summary>
        /// <param name="currentSettings">The current settings to update.</param>
        /// <param name="newSettings">The new settings to update values to.</param>
        internal void Reload(object currentSettings, object newSettings)
        {
            if (_isParentSavedPropertyNull)
                return;

            for (int i = 0; i < _savedPropertyInfoValues.Length; i++)
            {
                PropertyInfo propertyInfo = _savedPropertyInfoValues[i].PropertyInfo;
                object? value = _savedPropertyInfoValues[i].Value;//propertyInfo.GetValue(newSettings);

                propertyInfo.SetValue(currentSettings, value);
                //_savedPropertyInfoValues[i] = _savedPropertyInfoValues[i] with { Value = value };
            }

            for (int i = 0; i < _subClassPropertyEqualityCheckers.Length; i++)
            {
                PropertyEqualityChecker propertyEqualityChecker = _subClassPropertyEqualityCheckers[i];
                if (propertyEqualityChecker._isParentSavedPropertyNull)
                {
                    propertyEqualityChecker.SetPropertyValue(currentSettings, null);
                    continue;
                }

                object? currentValue = propertyEqualityChecker.GetPropertyValue(currentSettings);
                object? newValue = propertyEqualityChecker.GetPropertyValue(newSettings);
                if (newValue is null && currentValue is not null)
                {
                    newValue = propertyEqualityChecker.CreateInstance();
                    propertyEqualityChecker.LoadDefaultValues(newValue);
                    propertyEqualityChecker.SetPropertyValue(newSettings, newValue);
                    continue;
                }
                else if (newValue is not null && currentValue is null)
                {
                    //New value is not null, need to instantiate the current value so properties can be set
                    currentValue = propertyEqualityChecker.CreateInstance();
                    propertyEqualityChecker.SetPropertyValue(currentSettings, currentValue);
                }
                propertyEqualityChecker.Reload(currentValue, newValue);
            }
        }

        private void LoadDefaultValues(object currentSettings)
        {
            for (int i = 0; i < _defaultPropertyInfoValues.Length; i++)
            {
                _defaultPropertyInfoValues[i].PropertyInfo.SetValue(currentSettings, _defaultPropertyInfoValues[i].Value);
            }
        }

        internal void UpdateSavedValues(object currentSettings)
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
                propertyEqualityChecker._isParentSavedPropertyNull = value is null;

                if(value is not null)
                    propertyEqualityChecker.UpdateSavedValues(value);
            }
        }

        internal bool CheckIsDirty(object? currentSettings)
        {
            for (int i = 0; i < _savedPropertyInfoValues.Length; i++)
            {
                PropertyInfoValue savedPropertyInfoValue = _savedPropertyInfoValues[i];
                object? value = savedPropertyInfoValue.PropertyInfo.GetValue(currentSettings);

                if (!value?.Equals(savedPropertyInfoValue.Value) ?? savedPropertyInfoValue.Value is not null)
                {
                    return true;
                }
            }

            for (int i = 0; i < _subClassPropertyEqualityCheckers.Length; i++)
            {
                PropertyEqualityChecker propertyEqualityChecker = _subClassPropertyEqualityCheckers[i];
                object? currentValue = propertyEqualityChecker.GetPropertyValue(currentSettings);

                if (currentValue is null)
                {
                    if (propertyEqualityChecker._isParentSavedPropertyNull)
                        continue;
                    else if (!propertyEqualityChecker._isParentSavedPropertyNull)
                        return true;
                }
                else if (propertyEqualityChecker.CheckIsDirty(currentValue))
                    return true;
            }

            return false;
        }

        #endregion
    }
}
