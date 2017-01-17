using static System.Math;

namespace NumericalAnalysis
{
	public class Functions
	{
		const double c = 5;
		const int N = 16;

		public static AF Get(int n)
		{
			return (n == 0) ? Get0()
				 : (n == 1) ? Get1()
				 : (n == 2) ? Get2()
				 : Get3();
		}
		public static AF Get0()
		{
			const double N2 = N % 2 + 1;
			const double N7 = (N % 7 + 1) / 2d;

			string f = (N2 == 0) ? "x + "
					 : (N2 == 1) ? "x*exp(x) + "
					 : (N2 == -1) ? "x*exp(-x) + "
					 : "x*exp(" + N2 + "*x) + ";

			f += (N7 == 0) ? "1"
			   : (N7 == 1) ? "sin(x)"
			   : (N7 == -1) ? "sin(-x)"
			   : "sin(" + N7 + "*x)";

			return new Func(new SimpleF(
				eval: (k, x) =>
				{
					double left = k * Pow(N2, k - 1) * Exp(N2 * x)
									+ Pow(N2, k) * x * Exp(N2 * x);

					double right = Pow(N7, k);

					right *= (k % 4 == 0) ? Sin(N7 * x)
							: (k % 4 == 1) ? Cos(N7 * x)
							: (k % 4 == 2) ? -Sin(N7 * x)
							: -Cos(N7 * x);

					return left + right;
				},
				name: Name(f)
			));
		}
		public static AF Get1()
		{
			return new Func(new SimpleF(
				eval: (k, x) =>
				{
					if (k == 0)
						return x / 2 + Sin(x) * Sin(x);

					if (k == 1)
						return 0.5 + Sin(2 * x);

					double result = Pow(2, k - 1);

					result *= (k % 4 == 2) ? Cos(2 * x)
							: (k % 4 == 3) ? -Sin(2 * x)
							: (k % 4 == 0) ? -Cos(2 * x)
							: Sin(2 * x);

					return result;
				},
				name: Name("x/2 + sin(x)^2")
			));
		}
		public static AF Get2()
		{
			return new Func(new SimpleF(
				eval: (k, x) =>
				{
					if (k == 0)
						return 3 * x - Cos(x) - 1;

					if (k == 1)
						return Sin(x) + 3;

					double result = (k % 4 == 0) ? -Cos(x)
								  : (k % 4 == 1) ? Sin(x)
								  : (k % 4 == 2) ? Cos(x)
								  : -Sin(x);

					return result;
				},
				name: Name("3*x - cos(x) - 1")
			));
		}
		public static AF Get3()
		{
			return new Func(new SimpleF(
				eval: (k, x) => Exp(x),
				name: Name("exp(x)")
			));
		}

		public static double InitializeX0()
		{
			return -N % 3;
		}
		public static double[] InitializeX()
		{
			double[] x = new double[5];

			x[0] = -N % 3;
			x[1] = x[0] + 0.1;
			x[2] = x[0] + 0.3;
			x[3] = x[0] + 0.45;
			x[4] = x[0] + 0.5;

			return x;
		}
		public static void Initialize(out double[] x, out int[] r)
		{
			x = new double[3];
			x[0] = -N % 3;
			x[1] = x[0] + 0.1;
			x[2] = x[0] + 0.3;

			r = new int[3];
			r[0] = 1;
			r[1] = 2;
			r[2] = 3;
		}

		public static AF GetIntegrableFunc()
		{
			return new Func(new SimpleF(
			eval: (k, x) =>
			{
				// implement for k <= 7
				double result = (k == 0) ? 1
						: (k == 1) ? -2 * x
						: (k == 2) ? -2 * (c - 3 * x * x)
						: (k == 3) ? 24 * x * (c - x * x)
						: (k == 4) ? 24 * (Pow(c, 2) - 10 * c * Pow(x, 2)
							+ 5 * Pow(x, 4))
						: (k == 5) ? -240 * (3 * Pow(c, 2) * x -
							-10 * c * Pow(x, 3) + 3 * Pow(x, 5))
						: (k == 6) ? -720 * (Pow(c, 3) - 21 * Pow(c, 2) *
							Pow(x, 2) + 35 * c * Pow(x, 4) - 7 * Pow(x, 6))
						: 40320 * x * (Pow(c, 3) - 7 * Pow(c, 2) * Pow(x, 2) +
							+7 * c * Pow(x, 4) - Pow(x, 6));

				result /= Pow(x * x + c, k + 1);

				return result;
			},
			name: Name("1 / (x^2 + " + c + ")")
			));
		}
		public static double IntegrableDerBound(int k)
		{
			return Worker.Factorial(k) / Pow(Sqrt(c), k + 2);
		}
		public static double Integrate(double a, double b)
		{
			F f = (x) => Atan(x / Sqrt(c)) / Sqrt(c);

			return (a == 0) ? f(b) : f(b) - f(a);
		}

		public static SimpleF.S Name(string f)
		{
			return (k, a) =>
			{
				string g = (k >= 1 || a < 0) ? "(" + f + ")" : f;

				string result = (a == 1) ? " + " + g
							  : (a == -1) ? " - " + g
							  : a.Signed() + g;

				result += GetDerSymbol(k);

				return result;
			};
		}
		public static string GetDerSymbol(int k)
		{
			return (k <= 3) ? new string('\'', k) : "'[" + k + "]";
		}

		delegate double F(double x);
		enum Sign { Positive, Negative };
	}
}