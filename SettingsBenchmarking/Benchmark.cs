
using BenchmarkDotNet.Attributes;
using Settings;

namespace SettingsBenchmarking
{
	[MemoryDiagnoser]
	public class Benchmark
	{

		private Settings _settings;
		private Settings _changedSettings;


		[GlobalSetup]
		public void Setup()
		{
			_settings = new Settings(new InMemorySettingsSaver());
			_changedSettings = new Settings(new InMemorySettingsSaver())
			{
				id = new Guid(),
				Name = "Joe",
				Age = 27
			};
		}

		[Benchmark]
		public bool CheckIsDirty()
		{
			return _settings.CheckIsDirty();
		}

		[Benchmark]
		public bool Reload()
		{
			return _settings.Reload();
		}

		[Benchmark]
		public bool Save()
		{
			return _changedSettings.Save();
		}

		[Benchmark]
		public void Reset()
		{
			_settings.Reset();
		}
	}
}
