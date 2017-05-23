using System;
using System.Collections.Generic;
using static System.Console;
using static System.Math;

namespace NumericalAnalysis
{
	class Task5
	{
		static void Main(string[] args)
		{
			do
				Start();
			while (ReadKey().Key != ConsoleKey.Q);
		}

		static void Start()
		{
			N = Menu.ReadInt("Enter N = ", min: 1);
			Greeting();

			double[] x = Worker.GetX(a, b, 2 * N);
			y = f.Evaluate(x);

			foreach (var q in quadratures)
			{
				double nvalue = q();
				double error = Abs(exact - nvalue);
				double bound = ErrorBound(q);
				char s = (error < bound) ? '<'
					   : (error > bound) ? '>' : '=';

				Output(nvalue, error, s, bound, q.Method.Name);
			}
			WriteLine();
		}

		static void Greeting()
		{
			Clear();
			WriteLine("f(x) = {0}\n", f);
			WriteLine("exact value of integral from {0} to {1}", a, b);
			WriteLine(exact.ToString(-t));
			WriteLine();
			Write("{0,-" + (t + 3) + "}", "nvalue");
			Write("{0,-" + (t + 5) + "}", "error");
			Write("{0,-" + (t + 3) + "}", "errorBound");
			Write("{0,-" + (t + 3) + "}", "   N = " + N);
			WriteLine();
		}
		static void Output(double nvalue, double error, char s, double bound,
			string methodName)
		{
			Write("{0}   ", nvalue.ToString(-t));
			Write("{0}  ", error.ToString(-t));
			Write("{0}  ", s);
			Write("{0}   ", bound.ToString(-t));
			Write("{0}", methodName);
			WriteLine();
		}

		delegate double Quadrature();
		static double Simpson()
		{
			double result = y[0] + y[y.Length - 1];

			for (int i = 1; i <= y.Length - 2; i += 2)
				result += 4 * y[i];

			for (int i = 2; i <= y.Length - 3; i += 2)
				result += 2 * y[i];

			result *= (b - a) / 6 / N;

			return result;
		}
		static double Trapezoid()
		{
			double result = y[0] + y[y.Length - 1];

			for (int i = 2; i <= y.Length - 3; i += 2)
				result += 2 * y[i];

			result *= (b - a) / 2 / N;

			return result;
		}
		static double LeftRectangle() => Rectangle(0);
		static double CentralRectangle() => Rectangle(1);
		static double RightRectangle() => Rectangle(2);
		static double Rectangle(int start)
		{
			double result = 0;
			for (int i = start; i <= y.Length + start - 3; i += 2)
				result += y[i];

			result *= (b - a) / N;

			return result;
		}

		static double ErrorBound(Quadrature q)
		{
			double C = (q == Simpson) ? 1d / 2880
					 : (q == Trapezoid) ? 1d / 12
					 : (q == CentralRectangle) ? 1d / 24
					 : 1d / 2;

			int d = (q == Simpson) ? 3
				   : (q == Trapezoid) ? 1
				   : (q == CentralRectangle) ? 1
				   : 0;

			double M = f.DerBound(d + 1, a, b);

			return C * (b - a) * Pow((b - a) / N, d + 1) * M;
		}

		static int N;
		const int t = 13;
		static double[] y;
		const double a = 0, b = 1;
		static AFunc f = Functions.GetIntegrableFunc();
		static double exact = Functions.Integrate(a, b);
		static readonly List<Quadrature> quadratures = new List<Quadrature>() {
		 LeftRectangle, RightRectangle, CentralRectangle, Trapezoid, Simpson };
	}
}
