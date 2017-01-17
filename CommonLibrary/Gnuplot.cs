using static System.String;

namespace NumericalAnalysis
{
	public class Gnuplot
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
			using (var file = new System.IO.StreamWriter("gnuplotRun.txt"))
			{
				S = (string s, object[] args) =>
					file.WriteLine(Format(s, args));

				SetWindow(title);
				SetLabels(x, arrayName, z[0], z[z.Length - 1]);

				Plot(functions, z[0], z[z.Length - 1]);
			}

			for (int i = 0; i < y.GetLength(0); i++)
				using (var file = new System.IO.StreamWriter("data" + i))
					for (int j = 0; j < xx.Length; j++)
						file.WriteLine("{0,-10} {1}", xx[j], y[i][j]);

			Start();
		}

		public static void Run(double[] x, double[] xx,
			string title, string arrayName, params double[][] y)
			=> Run(x, xx, "", x, title, arrayName, y);

		public static void Run(double[] x, double[] xx, string functions,
			string title, string arrayName, params double[][] y)
			=> Run(x, xx, functions, x, title, arrayName, y);

		public static void Start()
		{
			var cmd = new System.Diagnostics.Process();
			//cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			cmd.StartInfo.FileName = "cmd.exe";
			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = false;
			cmd.Start();
			cmd.StandardInput.WriteLine(
				@"""C:\Program Files (x86)\gnuplot\bin\gnuplot.exe""" +
				@" -persist gnuplotRun.txt");
			cmd.StandardInput.Flush();
			cmd.StandardInput.Close();
			cmd.WaitForExit();
			cmd.Close();
			//System.Console.WriteLine(cmd.StandardOutput.ReadToEnd());
		}

		static W S;
		static readonly styles style = styles.points;
		enum styles { dots, impulses, lines, points, steps };
		delegate void W(string s, params object[] args);
	}
}
