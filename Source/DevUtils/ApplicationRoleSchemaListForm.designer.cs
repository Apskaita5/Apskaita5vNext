namespace DeveloperUtils
{
    partial class ApplicationRoleSchemaListForm
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
            this.EditableDataListView = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = new BrightIdeasSoftware.OLVColumn();
            this.olvColumn2 = new BrightIdeasSoftware.OLVColumn();
            this.olvColumn3 = new BrightIdeasSoftware.OLVColumn();
            this.olvColumn4 = new BrightIdeasSoftware.OLVColumn();
            this.olvColumn5 = new BrightIdeasSoftware.OLVColumn();
            this.olvColumn6 = new BrightIdeasSoftware.OLVColumn();
            this.olvColumn7 = new BrightIdeasSoftware.OLVColumn();
            this.olvColumn8 = new BrightIdeasSoftware.OLVColumn();
            this.applicationRoleSchemaListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.EditableDataListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.applicationRoleSchemaListBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // EditableDataListView
            // 
            this.EditableDataListView.AllColumns.Add(this.olvColumn1);
            this.EditableDataListView.AllColumns.Add(this.olvColumn2);
            this.EditableDataListView.AllColumns.Add(this.olvColumn3);
            this.EditableDataListView.AllColumns.Add(this.olvColumn4);
            this.EditableDataListView.AllColumns.Add(this.olvColumn5);
            this.EditableDataListView.AllColumns.Add(this.olvColumn6);
            this.EditableDataListView.AllColumns.Add(this.olvColumn7);
            this.EditableDataListView.AllColumns.Add(this.olvColumn8);
            this.EditableDataListView.AllowColumnReorder = true;
            this.EditableDataListView.AutoGenerateColumns = false;
            this.EditableDataListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.EditableDataListView.CellEditEnterChangesRows = true;
            this.EditableDataListView.CellEditTabChangesRows = true;
            this.EditableDataListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn7,
            this.olvColumn8});
            this.EditableDataListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.EditableDataListView.DataSource = this.applicationRoleSchemaListBindingSource;
            this.EditableDataListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditableDataListView.FullRowSelect = true;
            this.EditableDataListView.HasCollapsibleGroups = false;
            this.EditableDataListView.HeaderWordWrap = true;
            this.EditableDataListView.HideSelection = false;
            this.EditableDataListView.IncludeColumnHeadersInCopy = true;
            this.EditableDataListView.IsSimpleDragSource = true;
            this.EditableDataListView.IsSimpleDropSink = true;
            this.EditableDataListView.Location = new System.Drawing.Point(0, 0);
            this.EditableDataListView.Name = "EditableDataListView";
            this.EditableDataListView.RenderNonEditableCheckboxesAsDisabled = true;
            this.EditableDataListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.EditableDataListView.SelectedBackColor = System.Drawing.Color.PaleGreen;
            this.EditableDataListView.SelectedForeColor = System.Drawing.Color.Black;
            this.EditableDataListView.ShowCommandMenuOnRightClick = true;
            this.EditableDataListView.ShowGroups = false;
            this.EditableDataListView.ShowImagesOnSubItems = true;
            this.EditableDataListView.ShowItemCountOnGroups = true;
            this.EditableDataListView.ShowItemToolTips = true;
            this.EditableDataListView.Size = new System.Drawing.Size(1029, 524);
            this.EditableDataListView.TabIndex = 4;
            this.EditableDataListView.UnfocusedSelectedBackColor = System.Drawing.Color.PaleGreen;
            this.EditableDataListView.UnfocusedSelectedForeColor = System.Drawing.Color.Black;
            this.EditableDataListView.UseCellFormatEvents = true;
            this.EditableDataListView.UseCompatibleStateImageBehavior = false;
            this.EditableDataListView.UseFilterIndicator = true;
            this.EditableDataListView.UseFiltering = true;
            this.EditableDataListView.UseHotItem = true;
            this.EditableDataListView.UseNotifyPropertyChanged = true;
            this.EditableDataListView.View = System.Windows.Forms.View.Details;
            this.EditableDataListView.ModelCanDrop += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.EditableDataListView_ModelCanDrop);
            this.EditableDataListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditableDataListView_KeyDown);
            this.EditableDataListView.ModelDropped += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.EditableDataListView_ModelDropped);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.Text = "Name";
            this.olvColumn1.Width = 226;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Description";
            this.olvColumn2.Text = "Description";
            this.olvColumn2.Width = 168;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "IsLookUpSubrole";
            this.olvColumn3.CheckBoxes = true;
            this.olvColumn3.IsHeaderVertical = true;
            this.olvColumn3.Text = "Is LookUp Role";
            this.olvColumn3.Width = 45;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "HasSelectSubrole";
            this.olvColumn4.CheckBoxes = true;
            this.olvColumn4.IsHeaderVertical = true;
            this.olvColumn4.Text = "Select";
            this.olvColumn4.Width = 34;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "HasInsertSubrole";
            this.olvColumn5.CheckBoxes = true;
            this.olvColumn5.IsHeaderVertical = true;
            this.olvColumn5.Text = "Insert";
            this.olvColumn5.Width = 32;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "HasUpdateSubrole";
            this.olvColumn6.CheckBoxes = true;
            this.olvColumn6.IsHeaderVertical = true;
            this.olvColumn6.Text = "Update";
            this.olvColumn6.Width = 32;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "HasExecuteSubrole";
            this.olvColumn7.CheckBoxes = true;
            this.olvColumn7.IsHeaderVertical = true;
            this.olvColumn7.Text = "Execute";
            this.olvColumn7.Width = 29;
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "RequiredLookUpRoles";
            this.olvColumn8.Text = "Required LookUp";
            this.olvColumn8.Width = 393;
            // 
            // applicationRoleSchemaListBindingSource
            // 
            this.applicationRoleSchemaListBindingSource.DataSource = typeof(Apskaita5.DAL.Common.ApplicationRoleSchema);
            // 
            // ApplicationRoleSchemaListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 524);
            this.Controls.Add(this.EditableDataListView);
            this.Name = "ApplicationRoleSchemaListForm";
            this.ShowInTaskbar = false;
            this.Text = "Application Role Schema";
            this.Load += new System.EventHandler(this.ApplicationRoleSchemaListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.EditableDataListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.applicationRoleSchemaListBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal BrightIdeasSoftware.DataListView EditableDataListView;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private System.Windows.Forms.BindingSource applicationRoleSchemaListBindingSource;
    }
}