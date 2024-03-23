using FluentAssertions;
using Settings;
using Xunit;

namespace SettingsTests
{

    internal class SimpleSettings : SettingsBase<SimpleSettings>
    {
        public string Name { get; set; } = "Bob";
        public int Age { get; set; } = 33;
        public Guid Id { get; set; } = Guid.NewGuid();

        public SimpleSettings(ISettingsSaver settingsSaver): base(settingsSaver)
        {

        }
    }

    public class ValueTypeSettingsTests
    {

        [Fact]
        public void SaveTest()
        {
            //Arrange
            SimpleSettings settings = new SimpleSettings(new InMemorySettingsSaver());

            //Act
            settings.Name = "Joe";
            settings.Save();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void IsDirtyTest()
        {
            //Arrange
            SimpleSettings settings = new SimpleSettings(new InMemorySettingsSaver());

            //Act
            settings.Name = "Joe";

            //Assert
            settings.CheckIsDirty().Should().BeTrue();
        }
    }
}
