using System;
using static System.Math;
using static System.Console;
using static System.String;

namespace NumericalAnalysis
{
	class Task2
	{
		// Start( n  X  [idFunc = 0] [m = 100] )
		static void Main()
		{
			WriteLine("[a,b] = [{0},{1}]", x0, x0 + l);
			do
			{
				int n = Menu.ReadInt("Enter polynomial degree: ",
					0, Polynomial.maxDegree);

				double X = Menu.ReadDouble("Enter point: ", x0, x0 + l);

				WriteLine("Error: {0:E2}\n", Start(n, X));
			}
			while (ReadKey().Key != ConsoleKey.F4);
		}

		static double Start(int n, double X, int idFunc = 0, int m = 100)
		{
			double h = l / m;
			double[] x = new double[m + 1];
			for (int i = 0; i < m + 1; i++)
				x[i] = Round(x0 + h * i, 15);

			AF f = Functions.Get(idFunc);

			double d = (X - x[0]) / h;
			int start = (int)Truncate(d);
			start += (d - start <= 0.5) ? 0 : 1;

			Method method = GetTableMethod(m, d, start);

			double[] z = GetZ(method, x, n, start);
			double[] y = f.Evaluate(z);

			Polynomial p = method(z, y);

			#region correction
			if (p.Deg < n)
			{
				d = (z[0] == x[0])
					? (x[0] - x[0]) / h
					: (x[m] - x[0]) / h;

				start = (int)Truncate(d);
				start += (d - start <= 0.5) ? 0 : 1;

				method = GetTableMethod(m, d, start);

				z = GetZ(method, x, n, start);
				y = f.Evaluate(z);

				p = method(z, y);
			}
			#endregion

			Polynomial errorP = Task1.ErrorEstimate(f, z);
			double error = Abs(p.Eval(X) - f.Eval(X));

			Plot(f, p, errorP, z, method.Method.Name, X, error);
			return error;
		}

		static Method GetTableMethod(int m, double d, int start)
		{
			if (0 <= d && d <= 1)
				return Forward;

			if (m - 1 <= d && d <= m)
				return Backward;

			if (start <= d)
				return MiddleUp;
			else
				return MiddleDown;
		}

		delegate Polynomial Method(double[] x, double[] y);
		static Polynomial Forward(double[] x, double[] y)
		{
			var result = new Polynomial(y[0]);

			for (int i = 0; i < x.Length - 1; i++)
			{
				var l = new Polynomial(1);
				double h = x[1] - x[0];

				for (int j = 0; j <= i; j++)
					l *= (new Polynomial(-x[0], 1) / h - j) / (j + 1);

				result += l * FiniteDiff(y, 0, i + 1);
			}

			return result;
		}
		static Polynomial Backward(double[] x, double[] y)
		{
			int n = x.Length - 1;
			var result = new Polynomial(y[n]);

			for (int i = 0; i < x.Length - 1; i++)
			{
				var l = new Polynomial(1);
				double h = x[1] - x[0];

				for (int j = 0; j <= i; j++)
					l *= (new Polynomial(-x[n], 1) / h + j) / (j + 1);

				result += l * FiniteDiff(y, n - i - 1, i + 1);
			}

			return result;
		}
		static Polynomial MiddleUp(double[] x, double[] y)
		{
			int start = (x.Length - 1) / 2;
			var result = new Polynomial(y[start]);

			for (int i = 0; i < x.Length - 1; i++)
			{
				var l = new Polynomial(1);
				double h = x[1] - x[0];

				for (int j = 0; j <= i; j++)
				{
					var t = new Polynomial(-x[start], 1) / h;
					int next = ((j + 1) % 2 == 0 ? 1 : -1) * (j + 1) / 2;
					l *= (t - next) / (j + 1);
				}

				result += l * FiniteDiff(y, start - (i + 1) / 2, i + 1);
			}

			return result;
		}
		static Polynomial MiddleDown(double[] x, double[] y)
		{
			int start = x.Length / 2;
			var result = new Polynomial(y[start]);

			for (int i = 0; i < x.Length - 1; i++)
			{
				var l = new Polynomial(1);
				double h = x[1] - x[0];

				for (int j = 0; j <= i; j++)
				{
					var t = new Polynomial(-x[start], 1) / h;
					int next = ((j + 1) % 2 == 0 ? 1 : -1) * (j + 1) / 2;
					l *= (t + next) / (j + 1);
				}

				result += l * FiniteDiff(y, start - (i + 2) / 2, i + 1);
			}

			return result;
		}

		static double[] GetZ(Method method, double[] x, int n, int start)
		{
			int m = x.Length - 1;
			n = Min(m, n);
			double[] z = new double[n + 1];

			if (method == Forward)
				Buffer.BlockCopy(x, 0, z, 0, (n + 1) * sizeof(double));
			else
			if (method == Backward)
				Buffer.BlockCopy(x, (m - n) * sizeof(double),
							  z, 0, (n + 1) * sizeof(double));
			else
			if (method == MiddleUp)
			{
				n = Min(Min(2 * start + 1, 2 * (m - start)), n);

				Array.Resize(ref z, n + 1);
				Buffer.BlockCopy(x, (start - n / 2) * sizeof(double),
									  z, 0, (n + 1) * sizeof(double));
			}
			else // MiddleDown
			{
				n = Min(Min(2 * start, 2 * (m - start) + 1), n);

				Array.Resize(ref z, n + 1);
				Buffer.BlockCopy(x, (start - (n + 1) / 2) * sizeof(double),
											z, 0, (n + 1) * sizeof(double));
			}
			return z;
		}
		static double FiniteDiff(double[] y, int start, int order)
		{
			return order == 0
				? y[start]
				: FiniteDiff(y, start + 1, order - 1)
					- FiniteDiff(y, start, order - 1);
		}

		static void Plot(AF f, AF p, AF errorF, double[] x,
			string methodName, double X, double error)
		{
			double[] xx = Worker.GetX(x);
			double[] y0 = (f - p).AbsEvaluate(xx);
			double[] y1 = errorF.AbsEvaluate(xx);

			string title = Format(
				"{0}\\n{{/Trebushet ~x{{0.8-}}}} = {1}, " +
				"error({{/Trebushet ~x{{0.8-}}}}) = {2:E1}" +
				"\\nf(x) = {3}\\nz = [{4}], N = {5}",
				methodName, X, error, f, Join(", ", x), x.Length);

			string functions = Format(
				"\"data0\" title \"error(x) = |f(x) - ({0})|\"" +
				", \"data1\" title \"error\\\\_estimation(x) = {1}\"",
				p, errorF);

			Gnuplot.Run(x, xx, functions, title, "z", y0, y1);
		}

		const double l = 1;
		static double x0 = Functions.InitializeX0();
	}
}
//static void TestForward(int minDegree = 0, int maxDegree = 3)
//{
//	T Task_1 = (double[] d) => Program.Task_1(0, Newton, false, d);

//	for (int n = minDegree; n <= maxDegree; n++)
//	{
//		double[] x = new double[n + 1];

//		for (int i = 0; i < n + 1; i++)
//			x[i] = -1 + 0.01 * i;

//		//Function f = new Function(0);

//		//var p = Interpolation(Newton, f, x, n);
//		//var q = Interpolation(Forward, f, x, n);

//		Task_2(n, -0.995);
//		Task_1(x);
//	}
//}
//static void TestMiddleDown()
//{
//	T Task_1 = (d) => Program.Task_1(0, Newton, false, d);

//	//Task_2(5, -0.009);
//	//Task_1(-0.02, -0.01, 0);

//	Task_2(5, -0.05);
//	Task_1(-0.07, -0.06, -0.05, -0.04, -0.03, -0.02);

//	//Task_2(0, -0.013);
//	//Task_1(-0.01);
//	//Task_2(1, -0.013);
//	//Task_1(-0.02, -0.01);
//	//Task_2(2, -0.013);
//	//Task_1(-0.02, -0.01, 0);
//	//Task_2(3, -0.013);
//	//Task_1(-0.03, -0.02, -0.01, 0);
//	//Task_2(4, -0.013);
//	//Task_1(-0.03, -0.02, -0.01, 0);
//}
//static void TestBackward(int minDegree = 0, int maxDegree = 3)
//{
//	T Task_1 = (double[] d) => Program.Task_1(0, Newton, false, d);

//	for (int n = minDegree; n <= maxDegree; n++)
//	{
//		double[] x = new double[n + 1];
//		x[n] = 0;
//		for (int i = 1; i < n + 1; i++)
//			x[n - i] = x[n - i + 1] - 0.01;

//		//Function f = new Function(0);

//		//var p = Interpolation(Newton, f, x, n);
//		//var q = Interpolation(Forward, f, x, n);

//		Task_2(n, -0.005);
//		Task_1(x);
//	}
//}