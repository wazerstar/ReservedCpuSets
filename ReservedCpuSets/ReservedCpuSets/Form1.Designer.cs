namespace ReservedCpuSets
{
    partial class Form1
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
            this.cpuListBox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.invertSelection = new System.Windows.Forms.Button();
            this.checkAll = new System.Windows.Forms.Button();
            this.uncheckAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cpuListBox
            // 
            this.cpuListBox.CheckOnClick = true;
            this.cpuListBox.ColumnWidth = 92;
            this.cpuListBox.FormattingEnabled = true;
            this.cpuListBox.Location = new System.Drawing.Point(12, 50);
            this.cpuListBox.MultiColumn = true;
            this.cpuListBox.Name = "cpuListBox";
            this.cpuListBox.Size = new System.Drawing.Size(260, 154);
            this.cpuListBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "The reserved CPUs setting controls which CPUs \r\nnot to schedule tasks on";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 268);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(260, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // invertSelection
            // 
            this.invertSelection.Location = new System.Drawing.Point(12, 239);
            this.invertSelection.Name = "invertSelection";
            this.invertSelection.Size = new System.Drawing.Size(260, 23);
            this.invertSelection.TabIndex = 4;
            this.invertSelection.Text = "Invert Selection";
            this.invertSelection.UseVisualStyleBackColor = true;
            this.invertSelection.Click += new System.EventHandler(this.invertSelection_Click);
            // 
            // checkAll
            // 
            this.checkAll.Location = new System.Drawing.Point(12, 211);
            this.checkAll.Name = "checkAll";
            this.checkAll.Size = new System.Drawing.Size(126, 23);
            this.checkAll.TabIndex = 5;
            this.checkAll.Text = "Check All";
            this.checkAll.UseVisualStyleBackColor = true;
            this.checkAll.Click += new System.EventHandler(this.checkAll_Click);
            // 
            // uncheckAll
            // 
            this.uncheckAll.Location = new System.Drawing.Point(146, 211);
            this.uncheckAll.Name = "uncheckAll";
            this.uncheckAll.Size = new System.Drawing.Size(126, 23);
            this.uncheckAll.TabIndex = 6;
            this.uncheckAll.Text = "Uncheck All";
            this.uncheckAll.UseVisualStyleBackColor = true;
            this.uncheckAll.Click += new System.EventHandler(this.uncheckAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 303);
            this.Controls.Add(this.uncheckAll);
            this.Controls.Add(this.checkAll);
            this.Controls.Add(this.invertSelection);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cpuListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "ReservedCpuSets v0.1.1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox cpuListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button invertSelection;
        private System.Windows.Forms.Button checkAll;
        private System.Windows.Forms.Button uncheckAll;
    }
}

