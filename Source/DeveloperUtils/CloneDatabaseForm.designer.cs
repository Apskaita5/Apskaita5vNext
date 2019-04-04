namespace DeveloperUtils
{
    partial class CloneDatabaseForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.openSourceDatabaseFileButton = new System.Windows.Forms.Button();
            this.sourceDatabaseTextBox = new System.Windows.Forms.TextBox();
            this.sourceConnectionStringTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sourceSqlAgentsComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.openTargetDatabaseFileButton = new System.Windows.Forms.Button();
            this.targetDatabaseTextBox = new System.Windows.Forms.TextBox();
            this.targetConnectionStringTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.targetSqlAgentsComboBox = new System.Windows.Forms.ComboBox();
            this.BeginCloneButton = new System.Windows.Forms.Button();
            this.openSchemaButton = new System.Windows.Forms.Button();
            this.schemaTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.enforceSchemaCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.openSourceDatabaseFileButton);
            this.groupBox1.Controls.Add(this.sourceDatabaseTextBox);
            this.groupBox1.Controls.Add(this.sourceConnectionStringTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.sourceSqlAgentsComboBox);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(624, 103);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Database Connection";
            // 
            // openSourceDatabaseFileButton
            // 
            this.openSourceDatabaseFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openSourceDatabaseFileButton.Location = new System.Drawing.Point(574, 72);
            this.openSourceDatabaseFileButton.Name = "openSourceDatabaseFileButton";
            this.openSourceDatabaseFileButton.Size = new System.Drawing.Size(26, 20);
            this.openSourceDatabaseFileButton.TabIndex = 3;
            this.openSourceDatabaseFileButton.Text = "...";
            this.openSourceDatabaseFileButton.UseVisualStyleBackColor = true;
            this.openSourceDatabaseFileButton.Click += new System.EventHandler(this.openSourceDatabaseFileButton_Click);
            // 
            // sourceDatabaseTextBox
            // 
            this.sourceDatabaseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceDatabaseTextBox.Location = new System.Drawing.Point(120, 72);
            this.sourceDatabaseTextBox.Name = "sourceDatabaseTextBox";
            this.sourceDatabaseTextBox.Size = new System.Drawing.Size(448, 20);
            this.sourceDatabaseTextBox.TabIndex = 2;
            // 
            // sourceConnectionStringTextBox
            // 
            this.sourceConnectionStringTextBox.AcceptsReturn = true;
            this.sourceConnectionStringTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceConnectionStringTextBox.Location = new System.Drawing.Point(120, 46);
            this.sourceConnectionStringTextBox.Name = "sourceConnectionStringTextBox";
            this.sourceConnectionStringTextBox.Size = new System.Drawing.Size(480, 20);
            this.sourceConnectionStringTextBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Database:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Connection String:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "SQL Implementation:";
            // 
            // sourceSqlAgentsComboBox
            // 
            this.sourceSqlAgentsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceSqlAgentsComboBox.FormattingEnabled = true;
            this.sourceSqlAgentsComboBox.Location = new System.Drawing.Point(120, 19);
            this.sourceSqlAgentsComboBox.Name = "sourceSqlAgentsComboBox";
            this.sourceSqlAgentsComboBox.Size = new System.Drawing.Size(480, 21);
            this.sourceSqlAgentsComboBox.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.openTargetDatabaseFileButton);
            this.groupBox2.Controls.Add(this.targetDatabaseTextBox);
            this.groupBox2.Controls.Add(this.targetConnectionStringTextBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.targetSqlAgentsComboBox);
            this.groupBox2.Location = new System.Drawing.Point(3, 112);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(624, 103);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target Database Connection";
            // 
            // openTargetDatabaseFileButton
            // 
            this.openTargetDatabaseFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openTargetDatabaseFileButton.Location = new System.Drawing.Point(574, 72);
            this.openTargetDatabaseFileButton.Name = "openTargetDatabaseFileButton";
            this.openTargetDatabaseFileButton.Size = new System.Drawing.Size(26, 20);
            this.openTargetDatabaseFileButton.TabIndex = 3;
            this.openTargetDatabaseFileButton.Text = "...";
            this.openTargetDatabaseFileButton.UseVisualStyleBackColor = true;
            this.openTargetDatabaseFileButton.Click += new System.EventHandler(this.openTargetDatabaseFileButton_Click);
            // 
            // targetDatabaseTextBox
            // 
            this.targetDatabaseTextBox.AcceptsReturn = true;
            this.targetDatabaseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetDatabaseTextBox.Location = new System.Drawing.Point(120, 72);
            this.targetDatabaseTextBox.Name = "targetDatabaseTextBox";
            this.targetDatabaseTextBox.Size = new System.Drawing.Size(448, 20);
            this.targetDatabaseTextBox.TabIndex = 2;
            this.targetDatabaseTextBox.TextChanged += new System.EventHandler(this.targetDatabaseTextBox_TextChanged);
            // 
            // targetConnectionStringTextBox
            // 
            this.targetConnectionStringTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetConnectionStringTextBox.Location = new System.Drawing.Point(120, 46);
            this.targetConnectionStringTextBox.Name = "targetConnectionStringTextBox";
            this.targetConnectionStringTextBox.Size = new System.Drawing.Size(480, 20);
            this.targetConnectionStringTextBox.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Database:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Connection String:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "SQL Implementation:";
            // 
            // targetSqlAgentsComboBox
            // 
            this.targetSqlAgentsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetSqlAgentsComboBox.FormattingEnabled = true;
            this.targetSqlAgentsComboBox.Location = new System.Drawing.Point(120, 19);
            this.targetSqlAgentsComboBox.Name = "targetSqlAgentsComboBox";
            this.targetSqlAgentsComboBox.Size = new System.Drawing.Size(480, 21);
            this.targetSqlAgentsComboBox.TabIndex = 0;
            // 
            // BeginCloneButton
            // 
            this.BeginCloneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BeginCloneButton.Location = new System.Drawing.Point(552, 256);
            this.BeginCloneButton.Name = "BeginCloneButton";
            this.BeginCloneButton.Size = new System.Drawing.Size(75, 23);
            this.BeginCloneButton.TabIndex = 2;
            this.BeginCloneButton.Text = "Clone";
            this.BeginCloneButton.UseVisualStyleBackColor = true;
            this.BeginCloneButton.Click += new System.EventHandler(this.BeginCloneButton_Click);
            // 
            // openSchemaButton
            // 
            this.openSchemaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openSchemaButton.Location = new System.Drawing.Point(601, 219);
            this.openSchemaButton.Name = "openSchemaButton";
            this.openSchemaButton.Size = new System.Drawing.Size(26, 20);
            this.openSchemaButton.TabIndex = 12;
            this.openSchemaButton.Text = "...";
            this.openSchemaButton.UseVisualStyleBackColor = true;
            this.openSchemaButton.Click += new System.EventHandler(this.openSchemaButton_Click);
            // 
            // schemaTextBox
            // 
            this.schemaTextBox.AcceptsReturn = true;
            this.schemaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.schemaTextBox.Location = new System.Drawing.Point(144, 221);
            this.schemaTextBox.Name = "schemaTextBox";
            this.schemaTextBox.ReadOnly = true;
            this.schemaTextBox.Size = new System.Drawing.Size(451, 20);
            this.schemaTextBox.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 224);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Enforce Schema:";
            // 
            // enforceSchemaCheckBox
            // 
            this.enforceSchemaCheckBox.AutoSize = true;
            this.enforceSchemaCheckBox.Location = new System.Drawing.Point(123, 225);
            this.enforceSchemaCheckBox.Name = "enforceSchemaCheckBox";
            this.enforceSchemaCheckBox.Size = new System.Drawing.Size(15, 14);
            this.enforceSchemaCheckBox.TabIndex = 14;
            this.enforceSchemaCheckBox.UseVisualStyleBackColor = true;
            // 
            // CloneDatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 287);
            this.Controls.Add(this.enforceSchemaCheckBox);
            this.Controls.Add(this.openSchemaButton);
            this.Controls.Add(this.schemaTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.BeginCloneButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "CloneDatabaseForm";
            this.ShowInTaskbar = false;
            this.Text = "Clone Database";
            this.Load += new System.EventHandler(this.CloneDatabaseForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button openSourceDatabaseFileButton;
        private System.Windows.Forms.TextBox sourceDatabaseTextBox;
        private System.Windows.Forms.TextBox sourceConnectionStringTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox sourceSqlAgentsComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button openTargetDatabaseFileButton;
        private System.Windows.Forms.TextBox targetDatabaseTextBox;
        private System.Windows.Forms.TextBox targetConnectionStringTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox targetSqlAgentsComboBox;
        private System.Windows.Forms.Button BeginCloneButton;
        private System.Windows.Forms.Button openSchemaButton;
        private System.Windows.Forms.TextBox schemaTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox enforceSchemaCheckBox;
    }
}