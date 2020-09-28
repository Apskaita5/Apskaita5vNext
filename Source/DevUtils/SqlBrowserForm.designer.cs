namespace DeveloperUtils
{
    partial class SqlBrowserForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.executeButton = new System.Windows.Forms.Button();
            this.queryTextBox = new System.Windows.Forms.TextBox();
            this.openStructureButton = new System.Windows.Forms.Button();
            this.refreshSchemaButton = new System.Windows.Forms.Button();
            this.structureTreeView = new System.Windows.Forms.TreeView();
            this.resultDataGridView = new System.Windows.Forms.DataGridView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.resultDataGridView);
            this.splitContainer1.Size = new System.Drawing.Size(894, 451);
            this.splitContainer1.SplitterDistance = 212;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.executeButton);
            this.splitContainer2.Panel1.Controls.Add(this.queryTextBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.openStructureButton);
            this.splitContainer2.Panel2.Controls.Add(this.refreshSchemaButton);
            this.splitContainer2.Panel2.Controls.Add(this.structureTreeView);
            this.splitContainer2.Size = new System.Drawing.Size(894, 212);
            this.splitContainer2.SplitterDistance = 678;
            this.splitContainer2.TabIndex = 0;
            // 
            // executeButton
            // 
            this.executeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.executeButton.Location = new System.Drawing.Point(600, 184);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(75, 23);
            this.executeButton.TabIndex = 1;
            this.executeButton.Text = "Execute";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // queryTextBox
            // 
            this.queryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.queryTextBox.Location = new System.Drawing.Point(3, 3);
            this.queryTextBox.Multiline = true;
            this.queryTextBox.Name = "queryTextBox";
            this.queryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.queryTextBox.Size = new System.Drawing.Size(672, 175);
            this.queryTextBox.TabIndex = 0;
            this.queryTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.queryTextBox_DragDrop);
            this.queryTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.queryTextBox_DragEnter);
            // 
            // openStructureButton
            // 
            this.openStructureButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.openStructureButton.Location = new System.Drawing.Point(3, 183);
            this.openStructureButton.Name = "openStructureButton";
            this.openStructureButton.Size = new System.Drawing.Size(62, 23);
            this.openStructureButton.TabIndex = 2;
            this.openStructureButton.Text = "Details";
            this.openStructureButton.UseVisualStyleBackColor = true;
            this.openStructureButton.Click += new System.EventHandler(this.openStructureButton_Click);
            // 
            // refreshSchemaButton
            // 
            this.refreshSchemaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshSchemaButton.Image = global::DevUtils.Properties.Resources.Button_Reload_icon_16p;
            this.refreshSchemaButton.Location = new System.Drawing.Point(175, 182);
            this.refreshSchemaButton.Name = "refreshSchemaButton";
            this.refreshSchemaButton.Size = new System.Drawing.Size(25, 25);
            this.refreshSchemaButton.TabIndex = 3;
            this.refreshSchemaButton.UseVisualStyleBackColor = true;
            this.refreshSchemaButton.Click += new System.EventHandler(this.refreshSchemaButton_Click);
            // 
            // structureTreeView
            // 
            this.structureTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.structureTreeView.Location = new System.Drawing.Point(0, 0);
            this.structureTreeView.Name = "structureTreeView";
            this.structureTreeView.Size = new System.Drawing.Size(212, 178);
            this.structureTreeView.TabIndex = 0;
            this.structureTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.structureTreeView_ItemDrag);
            // 
            // resultDataGridView
            // 
            this.resultDataGridView.AllowUserToAddRows = false;
            this.resultDataGridView.AllowUserToDeleteRows = false;
            this.resultDataGridView.AllowUserToOrderColumns = true;
            this.resultDataGridView.CausesValidation = false;
            this.resultDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.resultDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultDataGridView.Location = new System.Drawing.Point(0, 0);
            this.resultDataGridView.Name = "resultDataGridView";
            this.resultDataGridView.ReadOnly = true;
            this.resultDataGridView.RowHeadersVisible = false;
            this.resultDataGridView.Size = new System.Drawing.Size(894, 235);
            this.resultDataGridView.TabIndex = 0;
            // 
            // SqlBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 451);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SqlBrowserForm";
            this.Text = "SqlBrowserForm";
            this.Load += new System.EventHandler(this.SqlBrowserForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resultDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView resultDataGridView;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox queryTextBox;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.TreeView structureTreeView;
        private System.Windows.Forms.Button openStructureButton;
        private System.Windows.Forms.Button refreshSchemaButton;
    }
}