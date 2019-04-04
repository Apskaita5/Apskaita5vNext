namespace DeveloperUtils
{
    partial class DbSchemaErrorsForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.openSchemaPathButton = new System.Windows.Forms.Button();
            this.schemaPathTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ErrorTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TableColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FieldColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsRepairableColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SqlStatementsToRepairColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // openSchemaPathButton
            // 
            this.openSchemaPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openSchemaPathButton.Location = new System.Drawing.Point(850, 10);
            this.openSchemaPathButton.Name = "openSchemaPathButton";
            this.openSchemaPathButton.Size = new System.Drawing.Size(26, 20);
            this.openSchemaPathButton.TabIndex = 12;
            this.openSchemaPathButton.Text = "...";
            this.openSchemaPathButton.UseVisualStyleBackColor = true;
            this.openSchemaPathButton.Click += new System.EventHandler(this.openSchemaPathButton_Click);
            // 
            // schemaPathTextBox
            // 
            this.schemaPathTextBox.AcceptsReturn = true;
            this.schemaPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.schemaPathTextBox.Location = new System.Drawing.Point(90, 11);
            this.schemaPathTextBox.Name = "schemaPathTextBox";
            this.schemaPathTextBox.Size = new System.Drawing.Size(754, 20);
            this.schemaPathTextBox.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Schema Path:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.schemaPathTextBox);
            this.panel1.Controls.Add(this.openSchemaPathButton);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(887, 64);
            this.panel1.TabIndex = 14;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(722, 37);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 20);
            this.button1.TabIndex = 15;
            this.button1.Text = "Check For Errors";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(Apskaita5.DAL.Common.DbSchemaError);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ErrorTypeColumn,
            this.DescriptionColumn,
            this.TableColumn,
            this.FieldColumn,
            this.IsRepairableColumn,
            this.SqlStatementsToRepairColumn});
            this.dataGridView1.DataSource = this.bindingSource1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 64);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(887, 386);
            this.dataGridView1.TabIndex = 15;
            // 
            // ErrorTypeColumn
            // 
            this.ErrorTypeColumn.DataPropertyName = "ErrorType";
            this.ErrorTypeColumn.HeaderText = "Error Type";
            this.ErrorTypeColumn.Name = "ErrorTypeColumn";
            this.ErrorTypeColumn.ReadOnly = true;
            this.ErrorTypeColumn.Width = 75;
            // 
            // DescriptionColumn
            // 
            this.DescriptionColumn.DataPropertyName = "Description";
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DescriptionColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.DescriptionColumn.HeaderText = "Description";
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.ReadOnly = true;
            this.DescriptionColumn.Width = 85;
            // 
            // TableColumn
            // 
            this.TableColumn.DataPropertyName = "Table";
            this.TableColumn.HeaderText = "Table";
            this.TableColumn.Name = "TableColumn";
            this.TableColumn.ReadOnly = true;
            this.TableColumn.Width = 59;
            // 
            // FieldColumn
            // 
            this.FieldColumn.DataPropertyName = "Field";
            this.FieldColumn.HeaderText = "Field";
            this.FieldColumn.Name = "FieldColumn";
            this.FieldColumn.ReadOnly = true;
            this.FieldColumn.Width = 54;
            // 
            // IsRepairableColumn
            // 
            this.IsRepairableColumn.DataPropertyName = "IsRepairable";
            this.IsRepairableColumn.HeaderText = "Repairable";
            this.IsRepairableColumn.Name = "IsRepairableColumn";
            this.IsRepairableColumn.ReadOnly = true;
            this.IsRepairableColumn.Width = 64;
            // 
            // SqlStatementsToRepairColumn
            // 
            this.SqlStatementsToRepairColumn.DataPropertyName = "SqlStatementsToRepair";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SqlStatementsToRepairColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.SqlStatementsToRepairColumn.HeaderText = "Sql Statements";
            this.SqlStatementsToRepairColumn.Name = "SqlStatementsToRepairColumn";
            this.SqlStatementsToRepairColumn.ReadOnly = true;
            this.SqlStatementsToRepairColumn.Width = 95;
            // 
            // DbSchemaErrorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 450);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Name = "DbSchemaErrorsForm";
            this.Text = "Db Schema Errors";
            this.Load += new System.EventHandler(this.DbSchemaErrorsForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button openSchemaPathButton;
        private System.Windows.Forms.TextBox schemaPathTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1; 
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ErrorTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TableColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsRepairableColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SqlStatementsToRepairColumn;
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}