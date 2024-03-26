using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsTests.ReferenceType.SettingsClasses
{
    /// <summary>
    /// Class containing a reference-type property that contains another reference-type property within it. The sub-property has a default value as null.
    /// </summary>
    internal class ReferenceTypeClass6 : SettingsBase<ReferenceTypeClass6>
    {
        public DateTimeOffset SavedDateTime { get; set; }

        public Person? PrimaryUser { get; set; } = new Person() { Name = null, Age = 33 };

        public ReferenceTypeClass6(ISettingsSaver settingsSaver) : base(settingsSaver)
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
            public Name? Name { get; set; }
            public int Age { get; set; }
        }

        public class Name
        {
            public string First { get; set; } = "";
            public string? Last { get; set; }
        }
    }
}
