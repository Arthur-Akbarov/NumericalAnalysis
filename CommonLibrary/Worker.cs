﻿using static System.Math;
using static System.String;

namespace NumericalAnalysis
{
	public static class Worker
	{
		// should rewrite
		public static string ToString(this double d, int maxLength)
		{
			string str = Format("{0:N" + Abs(maxLength) + "}", d);

			if (str.Length > Abs(maxLength))
				str = str.Substring(0, Abs(maxLength));

			if (str.Contains("."))
			{
				str = str.TrimEnd('0');

				if (str.EndsWith("."))
					str.Remove(str.Length - 1);
			}

			return (maxLength > 0)
				? str.TrimEnd('.').PadLeft(maxLength)
				: str.TrimEnd('.').PadRight(-maxLength);
		}
		public static int Factorial(int n)
		{
			int result = 1;

			for (int i = 1; i <= n; i++)
				result *= i;

			return result;
		}
		public static int Binomial(int n, int k)
		{
			if (k < 0 || n < k)
				return 0;

			k = Min(n, n - k);

			int result = 1;
			for (int i = 0; i < k; i++)
				result *= n - i;

			result /= Factorial(k);

			return result;
		}
		public static double[] GetX(double[] x, int n = 100)
		{
			return GetX(x[0], x[x.Length - 1], n);
		}
		public static double[] GetX(double a, double b, int n = 100)
		{
			double h = (b - a) / n;

			double[] x = new double[n + 1];
			for (int i = 0; i < x.Length; i++)
				x[i] = Round(a + i * h, 15);

			return x;
		}
		public static string Signed(this double a)
		{
			return a.ToString(Format(
				" + 0.{0}; - 0.{0}; + 0", new string('#', 2)));
		}
		public static void BeautifyLeadingSign(System.Text.StringBuilder s)
		{
			switch (s[1])
			{
				case '-':
					s.Remove(0, 1);
					s.Remove(1, 1);
					break;
				case '+':
					s.Remove(0, 3);
					break;
			}
		}
	}
}
