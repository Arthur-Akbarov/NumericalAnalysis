using System;
using System.IO;

namespace NumericalAnalysis
{
	public static class Gnuplot
	{
		static void SetWindow(string title)
		{
			S("set term wxt position 0,0");
			S("set term wxt size {0}-8,{1}-110",
				System.Windows.Forms.SystemInformation.WorkingArea.Width,
				System.Windows.Forms.SystemInformation.WorkingArea.Height);

			S("set title \"{0}\"", title.Replace("'", "~ {.3'}"));
			S("set title font 'Consolas,12'");
			S("set key samplen 1 font 'Consolas,12'");
			S("set style data " + style);
			S("set style function " + style);
			S("set pointsize 2");
		}
		static void SetLabels(double[] x, string arrayName, double a, double b)
		{
			for (int i = 1; i < x.Length - 1; i++)
				S("set label '{0}_{{{1}}}' at {2} offset 0,-1 "
					+ "point pointtype 7", arrayName, i, x[i]);

			if (x[0] == a)
			{
				S("set label 'a = {0}_{{{1}}}' at {2} offset -3,-1 " +
					"point pointtype 7", arrayName, 0, a);

				S("set label 'b = {0}_{{{1}}}' at {2} offset -3,-1 " +
					"point pointtype 7", arrayName, x.Length - 1, b);
			}
			else
			{
				for (int i = 0; i < x.Length; i += x.Length - 1)
					S("set label '{0}_{{{1}}}' at {2} offset 0,-1 "
						+ "point pointtype 7", arrayName, i, x[i]);

				S("set label 'a' at {0} offset -2.5,-1 point pointtype 7", a);
				S("set label 'b' at {0} offset 0,-1 point pointtype 7", b);
			}
		}
		static void Plot(string functions, double a, double b)
		{
			S("set xrange [{0} - {2}:{1} + {2}]", a, b, 1E-7);
			S("plot {0}", functions.Replace("'", "~ {.3'}"));
			S("set xrange [{0} - 0.05*({1} - {0}) - {2}:" +
						  "{1} + 0.05*({1} - {0}) + {2}]", a, b, 1E-7);
			S("set yrange [{0} - 0.10*({1} - {0}) - {2}:" +
						  "{1} + 0.15*({1} - {0}) + {2}]",
				"GPVAL_DATA_Y_MIN", "GPVAL_DATA_Y_MAX", 1E-15);
			S("replot");
		}

		public static void Run(double[] x, double[] xx, string functions,
			double[] z, string title, string arrayName, params double[][] y)
		{
			using (var file = new StreamWriter(script))
			{
				S = (s, args) => file.WriteLine(string.Format(s, args));

				SetWindow(title);
				SetLabels(x, arrayName, z[0], z[z.Length - 1]);

				Plot(functions, z[0], z[z.Length - 1]);
			}

			for (int i = 0; i < y.GetLength(0); i++)
				using (var file = new StreamWriter("data" + i))
					for (int j = 0; j < xx.Length; j++)
						file.WriteLine("{0,-10} {1}", xx[j], y[i][j]);

			Start();
		}

		#region Run() overloadings
		public static void Run(double[] x, double[] xx,
			string title, string arrayName, params double[][] y)
		{
			Run(x, xx, "", x, title, arrayName, y);
		}

		public static void Run(double[] x, double[] xx, string functions,
			string title, string arrayName, params double[][] y)
		{
			Run(x, xx, functions, x, title, arrayName, y);
		}
		#endregion

		public static void Start()
		{
			if (!exist)
				return;

			if (!File.Exists(path))
			{
				System.Windows.Forms.MessageBox.Show(path + " was not found");
				exist = false;
				return;
			}

			var cmd = new System.Diagnostics.Process();
			//cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			cmd.StartInfo.FileName = "cmd.exe";
			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = false;
			cmd.Start();
			cmd.StandardInput.WriteLine('"' + path + "\" -persist " + script);
			cmd.StandardInput.Flush();
			cmd.StandardInput.Close();
			cmd.WaitForExit();
			cmd.Close();
			//System.Console.WriteLine(cmd.StandardOutput.ReadToEnd());
		}

		const string script = "gnuplotRun.txt";
		const string src = @"%ProgramFiles(x86)%\gnuplot\bin\gnuplot.exe";
		static string path = Environment.ExpandEnvironmentVariables(src);
		static bool exist = true;

		static readonly Styles style = Styles.points;
		enum Styles { dots, impulses, lines, points, steps };

		static Writer S;
		delegate void Writer(string s, params object[] args);
	}
}
