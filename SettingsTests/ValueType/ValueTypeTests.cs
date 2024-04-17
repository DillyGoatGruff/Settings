using FluentAssertions;
using Settings;
using Xunit;

namespace SettingsTests.ValueType
{

    internal class SimpleSettings : SettingsBase<SimpleSettings>
    {
        public string Name { get; set; } = "Bob";
        public int Age { get; set; } = 33;
        public Guid Id { get; set; } = Guid.NewGuid();

        public SimpleSettings(ISettingsSaver settingsSaver) : base(settingsSaver)
        {

        }

        protected override void InitializeDefaultValues()
        {

        }
    }

    public class ValueTypeTests
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

        [Fact]
        public void PreviouslySavedTest()
        {
            //Arrange
            string previouslySavedSerialization = """{ "Name": "Joe", "Age": 33, "Id": "704013b6-f37d-4d78-abc6-7094708d6a45" }""";
            SimpleSettings settings = new SimpleSettings(new InMemorySettingsSaver(previouslySavedSerialization));

            //Act
            settings.Reload();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
            settings.Name.Should().Be("Joe");
        }
    }
}
