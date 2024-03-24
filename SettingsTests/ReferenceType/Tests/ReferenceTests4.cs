using FluentAssertions;
using Settings;
using SettingsTests.ReferenceType.SettingsClasses;
using Xunit;

namespace SettingsTests.ReferenceType.Tests
{
    public class ReferenceTests4
    {

        [Fact]
        public void SaveTest()
        {
            //Arrange
            ReferenceTypeClass4 settings = new ReferenceTypeClass4(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass4.Person() { FirstName = "Joe", Age = 33 };
            settings.Save();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ReloadTest()
        {
            //Arrange
            ReferenceTypeClass4 settings = new ReferenceTypeClass4(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass4.Person();
            settings.Reload();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ResetTest()
        {
            //Arrange
            ReferenceTypeClass4 settings = new ReferenceTypeClass4(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass4.Person();
            settings.Reset();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void IsDirtyTest_DifferentSubParameters()
        {
            //Arrange
            ReferenceTypeClass4 settings = new ReferenceTypeClass4(new InMemorySettingsSaver());
            settings.PrimaryUser = new ReferenceTypeClass4.Person() { FirstName = "Bob", Age = 33 };
            settings.Save();

            //Act
            settings.PrimaryUser.FirstName = "Joe";

            //Assert
            settings.CheckIsDirty().Should().BeTrue();
        }

        [Fact]
        public void IsDirtyTest_DifferentObjectSameParameters()
        {
            //Arrange
            ReferenceTypeClass4 settings = new ReferenceTypeClass4(new InMemorySettingsSaver());
            settings.PrimaryUser = new ReferenceTypeClass4.Person() { FirstName = "Bob", Age = 33 };
            settings.Save();

            //Act
            settings.PrimaryUser = new ReferenceTypeClass4.Person()
            {
                FirstName = settings.PrimaryUser.FirstName,
                Age = settings.PrimaryUser.Age
            };

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }


    }
}
