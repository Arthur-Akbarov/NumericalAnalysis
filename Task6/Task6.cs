using System.Collections.Generic;
using static System.Console;
using static System.Math;

namespace NumericalAnalysis
{
	class Task6
	{
		static void Main(string[] args)
		{
			Start_1();
			Start_2();
			ReadKey();
		}

		static void Start_1()
		{
			double exact = Functions.Integrate(a, b);
			Greeting(exact);

			foreach (var q in quadratures5)
			{
				double nvalue = q();
				double error = Abs(exact - nvalue);
				double bound = ErrorBound(q);
				char s = (error < bound) ? '<'
					   : (error > bound) ? '>' : '=';

				Output(nvalue, error, s, bound, q.Method.Name);
			}
			WriteLine();
			WriteLine();
		}
		static void Greeting(double exact)
		{
			WriteLine("Part I");
			WriteLine(new string('*', 3 * t + 18));
			WriteLine("f(x) = {0}\n", f);
			WriteLine("exact value of integral from {0} to {1}", a, b);
			WriteLine(exact.Formatted(-t));
			WriteLine();
			//WriteLine(new string(' ', t + 3) + "error");
			Write("{0,-" + (t + 3) + "}", "nvalue");
			//Write("{0,-7}", "order");
			Write("{0,-" + (t + 5) + "}", "error");
			Write("{0,-" + (t + 3) + "}", "errorBound");
			Write("N = 5");
			WriteLine();
		}
		delegate double Quadrature5();
		static double Simpson()
		{
			int N = 2;
			double[] x = Worker.GetX(a, b, 2 * N);
			double[] y = f.Evaluate(x);

			double result = y[0] + 4 * y[1] + 2 * y[2] + 4 * y[3] + y[4];

			result *= (b - a) / 6 / N;

			return result;
		}
		static double Gauss()
		{
			double[] A = GetA();
			double[] x = GetX(a, b);
			double[] y = f.Evaluate(x);

			double result = 0;
			for (int i = 0; i < y.Length; i++)
				result += A[i] * y[i];

			result *= (b - a) / 2;

			return result;
		}
		static double[] GetA()
		{
			double[] A = { (322 - 13 * Sqrt(70)) / 900,
						   (322 + 13 * Sqrt(70)) / 900,
						   128d / 225,
						   (322 + 13 * Sqrt(70)) / 900,
						   (322 - 13 * Sqrt(70)) / 900 };

			return A;
		}
		static double[] GetX(double a, double b)
		{
			double[] t = { -Sqrt(5 + 2 * Sqrt(10d / 7)) / 3,
						   -Sqrt(5 - 2 * Sqrt(10d / 7)) / 3,
							0,
							Sqrt(5 - 2 * Sqrt(10d / 7)) / 3,
							Sqrt(5 + 2 * Sqrt(10d / 7)) / 3 };

			double[] x = new double[t.Length];
			for (int i = 0; i < x.Length; i++)
				x[i] = t[i] * (b - a) / 2 + (b + a) / 2;

			return x;
		}
		static double ErrorBound(Quadrature5 q)
		{
			int N = 2;
			if (q == Simpson)
				return (b - a) / 2880 * Pow((b - a) / N, 4)
					* Functions.IntegrableDerBound(2 * N);

			N = 5;
			return Pow((b - a) / 2, 2 * N + 1)
					* Pow(2, 2 * N + 1) * Pow(Worker.Factorial(N), 4)
					/ (2 * N + 1) / Pow(Worker.Factorial(2 * N), 3)
					* Functions.IntegrableDerBound(2 * N);
		}
		static void Output(double nvalue, double error, char s, double bound,
			string methodName)
		{
			Write("{0}   ", nvalue.Formatted(-t));
			//Write("{0,-7}", Ceiling(-Log(Abs(Abs(exact - nvalue)), 10)));
			Write("{0}  ", error.Formatted(-t));
			Write("{0}  ", s);
			Write("{0}   ", bound.Formatted(-t));
			Write("{0}", methodName);
			WriteLine();
		}

		static void Start_2()
		{
			F f = t => Sin(t);

			G mu = (i) => 4d / (4 * i + 3);
			Greeting(a, b);

			double[] x = { 0.25, 0.5, 0.75 };
			Polynomial w = GetW(x);
			double[] A = GetA(mu, x, a, b);
			Output(f, A, x, "Given");

			w = GetW(mu, x.Length);
			x = w.GetRoots(a, b);
			A = GetA(mu, x, a, b);
			Output(f, A, x, "Gaussian");

			WriteLine();
		}
		static Polynomial GetW(double[] root)
		{
			var result = new Polynomial(1);

			for (int i = 0; i < root.Length; i++)
				result *= new Polynomial(-root[i], 1);

			return result;
		}
		static void Greeting(double a, double b)
		{
			WriteLine("Part II");
			WriteLine(new string('*', 3 * t + 18));
			WriteLine("f(x) = {0}\n", f);
			WriteLine("exact value of integral from {0} to {1}", a, b);
			WriteLine(exact2.Formatted(-t));
			WriteLine();
			Write("{0,-" + (t + 3) + "}", "nvalue");
			Write("{0,-" + (t + 3) + "}", "error");
			Write("{0,-" + (t + 3) + "}", "N = 3");
			WriteLine();
		}
		static Polynomial GetW(G mu, int n)
		{
			var gauss = new GausMethod(
				n, n,
				(i, j) => mu((n - 1 + i - j)),
				(i) => -mu(n + i));

			double[] arr = new double[n + 1];
			for (int i = 0; i < n; i++)
				arr[i] = gauss.Answer[n - 1 - i];

			arr[n] = 1;

			return new Polynomial(arr);
		}
		static double[] GetA(G mu, double[] x, double a, double b)
		{
			double[] result = new double[x.Length];

			for (int k = 0; k < result.Length; k++)
			{
				var p = new Polynomial(1);

				for (int i = 0; i < x.Length; i++)
					if (i != k)
						p *= new Polynomial(-x[i], 1) / (x[k] - x[i]);

				for (int i = 0; i < result.Length; i++)
					result[k] += p.a[i] * mu(i);
			}

			return result;
		}
		static void Output(F f, double[] A, double[] x, string methodName)
		{
			double nvalue = QuadratureEval(f, A, x);
			Write("{0}   ", nvalue.Formatted(-t));
			Write("{0}   ", Abs(nvalue - exact2).Formatted(-t));
			Write("{0}", methodName);
			WriteLine();
		}
		static double QuadratureEval(F f, double[] A, double[] x)
		{
			double result = 0;

			for (int k = 0; k < A.Length; k++)
				result += f(x[k]) * A[k];

			return result;
		}

		const int t = 15;
		const double a = 0, b = 1;
		delegate double G(int i);
		delegate double F(double x);
		static AF f = Functions.GetIntegrableFunc();
		const double exact2 = 0.5284080848967414;
		static readonly List<Quadrature5> quadratures5 =
			new List<Quadrature5>() { Gauss, Simpson };
	}
}
