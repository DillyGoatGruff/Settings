# Example

The settings class must have a default constructor that SettingsBase can call, but this constructor should not be called directly. Instead, a constructor accepting an ISettingsSaver as a parameter should be used.

``` cs
    internal class SettingsClass : SettingsBase<SettingsClass>
    {
        public int Id { get; set; } = 1;

        public string User { get; set; } = "jdoe";

        public int Age { get; set; } = 33;

		//Settings class must have default constructor that is called from SettingsBase.
		//Exception will be thrown if default constructor is called directly.
        [Obsolete("Do not use", true)]
        public SettingsClass() { }

        public SettingsClass(ISettingsSaver settingsSaver) : base(settingsSaver)
        {

        }
    }
```

The Settings.Saver library comes with two implementations of ISettingsSaver: 
1) FileSettingsSaver
	- Saves files to a file.
2) InMemorySettingsSaver
	- Does not save to disk, stores saved settings in memory. Used primarily for testing.

``` cs
            SettingsClass settings = new SettingsClass(new FileSettingsSaver(@"C:\settings.config"));
```

# Equality Checking

Calling Save() will call CheckIsDirty() and only performs a save operation if one or more of the properties have changed.

CheckIsDirty evaluates changes by value-type, not reference type. If the settings class contains an object, it will check whether the value-types within that object have changed. It also supports objects within objects for property evaluation.

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

	[Obsolete("Do not use", true)]
	public SettingsClass() { }

	public SettingsClass(ISettingsSaver settingsSaver) : base(settingsSaver)
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