using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsTests
{
	internal class CollectionsContainingClass : SettingsBase<CollectionsContainingClass>
	{

		public List<Computer> Computers { get; set; } = new List<Computer>();


		public CollectionsContainingClass(ISettingsSaver settingsSaver) : base(settingsSaver)
		{

		}

        protected override void InitializeDefaultValues()
        {
        }
    }


	public class Computer
	{

		public string Name { get; set; }
	}
}
