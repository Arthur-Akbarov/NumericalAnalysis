using System;
using System.Collections.Generic;
using static System.Math;
using static System.Console;
using static System.String;

namespace NumericalAnalysis
{
	class Task8
	{
		const int k = 5;

		static void Main(string[] args)
		{
			do
				Start();
			while (ReadKey().Key != ConsoleKey.F4);
		}
		static void Start(int idEquation = 0)
		{
			Y = GetSolution(idEquation);
			WriteLine(solution);

			n = Menu.ReadInt("Enter n: ", min: k + 1);
			x = Worker.GetX(a, b, n);
			h = (b - a) / n;

			for (int i = 0; i < Methods.Count; i++)
			{
				yy[i] = Methods[i](n);
				Plot(x, yy[i], Methods[i].Method.Name);
			}

			Output(yy);
		}

		static G GetSolution(int idEquation)
		{
			if (idEquation == 0)
			{
				a = 1;
				b = 2;
				f = (x, y) => 3 * y / x + x * x * x + x;

				double y1 = 3;
				y0 = Pow(a, 4) - a * a + y1 * Abs(Pow(a, 3));
				solution = "x^4 - x^2 + " + y1 + "*abs(x^3)";
				return x => Pow(x, 4) - x * x + y1 * Abs(Pow(x, 3));
			}
			else
			{
				a = 0;
				b = 2;
				f = (x, y) => y;

				y0 = Exp(a);
				solution = y0 / Exp(a) + "*exp(x)";
				return t => y0 / Exp(a) * Exp(t);
			}
		}
		static void Output(double[][] yy)
		{
			for (int j = 0; j < Methods.Count; j++)
				Write("{0,-11}", Methods[j].Method.Name.Substring(0, 5));

			WriteLine();

			for (int i = 0; i < n + 1; i++)
			{
				for (int j = 0; j < Methods.Count; j++)
					Write("{0:e2}  ", Abs(Y(x[i]) - yy[j][i]));

				WriteLine();
			}
			WriteLine();
		}

		delegate double[] Method(int m);
		static double[] Euler(int m)
		{
			double[] x = Worker.GetX(a, b, n);
			double[] y = new double[m + 1];
			y[0] = y0;

			for (int i = 0; i < y.Length - 1; i++)
				y[i + 1] = y[i] + h * f(x[i], y[i]);

			return y;
		}
		static double[] ModifiedEuler(int m)
		{
			double[] x = Worker.GetX(a, b, n);
			double[] y = new double[m + 1];
			y[0] = y0;

			for (int i = 0; i < y.Length - 1; i++)
				y[i + 1] = y[i] + h * f(x[i] + h / 2,
										y[i] + h / 2 * f(x[i], y[i]));

			return y;
		}
		static double[] Trapezoidal(int m)
		{
			double[] y = new double[m + 1];
			y[0] = y0;

			for (int i = 0; i < y.Length - 1; i++)
				y[i + 1] = y[i] + h / 2 * (f(x[i], y[i]) +
					f(x[i + 1], y[i] + h * f(x[i], y[i])));

			return y;
		}
		static double[] RungeKutta(int m)
		{
			double[] x = Worker.GetX(a, b, n);
			double[] y = new double[m + 1];
			y[0] = y0;

			for (int i = 0; i < y.Length - 1; i++)
			{
				double k1 = f(x[i], y[i]);
				double k2 = f(x[i] + h / 2, y[i] + h / 2 * k1);
				double k3 = f(x[i] + h / 2, y[i] + h / 2 * k2);
				double k4 = f(x[i] + h, y[i] + h * k3);

				y[i + 1] = y[i] + h / 6 * (k1 + 2 * k2 + 2 * k3 + k4);
			}

			return y;
		}
		static double[] Adams(int m)
		{
			double[] x = Worker.GetX(a, b, n);
			double[] y = new double[m + 1];
			Buffer.BlockCopy(RungeKutta(k), 0, y, 0, (k + 1) * sizeof(double));

			double[,] q = GetQ(y);
			double[] A = GetA();

			for (int i = k; i < y.Length - 1; i++)
			{
				q[i, 0] = h * f(x[i], y[i]);
				y[i + 1] = y[i] + A[0] * q[i, 0];

				for (int j = 1; j <= k; j++)
				{
					q[i - j, j] = q[i - j + 1, j - 1] - q[i - j, j - 1];
					y[i + 1] += A[j] * q[i - j, j];
				}
			}

			return y;
		}

		static double[,] GetQ(double[] y)
		{
			double[,] result = new double[n, k + 1];
			for (int i = 0; i < k; i++)
				result[i, 0] = h * f(x[i], y[i]);

			for (int j = 1; j <= k; j++)
				for (int i = k - j; i >= 0; i--)
					result[i, j] = result[i + 1, j - 1] - result[i, j - 1];

			return result;
		}
		static double[] GetA()
		{
			double[] result = new double[k + 1];
			result[0] = 1;

			var p = new Polynomial(1);

			for (int j = 1; j <= k; j++)
			{
				p *= new Polynomial(j - 1, 1);
				result[j] = p.Integrate(0, 1) / Worker.Factorial(j);
			}

			return result;
		}

		static void Plot(double[] x, double[] y0, string methodName)
		{
			double[] y1 = new double[x.Length];
			for (int i = 0; i < x.Length; i++)
				y1[i] = Y(x[i]);

			string kk = (methodName == "Adams") ? ", k = " + k : "";

			string title = Format("{0}\\nn = {1}{2}\\nf(x) = {3}",
				methodName, n, kk, solution);

			string functions = "\"data0\" title \"" + methodName + "\"" +
							 ", \"data1\" title \"actual\"";

			Gnuplot.Run(x, x, functions, title, "x", y0, y1);
		}

		static F f;
		static G Y;
		static int n;
		static double a, b, h, y0;
		static double[] x;
		static string solution;
		delegate double G(double x);
		delegate double F(double x, double y);
		static readonly List<Method> Methods = new List<Method> {
			Euler, ModifiedEuler, Trapezoidal, RungeKutta, Adams };
		static double[][] yy = new double[Methods.Count][];
	}
}
// http://an-site.ru/kr/km2.htm
