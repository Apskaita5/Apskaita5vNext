using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Apskaita5.DAL.Common;
using Apskaita5.Common;
using Apskaita5.DAL.MySql;
using Apskaita5.DAL.Sqlite;

namespace DeveloperUtils
{
    public partial class CloneDatabaseForm : Form
    {

        public CloneDatabaseForm()
        {
            InitializeComponent();
        }

        private void openSourceDatabaseFileButton_Click(object sender, EventArgs e)
        {
            var agent = this.sourceSqlAgentsComboBox.SelectedValue as SqlAgentBase;
            if (agent == null || !agent.IsFileBased) return;

            using (var openFileDialog = new OpenFileDialog())
            {

                //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                openFileDialog.CheckFileExists = true;
                openFileDialog.RestoreDirectory = false;
                openFileDialog.Filter = "Database Files (*.db)|*.db|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    if (!filePath.IsNullOrWhiteSpace()) this.sourceDatabaseTextBox.Text = filePath;
                }

            }
        }

        private void openTargetDatabaseFileButton_Click(object sender, EventArgs e)
        {
            var agent = this.targetSqlAgentsComboBox.SelectedValue as SqlAgentBase;
            if (agent == null || !agent.IsFileBased) return;

            using (var openFileDialog = new OpenFileDialog())
            {

                //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                openFileDialog.CheckFileExists = false;
                openFileDialog.RestoreDirectory = false;
                openFileDialog.Filter = "Database Files (*.db)|*.db|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    if (!filePath.IsNullOrWhiteSpace()) this.targetDatabaseTextBox.Text = filePath;
                }

            }
        }

        private void CloneDatabaseForm_Load(object sender, EventArgs e)
        {
            var sourceAgents = new List<SqlAgentBase>()
                {
                    new MySqlAgent("fake conn string", "", false),
                    new SqliteAgent("fake conn string", "", false)
                };

            var targetAgents = new List<SqlAgentBase>()
                {
                    new MySqlAgent("fake conn string", "", false),
                    new SqliteAgent("fake conn string", "", false)
                };

            this.targetSqlAgentsComboBox.DisplayMember = "Name";
            this.targetSqlAgentsComboBox.DataSource = targetAgents;

            this.sourceSqlAgentsComboBox.DisplayMember = "Name";
            this.sourceSqlAgentsComboBox.DataSource = sourceAgents;
            
        }

        private void BeginCloneButton_Click(object sender, EventArgs e)
        {

            var sourceAgentType = this.sourceSqlAgentsComboBox.SelectedValue as SqlAgentBase;
            if (sourceAgentType == null)
            {
                MessageBox.Show("Source agent type not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var targetAgentType = this.targetSqlAgentsComboBox.SelectedValue as SqlAgentBase;
            if (targetAgentType == null)
            {
                MessageBox.Show("Target agent type not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.sourceConnectionStringTextBox.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Source connection string not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.targetConnectionStringTextBox.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Target connection string not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (sourceDatabaseTextBox.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Source database not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (targetDatabaseTextBox.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Target database not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DbSchema schema = null;
            SqlAgentBase sourceAgent;
            SqlAgentBase targetAgent;

            try
            {
                sourceAgent = (SqlAgentBase) Activator.CreateInstance(sourceAgentType.GetType(), 
                    this.sourceConnectionStringTextBox.Text, "", false);
                sourceAgent.CurrentDatabase = this.sourceDatabaseTextBox.Text;
                targetAgent = (SqlAgentBase)Activator.CreateInstance(targetAgentType.GetType(),
                    this.targetConnectionStringTextBox.Text, "", false);
                targetAgent.CurrentDatabase = this.targetDatabaseTextBox.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.enforceSchemaCheckBox.Checked)
            {
                try
                {
                    schema=new DbSchema();
                    schema.LoadXmlFile(this.schemaTextBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            using (var progressForm = new CloneDatabaseProgressForm(sourceAgent, targetAgent, schema))
            {
                progressForm.ShowDialog(this);
            }

        }

        private void openSchemaButton_Click(object sender, EventArgs e)
        {

            using (var openFileDialog = new OpenFileDialog())
            {

                //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                openFileDialog.CheckFileExists = true;
                openFileDialog.RestoreDirectory = false;
                openFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    if (!filePath.IsNullOrWhiteSpace()) this.schemaTextBox.Text = filePath;
                }

            }

        }

        private void targetDatabaseTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
