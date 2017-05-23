using System;
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

		static bool IsSatisfy<T>(T a, T min, T max)
			where T : IComparable
		{
			if (a.CompareTo(min) >= 0 && a.CompareTo(max) <= 0)
				return true;

			if (min.Equals(minValue[typeof(T)]))
				WriteLine("{0} isn't <= {1}", a, max);
			else
			if (max.Equals(maxValue[typeof(T)]))
				WriteLine("{0} isn't >= {1}", a, min);
			else
				WriteLine("{0} doesn't belong to [{1},{2}]", a, min, max);

			return false;
		}

		static readonly Dictionary<Type, object> maxValue =
			new Dictionary<Type, object>()
			{
				{ typeof(int), int.MaxValue },
				{ typeof(double), double.MaxValue }
			};
		static readonly Dictionary<Type, object> minValue =
			new Dictionary<Type, object>()
			{
				{ typeof(int), int.MinValue },
				{ typeof(double), double.MinValue }
			};
	}
}
