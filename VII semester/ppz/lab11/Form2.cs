using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace lab11
{
	public partial class frmInput : Form
	{
		public frmInput()
		{
			InitializeComponent();
		}

		private void frmInput_Load(object sender, EventArgs e)
		{
			checkLeastMult.Checked = frmMainForm.doLeastMult;
			checkSum.Checked = frmMainForm.doSum;
			textNum1.Text = frmMainForm.number1.ToString();
			textNum2.Text = frmMainForm.number2.ToString();
			textNum3.Text = frmMainForm.number3.ToString();
		}

		private void btnAccept_Click(object sender, EventArgs e)
		{
			bool result = int.TryParse(textNum1.Text, out frmMainForm.number1);
			if (!result)
			{
				MessageBox.Show("Number 1 box does not content a number!", "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			result = int.TryParse(textNum2.Text, out frmMainForm.number2);
			if (!result)
			{
				MessageBox.Show("Number 2 box does not content a number!", "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			result = int.TryParse(textNum3.Text, out frmMainForm.number3);
			if (!result)
			{
				MessageBox.Show("Number 3 box does not content a number!", "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			frmMainForm.doLeastMult = checkLeastMult.Checked;
			frmMainForm.doSum = checkSum.Checked;
			Close();
		}
	}
}
