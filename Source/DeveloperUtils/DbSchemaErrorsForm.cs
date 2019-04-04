using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apskaita5.DAL.Common;
using Apskaita5.Common;

namespace DeveloperUtils
{
    public partial class DbSchemaErrorsForm : Form
    {

        private SqlAgentBase _agent = null;
        private List<DbSchemaError> _errors = null;


        public DbSchemaErrorsForm(SqlAgentBase agent)
        {
            InitializeComponent();
            _agent = agent;
        }

        private void openSchemaPathButton_Click(object sender, EventArgs e)
        {
            using (var openFolderDialog = new FolderBrowserDialog())
            {

                openFolderDialog.ShowNewFolderButton = false;
                if (openFolderDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string path = openFolderDialog.SelectedPath;
                    if (path.Trim().Length > 2) this.schemaPathTextBox.Text = path;
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (_agent.CurrentDatabase.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Not connected to any database.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.schemaPathTextBox.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Schema folder path is not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _errors = _agent.GetDbSchemaErrorsAsync(this.schemaPathTextBox.Text).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bindingSource1.RaiseListChangedEvents = false;
            bindingSource1.DataSource = _errors;
            bindingSource1.RaiseListChangedEvents = true;
            bindingSource1.ResetBindings(false);

        }

        private void DbSchemaErrorsForm_Load(object sender, EventArgs e)
        {
            /// bindingSource1.DataSource = typeof(DbSchemaError);
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns.Count - 1)
            {
                e.Value = string.Join(Environment.NewLine, (string[])e.Value); // apply formating here
                e.FormattingApplied = true;
            }
        }

    }
}
