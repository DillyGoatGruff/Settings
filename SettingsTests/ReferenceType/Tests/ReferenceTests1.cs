using FluentAssertions;
using Settings;
using SettingsTests.ReferenceType.SettingsClasses;
using Xunit;

namespace SettingsTests.ReferenceType.Tests
{
    public class ReferenceTests1
	{

		[Fact]
		public void SaveTest()
		{
			//Arrange
			ReferenceTypeClass1 settings = new ReferenceTypeClass1(new InMemorySettingsSaver());

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
            ReferenceTypeClass1 settings = new ReferenceTypeClass1(new InMemorySettingsSaver());

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
			ReferenceTypeClass1 settings = new ReferenceTypeClass1(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser!.FirstName = "Joe";
			settings.Reload();

			//Assert
			settings.CheckIsDirty().Should().BeFalse();
		}

		[Fact]
		public void ResetTest()
		{
			//Arrange
			ReferenceTypeClass1 settings = new ReferenceTypeClass1(new InMemorySettingsSaver());

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
			ReferenceTypeClass1 settings = new ReferenceTypeClass1(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser!.FirstName = "Joe";

			//Assert
			settings.CheckIsDirty().Should().BeTrue();
		}

		[Fact]
		public void IsDirtyTest_DifferentObjectSameParameters()
		{
			//Arrange
			ReferenceTypeClass1 settings = new ReferenceTypeClass1(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser = new ReferenceTypeClass1.Person()
			{
				FirstName = settings.PrimaryUser!.FirstName,
				Age = settings.PrimaryUser.Age
			};

			//Assert
			settings.CheckIsDirty().Should().BeFalse();
		}

	}
}
