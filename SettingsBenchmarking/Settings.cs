using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsBenchmarking
{
	internal class Settings : SettingsBase<Settings>
	{
        public Guid id = Guid.NewGuid();
        public string Name { get; set; } = "Bob";
        public int Age { get; set; } = 33;

        public Persion Person { get; set; }


        public Settings(ISettingsSaver settingsSaver) : base(settingsSaver)
        {
        }

        protected override void InitializeDefaultValues()
        {
            Person = new Persion() { ID = Guid.NewGuid(), Name = "HI" };
        }
    }

    public class Persion
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public Persion()
        {
            
        }
    }
}

