#define table

using System.Linq;
using static System.Console;
using static System.Math;
using static System.String;

namespace NumericalAnalysis
{
	class Program
	{
		static double X = 1;
#if table
		static int tableHeight = 15;
		static double hBase = 10;
#else
		static int k = 500;
		static double hBase = 1.1;
#endif
		static void Main(string[] args)
		{
			do
				Start();
			while (ReadKey().Key != System.ConsoleKey.F4);
		}

		static void Start()
		{
			X = Menu.ReadDouble("Enter interpolation point: ");
			hBase = Menu.ReadDouble("Enter h base: ", 1);

			for (int i = 0; i < p.GetLength(0); i++)
				FillRow(i);

			optimal = GetOptimalH();
			Output();
		}

		static void FillRow(int i)
		{
			double h = 1 / Pow(hBase, i + 1);

			F y = j => f.Eval(X + j * h);
			#region fair way has problems with small h
			//const int m = 20;
			//double[] x = new double[m + 1];

			//x[0] = h * Truncate((X - m * h / 2) / h);
			//for (int j = 1; j < m + 1; j++)
			//	x[j] = x[j - 1] + h;
			//double[] y = f.EvaluateY(x);

			//double d = (X - x[0]) / h;
			//int start = (int)Truncate(d);

			//F y = j => f.Eval(x[start + j]);
			#endregion

			double[] der = { 0, f.Der(1, X), f.Der(2, X) };

			p[i, 0] = h;
			for (int j = 1; j <= 2; j++)
			{
				p[i, j] = Abs(ForwardDer(y, h, 1, j) - der[1]);
				p[i, 2 + j] = Abs(BackwardDer(y, h, 1, j) - der[1]);
				p[i, 4 + j] = Abs(CentralDer(y, h, j, 2) - der[j]);
			}
		}
		static bool[,] GetOptimalH()
		{
			bool[,] result = new bool[p.GetLength(0), p.GetLength(1)];

			for (int j = 1; j < p.GetLength(1); j++)
			{
				double min = double.MaxValue;

				for (int i = 0; i < p.GetLength(0); i++)
					if (p[i, j] < min)
					{
						min = p[i, j];
						row[j - 1] = i;
					}

				result[row[j - 1], j] = true;
			}

			return result;
		}
		static void Output()
		{
			Clear();
			WriteLine("f(x) = {0}\nX = {1}\n", f, X);
#if table
			Write("cells show place of first non-zero decimal digit of error");
			WriteLine();
#endif
			WriteLine("f12 - forward derivative of order 1 and accuracy 2");
			WriteLine("\n-lg(h)    " + Join(" ", name));
#if table
			for (int i = 0; i < p.GetLength(0); i++)
			{
				Write("{0,4}  =>  ", Ceiling(-Log(p[i, 0], hBase)));
				for (int j = 1; j < p.GetLength(1); j++)
				{
					if (optimal[i, j])
						ForegroundColor = System.ConsoleColor.Green;

					string s = Ceiling(-Log(p[i, j], 10)).ToString();
					Write("{0,-3} ", (s == "Infinity") ? "inf" : s);

					ForegroundColor = System.ConsoleColor.Gray;
				}
				WriteLine();
			}
			WriteLine(new string('*', 33));
#endif
			Write("exp_opt   ");
			WriteLine(Concat(row.Select(i => Format("{0,-4}", i + 1))));
			WriteLine("base: {0}\n", hBase);
			WriteLine("          h_optimal    nvalue");
			for (int j = 1; j < p.GetLength(1); j++)
			{
				for (int i = 0; i < p.GetLength(0); i++)
					if (optimal[i, j])
						Write(" {0}  =>  {1:E2}", name[j - 1], p[i, 0]);

				WriteLine("    {0:E2}", p[1, j]);
			}
		}

		static double ForwardDer(F y, double h, int order, int accuracy)
		{
			double[] arr = ForwardArr(order, accuracy);

			double result = 0;
			for (int i = 0; i < arr.Length; i++)
				result += arr[i] * y(i);

			result /= Pow(h, order);

			return result;
		}
		static double BackwardDer(F y, double h, int order, int accuracy)
		{
			double[] arr = ForwardArr(order, accuracy);
			double sign = (order % 2 == 0) ? 1 : -1;

			double result = 0;
			for (int i = 0; i < arr.Length; i++)
				result += sign * arr[i] * y(-i);

			result /= Pow(h, order);

			return result;
		}
		static double CentralDer(F y, double h, int order, int accuracy)
		{
			double[] arr = CentralArr(order, accuracy);

			double result = 0;
			for (int i = 0; i < arr.Length; i++)
				result += arr[i] * y(-(order + accuracy - 1) / 2 + i);

			result /= Pow(h, order);

			return result;
		}

		static double[] ForwardArr(int order = 1, int accuracy = 1)
		{
			int n = order + accuracy - 1;

			double[] arr = new double[n + 1];
			for (int i = 0; i < n + 1; i++)
				arr[i] = ForwardDiff(accuracy, n, i);

			for (int i = (order % 2 == 0) ? 1 : 0; i <= n; i += 2)
				arr[i] *= -1;

			return arr;
		}
		static double ForwardDiff(int accuracy, int n, int i)
		{
			if (accuracy == 1)
				return Worker.Binomial(n, n - i);

			// accuracy == 2
			return Worker.Binomial(n - 1, i) +
				(n - 1) * Worker.Binomial(n, i) / 2d;
		}
		static double[] CentralArr(int order, int accuracy)
		{
			double[] arr;

			if (order == 1 && accuracy == 2)
				arr = new double[] { -0.5, 0, 0.5 };
			else // order == 2 && accuracy == 2
				arr = new double[] { 1, -2, 1 };

			return arr;
		}

		static void TestCoefficient(int orderMax = 4)
		{
			WriteLine("Central finite difference");
			for (int i = 1; i <= 2; i++)
				Write("{0} {1} => {2}\n", i, 2, Join(", ", CentralArr(i, 2)));

			WriteLine();
			WriteLine("Forward finite difference");
			for (int i = 1; i <= orderMax; i++)
				for (int j = 1; j <= 2; j++)
				{
					Write("{0} {1} => ", i, j);
					WriteLine(Join(", ", ForwardArr(i, j)));
				}

			WriteLine();
			//https://www.wikiwand.com/en/Finite_difference_coefficient
		}

		static bool[,] optimal;
		delegate double F(long i);
		static AF f = Functions.Get(2);
		static string[] name = { "f11", "f12", "b11", "b12", "c21", "c22" };
		static int[] row = new int[name.Length];
		static double[,] p = new double[tableHeight, name.Length + 1];
	}
}