namespace DeveloperUtils
{
    partial class CreateDatabaseForm
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
            this.CreateButton = new System.Windows.Forms.Button();
            this.openDatabaseFileButton = new System.Windows.Forms.Button();
            this.databaseTextBox = new System.Windows.Forms.TextBox();
            this.connectionStringTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sqlAgentsComboBox = new System.Windows.Forms.ComboBox();
            this.schemaInFileButton = new System.Windows.Forms.RadioButton();
            this.schemaInFolderButton = new System.Windows.Forms.RadioButton();
            this.schemaPathTextBox = new System.Windows.Forms.TextBox();
            this.openSchemaPathButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CreateButton
            // 
            this.CreateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CreateButton.Location = new System.Drawing.Point(534, 155);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(75, 23);
            this.CreateButton.TabIndex = 18;
            this.CreateButton.Text = "Create";
            this.CreateButton.UseVisualStyleBackColor = true;
            this.CreateButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // openDatabaseFileButton
            // 
            this.openDatabaseFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openDatabaseFileButton.Location = new System.Drawing.Point(583, 65);
            this.openDatabaseFileButton.Name = "openDatabaseFileButton";
            this.openDatabaseFileButton.Size = new System.Drawing.Size(26, 20);
            this.openDatabaseFileButton.TabIndex = 14;
            this.openDatabaseFileButton.Text = "...";
            this.openDatabaseFileButton.UseVisualStyleBackColor = true;
            this.openDatabaseFileButton.Click += new System.EventHandler(this.openDatabaseFileButton_Click);
            // 
            // databaseTextBox
            // 
            this.databaseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseTextBox.Location = new System.Drawing.Point(121, 65);
            this.databaseTextBox.Name = "databaseTextBox";
            this.databaseTextBox.Size = new System.Drawing.Size(460, 20);
            this.databaseTextBox.TabIndex = 12;
            // 
            // connectionStringTextBox
            // 
            this.connectionStringTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionStringTextBox.Location = new System.Drawing.Point(121, 39);
            this.connectionStringTextBox.Name = "connectionStringTextBox";
            this.connectionStringTextBox.Size = new System.Drawing.Size(488, 20);
            this.connectionStringTextBox.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Database:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Connection String:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "SQL Implementation:";
            // 
            // sqlAgentsComboBox
            // 
            this.sqlAgentsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlAgentsComboBox.FormattingEnabled = true;
            this.sqlAgentsComboBox.Location = new System.Drawing.Point(121, 12);
            this.sqlAgentsComboBox.Name = "sqlAgentsComboBox";
            this.sqlAgentsComboBox.Size = new System.Drawing.Size(488, 21);
            this.sqlAgentsComboBox.TabIndex = 9;
            // 
            // schemaInFileButton
            // 
            this.schemaInFileButton.AutoSize = true;
            this.schemaInFileButton.Checked = true;
            this.schemaInFileButton.Location = new System.Drawing.Point(121, 91);
            this.schemaInFileButton.Name = "schemaInFileButton";
            this.schemaInFileButton.Size = new System.Drawing.Size(95, 17);
            this.schemaInFileButton.TabIndex = 22;
            this.schemaInFileButton.TabStop = true;
            this.schemaInFileButton.Text = "Schema In File";
            this.schemaInFileButton.UseVisualStyleBackColor = true;
            // 
            // schemaInFolderButton
            // 
            this.schemaInFolderButton.AutoSize = true;
            this.schemaInFolderButton.Location = new System.Drawing.Point(245, 91);
            this.schemaInFolderButton.Name = "schemaInFolderButton";
            this.schemaInFolderButton.Size = new System.Drawing.Size(108, 17);
            this.schemaInFolderButton.TabIndex = 23;
            this.schemaInFolderButton.TabStop = true;
            this.schemaInFolderButton.Text = "Schema In Folder";
            this.schemaInFolderButton.UseVisualStyleBackColor = true;
            // 
            // schemaPathTextBox
            // 
            this.schemaPathTextBox.Location = new System.Drawing.Point(121, 114);
            this.schemaPathTextBox.Name = "schemaPathTextBox";
            this.schemaPathTextBox.ReadOnly = true;
            this.schemaPathTextBox.Size = new System.Drawing.Size(460, 20);
            this.schemaPathTextBox.TabIndex = 24;
            // 
            // openSchemaPathButton
            // 
            this.openSchemaPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openSchemaPathButton.Location = new System.Drawing.Point(583, 114);
            this.openSchemaPathButton.Name = "openSchemaPathButton";
            this.openSchemaPathButton.Size = new System.Drawing.Size(26, 20);
            this.openSchemaPathButton.TabIndex = 25;
            this.openSchemaPathButton.Text = "...";
            this.openSchemaPathButton.UseVisualStyleBackColor = true;
            this.openSchemaPathButton.Click += new System.EventHandler(this.openSchemaPathButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Schema Path:";
            // 
            // CreateDatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 188);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.openSchemaPathButton);
            this.Controls.Add(this.schemaPathTextBox);
            this.Controls.Add(this.schemaInFolderButton);
            this.Controls.Add(this.schemaInFileButton);
            this.Controls.Add(this.CreateButton);
            this.Controls.Add(this.openDatabaseFileButton);
            this.Controls.Add(this.databaseTextBox);
            this.Controls.Add(this.connectionStringTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sqlAgentsComboBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateDatabaseForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "CreateDatabaseForm";
            this.Load += new System.EventHandler(this.CreateDatabaseForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button CreateButton;
        private System.Windows.Forms.Button openDatabaseFileButton;
        private System.Windows.Forms.TextBox databaseTextBox;
        private System.Windows.Forms.TextBox connectionStringTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox sqlAgentsComboBox;
        private System.Windows.Forms.RadioButton schemaInFileButton;
        private System.Windows.Forms.RadioButton schemaInFolderButton;
        private System.Windows.Forms.TextBox schemaPathTextBox;
        private System.Windows.Forms.Button openSchemaPathButton;
        private System.Windows.Forms.Label label4;
    }
}