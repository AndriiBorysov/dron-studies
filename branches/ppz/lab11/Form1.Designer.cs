namespace lab11
{
	partial class frmMainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.inputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.calcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.textSumResult = new System.Windows.Forms.TextBox();
			this.labelSum = new System.Windows.Forms.Label();
			this.labelLeast = new System.Windows.Forms.Label();
			this.textLeastResult = new System.Windows.Forms.TextBox();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.inputToolStripMenuItem,
			this.calcToolStripMenuItem,
			this.aboutToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(292, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// inputToolStripMenuItem
			// 
			this.inputToolStripMenuItem.Name = "inputToolStripMenuItem";
			this.inputToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
			this.inputToolStripMenuItem.Text = "Input";
			this.inputToolStripMenuItem.Click += new System.EventHandler(this.inputToolStripMenuItem_Click);
			// 
			// calcToolStripMenuItem
			// 
			this.calcToolStripMenuItem.Name = "calcToolStripMenuItem";
			this.calcToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.calcToolStripMenuItem.Text = "Calc";
			this.calcToolStripMenuItem.Click += new System.EventHandler(this.calcToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// textSumResult
			// 
			this.textSumResult.Location = new System.Drawing.Point(110, 60);
			this.textSumResult.Name = "textSumResult";
			this.textSumResult.ReadOnly = true;
			this.textSumResult.Size = new System.Drawing.Size(140, 20);
			this.textSumResult.TabIndex = 1;
			this.textSumResult.Visible = false;
			// 
			// labelSum
			// 
			this.labelSum.AutoSize = true;
			this.labelSum.Location = new System.Drawing.Point(73, 63);
			this.labelSum.Name = "labelSum";
			this.labelSum.Size = new System.Drawing.Size(31, 13);
			this.labelSum.TabIndex = 2;
			this.labelSum.Text = "Sum:";
			this.labelSum.Visible = false;
			// 
			// labelLeast
			// 
			this.labelLeast.AutoSize = true;
			this.labelLeast.Location = new System.Drawing.Point(30, 112);
			this.labelLeast.Name = "labelLeast";
			this.labelLeast.Size = new System.Drawing.Size(74, 13);
			this.labelLeast.TabIndex = 4;
			this.labelLeast.Text = "Least multiple:";
			this.labelLeast.Visible = false;
			// 
			// textLeastResult
			// 
			this.textLeastResult.Location = new System.Drawing.Point(110, 109);
			this.textLeastResult.Name = "textLeastResult";
			this.textLeastResult.ReadOnly = true;
			this.textLeastResult.Size = new System.Drawing.Size(140, 20);
			this.textLeastResult.TabIndex = 3;
			this.textLeastResult.Visible = false;
			// 
			// frmMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 173);
			this.Controls.Add(this.labelLeast);
			this.Controls.Add(this.textLeastResult);
			this.Controls.Add(this.labelSum);
			this.Controls.Add(this.textSumResult);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.Name = "frmMainForm";
			this.Text = "Lab 11";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem inputToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem calcToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.TextBox textSumResult;
		private System.Windows.Forms.Label labelSum;
		private System.Windows.Forms.Label labelLeast;
		private System.Windows.Forms.TextBox textLeastResult;
	}
}

