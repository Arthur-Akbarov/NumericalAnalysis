using static System.String;
using static System.Math;

namespace NumericalAnalysis
{
	public class Task1
	{
		// Start( [idFunc = 0] [method = Newton] [chebyshev = true] [params x])
		static void Main(string[] args)
		{
			Start(0, Newton);
		}

		public static void Start(int idFunc, Method method, bool chebyshev,
			params double[] x)
		{
			if (x.Length == 0)
				x = Functions.InitializeX();

			AFunc f = Functions.Get(idFunc);
			double[] y = f.Evaluate(x);

			Polynomial p = method(x, y);
			Polynomial errorP = ErrorEstimate(f, x);
			Plot(f, p, errorP, x, method.Method.Name);

			if (!chebyshev)
				return;

			double[] t = ChebyshevRoots(x);
			double[] u = f.Evaluate(t);

			Polynomial q = method(t, u);
			Polynomial errorQ = ErrorEstimate(f, t);
			double errorBound = ErrorBound(f, x);

			Plot(f, q, errorQ, errorBound, t, x, method.Method.Name);
		}

		public delegate Polynomial Method(double[] x, double[] y);
		public static Polynomial Lagrange(double[] x, double[] y)
		{
			var result = new Polynomial();

			for (int i = 0; i < x.Length; i++)
			{
				var l = new Polynomial(1);

				for (int j = 0; j < x.Length; j++)
					if (j != i)
						l *= new Polynomial(-x[j], 1) / (x[i] - x[j]);

				result += l * y[i];
			}

			return result;
		}
		public static Polynomial Newton(double[] x, double[] y)
		{
			var result = new Polynomial();

			for (int i = 0; i < x.Length; i++)
			{
				var l = new Polynomial(1);

				for (int j = 0; j < i; j++)
					l *= new Polynomial(-x[j], 1);

				result += l * DividedDiff(x, y, 0, i);
			}

			return result;
		}

		public static Polynomial ErrorEstimate(AFunc f, double[] x)
		{
			int n = x.Length - 1;
			Polynomial w = new Polynomial(1);

			for (int i = 0; i < x.Length; i++)
				w *= new Polynomial(-x[i], 1) / (i + 1);

			double M = f.DerBound(n + 1, x[0], x[n]);

			return w * M;
		}
		static double ErrorBound(AFunc f, double[] x)
		{
			int n = x.Length - 1;
			double result = 2;

			for (int i = 0; i < n + 1; i++)
				result *= (x[n] - x[0]) / 4 / (i + 1);

			double M = f.DerBound(n + 1, x[0], x[n]);

			return result * M;
		}
		static double[] ChebyshevRoots(double[] x)
		{
			int n = x.Length - 1;
			double[] t = new double[n + 1];

			for (int i = 0; i < n + 1; i++)
			{
				t[i] = Cos((2 * i + 1) * PI / 2 / (n + 1));
				t[i] = t[i] * (x[n] - x[0]) / 2 + (x[n] + x[0]) / 2;
			}

			return t;
		}
		static double DividedDiff(double[] x, double[] y, int start, int end)
		{
			return end == start
				? y[start]
				: (DividedDiff(x, y, start + 1, end)
				 - DividedDiff(x, y, start, end - 1))
						/ (x[end] - x[start]);
		}

		#region Start() overloadings
		static void Start(int idFunc, Method method, params double[] x)
		{
			Start(idFunc, method, true, x);
		}
		static void Start(int idFunc, bool chebyshev, params double[] x)
		{
			Start(idFunc, Newton, chebyshev, x);
		}
		static void Start(Method method, bool chebyshev, params double[] x)
		{
			Start(0, method, chebyshev, x);
		}
		static void Start(int idFunc, params double[] x)
		{
			Start(idFunc, Newton, true, x);
		}
		static void Start(Method method, params double[] x)
		{
			Start(0, method, true, x);
		}
		public static void Start(bool chebyshev, params double[] x)
		{
			Start(0, Newton, chebyshev, x);
		}
		public static void Start(params double[] x)
		{
			Start(0, Newton, true, x);
		}
		#endregion

		static void Plot(AFunc f, AFunc p, AFunc errorF, double[] x,
			string methodName)
		{
			double[] xx = Worker.GetX(x);
			double[] y0 = (f - p).AbsEvaluate(xx);
			double[] y1 = errorF.AbsEvaluate(xx);
			
			string title = Format(
				"{0}\\nf(x) = {1}\\nx = [{2}], N = {3}",
				methodName, f, Join(", ", x), x.Length);

			string functions = Format(
				"\"data0\" title \"error(x) = |f(x) - ({0})|\"" +
				", \"data1\" title \"error\\\\_estimation(x) = {1}\"",
				p, errorF);

			Gnuplot.Run(x, xx, functions, title, "x", y0, y1);
		}
		static void Plot(AFunc f, AFunc p, AFunc errorF, double errorBound,
			double[] t, double[] x, string methodName)
		{
			double[] xx = Worker.GetX(x);
			double[] y0 = (f - p).AbsEvaluate(xx);
			double[] y1 = errorF.AbsEvaluate(xx);

			string title = Format(
				"{0}\\nf(x) = {1}\\nt = [{2}], N = {3}",
				 methodName, f, Join(", ", t), x.Length);

			string functions = Format(
				"\"data0\" title \"error(x) = |f(x) - ({0})|\"" +
				", \"data1\" title \"error\\\\_estimation(x) = {1}\"" +
				", {2} title \"error\\\\_bound\" with lines",
				p, errorF, errorBound);

			Gnuplot.Run(t, xx, functions, x, title, "t", y0, y1);
		}
	}
}
// english pdf with formulas
// http://www.uio.no/studier/emner/matnat/math/MAT-INF4140/v14/undervisningsmateriale/newton.pdf
