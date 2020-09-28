using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Apskaita5.DAL.Common;
using Apskaita5.Common;
using Apskaita5.DAL.MySql;
using Apskaita5.DAL.SQLite;
using System.Threading.Tasks;
using Apskaita5.DAL.Common.DbSchema;

namespace DeveloperUtils
{
    public partial class CreateDatabaseForm : Form
    {

        public CreateDatabaseForm()
        {
            InitializeComponent();
        }

        private void CreateDatabaseForm_Load(object sender, EventArgs e)
        {
            var agents = new List<SqlAgentBase>()
                {
                    new MySqlAgent("fake conn string", string.Empty, null, null),
                    new SqliteAgent("fake conn string", "fake path", null, null)
                };

            this.sqlAgentsComboBox.DisplayMember = "Name";
            this.sqlAgentsComboBox.DataSource = agents;
            this.connectionStringTextBox.Text = "Server=localhost;Uid=root;Pwd=myPass;CharSet=utf8;";
        }

        private async void CreateButton_Click(object sender, EventArgs e)
        {

            var agentPrototype = this.sqlAgentsComboBox.SelectedValue as SqlAgentBase;
            if (agentPrototype == null)
            {
                MessageBox.Show("Agent type not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (databaseTextBox.Text.IsNullOrWhiteSpace())
            {
                if (agentPrototype.IsFileBased)
                {
                    MessageBox.Show("Database file not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else 
                {
                    MessageBox.Show("Database name not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            if (this.schemaPathTextBox.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Database schema path not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SqlAgentBase agent = null;

            var currentCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            this.CreateButton.Enabled = false;

            try
            {

                agent = (SqlAgentBase)Activator.CreateInstance(agentPrototype.GetType(),
                    this.connectionStringTextBox.Text, this.databaseTextBox.Text, null, null);                
                

                var schema = new DbSchema();
                if (this.schemaInFileButton.Checked)
                {
                    schema.LoadXmlFile(this.schemaPathTextBox.Text);
                }
                else
                {
                    schema.LoadXmlFolder(this.schemaPathTextBox.Text);
                }

                await Task.Run(async () => await agent.GetDefaultSchemaManager().CreateDatabaseAsync(schema));
            }
            catch (Exception ex)
            {
                this.Cursor = currentCursor;
                this.CreateButton.Enabled = true;
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Cursor = currentCursor;
            this.CreateButton.Enabled = true;

        }

        private void openDatabaseFileButton_Click(object sender, EventArgs e)
        {

            var agent = this.sqlAgentsComboBox.SelectedValue as SqlAgentBase;
            if (agent == null || !agent.IsFileBased) return;

            using (var saveFileDialog = new SaveFileDialog())
            {

                //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveFileDialog.CheckFileExists = false;
                saveFileDialog.AddExtension = true;
                saveFileDialog.RestoreDirectory = false;
                saveFileDialog.DefaultExt = ".db";
                saveFileDialog.Filter = "Database Files (*.db)|*.db|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    if (!filePath.IsNullOrWhiteSpace()) this.databaseTextBox.Text = filePath;
                }

            }

        }

        private void openSchemaPathButton_Click(object sender, EventArgs e)
        {
            if (this.schemaInFileButton.Checked)
            {

                using (var openFileDialog = new OpenFileDialog())
                {

                    //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.RestoreDirectory = false;
                    openFileDialog.Filter = "Schema Files (*.xml)|*.xml|All Files (*.*)|*.*";
                    if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        if (!filePath.IsNullOrWhiteSpace()) this.schemaPathTextBox.Text = filePath;
                    }

                }

            }
            else
            {

                using (var openFolderDialog = new FolderBrowserDialog())
                {
                    openFolderDialog.ShowNewFolderButton = false;
                    if (openFolderDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        string folderPath = openFolderDialog.SelectedPath;
                        if (!folderPath.IsNullOrWhiteSpace()) this.schemaPathTextBox.Text = folderPath;
                    }
                }

            }

        }
    }
}
