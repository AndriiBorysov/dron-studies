namespace lab11
{
	partial class frmInput
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textNum1 = new System.Windows.Forms.TextBox();
			this.textNum2 = new System.Windows.Forms.TextBox();
			this.textNum3 = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkLeastMult = new System.Windows.Forms.CheckBox();
			this.checkSum = new System.Windows.Forms.CheckBox();
			this.btnAccept = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 74);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Number 3:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Number 2:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Number 1:";
			// 
			// textNum1
			// 
			this.textNum1.Location = new System.Drawing.Point(68, 19);
			this.textNum1.MaxLength = 10;
			this.textNum1.Name = "textNum1";
			this.textNum1.Size = new System.Drawing.Size(127, 20);
			this.textNum1.TabIndex = 3;
			// 
			// textNum2
			// 
			this.textNum2.Location = new System.Drawing.Point(68, 45);
			this.textNum2.MaxLength = 10;
			this.textNum2.Name = "textNum2";
			this.textNum2.Size = new System.Drawing.Size(127, 20);
			this.textNum2.TabIndex = 3;
			// 
			// textNum3
			// 
			this.textNum3.Location = new System.Drawing.Point(68, 71);
			this.textNum3.MaxLength = 10;
			this.textNum3.Name = "textNum3";
			this.textNum3.Size = new System.Drawing.Size(127, 20);
			this.textNum3.TabIndex = 3;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textNum2);
			this.groupBox1.Controls.Add(this.textNum3);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textNum1);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(3, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(201, 102);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Numbers";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkLeastMult);
			this.groupBox2.Controls.Add(this.checkSum);
			this.groupBox2.Location = new System.Drawing.Point(210, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(125, 102);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Action";
			// 
			// checkLeastMult
			// 
			this.checkLeastMult.AutoSize = true;
			this.checkLeastMult.Location = new System.Drawing.Point(20, 60);
			this.checkLeastMult.Name = "checkLeastMult";
			this.checkLeastMult.Size = new System.Drawing.Size(90, 17);
			this.checkLeastMult.TabIndex = 1;
			this.checkLeastMult.Text = "Least multiple";
			this.checkLeastMult.UseVisualStyleBackColor = true;
			// 
			// checkSum
			// 
			this.checkSum.AutoSize = true;
			this.checkSum.Location = new System.Drawing.Point(20, 22);
			this.checkSum.Name = "checkSum";
			this.checkSum.Size = new System.Drawing.Size(47, 17);
			this.checkSum.TabIndex = 0;
			this.checkSum.Text = "Sum";
			this.checkSum.UseVisualStyleBackColor = true;
			// 
			// btnAccept
			// 
			this.btnAccept.Location = new System.Drawing.Point(113, 120);
			this.btnAccept.Name = "btnAccept";
			this.btnAccept.Size = new System.Drawing.Size(121, 23);
			this.btnAccept.TabIndex = 6;
			this.btnAccept.Text = "Accept";
			this.btnAccept.UseVisualStyleBackColor = true;
			this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
			// 
			// frmInput
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(347, 160);
			this.Controls.Add(this.btnAccept);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmInput";
			this.Text = "Input";
			this.Load += new System.EventHandler(this.frmInput_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textNum1;
		private System.Windows.Forms.TextBox textNum2;
		private System.Windows.Forms.TextBox textNum3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkLeastMult;
		private System.Windows.Forms.CheckBox checkSum;
		private System.Windows.Forms.Button btnAccept;
	}
}