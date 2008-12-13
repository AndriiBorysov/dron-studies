using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace lab11
{
	public partial class frmMainForm : Form
	{
		static public int number1;
		static public int number2;
		static public int number3;
		static public bool doSum;
		static public bool doLeastMult;
		static public int sumResult;
		static public int leastResult;

		public frmMainForm()
		{
			InitializeComponent();
			number1 = 0;
			number2 = 0;
			number3 = 0;
			doSum = false;
			doLeastMult = false;
			sumResult = 0;
			leastResult = 0;
		}

		private void inputToolStripMenuItem_Click(object sender, EventArgs e)
		{
			labelLeast.Visible = false;
			labelSum.Visible = false;
			textSumResult.Visible = false;
			textLeastResult.Visible = false;
			frmInput input = new frmInput();
			input.ShowDialog();
			input.Dispose();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmAbout aboutBox = new frmAbout();
			aboutBox.ShowDialog();
			aboutBox.Dispose();
		}

		private void calcToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ( doSum )
			{
				int sum = number1 + number2 + number3;
				textSumResult.Text = sum.ToString();
				labelSum.Visible = true;
				textSumResult.Visible = true;
			}
			if (doLeastMult && number1 > 0 && number2 > 0)
			{
				textLeastResult.Text = 
					(number1 * number2 / gcd((uint)number1, (uint)number2)).ToString();
				labelLeast.Visible = true;
				textLeastResult.Visible = true;
			}
		}
		private uint gcd(uint u, uint v)
		{
			int shift;
			 /* GCD(0,x) := x */
			if (u == 0 || v == 0)
				return u | v;
			 /* Let shift := lg K, where K is the greatest power of 2
				dividing both u and v. */
			for (shift = 0; ((u | v) & 1) == 0; ++shift)
			{
				u >>= 1;
				v >>= 1;
			}
			while ((u & 1) == 0)
				u >>= 1;
		 
			 /* From here on, u is always odd. */
			do
			{
				while ((v & 1) == 0)  /* Loop X */
					v >>= 1;

				/* Now u and v are both odd, so diff(u, v) is even.
				Let u = min(u, v), v = diff(u, v)/2. */
				if (u < v)
				{
					v -= u;
				}
				else
				{
					uint diff = u - v;
					u = v;
					v = diff;
				}
				v >>= 1;
			}while (v != 0);
		return u << shift;
		}
	}
}
