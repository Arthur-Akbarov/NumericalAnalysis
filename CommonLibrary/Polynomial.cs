using static System.Math;

namespace NumericalAnalysis
{
	public class Polynomial : AFunc
	{
		public const double eps = 1e-10;
		public const int maxDegree = 40;
		/// <summary>
		/// coefficients of polynomial ( a0, a1, a2 ... )
		/// </summary>
		public double[] a = new double[maxDegree + 1];

		public Polynomial(params double[] args)
		{
			System.Buffer.BlockCopy(args, 0, a, 0,
				args.Length * sizeof(double));
		}

		public int Deg
		{
			get
			{
				for (int i = maxDegree; i >= 0; i--)
					if (a[i] != 0)
						return i;
				// for zero polynomial
				return -1;
			}
		}

		public static Polynomial operator +(Polynomial p, Polynomial q)
		{
			for (int i = 0; i <= maxDegree; i++)
				p.a[i] += q.a[i];

			return p;
		}
		public static Polynomial operator +(Polynomial p, double b)
		{
			p.a[0] += b;

			return p;
		}
		public static Polynomial operator +(double b, Polynomial p)
		{
			p.a[0] += b;

			return p;
		}
		public static Polynomial operator -(Polynomial p, double b)
		{
			p.a[0] -= b;

			return p;
		}
		public static Polynomial operator -(double b, Polynomial p)
		{
			return p - b;
		}
		public static Polynomial operator *(Polynomial p, Polynomial q)
		{
			for (int i = maxDegree; i >= 0; i--)
			{
				double sum = 0;
				for (int j = 0; j <= i; j++)
					sum += p.a[j] * q.a[i - j];

				p.a[i] = sum;
			}

			return p;
		}
		public static Polynomial operator *(Polynomial p, double b)
		{
			for (int i = 0; i <= maxDegree; i++)
				p.a[i] *= b;

			return p;
		}
		public static Polynomial operator *(double b, Polynomial p)
		{
			return p * b;
		}
		public static Polynomial operator /(Polynomial p, double b)
		{
			for (int i = 0; i <= maxDegree; i++)
				p.a[i] /= b;

			return p;
		}
		public static Polynomial operator ^(Polynomial p, int n)
		{
			if (n < 1)
				p = new Polynomial();
			else
			{
				var t = p.Clone();

				for (int i = 0; i < n - 1; i++)
					p *= t;
			}

			return p;
		}

		public Polynomial Clone()
		{
			return new Polynomial(a);
		}
		public static implicit operator SimpleF(Polynomial p)
		{
			var r = p.Clone();

			return new SimpleF(
				eval: (k, a) => r.Der(k, a),
				name: Functions.Name(r.GetADer().ToString())
			);
		}

		public Polynomial GetIntegral(int n)
		{
			var result = new Polynomial();

			for (int i = n; i <= maxDegree; i++)
			{
				result.a[i] = a[i - n];

				for (int j = 0; j < n; j++)
					result.a[i] /= (i - j);
			}

			return result;
		}
		public double Integrate(double a, double b)
		{
			Polynomial q = GetIntegral(1);

			for (int i = 1; i <= maxDegree; i++)
				q.a[i] = this.a[i - 1] / i;

			return q.Eval(b) - q.Eval(a);
		}

		public override double Eval(double x)
		{
			double result = 0;
			for (int i = 0; i <= maxDegree; i++)
				result += a[i] * Pow(x, i);

			return result;
		}
		public override double Der(int k, double x)
		{
			double result = 0;
			int d = Deg;

			for (int i = 0; i <= d - k; i++)
			{
				double l = a[i + k] * Pow(x, i);

				for (int j = 1; j <= k; j++)
					l *= i + j;

				result += l;
			}

			return result;
		}
		public override AFunc GetADer(int k = 1)
		{
			return GetDer(k);
		}
		public Polynomial GetDer(int k = 1)
		{
			var result = new Polynomial();
			int d = Deg;

			for (int i = 0; i <= d - k; i++)
			{
				result.a[i] = a[i + k];
				for (int j = 1; j <= k; j++)
					result.a[i] *= (i + j);
			}

			return result;
		}

		public override string ToString()
		{
			int d = Deg;

			if (d == -1)
				return "0";

			var sb = new System.Text.StringBuilder();

			for (int i = d; i > 1; i--)
				if (Abs(a[i]) > eps)
				{
					sb.Append(a[i].Signed() + "x^");

					sb = (i >= 10) ? sb.Append("{" + i + "}")
								   : sb.Append(i);
				}

			if (Abs(a[1]) > eps)
				sb.Append(a[1].Signed() + "x");

			if (Abs(a[0]) > eps)
				sb.Append(a[0].Signed());

			sb.Replace(" 1x", " x");
			Worker.BeautifyLeadingSign(sb);

			return sb.ToString();
		}
	}
}
