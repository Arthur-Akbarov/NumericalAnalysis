using System.Collections.Generic;
using System.Linq;

namespace NumericalAnalysis
{
	public class Func : AF
	{
		// guarantee every key is not empty
		public Dictionary<SimpleF, Dictionary<int, double>> D;

		public Func()
		{
			D = new Dictionary<SimpleF, Dictionary<int, double>>();
		}
		public Func(SimpleF q, int[] k, double[] a) : this()
		{
			D.Add(q, new Dictionary<int, double>());

			for (int i = 0; i < k.Length; i++)
				D[q].Add(k[i], a[i]);
		}
		public Func(SimpleF q, double[] a)
			: this(q, new int[a.Length], a) { }
		public Func(SimpleF q)
			: this(q, new int[1], new double[] { 1 }) { }

		public override double Eval(double x)
		{
			double result = 0;

			foreach (var q in D.Keys)
				foreach (var k in D[q].Keys)
					result += D[q][k] * q.Eval(k, x);

			return result;
		}
		public override double Der(int n, double x)
		{
			double result = 0;

			foreach (var q in D.Keys)
				foreach (var k in D[q].Keys)
					result += D[q][k] * q.Eval(k + n, x);

			return result;
		}
		public override AF GetDer(int n = 1)
		{
			var result = new Func();

			foreach (var q in D.Keys)
			{
				result.Add(q);

				foreach (var k in D[q].Keys)
					result.D[q].Add(k + n, D[q][k]);
			}

			return result;
		}

		public Func Clone()
		{
			var result = new Func();

			foreach (var q in D.Keys)
				foreach (var k in D[q].Keys)
					result.Add(q, k, D[q][k]);
			//result.D.Add(q, D[q].ToDictionary(entry => entry.Key,
			//entry => -entry.Value));

			return result;
		}
		public void Add(SimpleF q)
		{
			D.Add(q, new Dictionary<int, double>());
		}
		public void Add(SimpleF q, int k, double a)
		{
			if (D.ContainsKey(q))
				if (D[q].ContainsKey(k))
				{
					D[q][k] += a;

					if (D[q][k] == 0)
					{
						D[q].Remove(k);

						if (D[q].Count == 0)
							D.Remove(q);
					}
				}
				else
					D[q].Add(k, a);
			else
				D.Add(q, new Dictionary<int, double> { { k, a } });
		}
		public void Add(Polynomial p, int k = 0, double a = 1)
		{
			D.Add(p, new Dictionary<int, double> { { k, a } });
		}

		public static Func operator -(Func f)
		{
			var result = f.Clone();

			foreach (var q in result.D.Keys)
				foreach (var k in result.D[q].Keys.ToList())
					result.D[q][k] *= -1;

			return result;
		}
		// should test for deep copy
		public static Func operator +(Func f, Func g)
		{
			var result = f.Clone();

			foreach (var q in g.D.Keys)
				if (result.D.ContainsKey(q))
				{
					foreach (var k in g.D[q].Keys)
						if (result.D[q].ContainsKey(k))
						{
							result.D[q][k] += g.D[q][k];

							if (result.D[q][k] == 0)
								result.D[q].Remove(k);
						}
						else
							result.D[q].Add(k, g.D[q][k]);

					if (result.D[q].Count == 0)
						result.D.Remove(q);
				}
				else
					result.D.Add(q, g.D[q].ToDictionary(
						entry => entry.Key,
						entry => entry.Value));

			//foreach (var k in g.D[q].Keys)
			//		f.D[q].Add(k, g.D[q][k]);

			return result;
		}
		public static Func operator +(Func f, Polynomial p)
		{
			var result = f.Clone();
			var r = p.Clone();

			result.Add(r);

			return result;
		}
		public static Func operator +(Polynomial p, Func f)
		{
			return f + p;
		}
		public static Func operator -(Func f, Func g)
		{
			return f + -g;
		}
		public static Func operator -(Func f, Polynomial p)
		{
			return -(-f + p);
		}
		public static Func operator -(Polynomial p, Func f)
		{
			return -f + p;
		}
		//public static Func operator *(Func f, Func g)
		//{
		//	// should use matrix a or mb smth more sophisticated
		//}

		public override string ToString()
		{
			var sb = new System.Text.StringBuilder();

			foreach (var q in D.Keys)
				foreach (var e in D[q])
					sb.Append(q.Name(e.Key, e.Value));

			Worker.BeautifyLeadingSign(sb);

			return sb.ToString();
		}
	}
}
