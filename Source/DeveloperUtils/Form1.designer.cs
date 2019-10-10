namespace DeveloperUtils
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.insertTimeLabel = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.executeInsertButton = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.selectTimeLabel = new System.Windows.Forms.Label();
            this.executeSelectButton = new System.Windows.Forms.Button();
            this.selectTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(643, 485);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.insertTimeLabel);
            this.tabPage2.Controls.Add(this.progressBar1);
            this.tabPage2.Controls.Add(this.executeInsertButton);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(635, 459);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Insert";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // insertTimeLabel
            // 
            this.insertTimeLabel.AutoSize = true;
            this.insertTimeLabel.Location = new System.Drawing.Point(99, 45);
            this.insertTimeLabel.Name = "insertTimeLabel";
            this.insertTimeLabel.Size = new System.Drawing.Size(88, 13);
            this.insertTimeLabel.TabIndex = 2;
            this.insertTimeLabel.Text = "Executed in 0 ms";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(102, 19);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(525, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // executeInsertButton
            // 
            this.executeInsertButton.Location = new System.Drawing.Point(8, 19);
            this.executeInsertButton.Name = "executeInsertButton";
            this.executeInsertButton.Size = new System.Drawing.Size(75, 23);
            this.executeInsertButton.TabIndex = 0;
            this.executeInsertButton.Text = "Execute";
            this.executeInsertButton.UseVisualStyleBackColor = true;
            this.executeInsertButton.Click += new System.EventHandler(this.executeInsertButton_ClickAsync);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Controls.Add(this.selectTimeLabel);
            this.tabPage1.Controls.Add(this.executeSelectButton);
            this.tabPage1.Controls.Add(this.selectTextBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(635, 459);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Select";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.CausesValidation = false;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(8, 127);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(619, 324);
            this.dataGridView1.TabIndex = 3;
            // 
            // selectTimeLabel
            // 
            this.selectTimeLabel.AutoSize = true;
            this.selectTimeLabel.Location = new System.Drawing.Point(8, 99);
            this.selectTimeLabel.Name = "selectTimeLabel";
            this.selectTimeLabel.Size = new System.Drawing.Size(88, 13);
            this.selectTimeLabel.TabIndex = 2;
            this.selectTimeLabel.Text = "Executed in 0 ms";
            // 
            // executeSelectButton
            // 
            this.executeSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.executeSelectButton.Location = new System.Drawing.Point(552, 94);
            this.executeSelectButton.Name = "executeSelectButton";
            this.executeSelectButton.Size = new System.Drawing.Size(75, 23);
            this.executeSelectButton.TabIndex = 1;
            this.executeSelectButton.Text = "Execute";
            this.executeSelectButton.UseVisualStyleBackColor = true;
            // 
            // selectTextBox
            // 
            this.selectTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selectTextBox.Location = new System.Drawing.Point(8, 6);
            this.selectTextBox.Multiline = true;
            this.selectTextBox.Name = "selectTextBox";
            this.selectTextBox.Size = new System.Drawing.Size(619, 82);
            this.selectTextBox.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(364, 174);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 485);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label selectTimeLabel;
        private System.Windows.Forms.Button executeSelectButton;
        private System.Windows.Forms.TextBox selectTextBox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button executeInsertButton;
        private System.Windows.Forms.Label insertTimeLabel;
        private System.Windows.Forms.Button button1;
    }
}

