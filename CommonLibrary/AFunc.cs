using static System.Math;

namespace NumericalAnalysis
{
	public abstract class AFunc
	{
		public abstract double Eval(double x);
		public abstract double Der(int n, double x);
		public abstract AFunc GetADer(int n = 1);

		public static AFunc operator +(AFunc f, AFunc g)
		{
			if (f is Polynomial && g is Polynomial)
				return (Polynomial)f + (Polynomial)g;

			if (f is Func && g is Polynomial)
				return (Func)f + (Polynomial)g;

			if (f is Polynomial && g is Func)
				return (Polynomial)f + (Func)g;

			return (Func)f + (Func)g;
		}
		public static AFunc operator -(AFunc f, AFunc g)
		{
			if (f is Polynomial && g is Polynomial)
				return (Polynomial)f - (Polynomial)g;

			if (f is Func && g is Polynomial)
				return (Func)f - (Polynomial)g;

			if (f is Polynomial && g is Func)
				return (Polynomial)f - (Func)g;

			return (Func)f - (Func)g;
		}

		public double[] AbsEvaluate(double[] x)
		{
			double[] y = new double[x.Length];
			for (int i = 0; i < x.Length; i++)
				y[i] = Abs(Eval(x[i]));

			return y;
		}
		public double[] Evaluate(double[] x)
		{
			double[] y = new double[x.Length];
			for (int i = 0; i < x.Length; i++)
				y[i] = Eval(x[i]);

			return y;
		}
		public double DerBound(int n, double a, double b)
		{
			double result = Max(Abs(Der(n, a)), Abs(Der(n, b)));

			double[] stationary = GetADer(n + 1).GetRoots(a, b);

			foreach (var point in stationary)
				result = Max(result, Abs(Der(n, point)));

			return result;
		}
		public double[] GetRoots(double a, double b)
		{
			double[] x = Worker.GetX(a, b, 1000);
			double[] y = Evaluate(x);
			double[] result = new double[x.Length];

			int next = 0;
			for (int i = 0; i < y.Length - 1; i++)
				if (y[i + 1] == 0)
					result[next++] = x[i++ + 1];
				else
				if (y[i] * y[i + 1] < 0)
					result[next++] = Tangent(x[i], x[i + 1]);

			System.Array.Resize(ref result, next);

			return result;
		}
		public double Tangent(double a, double b)
		{
			double e = 1e-15;
			a = Eval(a) * Der(2, a) > 0 ? a : b;
			b = Eval(a) / Der(1, a);
			int step = 1;

			while (Abs(b) > e && step < 99)
			{
				b = Eval(a) / Der(1, a);
				a -= b;
				step++;
			}

			return a;
		}

		public string ToGnuplotString() => ToString().Replace("^", "**");
	}
}