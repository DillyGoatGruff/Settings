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
	public class ListTests
	{


		[Fact]
		public void Computer_Test()
		{
			//Arrange
			CollectionsContainingClass settings = new CollectionsContainingClass(new InMemorySettingsSaver());

			//Act
			settings.Computers.Add(new Computer() { Name = "Test" });


			//Assert
			settings.CheckIsDirty().Should().BeTrue();
		}

	}
}
