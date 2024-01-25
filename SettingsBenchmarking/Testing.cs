using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsBenchmarking
{
	[MemoryDiagnoser]
	public class Testing
	{

		static object m_obj = new Testing();

		[Benchmark]
		public string GetString()
		{
			return m_obj.GetType().Name;
		}

		[Benchmark]	
		public Type GetType()
		{
			return m_obj.GetType();
		}


	}
}
