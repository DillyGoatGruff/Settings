using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsTests
{
	public class ComplexSettingsClass : SettingsBase2<ComplexSettingsClass>
	{
		public DateTimeOffset SavedDateTime { get; set; } = DateTimeOffset.Now;
		public Person PrimaryUser { get; set; } = new Person() { FirstName = "Bob", Age = 33 };

		[Obsolete("", true)]
		public ComplexSettingsClass() { }

		public ComplexSettingsClass(ISettingsSaver settingsSaver) : base(settingsSaver)
		{
		}
	}

	public class Person
	{
		public string FirstName { get; set; } 
		public int Age { get; set; }
	}
}
