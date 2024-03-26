using FluentAssertions;
using Settings;
using SettingsTests.ReferenceType.SettingsClasses;
using Xunit;

namespace SettingsTests.ReferenceType.Tests
{
    public class ReferenceTests3
    {

        [Fact]
        public void SaveTest()
        {
            //Arrange
            ReferenceTypeClass3 settings = new ReferenceTypeClass3(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass3.Person() { FirstName = "Joe", Age = 33 };
            settings.Save();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void SavePropertyAsNotNull()
        {
            //Arrange
            ReferenceTypeClass3 settings = new ReferenceTypeClass3(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass3.Person() { FirstName = "Bob", Age = 33 };
            settings.Save();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ReloadTest()
        {
            //Arrange
            ReferenceTypeClass3 settings = new ReferenceTypeClass3(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass3.Person();
            settings.Reload();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ReloadPropertyAsNotNull()
        {
            //Arrange
            ReferenceTypeClass3 settings = new ReferenceTypeClass3(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass3.Person() { FirstName = "Bob", Age = 33 };
            settings.Reload();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ResetTest()
        {
            //Arrange
            ReferenceTypeClass3 settings = new ReferenceTypeClass3(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass3.Person();
            settings.Reset();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ResetPropertyAsNotNull()
        {
            //Arrange
            ReferenceTypeClass3 settings = new ReferenceTypeClass3(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass3.Person() { FirstName = "Bob", Age = 33 };
            settings.Reset();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void IsDirtyTest_DifferentSubParameters()
        {
            //Arrange
            ReferenceTypeClass3 settings = new ReferenceTypeClass3(new InMemorySettingsSaver());
            settings.PrimaryUser = new ReferenceTypeClass3.Person() { FirstName = "Bob", Age = 33 };
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
            ReferenceTypeClass3 settings = new ReferenceTypeClass3(new InMemorySettingsSaver());
            settings.PrimaryUser = new ReferenceTypeClass3.Person() { FirstName = "Bob", Age = 33 };
            settings.Save();

            //Act
            settings.PrimaryUser = new ReferenceTypeClass3.Person()
            {
                FirstName = settings.PrimaryUser.FirstName,
                Age = settings.PrimaryUser.Age
            };

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }


    }
}
