using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsTests.ReferenceType.SettingsClasses
{
    /// <summary>
    /// Class containing a reference-type property that <see langword="null"/> default value.
    /// </summary>
    internal class ReferenceTypeClass4 : SettingsBase<ReferenceTypeClass4>
    {
        public DateTimeOffset SavedDateTime { get; set; }

        public Person PrimaryUser { get; set; }

        public ReferenceTypeClass4(ISettingsSaver settingsSaver) : base(settingsSaver)
        {
        }

        protected override void InitializeDefaultValues()
        {
            SavedDateTime = new DateTimeOffset(2024, 3, 23, 13, 2, 0, TimeSpan.FromHours(-6));
        }

        /// <summary>
        /// A class with a property that has a <see langword="null"/> default value. Has a default constructor.
        /// </summary>
        public class Person
        {
            public string? FirstName { get; set; }
            public int Age { get; set; }
        }
    }
}
