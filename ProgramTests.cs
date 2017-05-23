using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace вычи1.Tests
{
	[TestClass()]
	public class ProgramTests
	{
		public delegate void method(string s, double[] x, double[] y);

		[TestMethod()]
		public void LagrangeTest()
		{
			Test(delegate (string s, double[] x, double[] y)
			{
				Assert.AreEqual(s, Program.Lagrange(x, y).ToString());
			});
		}
		[TestMethod()]
		public void NewtonTest()
		{
			Test(delegate (string s, double[] x, double[] y)
			{
				Assert.AreEqual(s, Program.Newton(x, y).ToString());
			});
		}
		public void Test(method S)
		{
			S(Polynomial.zero,
				new double[] { 0 },
				new double[] { 0 });
			S(Polynomial.zero,
				new double[] { 1 },
				new double[] { 0 });
			S(Polynomial.zero,
				new double[] { 1, -2 },
				new double[] { 0, 0 });
			S("20",
				new double[] { 10 },
				new double[] { 20 });
			S("2",
				new double[] { 101, 1001, 10001 },
				new double[] { 2.0, 2.00, 2.000 });
			S("x",
				new double[] { 0, 1, 2, -3 },
				new double[] { 0, 1, 2, -3 });
			S("x",
				new double[] { -1, -2, -3, -4 },
				new double[] { -1, -2, -3, -4 });
			S("x",
				new double[] { 0, 1, 20, 30, 40, 50 },
				new double[] { 0, 1, 20, 30, 40, 50 });
			S("x + 5",
				new double[] { 0, 1, 2, 30.7 },
				new double[] { 5, 6, 7, 35.7 });
			S("x + 5.55",
				new double[] { -4, -3, -2, -1, 0 },
				new double[] { 1.55, 2.55, 3.55, 4.55, 5.55 });
			S("3x",
				new double[] { 1, 2, 3 },
				new double[] { 3, 6, 9 });
			S("x^2",
				new double[] { 0, 2, 4, 10, 11 },
				new double[] { 0, 4, 16, 100, 121 });
			S("-3x^3",
				new double[] { -1, 0, 2, 5 },
				new double[] { 3, 0, -24, -375 });
			S("x^2",
				new double[] { 0, 2, 4 },
				new double[] { 0, 4, 16 });
			S("x^3 + x",
				new double[] { -1, 0, 1, 2 },
				new double[] { -2, 0, 2, 10 });
			S("0.5x^4",
				new double[] { -2, -1, 1, 2, 5 },
				new double[] { 8, 0.5, 0.5, 8, 625 / 2d });
			S("x^2 - 2x + 1",
				new double[] { -1, 2, 3, 4, 5 },
				new double[] { 4, 1, 4, 9, 16 });
		}
	}
}