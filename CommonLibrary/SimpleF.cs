namespace NumericalAnalysis
{
	public class SimpleF
	{
		public Q Eval;
		public S Name;

		public SimpleF(Q eval, S name)
		{
			Eval = eval;
			Name = name;
		}

		public string ToString(int k, double a) => Name(k, a);

		public delegate double Q(int k, double x);
		public delegate string S(int k, double a);
	}
}
