using System.Linq;
using static System.String;

namespace NumericalAnalysis
{
	class Task3
	{
		// Start( [idFunc = 0] [x0, r0, x1, r1, ...] )
		static void Main(string[] args)
		{
			Start();
			//Task1.Start(false, 0.99, 1, 1.01, 1.2, 1.45, 1.5);
			//Start(0, 1, 3, 1.2, 1, 1.5, 2);
		}

		static void Start(int idFunc, double[] x, int[] r)
		{
			var f = Functions.Get(idFunc);

			double[] z = EvaluateZ(x, r);
			double[][] dd = TableDividedDiff(f, z);

			var p = Hermite(z, dd);
			var errorF = Task1.ErrorEstimate(f, z);

			Plot(f, p, errorF, x, r);
			for (int i = 1; i < r.Max(); i++)
				Plot(f, p, x, r, i);
		}

		static double[] EvaluateZ(double[] x, int[] r)
		{
			double[] z = new double[r.Sum()];
			int k = 0;
			for (int i = 0; i < x.Length; i++)
				for (int j = 0; j < r[i]; j++)
					z[k++] = x[i];

			return z;
		}
		static double[][] TableDividedDiff(AF f, double[] z)
		{
			int N = z.Length;

			double[][] dd = new double[N][];
			for (int i = 0; i < N; i++)
				dd[i] = new double[N - i];

			for (int order = 0; order < N; order++)
				for (int start = 0; start < N - order; start++)
					if (z[start] == z[start + order])
						dd[start][order] = f.Der(order, z[start])
										  / Worker.Factorial(order);
					else
						dd[start][order] =
							(dd[start + 1][order - 1] - dd[start][order - 1])
								  / (z[start + order] - z[start]);

			return dd;
		}
		public static Polynomial Hermite(double[] z, double[][] dd)
		{
			var result = new Polynomial();

			for (int i = 0; i < z.Length; i++)
			{
				var l = new Polynomial(1);

				for (int j = 0; j < i; j++)
					l *= new Polynomial(-z[j], 1);

				result += l * dd[0][i];
			}

			return result;
		}

		static void Start(int idFunc, params double[] k)
		{
			double[] x = new double[k.Length / 2];
			int[] r = new int[x.Length];

			if (k.Length == 0)
				Functions.Initialize(out x, out r);
			else
				for (int i = 0; i < k.Length / 2; i++)
				{
					x[i] = k[2 * i];
					r[i] = (int)k[2 * i + 1];
				}

			Start(idFunc, x, r);
		}
		static void Start(params double[] k) => Start(0, k);

		static void Plot(AF f, AF p, AF errorF, double[] x, int[] r)
		{
			double[] xx = Worker.GetX(x);
			double[] y0 = (f - p).AbsEvaluate(xx);
			double[] y1 = errorF.AbsEvaluate(xx);

			string title = Format(
				"Hermite\\nx = [{0}]\\nr = [{1}], N = {2}\\nf(x) = {3}",
				 Join(", ", x), Join(", ", r), r.Sum(), f);

			string functions = Format(
				"\"data0\" title \"error(x) = |f(x) - ({0})|\"" +
				", \"data1\" title \"error\\\\_estimation(x) = {1}\"",
				p, errorF);

			Gnuplot.Run(x, xx, functions, title, "x", y0, y1);
		}
		static void Plot(AF f, AF p, double[] x, int[] r, int k)
		{
			AF fk = f.GetDer(k);
			AF pk = p.GetDer(k);

			double[] xx = Worker.GetX(x);
			double[] y0 = (fk - pk).AbsEvaluate(xx);

			string title = Format(
				"Hermite\\nx = [{0}]\\nr = [{1}], N = {2}\\nf(x) = {3}",
				 Join(", ", x), Join(", ", r), r.Sum(), f);

			string der = Functions.GetDerSymbol(k);
			string functions = Format(
				"\"data0\" title \"error^{0}(x) = |f{1}(x) - ({2}))|\"",
				k, der, pk);

			Gnuplot.Run(x, xx, functions, title, "x", y0);
		}

		delegate void S(string s, params object[] args);
	}
}