using FluentAssertions;
using Settings;
using SettingsTests.ReferenceType.SettingsClasses;
using Xunit;

namespace SettingsTests.ReferenceType.Tests
{
    public class ReferenceTests2
	{

		[Fact]
		public void SaveTest()
		{
			//Arrange
			ReferenceTypeClass2 settings = new ReferenceTypeClass2(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser!.FirstName = "Joe";
			settings.Save();

			//Assert
			settings.CheckIsDirty().Should().BeFalse();
		}

        [Fact]
        public void SavePropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass2 settings = new ReferenceTypeClass2(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = null;
            settings.Save();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
		public void ReloadTest()
		{
			//Arrange
			ReferenceTypeClass2 settings = new ReferenceTypeClass2(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser!.FirstName = "Joe";
			settings.Reload();

			//Assert
			settings.CheckIsDirty().Should().BeFalse();
		}

        [Fact]
        public void ReloadPropertyAsNull()
        {
            //Arrange
            ReferenceTypeClass2 settings = new ReferenceTypeClass2(new InMemorySettingsSaver());

            //Act
            settings.PrimaryUser = null;
			settings.Reload();

            //Assert
            settings.CheckIsDirty().Should().BeFalse();
        }

        [Fact]
		public void ResetTest()
		{
			//Arrange
			ReferenceTypeClass2 settings = new ReferenceTypeClass2(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser!.FirstName = "Joe";
			settings.Reset();

			//Assert
			settings.CheckIsDirty().Should().BeFalse();
		}

		[Fact]
		public void IsDirtyTest_DifferentSubParameters()
		{
			//Arrange
			ReferenceTypeClass2 settings = new ReferenceTypeClass2(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser!.FirstName = "Joe";

			//Assert
			settings.CheckIsDirty().Should().BeTrue();
		}

		[Fact]
		public void IsDirtyTest_DifferentObjectSameParameters()
		{
			//Arrange
			ReferenceTypeClass2 settings = new ReferenceTypeClass2(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser = new ReferenceTypeClass2.Person()
			{
				FirstName = settings.PrimaryUser!.FirstName,
				Age = settings.PrimaryUser.Age
			};

			//Assert
			settings.CheckIsDirty().Should().BeFalse();
		}


	}
}
