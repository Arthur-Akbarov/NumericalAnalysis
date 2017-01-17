using System.Collections.Generic;
using static System.Console;

namespace NumericalAnalysis
{
	public static class Menu
	{
		/// <param name="s">приветственная строка</param>
		public static int ReadInt(string s = "",
			int min = int.MinValue,
			int max = int.MaxValue)
		{
			int n = 0;
			bool got = false;
			while (!got)
			{
				Write(s);
				try
				{
					n = int.Parse(ReadLine());
					got = IsSatisfy(n, min, max);
				}
				catch { WriteLine("Integer expression expected!"); }
			}
			
			return n;
		}
		/// <param name="s">приветственная строка</param>
		public static double ReadDouble(string s = "",
			double min = double.MinValue,
			double max = double.MaxValue)
		{
			double x = 0;
			bool got = false;
			while (!got)
			{
				Write(s);
				try
				{
					x = double.Parse(ReadLine());
					got = IsSatisfy(x, min, max);
				}
				catch { WriteLine("Double expression expected!"); }
			}

			return x;
		}

		static bool IsSatisfy(dynamic a, dynamic min, dynamic max)
		{
			if (min <= a && a <= max)
				return true;

			if (min == a.GetType().GetField("MinValue").GetValue(null))
				WriteLine("{0} isn't <= {1}", a, max);
			else
			if (max == a.GetType().GetField("MaxValue").GetValue(null))
				WriteLine("{0} isn't >= {1}", a, min);
			else
				WriteLine("{0} doesn't belong to [{1},{2}]", a, min, max);

			return false;
		}
		internal class MaxValueCache
		{
			private static readonly Dictionary<System.Type, object> maxValues =
				new Dictionary<System.Type, object>()
			{
				{ typeof(int), int.MaxValue},
				{ typeof(double), double.MaxValue}
			};

			public static object MaxValue(System.Type type)
			{
				return maxValues[type];
			}
		}
	}
}
