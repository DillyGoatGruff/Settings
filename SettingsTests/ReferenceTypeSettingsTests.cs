using FluentAssertions;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SettingsTests
{
	public class ReferenceTypeSettingsTests
	{

		[Fact]
		public void SaveTest()
		{
			//Arrange
			ComplexSettingsClass settings = new ComplexSettingsClass(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser.FirstName = "Joe";
			settings.Save();

			//Assert
			settings.CheckIsDirty().Should().BeFalse();
		}

		[Fact]
		public void ReloadTest()
		{
			//Arrange
			ComplexSettingsClass settings = new ComplexSettingsClass(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser.FirstName = "Joe";
			settings.Reload();

			//Assert
			settings.CheckIsDirty().Should().BeFalse();
		}

		[Fact]
		public void ResetTest()
		{
			//Arrange
			ComplexSettingsClass settings = new ComplexSettingsClass(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser.FirstName = "Joe";
			settings.Reset();

			//Assert
			settings.CheckIsDirty().Should().BeFalse();
		}

		[Fact]
		public void IsDirtyTest_DifferentSubParameters()
		{
			//Arrange
			ComplexSettingsClass settings = new ComplexSettingsClass(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser.FirstName = "Joe";

			//Assert
			settings.CheckIsDirty().Should().BeTrue();
		}

		[Fact]
		public void IsDirtyTest_DifferentObjectSameParameters()
		{
			//Arrange
			ComplexSettingsClass settings = new ComplexSettingsClass(new InMemorySettingsSaver());

			//Act
			settings.PrimaryUser = new Person()
			{
				FirstName = settings.PrimaryUser.FirstName,
				Age = settings.PrimaryUser.Age
			};

			//Assert
			settings.CheckIsDirty().Should().BeFalse();
		}


	}
}
