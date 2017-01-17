namespace NumericalAnalysis
{
	public class GausMethod
	{
		int rowCount;
		int columCount;
		double[,] matrix;
		double[] rightPart;
		public double[] Answer { get; }

		public GausMethod(double[,] matrix, double[] rightPart)
		{
			this.rightPart = rightPart;
			this.matrix = matrix;
			rowCount = matrix.GetLength(0);
			columCount = matrix.GetLength(1);
			Answer = new double[rowCount];
		}
		public GausMethod(int m, int n, A a, B b)
		{
			matrix = new double[m, n];
			for (int i = 0; i < m; i++)
				for (int j = 0; j < n; j++)
					matrix[i, j] = a(i, j);

			rightPart = new double[m];
			for (int i = 0; i < rightPart.Length; i++)
				rightPart[i] = b(i);

			rowCount = matrix.GetLength(0);
			columCount = matrix.GetLength(1);
			Answer = new double[rowCount];

			SolveMatrix();
		}

		private void SortRows(int SortIndex)
		{
			double MaxElement = matrix[SortIndex, SortIndex];
			int MaxElementIndex = SortIndex;
			for (int i = SortIndex + 1; i < rowCount; i++)
				if (matrix[i, SortIndex] > MaxElement)
				{
					MaxElement = matrix[i, SortIndex];
					MaxElementIndex = i;
				}

			//теперь найден максимальный элемент ставим его на верхнее место
			if (MaxElementIndex > SortIndex) //если это не первый элемент
			{
				double Temp;

				Temp = rightPart[MaxElementIndex];
				rightPart[MaxElementIndex] = rightPart[SortIndex];
				rightPart[SortIndex] = Temp;

				for (int i = 0; i < columCount; i++)
				{
					Temp = matrix[MaxElementIndex, i];
					matrix[MaxElementIndex, i] = matrix[SortIndex, i];
					matrix[SortIndex, i] = Temp;
				}
			}
		}
		public int SolveMatrix()
		{
			if (rowCount != columCount)
				return 1; //нет решения

			for (int i = 0; i < rowCount - 1; i++)
			{
				SortRows(i);
				for (int j = i + 1; j < rowCount; j++)
					if (matrix[i, i] != 0) //если главный элемент не 0, то производим вычисления
					{
						double MultElement = matrix[j, i] / matrix[i, i];
						for (int k = i; k < columCount; k++)
							matrix[j, k] -= matrix[i, k] * MultElement;
						rightPart[j] -= rightPart[i] * MultElement;
					}
					//для нулевого главного элемента просто пропускаем данный шаг
			}

			//ищем решение
			for (int i = rowCount - 1; i >= 0; i--)
			{
				Answer[i] = rightPart[i];

				for (int j = rowCount - 1; j > i; j--)
					Answer[i] -= matrix[i, j] * Answer[j];

				if (matrix[i, i] == 0)
					if (rightPart[i] == 0)
						return 2; //множество решений
					else
						return 1; //нет решения

				Answer[i] /= matrix[i, i];

			}
			return 0;
		}

		public override string ToString()
		{
			var s = new System.Text.StringBuilder();

			for (int i = 0; i < rowCount; i++)
			{
				s.Append("\r\n");

				for (int j = 0; j < columCount; j++)
					s.Append(matrix[i, j].ToString("F04") + "\t");

				s.Append("\t" + Answer[i].ToString("F08"));
				s.Append("\t" + rightPart[i].ToString("F04"));
			}

			return s.ToString();
		}

		public delegate double A(int x, int y);
		public delegate double B(int i);
	}
}

// http://www.cyberforum.ru/csharp-beginners/thread510942.html