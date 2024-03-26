using FluentAssertions;
using Settings;
using SettingsTests.ReferenceType.SettingsClasses;
using Xunit;

namespace SettingsTests.ReferenceType.Tests
{
    public class ReferenceTests6
    {

        [Fact]
        public void SavePropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass6 settings = new ReferenceTypeClass6(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = null;
            settings.Save();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
            settings.PrimaryUser.Should().BeNull();
        }

        [Fact]
        public void SavePropertyWithSubPropertyAsNotNull()
        {
            //Arrange
            ReferenceTypeClass6 settings = new ReferenceTypeClass6(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser!.Name = new ReferenceTypeClass6.Name() { First = "Joe" };
            settings.Save();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
            settings.PrimaryUser.Should().NotBeNull();
            settings.PrimaryUser.Name.First.Should().Be("Joe");
        }

        [Fact]
        public void ReloadPropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass6 settings = new ReferenceTypeClass6(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = null;
            settings.Reload();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
            settings.PrimaryUser.Should().NotBeNull();
            settings.PrimaryUser!.Name.Should().BeNull();
        }

        [Fact]
        public void ReloadPropertyWithSubPropertyAsNotNull()
        {
            //Arrange
            ReferenceTypeClass6 settings = new ReferenceTypeClass6(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser!.Name = new ReferenceTypeClass6.Name() { First = "Joe" };
            settings.Reload();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
            settings.PrimaryUser.Should().NotBeNull();
            settings.PrimaryUser.Name.Should().BeNull();
        }

        [Fact]
        public void ResetPropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass6 settings = new ReferenceTypeClass6(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = null;
            settings.Reset();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
            settings.PrimaryUser.Should().NotBeNull();
            settings.PrimaryUser!.Name.Should().BeNull();
        }

        [Fact]
        public void ResetPropertyWithSubPropertyAsNotNull()
        {
            //Arrange
            ReferenceTypeClass6 settings = new ReferenceTypeClass6(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser!.Name = new ReferenceTypeClass6.Name() { First = "Joe" };
            settings.Reset();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
            settings.PrimaryUser.Should().NotBeNull();
            settings.PrimaryUser.Name.Should().BeNull();
        }

        [Fact]
        public void IsDirtyTest_DifferentObjectSameParameters()
        {
            //Arrange
            ReferenceTypeClass6 settings = new ReferenceTypeClass6(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass6.Person()
            {
                Name = settings.PrimaryUser!.Name,
                Age = settings.PrimaryUser.Age
            };

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }


    }
}
