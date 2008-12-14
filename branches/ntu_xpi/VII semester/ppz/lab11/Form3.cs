using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace lab11
{
	public partial class frmMatrix : Form
	{
		static int column_count = 10;
		public frmMatrix()
		{
			InitializeComponent();
		}

		private void frmMatrix_Load(object sender, EventArgs e)
		{
			dataMatrix.ColumnCount = column_count;
			dataMatrix.RowCount = column_count;
			for (int i = 0; i < column_count; i++ )
				dataMatrix.Columns[i].Width = 50;
			dataMatrix.Width = 
				column_count * dataMatrix.Columns[0].Width + column_count;
			dataMatrix.Height = 
				column_count * dataMatrix.Rows[0].Height + column_count;
		}

		private void btnSmooth_Click(object sender, EventArgs e)
		{
			double[,] matrix = new double[column_count, column_count];
			for (int i = 0; i < dataMatrix.ColumnCount; i++ )
			{
				for (int j = 0; j < dataMatrix.RowCount; j++ )
				{
					matrix[i,j] = Convert.ToDouble(dataMatrix[i,j].Value);
				}
			}
			for (int i = 1; i < dataMatrix.ColumnCount - 1; i++)
			{
				for (int j = 1; j < dataMatrix.RowCount - 1; j++)
				{
					double sum = 0;
					int cell_count = 0;
					for (int m = i - 1; m <= i + 1; m++)
					{
						for (int n = j - 1; n <= j + 1; n++)
						{
							if (!(m == i && n == j))
							{
								cell_count++;
								sum += matrix[m, n];
							}
						}
					}
					dataMatrix[i, j].Value = sum * 1.0 / cell_count;
				}
			}

		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnSum_Click(object sender, EventArgs e)
		{
			double sum = 0;
			for (int i = 0; i < dataMatrix.ColumnCount - 1; i++)
			{
				for (int j = i+1; j < dataMatrix.RowCount; j++)
				{
					sum += Convert.ToDouble(dataMatrix[i, j].Value);
				}
			}
			textSum.Text = sum.ToString();
		}
	}
}
