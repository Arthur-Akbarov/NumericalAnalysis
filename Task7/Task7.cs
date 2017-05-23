using System;
using System.Collections.Generic;
using static System.Console;
using static System.Math;

namespace NumericalAnalysis
{
	// fail to -1 1 1
	class Task7
	{
		static void Main(string[] args)
		{
			do
				Start();
			while (ReadKey().Key != ConsoleKey.Q);
		}

		static void Start()
		{
			//Gnuplot.Run(f.ToGnuplotString() + 0);
			a = Menu.ReadDouble("Enter left point: ");
			b = Menu.ReadDouble("Enter right point: ", a);
			e = Pow(0.1, Menu.ReadInt("Enter precision order: "));

			Greeting();

			double[] x = Worker.GetX(a, b, 100);
			double[] y = f.Evaluate(x);

			for (int i = 0; i < y.Length - 1; i++)
				if (y[i + 1] == 0)
					Output(x[i++ + 1]);
				else
				if (y[i] * y[i + 1] < 0)
					Output(x[i], x[i + 1]);

			WriteLine("End");
		}

		static void Greeting()
		{
			Clear();
			WriteLine("f(x) = {0}", f);
			WriteLine("[a,b] = [{0},{1}]", a, b);
			WriteLine("precision = {0}", e);
			WriteLine();
		}
		static void Output(double x)
		{
			WriteLine("lucky root = {0}", x);
			WriteLine(new string('*', 40));
			WriteLine();
		}
		static void Output(double a, double b)
		{
			int t = 15;
			WriteLine(new string(' ', t + 5) + "residual");
			Write("{0,-" + (t + 6) + "}", "nroot");
			Write("{0,-8}", "order");
			Write("{0,-6}", "step");
			//Write("{0,-" + (t + 5) + "}", "method");
			WriteLine();
			foreach (var method in findRoots)
			{
				int step;
				double root = method(out step);
				string order = Ceiling(-Log(Abs(f.Eval(root)), 10)).ToString();

				Write("{0,-" + (t + 6) + ":N" + t + "}", root);
				Write("{0,-8}", (order == "Infinity") ? "inf" : order);
				Write("{0,-6}", step);
				Write(method.Method.Name);
				WriteLine();
			}
			WriteLine(new string('*', 40));
		}

		delegate double FindRoot(out int step);
		static double Bisection(out int step)
		{
			step = 0;

			while (b - a > e && step < stepMax)
			{
				double c = (b + a) / 2;

				if (f.Eval(c) * f.Eval(b) <= 0)
					a = c;
				else
					b = c;

				step++;
			}
			return a;
		}
		static double Newton(out int step)
		{
			a = f.Eval(a) * f.Der(2, a) > 0 ? a : b;
			b = a - b;
			step = 1;

			while (Abs(b) > e && step < stepMax)
			{
				b = f.Eval(a) / f.Der(1, a);
				a -= b;
				step++;
			}
			return a;
		}
		static double ModifiedNewton(out int step)
		{
			a = f.Eval(a) * f.Der(2, a) > 0 ? b : a;
			b = a - b;
			step = 1;
			double a0 = a;

			while (Abs(b) > e && step < stepMax)
			{
				b = f.Eval(a) / f.Der(1, a0);
				a -= b;
				step++;
			}
			return a;
		}
		static double Secant(out int step)
		{
			double c = a - f.Eval(a) * (b - a) / (f.Eval(b) - f.Eval(a));
			a = f.Eval(a) * f.Der(2, a) > 0 ? a : b;

			step = 1;

			while (Abs(b - c) > e && step < stepMax)
			{
				a = b;
				b = c;
				c = a - f.Eval(a) * (b - a) / (f.Eval(b) - f.Eval(a));
				step++;
			}
			return c;
		}
		static double FixedPointIteration(out int step)
		{
			double q = f.DerBound(1, a, b);
			double a0 = a;
			a = (a + b) / 2;
			b = a - b;
			step = 1;

			while (Abs(b) > Abs((1 - q) * e / q) && step < stepMax)
			{
				b = f.Eval(a) / (f.Der(1, a) + 1E-10);
				a -= b;
				step++;
			}
			return a;
		}

		static double a, b, e;
		const int stepMax = 99;
		static AFunc f = Functions.Get2();
		static readonly List<FindRoot> findRoots = new List<FindRoot> {
			Bisection, Newton, ModifiedNewton, Secant, FixedPointIteration };
	}
}
