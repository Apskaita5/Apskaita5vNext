using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Apskaita5.DAL.Common.DbSchema;
using Apskaita5.Common;
using Apskaita5.DAL.MySql;
using Apskaita5.DAL.SQLite;
using DeveloperUtils.TestClasses;
using Apskaita5.DAL.Common;

namespace DeveloperUtils
{
    public partial class DbSchemaForm : Form, IStandardActionForm
    {

        private DbSchema _currentSource = null;
        private string _currentFilePath = string.Empty;
        private readonly string _dbName = "";
        private string _clipboardForFields = "";


        public bool CanSave
        {
            get { return true; }
        }

        public bool CanCreate
        {
            get { return true; }
        }

        public bool CanPaste
        {
            get { return true; }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
        }


        public DbSchemaForm()
        {
            InitializeComponent();
        }

        public DbSchemaForm(DbSchema schema, string dbName)
        {
            InitializeComponent();
            _currentSource = schema;
            _dbName = dbName;
        }

        public DbSchemaForm(string filePath, DbSchema schema)
        {
            InitializeComponent();
            _currentSource = schema;
            _currentFilePath = filePath;
        }


        private void DbSchemaForm_Load(object sender, EventArgs e)
        {

            var sqlTypes = Enum.GetValues(typeof (DbDataType)).Cast<DbDataType>().ToList();
            this.dataGridViewTextBoxColumn6.DataSource = sqlTypes;

            var collationTypes = Enum.GetValues(typeof(DbFieldCollationType)).Cast<DbFieldCollationType>().ToList();
            this.dataGridViewTextBoxColumn16.DataSource = collationTypes;

            var indexTypes = Enum.GetValues(typeof(DbIndexType)).Cast<DbIndexType>().ToList();
            this.dataGridViewTextBoxColumn10.DataSource = indexTypes;

            var changeActionTypes = Enum.GetValues(typeof(DbForeignKeyActionType)).
                Cast<DbForeignKeyActionType>().ToList();
            this.dataGridViewTextBoxColumn12.DataSource = changeActionTypes;
            this.dataGridViewTextBoxColumn13.DataSource = changeActionTypes;

            var agent2 = new SqliteAgent("fake conn string", "fake path", null, null);
            var agent1 = new MySqlAgent("fake conn string", string.Empty, null, null);
            var agents = new List<SqlAgentBase>() {agent1, agent2};

            this.sqlAgentsComboBox.DisplayMember = "Name";
            this.sqlAgentsComboBox.DataSource = agents;

            if (_currentSource == null)
            {
                _currentSource = new DbSchema();
                this.Text = "New Database Schema";
            }
            else if (!_currentFilePath.IsNullOrWhiteSpace())
            {
                this.Text = "Database Schema: " + _currentFilePath;
            }
            else
            {
                this.Text = "Database Schema For " + _dbName;
            }

            this.dbSchemaBindingSource.DataSource = _currentSource;

            ContextMenuStrip mnu = new ContextMenuStrip();
            ToolStripMenuItem mnuCopy = new ToolStripMenuItem("Copy");
            ToolStripMenuItem mnuPaste = new ToolStripMenuItem("Paste");
            //Assign event handlers
            mnuCopy.Click += new EventHandler(mnuCopy_Click);
            mnuPaste.Click += new EventHandler(mnuPaste_Click);
            //Add to main context menu
            mnu.Items.AddRange(new ToolStripItem[] { mnuCopy, mnuPaste });
            //Assign to datagridview
            this.fieldsDataGridView.ContextMenuStrip = mnu;

        }


        private void tablesDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (_currentSource == null) return;

            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Subtract)
            {
                if (this.tablesDataGridView.SelectedRows != null
                    && this.tablesDataGridView.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in this.tablesDataGridView.SelectedRows)
                    {
                        var item = row.DataBoundItem as DbTableSchema;
                        if (item != null)
                        {
                            _currentSource.Tables.Remove(item);
                        }
                    }
                    this.tablesBindingSource.ResetBindings(false);
                }
            }

            if (e.KeyData == Keys.Insert || e.KeyData == Keys.Add)
            {
                _currentSource.Tables.Add(new DbTableSchema());

                this.tablesBindingSource.ResetBindings(false);

                this.tablesDataGridView.FirstDisplayedScrollingRowIndex =
                    this.tablesDataGridView.RowCount - 1;
            }

            if (e.KeyData == Keys.F2)
            {
                Clipboard.SetText(((DbTableSchema)this.tablesBindingSource.Current).ToClass());
            }
            
        }

        private void fieldsDataGridView_KeyDown(object sender, KeyEventArgs e)
        {

            if (_currentSource == null) return;

            var table = tablesBindingSource.Current as DbTableSchema;
            if (table == null) return;

            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Subtract)
            {
                if (this.fieldsDataGridView.SelectedRows != null
                    && this.fieldsDataGridView.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in this.fieldsDataGridView.SelectedRows)
                    {
                        var item = row.DataBoundItem as DbFieldSchema;
                        if (item != null)
                        {
                            table.Fields.Remove(item);
                        }
                    }
                    this.fieldsBindingSource.ResetBindings(false);
                }
            }

            if (e.KeyData == Keys.Insert || e.KeyData == Keys.Add)
            {
                if (this.fieldsDataGridView.CurrentCell != null && this.fieldsDataGridView.CurrentCell.RowIndex 
                    < this.fieldsDataGridView.Rows.Count - 1)
                {
                    table.Fields.Insert(this.fieldsDataGridView.CurrentCell.RowIndex, new DbFieldSchema());
                }
                else if (this.fieldsDataGridView.CurrentRow != null && this.fieldsDataGridView.CurrentRow.Index
                    < this.fieldsDataGridView.Rows.Count - 1)
                {
                    table.Fields.Insert(this.fieldsDataGridView.CurrentRow.Index, new DbFieldSchema());
                }
                else
                {
                    table.Fields.Add(new DbFieldSchema());
                }


                this.fieldsBindingSource.ResetBindings(false);

                this.fieldsDataGridView.FirstDisplayedScrollingRowIndex =
                    this.fieldsDataGridView.RowCount - 1;
            }

        }

        private void tablesBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            this.fieldsBindingSource.EndEdit();
        }

        private void fieldsDataGridView_CellValidating(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var item = fieldsDataGridView.Rows[e.RowIndex].DataBoundItem as DbFieldSchema;
            if (item == null) return;

            if (fieldsDataGridView.Columns[e.ColumnIndex].DataPropertyName == nameof(DbFieldSchema.IndexType))
            {
                if (item.IndexType == DbIndexType.ForeignKey || item.IndexType == DbIndexType.ForeignPrimary)
                {
                    item.IndexName = string.Format("{0}_{1}_fk", ((DbTableSchema)tablesBindingSource.Current).Name.ToLower().Trim(), item.Name.ToLower().Trim());
                }
                if (item.IndexType == DbIndexType.Simple || item.IndexType == DbIndexType.Unique)
                {
                    item.IndexName = string.Format("{0}_{1}_idx", ((DbTableSchema)tablesBindingSource.Current).Name.ToLower().Trim(), item.Name.ToLower().Trim());
                }
            }

            var errors = item.GetDataErrors();

            DeveloperUtils.Utilities.SetErrors(fieldsDataGridView.Rows[e.RowIndex], errors);

        }

        private void tablesDataGridView_CellValidating(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var item = tablesDataGridView.Rows[e.RowIndex].DataBoundItem as DbTableSchema;
            if (item == null) return;

            var errors = item.GetDataErrors();

            DeveloperUtils.Utilities.SetErrors(tablesDataGridView.Rows[e.RowIndex], errors);

        }

        private void getCreateSqlButton_Click(object sender, EventArgs e)
        {

            if (_currentSource == null) return;

            var sqlAgent = sqlAgentsComboBox.SelectedValue as SqlAgentBase;
            if (sqlAgent == null) return;

            try
            {
                Clipboard.SetText(sqlAgent.GetDefaultSchemaManager().GetCreateDatabaseSql(_currentSource), TextDataFormat.UnicodeText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("CREATE DATABASE SQL Script has been copied to the clipboard.",
                "", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }


        public void Save(string filePath)
        {

            if (_currentSource == null || filePath.IsNullOrWhiteSpace()) return;

            try
            {
                _currentSource.SaveXmlFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _currentFilePath = filePath;

            this.Text = "Database Schema: " + _currentFilePath;

            MessageBox.Show("The file has been saved.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public void Open(string filePath)
        {

            if (filePath.IsNullOrWhiteSpace()) return;

            if (!SaveCurrentSource()) return;

            var result = new DbSchema();
            try
            {
                result.LoadXmlFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            RebindDataSource(result);

            _currentFilePath = filePath;

            this.Text = "Database Schema: " + _currentFilePath;

        }

        public void Create()
        {

            if (!SaveCurrentSource()) return;

            var result = new DbSchema();

            RebindDataSource(result);

            _currentFilePath = string.Empty;

            this.Text = "New Database Schema";

        }

        public void Paste(string source)
        {

            if (_currentSource == null) return;

            var table = tablesBindingSource.Current as DbTableSchema;
            if (table == null) return;

            try
            {
                table.LoadDelimitedString(source, Environment.NewLine, "\t");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }  

            fieldsBindingSource.ResetBindings(false);

            this.fieldsDataGridView.FirstDisplayedScrollingRowIndex =
                this.fieldsDataGridView.RowCount - 1;

        }

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            if (this.fieldsDataGridView.SelectedRows == null) return;

            var itemsToCopy = new List<DbFieldSchema>();

            foreach (DataGridViewRow row in this.fieldsDataGridView.SelectedRows)
            {
                itemsToCopy.Add((DbFieldSchema)row.DataBoundItem);
            }

            if (itemsToCopy.Count > 0) _clipboardForFields = Apskaita5.Common.Utilities.WriteToXml(itemsToCopy, null);
        }

        private void mnuPaste_Click(object sender, EventArgs e)
        {

            if (_clipboardForFields.IsNullOrWhiteSpace() || tablesBindingSource.Current == null) return;

            var itemsToCopy = Apskaita5.Common.Utilities.CreateFromXml<List<DbFieldSchema>>(_clipboardForFields);
            itemsToCopy.Reverse();
            
            if (this.fieldsDataGridView.CurrentCell != null && this.fieldsDataGridView.CurrentCell.RowIndex
                    < this.fieldsDataGridView.Rows.Count - 1)
            {
                ((DbTableSchema)tablesBindingSource.Current).Fields.InsertRange(
                    this.fieldsDataGridView.CurrentCell.RowIndex, itemsToCopy);
            }
            else if (this.fieldsDataGridView.CurrentRow != null && this.fieldsDataGridView.CurrentRow.Index
                < this.fieldsDataGridView.Rows.Count - 1)
            {
                ((DbTableSchema)tablesBindingSource.Current).Fields.InsertRange(
                    this.fieldsDataGridView.CurrentRow.Index, itemsToCopy);
            }
            else
            {
                ((DbTableSchema)tablesBindingSource.Current).Fields.AddRange(itemsToCopy);
            }

            fieldsBindingSource.ResetBindings(false);

        }


        private bool SaveCurrentSource()
        {

            if (_currentSource == null || _currentSource.Tables.Count < 1) return true;

            var response = MessageBox.Show("Save the current file before closing?", "",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (response == DialogResult.No) return true;

            if (response == DialogResult.Cancel) return false;

            var filePath = _currentFilePath;
            if (filePath.IsNullOrWhiteSpace())
            {

                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    saveFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        filePath = saveFileDialog.FileName;
                    }
                }

            }

            if (filePath.IsNullOrWhiteSpace()) return false;

            try
            {
                _currentSource.SaveXmlFile(filePath.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;

        }

        private void RebindDataSource(DbSchema newSource)
        {

            this.dbSchemaBindingSource.RaiseListChangedEvents = false;
            this.tablesBindingSource.RaiseListChangedEvents = false;
            this.fieldsBindingSource.RaiseListChangedEvents = false;
            UnbindBindingSource(fieldsBindingSource);
            UnbindBindingSource(tablesBindingSource);
            UnbindBindingSource(dbSchemaBindingSource);
            this.fieldsBindingSource.EndEdit();
            this.tablesBindingSource.EndEdit();
            this.dbSchemaBindingSource.EndEdit();
            
            this.dbSchemaBindingSource.DataSource = null;

            _currentSource = newSource;

            this.fieldsBindingSource.DataSource = this.tablesBindingSource;
            this.tablesBindingSource.DataSource = this.dbSchemaBindingSource;
            this.dbSchemaBindingSource.DataSource = _currentSource;

            this.dbSchemaBindingSource.RaiseListChangedEvents = true;
            this.tablesBindingSource.RaiseListChangedEvents = true;
            this.fieldsBindingSource.RaiseListChangedEvents = true;

            this.fieldsBindingSource.ResetBindings(false);
            this.tablesBindingSource.ResetBindings(false);
            this.dbSchemaBindingSource.ResetBindings(false);

            this.tablesDataGridView.Select();

        }

        private void UnbindBindingSource(BindingSource source)
        {
            var current = source.Current as IEditableObject;

            if (!(source.DataSource is BindingSource))
                source.DataSource = null;

            if ((current != null)) current.EndEdit();

        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.Cancel = true;
            e.ThrowException = false;
        }
        
    }
}
