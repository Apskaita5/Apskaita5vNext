namespace DeveloperUtils
{
    partial class InitSqlAgentForm
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
            this.sqlAgentsComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.connectionStringTextBox = new System.Windows.Forms.TextBox();
            this.databaseTextBox = new System.Windows.Forms.TextBox();
            this.openDatabaseFileButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.openSqlRepositoryFolderButton = new System.Windows.Forms.Button();
            this.sqlRepositoryFolderTextBox = new System.Windows.Forms.TextBox();
            this.useSqlRepositoryCheckBox = new System.Windows.Forms.CheckBox();
            this.LoginButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sqlAgentsComboBox
            // 
            this.sqlAgentsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlAgentsComboBox.FormattingEnabled = true;
            this.sqlAgentsComboBox.Location = new System.Drawing.Point(123, 12);
            this.sqlAgentsComboBox.Name = "sqlAgentsComboBox";
            this.sqlAgentsComboBox.Size = new System.Drawing.Size(409, 21);
            this.sqlAgentsComboBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "SQL Implementation:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Connection String:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Database:";
            // 
            // connectionStringTextBox
            // 
            this.connectionStringTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionStringTextBox.Location = new System.Drawing.Point(123, 39);
            this.connectionStringTextBox.Name = "connectionStringTextBox";
            this.connectionStringTextBox.Size = new System.Drawing.Size(409, 20);
            this.connectionStringTextBox.TabIndex = 1;
            // 
            // databaseTextBox
            // 
            this.databaseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseTextBox.Location = new System.Drawing.Point(123, 65);
            this.databaseTextBox.Name = "databaseTextBox";
            this.databaseTextBox.Size = new System.Drawing.Size(382, 20);
            this.databaseTextBox.TabIndex = 2;
            // 
            // openDatabaseFileButton
            // 
            this.openDatabaseFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openDatabaseFileButton.Location = new System.Drawing.Point(506, 65);
            this.openDatabaseFileButton.Name = "openDatabaseFileButton";
            this.openDatabaseFileButton.Size = new System.Drawing.Size(26, 20);
            this.openDatabaseFileButton.TabIndex = 3;
            this.openDatabaseFileButton.Text = "...";
            this.openDatabaseFileButton.UseVisualStyleBackColor = true;
            this.openDatabaseFileButton.Click += new System.EventHandler(this.openDatabaseFileButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "SQL Repository:";
            // 
            // openSqlRepositoryFolderButton
            // 
            this.openSqlRepositoryFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openSqlRepositoryFolderButton.Location = new System.Drawing.Point(506, 91);
            this.openSqlRepositoryFolderButton.Name = "openSqlRepositoryFolderButton";
            this.openSqlRepositoryFolderButton.Size = new System.Drawing.Size(26, 20);
            this.openSqlRepositoryFolderButton.TabIndex = 5;
            this.openSqlRepositoryFolderButton.Text = "...";
            this.openSqlRepositoryFolderButton.UseVisualStyleBackColor = true;
            this.openSqlRepositoryFolderButton.Click += new System.EventHandler(this.openSqlRepositoryFolderButton_Click);
            // 
            // sqlRepositoryFolderTextBox
            // 
            this.sqlRepositoryFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlRepositoryFolderTextBox.Location = new System.Drawing.Point(144, 91);
            this.sqlRepositoryFolderTextBox.Name = "sqlRepositoryFolderTextBox";
            this.sqlRepositoryFolderTextBox.ReadOnly = true;
            this.sqlRepositoryFolderTextBox.Size = new System.Drawing.Size(361, 20);
            this.sqlRepositoryFolderTextBox.TabIndex = 8;
            this.sqlRepositoryFolderTextBox.TabStop = false;
            // 
            // useSqlRepositoryCheckBox
            // 
            this.useSqlRepositoryCheckBox.AutoSize = true;
            this.useSqlRepositoryCheckBox.Location = new System.Drawing.Point(123, 95);
            this.useSqlRepositoryCheckBox.Name = "useSqlRepositoryCheckBox";
            this.useSqlRepositoryCheckBox.Size = new System.Drawing.Size(15, 14);
            this.useSqlRepositoryCheckBox.TabIndex = 4;
            this.useSqlRepositoryCheckBox.UseVisualStyleBackColor = true;
            // 
            // LoginButton
            // 
            this.LoginButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LoginButton.Location = new System.Drawing.Point(373, 125);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(75, 23);
            this.LoginButton.TabIndex = 6;
            this.LoginButton.Text = "Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(457, 125);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // InitSqlAgentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 160);
            this.ControlBox = false;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.LoginButton);
            this.Controls.Add(this.useSqlRepositoryCheckBox);
            this.Controls.Add(this.openSqlRepositoryFolderButton);
            this.Controls.Add(this.sqlRepositoryFolderTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.openDatabaseFileButton);
            this.Controls.Add(this.databaseTextBox);
            this.Controls.Add(this.connectionStringTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sqlAgentsComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "InitSqlAgentForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Login SQL Server";
            this.Load += new System.EventHandler(this.InitSqlAgentForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox sqlAgentsComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox connectionStringTextBox;
        private System.Windows.Forms.TextBox databaseTextBox;
        private System.Windows.Forms.Button openDatabaseFileButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button openSqlRepositoryFolderButton;
        private System.Windows.Forms.TextBox sqlRepositoryFolderTextBox;
        private System.Windows.Forms.CheckBox useSqlRepositoryCheckBox;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Button cancelButton;
    }
}