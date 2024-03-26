using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsTests.ReferenceType.SettingsClasses
{
    /// <summary>
    /// Class containing a reference-type property that has a default value that is assigned directly to the property definition.
    /// </summary>
    internal class ReferenceTypeClass2 : SettingsBase<ReferenceTypeClass2>
    {
        public DateTimeOffset SavedDateTime { get; set; }

        public Person? PrimaryUser { get; set; } = new Person() { FirstName = "Bob", Age = 33 };

        public ReferenceTypeClass2(ISettingsSaver settingsSaver) : base(settingsSaver)
        {
        }

        protected override void InitializeDefaultValues()
        {
            SavedDateTime = new DateTimeOffset(2024, 3, 23, 13, 2, 0, TimeSpan.FromHours(-6));
        }

        /// <summary>
        /// A simple class with no properties that have a default value of <see langword="null"/> and has a default constructor.
        /// </summary>
        public class Person
        {
            public string FirstName { get; set; } = "";
            public int Age { get; set; }
        }
    }
}
