using FluentAssertions;
using Settings;
using SettingsTests.ReferenceType.SettingsClasses;
using Xunit;

namespace SettingsTests.ReferenceType.Tests
{
    public class ReferenceTests5
    {

        [Fact]
        public void SaveTest()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser!.Name.First = "Joe";
            settings.Save();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void SavePropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = null;
            settings.Save();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void SavePropertyWithSubPropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser!.Name.Last = null;
            settings.Save();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ReloadTest()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser!.Name.First = "Joe";
            settings.Reload();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ReloadPropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = null;
            settings.Reload();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ReloadPropertyWithSubPropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser!.Name.Last = null;
            settings.Reload();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ResetTest()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser!.Name.First = "Joe";
            settings.Reset();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ResetPropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = null;
            settings.Reset();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void ResetPropertyWithSubPropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser!.Name.Last = null;
            settings.Reset();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
        public void IsDirtyTest_DifferentSubParameters()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser!.Name.First = "Joe";

            //Assert
            settings.CheckIsDirty().Should().BeTrue();
        }

        [Fact]
        public void IsDirtyTest_DifferentObjectSameParameters()
        {
            //Arrange
            ReferenceTypeClass5 settings = new ReferenceTypeClass5(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = new ReferenceTypeClass5.Person()
            {
                Name = new ReferenceTypeClass5.Name()
                {
                    First = settings.PrimaryUser!.Name.First,
                    Last = settings.PrimaryUser!.Name.Last
                },
                Age = settings.PrimaryUser.Age
            };

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }


    }
}
