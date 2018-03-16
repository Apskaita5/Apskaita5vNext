using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Apskaita5.DAL.Common;
using Apskaita5.Common;

namespace DeveloperUtils
{
    public partial class SqlBrowserForm : Form
    {

        private SqlAgentBase _agent = null;
        private DbSchema _schema = null;


        public SqlBrowserForm(SqlAgentBase agent)
        {
            InitializeComponent();
            _agent = agent;
        }


        private void SqlBrowserForm_Load(object sender, EventArgs e)
        {

            this.resultDataGridView.AutoGenerateColumns = true;

        }


        private void queryTextBox_DragDrop(object sender, DragEventArgs e)
        {

            var lastChar = this.queryTextBox.Text[this.queryTextBox.Text.Length - 1];

            if (!string.IsNullOrEmpty(this.queryTextBox.Text) &&
                lastChar != ' ' && lastChar != ',' && lastChar != '.')
            {
                this.queryTextBox.Text = this.queryTextBox.Text + ", " + e.Data.GetData(DataFormats.Text);
            }
            else
            {
                this.queryTextBox.Text = this.queryTextBox.Text + e.Data.GetData(DataFormats.Text);
            }

        }

        private void structureTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            structureTreeView.DoDragDrop(((TreeNode) e.Item).Text, DragDropEffects.Copy);
        }

        private void queryTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void executeButton_Click(object sender, EventArgs e)
        {

            if (this.queryTextBox.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Query not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable result = null;

            try
            {
                if (this.queryTextBox.Text.Trim().ToLower().StartsWith("select ") ||
                    this.queryTextBox.Text.Trim().ToLower().StartsWith("show ") ||
                    this.queryTextBox.Text.Trim().ToLower().StartsWith("explain ") ||
                    this.queryTextBox.Text.Trim().ToLower().StartsWith("pragma "))
                {
                    var data = _agent.FetchTableRaw(this.queryTextBox.Text, null);
                    result = data.ToDataTable();
                }
                else if (this.queryTextBox.Text.Trim().ToLower().StartsWith("insert "))
                {
                    var data = _agent.ExecuteInsertRaw(this.queryTextBox.Text, null);
                    result = new DataTable();
                    result.Columns.Add("Last Insert ID");
                    result.Rows.Add();
                    result.Rows[0][0] = data;
                }
                else
                {
                    var data = _agent.ExecuteInsertRaw(this.queryTextBox.Text, null);
                    result = new DataTable();
                    result.Columns.Add("Affected Rows");
                    result.Rows.Add();
                    result.Rows[0][0] = data;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.resultDataGridView.DataSource != null)
            {
                ((DataTable)this.resultDataGridView.DataSource).Dispose();
                this.resultDataGridView.DataSource = null;
            }

            this.resultDataGridView.DataSource = result;

        }

        private void openStructureButton_Click(object sender, EventArgs e)
        {
            if (_schema == null) return;
            Form childForm = new DbSchemaForm(_schema, _agent.CurrentDatabase);
            childForm.MdiParent = this.MdiParent;
            childForm.Show();
        }

        private void refreshSchemaButton_Click(object sender, EventArgs e)
        {

            if (_agent.CurrentDatabase.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Not connected to any database.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _schema = _agent.GetDbSchema();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            structureTreeView.Nodes.Clear();

            foreach (var table in _schema.Tables)
            {
                var currentTableNode = structureTreeView.Nodes.Add(table.Name);
                currentTableNode.ToolTipText = table.Description;
                foreach (var field in table.Fields)
                {
                    var currentFieldNode = currentTableNode.Nodes.Add(field.Name);
                    currentFieldNode.ToolTipText = field.GetDefinition();
                }
            }

        }

    }
}
