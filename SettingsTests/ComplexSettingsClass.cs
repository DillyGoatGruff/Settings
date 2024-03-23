using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsTests
{
	public class ComplexSettingsClass : SettingsBase<ComplexSettingsClass>
	{
		public DateTimeOffset SavedDateTime { get; set; }
		public Person PrimaryUser { get; set; }

		public ComplexSettingsClass(ISettingsSaver settingsSaver) : base(settingsSaver)
		{
		}

        public ComplexSettingsClass() : base(null)
        {
            
        }

        protected override void InitializeDefaultValues()
        {
			SavedDateTime = new DateTimeOffset(2024, 3, 23, 13, 2, 0, TimeSpan.FromHours(-6));
			PrimaryUser = new Person() { FirstName = "Bob", Age = 33 };
        }
    }

	public class Person
	{
		public string FirstName { get; set; }
		public int Age { get; set; }
	}
}
