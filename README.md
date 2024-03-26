# 2.0 Breaking Changes
- Classes inheriting `SettingsBase<T>` no loner require a default constructor
- Default values cannot be set in the constructor
	- Default values must either be in-line assignments to properties or assigned in the `InitializeDefaultValues()`
- Default constructors are no longer required for the settings class
	- Default constructor of `SettingsBase<T>` uses a `FileSettingsSaver` that saves in the same directory as the assembly it is being executed in and a filename the same as the executable, but with a `.cfg` extension
	- Any classes contained within the settings class that is inheriting `SettingsBase<T>` must still have a default constructor

# Example
Default values must be assign inline (like shown below for the `Age` property) or within the `InitializeDefaultValues()` method. In the below example the default values would be:
- Age = 33
- FirstName = "Joe"
- LastName = `null`
Because values are being set in the constructor, a call to `IsDirtyCheck()` would return `true` because the default value for Age is 33, but after instantiating the ExampleClass object, Age would be 45 (similarly, LastName would be "Doe" and not the default value of `null`).
``` cs
internal class ExampleClass : SettingsBase<ExampleClass>
{
	public int Age { get; set; } = 33;

	public string FirstName { get; set; }
	public string? LastName { get; set; }

	public ExampleClass()
	{
		Age = 45;
		LastName = "Doe";
	}

	public override void InitializeDefaultValues()
	{
		FirstName = "Joe";
	}
}		 

```

The Settings.Saver library comes with two implementations of ISettingsSaver: 
1) FileSettingsSaver
	- Saves settings to a file.
2) InMemorySettingsSaver
	- Does not save to disk, stores saved settings in memory. Used primarily for testing.

``` cs
SimpleSettings settings = new SimpleClass(new FileSettingsSaver(@"C:\settings.cfg"));




internal class SimpleSettings : SettingsBase<SimpleSettings>
{
	public int Age { get; set; } = 33;

	public SimpleSettings(ISettingsSaver settingsSaver) : base(settingsSaver)
	{
	}

	public override void InitializeDefaultValues()
	{
		
	}
}		 
```

# Equality Checking

Calling Save() will call `CheckIsDirty()` and only performs a save operation if one or more of the properties have changed.

`CheckIsDirty()` evaluates changes by value-type, not reference type. If the settings class contains an object, it will check whether the value-types within that object have changed. It also supports objects within objects for property evaluation.

``` cs
internal class Address
{
	public string State { get; set; }

	public string City { get; set; }

	//A default constructor is required
	public Address()
	{
	
	}
}

internal class User
{
	public int Id { get; set; }

	public string User { get; set; }

	public int Age { get; set; }	

	public Address Address{ get; set; } = new Address();

	//A default constructor is required
	public User()
	{
	
	}
}


internal class SettingsClass : SettingsBase<SettingsClass>
{
	public User TheUser { get; set; } = new User();

	public SettingsClass(ISettingsSaver settingsSaver) : base(settingsSaver)
	{

	}
	
	public override void InitializeDefaultValues()
	{

	}
}

```

``` cs

SettingsClass settings = new (new InMemorySettingsSaver());
settings.TheUser = new User()
{
	Id = 1,
	Name = "John Doe",
	Age = 54,
	Address = new Address()
	{
		State = "IL",
		City = "Chicago"
	}
}
//Save settings
settings.Save();

// Create a new Address object, but set the properties to the same values as the Address that was saved.
settings.TheUser.Address = new Address()
{
	State = "IL",
	City = "Chicago"
}


bool hasChanged = settings.CheckIsDirty(); //hasChanged is false

```

# Disclaimer

SettingsSaver does not support properties that are arrays, lists, or collections.