namespace DeveloperUtils
{
    partial class SqlDictionaryForm
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ReplaceAllCheckBox = new System.Windows.Forms.CheckBox();
            this.filterByClassButton = new System.Windows.Forms.Button();
            this.RefreshNamespacesButton = new System.Windows.Forms.Button();
            this.doReplaceButton = new System.Windows.Forms.Button();
            this.replaceNewTextBox = new System.Windows.Forms.TextBox();
            this.ClassComboBox = new System.Windows.Forms.ComboBox();
            this.replaceOldTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.NamespaceComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.showErrorsButton = new System.Windows.Forms.Button();
            this.GetParametersButton = new System.Windows.Forms.Button();
            this.sqlRepositoryDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sqlRepositoryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sqlRepositoryDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sqlRepositoryBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 10;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel1.Controls.Add(this.ReplaceAllCheckBox, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.filterByClassButton, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.RefreshNamespacesButton, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.doReplaceButton, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.replaceNewTextBox, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.ClassComboBox, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.replaceOldTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.NamespaceComboBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.showErrorsButton, 8, 0);
            this.tableLayoutPanel1.Controls.Add(this.GetParametersButton, 8, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(966, 54);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ReplaceAllCheckBox
            // 
            this.ReplaceAllCheckBox.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.ReplaceAllCheckBox, 2);
            this.ReplaceAllCheckBox.Location = new System.Drawing.Point(820, 30);
            this.ReplaceAllCheckBox.Name = "ReplaceAllCheckBox";
            this.ReplaceAllCheckBox.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.ReplaceAllCheckBox.Size = new System.Drawing.Size(37, 20);
            this.ReplaceAllCheckBox.TabIndex = 1;
            this.ReplaceAllCheckBox.Text = "All";
            this.ReplaceAllCheckBox.UseVisualStyleBackColor = true;
            // 
            // filterByClassButton
            // 
            this.filterByClassButton.Image = global::DeveloperUtils.Properties.Resources.funnel;
            this.filterByClassButton.Location = new System.Drawing.Point(817, 0);
            this.filterByClassButton.Margin = new System.Windows.Forms.Padding(0);
            this.filterByClassButton.Name = "filterByClassButton";
            this.filterByClassButton.Size = new System.Drawing.Size(27, 27);
            this.filterByClassButton.TabIndex = 4;
            this.filterByClassButton.UseVisualStyleBackColor = true;
            this.filterByClassButton.Click += new System.EventHandler(this.filterByClassButton_Click);
            // 
            // RefreshNamespacesButton
            // 
            this.RefreshNamespacesButton.Image = global::DeveloperUtils.Properties.Resources.Button_Reload_icon_16p;
            this.RefreshNamespacesButton.Location = new System.Drawing.Point(411, 0);
            this.RefreshNamespacesButton.Margin = new System.Windows.Forms.Padding(0);
            this.RefreshNamespacesButton.Name = "RefreshNamespacesButton";
            this.RefreshNamespacesButton.Size = new System.Drawing.Size(27, 27);
            this.RefreshNamespacesButton.TabIndex = 1;
            this.RefreshNamespacesButton.UseVisualStyleBackColor = true;
            this.RefreshNamespacesButton.Click += new System.EventHandler(this.RefreshNamespacesButton_Click);
            // 
            // doReplaceButton
            // 
            this.doReplaceButton.Location = new System.Drawing.Point(411, 27);
            this.doReplaceButton.Margin = new System.Windows.Forms.Padding(0);
            this.doReplaceButton.Name = "doReplaceButton";
            this.doReplaceButton.Size = new System.Drawing.Size(27, 27);
            this.doReplaceButton.TabIndex = 1;
            this.doReplaceButton.Text = "->";
            this.doReplaceButton.UseVisualStyleBackColor = true;
            this.doReplaceButton.Click += new System.EventHandler(this.doReplaceButton_Click);
            // 
            // replaceNewTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.replaceNewTextBox, 2);
            this.replaceNewTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replaceNewTextBox.Location = new System.Drawing.Point(441, 30);
            this.replaceNewTextBox.Name = "replaceNewTextBox";
            this.replaceNewTextBox.Size = new System.Drawing.Size(373, 20);
            this.replaceNewTextBox.TabIndex = 2;
            // 
            // ClassComboBox
            // 
            this.ClassComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClassComboBox.FormattingEnabled = true;
            this.ClassComboBox.Location = new System.Drawing.Point(482, 3);
            this.ClassComboBox.Name = "ClassComboBox";
            this.ClassComboBox.Size = new System.Drawing.Size(332, 21);
            this.ClassComboBox.TabIndex = 2;
            // 
            // replaceOldTextBox
            // 
            this.replaceOldTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replaceOldTextBox.Location = new System.Drawing.Point(76, 30);
            this.replaceOldTextBox.Name = "replaceOldTextBox";
            this.replaceOldTextBox.Size = new System.Drawing.Size(332, 20);
            this.replaceOldTextBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 27);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label3.Size = new System.Drawing.Size(50, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Replace:";
            // 
            // NamespaceComboBox
            // 
            this.NamespaceComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NamespaceComboBox.FormattingEnabled = true;
            this.NamespaceComboBox.Location = new System.Drawing.Point(76, 3);
            this.NamespaceComboBox.Name = "NamespaceComboBox";
            this.NamespaceComboBox.Size = new System.Drawing.Size(332, 21);
            this.NamespaceComboBox.TabIndex = 1;
            this.NamespaceComboBox.SelectedIndexChanged += new System.EventHandler(this.NamespaceComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label1.Size = new System.Drawing.Size(67, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Namespace:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(441, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label2.Size = new System.Drawing.Size(35, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Class:";
            // 
            // showErrorsButton
            // 
            this.showErrorsButton.Location = new System.Drawing.Point(884, 0);
            this.showErrorsButton.Margin = new System.Windows.Forms.Padding(0);
            this.showErrorsButton.Name = "showErrorsButton";
            this.showErrorsButton.Size = new System.Drawing.Size(61, 27);
            this.showErrorsButton.TabIndex = 5;
            this.showErrorsButton.Text = "Errors";
            this.showErrorsButton.UseVisualStyleBackColor = true;
            this.showErrorsButton.Click += new System.EventHandler(this.showErrorsButton_Click);
            // 
            // GetParametersButton
            // 
            this.GetParametersButton.Location = new System.Drawing.Point(884, 27);
            this.GetParametersButton.Margin = new System.Windows.Forms.Padding(0);
            this.GetParametersButton.Name = "GetParametersButton";
            this.GetParametersButton.Size = new System.Drawing.Size(61, 27);
            this.GetParametersButton.TabIndex = 6;
            this.GetParametersButton.Text = "Params";
            this.GetParametersButton.UseVisualStyleBackColor = true;
            this.GetParametersButton.Click += new System.EventHandler(this.GetParametersButton_Click);
            // 
            // sqlRepositoryDataGridView
            // 
            this.sqlRepositoryDataGridView.AllowUserToAddRows = false;
            this.sqlRepositoryDataGridView.AllowUserToDeleteRows = false;
            this.sqlRepositoryDataGridView.AllowUserToOrderColumns = true;
            this.sqlRepositoryDataGridView.AutoGenerateColumns = false;
            this.sqlRepositoryDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.sqlRepositoryDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sqlRepositoryDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            this.sqlRepositoryDataGridView.DataSource = this.sqlRepositoryBindingSource;
            this.sqlRepositoryDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlRepositoryDataGridView.Location = new System.Drawing.Point(0, 54);
            this.sqlRepositoryDataGridView.Name = "sqlRepositoryDataGridView";
            this.sqlRepositoryDataGridView.RowHeadersWidth = 20;
            this.sqlRepositoryDataGridView.Size = new System.Drawing.Size(966, 386);
            this.sqlRepositoryDataGridView.TabIndex = 2;
            this.sqlRepositoryDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.sqlRepositoryDataGridView_CellValidating);
            this.sqlRepositoryDataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.sqlRepositoryDataGridView_KeyDown);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Token";
            this.dataGridViewTextBoxColumn1.HeaderText = "Token";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 200;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Query";
            this.dataGridViewTextBoxColumn2.HeaderText = "Query";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "UsedByTypes";
            this.dataGridViewTextBoxColumn3.FillWeight = 200F;
            this.dataGridViewTextBoxColumn3.HeaderText = "Used By Types";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // sqlRepositoryBindingSource
            // 
            this.sqlRepositoryBindingSource.DataSource = typeof(Apskaita5.DAL.Common.SqlRepositoryItem);
            // 
            // SqlDictionaryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 440);
            this.Controls.Add(this.sqlRepositoryDataGridView);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SqlDictionaryForm";
            this.ShowInTaskbar = false;
            this.Text = "SQL Dictionary";
            this.Load += new System.EventHandler(this.SqlDictionaryForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sqlRepositoryDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sqlRepositoryBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button doReplaceButton;
        private System.Windows.Forms.TextBox replaceNewTextBox;
        private System.Windows.Forms.ComboBox ClassComboBox;
        private System.Windows.Forms.TextBox replaceOldTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox NamespaceComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button RefreshNamespacesButton;
        private System.Windows.Forms.Button filterByClassButton;
        private System.Windows.Forms.CheckBox ReplaceAllCheckBox;
        private System.Windows.Forms.Button showErrorsButton;
        private System.Windows.Forms.Button GetParametersButton;
        private System.Windows.Forms.BindingSource sqlRepositoryBindingSource;
        private System.Windows.Forms.DataGridView sqlRepositoryDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    }
}