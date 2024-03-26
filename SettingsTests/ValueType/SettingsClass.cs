using Settings;

namespace SettingsTests.ValueType
{
    internal class SettingsClass : SettingsBase<SettingsClass>
    {
        public Guid Id { get; set; } = new Guid("2E8D77B3-DB90-46A1-A93A-AEBE5008EDC0");

        public string User { get; set; } = "admin";

        public int Age { get; set; } = 33;

        public SettingsClass(ISettingsSaver settingsSaver) : base(settingsSaver)
        {

        }

        protected override void InitializeDefaultValues()
        {
        }
    }
}
