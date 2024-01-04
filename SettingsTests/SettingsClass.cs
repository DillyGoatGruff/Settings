using Settings;

namespace SettingsTests
{
    internal class SettingsClass : SettingsBase<SettingsClass>
    {
        public Guid Id { get; set; } = new Guid("2E8D77B3-DB90-46A1-A93A-AEBE5008EDC0");

        public string User { get; set; } = "admin";

        public int Age { get; set; } = 33;

        [Obsolete("Do not use", true)]
        public SettingsClass() { }

        public SettingsClass(ISettingsSaver settingsSaver) : base(settingsSaver)
        {

        }
    }
}
