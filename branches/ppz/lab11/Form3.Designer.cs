namespace lab11
{
	partial class frmMatrix
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
			this.dataMatrix = new System.Windows.Forms.DataGridView();
			this.btnSmooth = new System.Windows.Forms.Button();
			this.btnSum = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.textSum = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.dataMatrix)).BeginInit();
			this.SuspendLayout();
			// 
			// dataMatrix
			// 
			this.dataMatrix.AllowUserToAddRows = false;
			this.dataMatrix.AllowUserToDeleteRows = false;
			this.dataMatrix.AllowUserToResizeColumns = false;
			this.dataMatrix.AllowUserToResizeRows = false;
			this.dataMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataMatrix.ColumnHeadersVisible = false;
			this.dataMatrix.Location = new System.Drawing.Point(12, 12);
			this.dataMatrix.MultiSelect = false;
			this.dataMatrix.Name = "dataMatrix";
			this.dataMatrix.RowHeadersVisible = false;
			this.dataMatrix.Size = new System.Drawing.Size(565, 266);
			this.dataMatrix.TabIndex = 0;
			// 
			// btnSmooth
			// 
			this.btnSmooth.Location = new System.Drawing.Point(12, 301);
			this.btnSmooth.Name = "btnSmooth";
			this.btnSmooth.Size = new System.Drawing.Size(131, 23);
			this.btnSmooth.TabIndex = 1;
			this.btnSmooth.Text = "Smooth";
			this.btnSmooth.UseVisualStyleBackColor = true;
			this.btnSmooth.Click += new System.EventHandler(this.btnSmooth_Click);
			// 
			// btnSum
			// 
			this.btnSum.Location = new System.Drawing.Point(12, 330);
			this.btnSum.Name = "btnSum";
			this.btnSum.Size = new System.Drawing.Size(131, 23);
			this.btnSum.TabIndex = 2;
			this.btnSum.Text = "Sum";
			this.btnSum.UseVisualStyleBackColor = true;
			this.btnSum.Click += new System.EventHandler(this.btnSum_Click);
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(12, 359);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(131, 23);
			this.btnExit.TabIndex = 3;
			this.btnExit.Text = "Exit";
			this.btnExit.UseVisualStyleBackColor = true;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// textSum
			// 
			this.textSum.Location = new System.Drawing.Point(149, 332);
			this.textSum.Name = "textSum";
			this.textSum.ReadOnly = true;
			this.textSum.Size = new System.Drawing.Size(157, 20);
			this.textSum.TabIndex = 4;
			// 
			// frmMatrix
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(667, 397);
			this.Controls.Add(this.textSum);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnSum);
			this.Controls.Add(this.btnSmooth);
			this.Controls.Add(this.dataMatrix);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmMatrix";
			this.Text = "Handle Matrix";
			this.Load += new System.EventHandler(this.frmMatrix_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataMatrix)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataMatrix;
		private System.Windows.Forms.Button btnSmooth;
		private System.Windows.Forms.Button btnSum;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.TextBox textSum;
	}
}