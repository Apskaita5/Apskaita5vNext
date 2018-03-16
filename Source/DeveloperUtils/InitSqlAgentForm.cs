using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Apskaita5.DAL.Common;
using Apskaita5.Common;
using Apskaita5.DAL.MySql;
using Apskaita5.DAL.Sqlite;

namespace DeveloperUtils
{
    public partial class InitSqlAgentForm : Form
    {

        private SqlAgentBase _agent = null;


        public SqlAgentBase Agent {
            get { return _agent; }
        }


        public InitSqlAgentForm()
        {
            InitializeComponent();
        }


        private void InitSqlAgentForm_Load(object sender, EventArgs e)
        {

            var agents = new List<SqlAgentBase>()
                {
                    new MySqlAgent("fake conn string", "", false),
                    new SqliteAgent("fake conn string", "", false)
                };

            this.sqlAgentsComboBox.DisplayMember = "Name";
            this.sqlAgentsComboBox.DataSource = agents;

        }

        private void openDatabaseFileButton_Click(object sender, EventArgs e)
        {

            var agent = this.sqlAgentsComboBox.SelectedValue as SqlAgentBase;
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
                    if (!filePath.IsNullOrWhiteSpace()) this.databaseTextBox.Text = filePath;
                }

            }

        }

        private void openSqlRepositoryFolderButton_Click(object sender, EventArgs e)
        {
            using (var openFolderDialog = new FolderBrowserDialog())
            {
                if (openFolderDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string folderPath = openFolderDialog.SelectedPath;
                    if (!folderPath.IsNullOrWhiteSpace()) this.sqlRepositoryFolderTextBox.Text = folderPath;
                }
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {

            var agent = this.sqlAgentsComboBox.SelectedValue as SqlAgentBase;
            if (agent == null)
            {
                MessageBox.Show("Agent type not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (agent.IsFileBased && databaseTextBox.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Database file not specified.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                _agent = (SqlAgentBase)Activator.CreateInstance(agent.GetType(), 
                    this.connectionStringTextBox.Text, this.sqlRepositoryFolderTextBox.Text,
                    this.useSqlRepositoryCheckBox.Checked);
                _agent.CurrentDatabase = this.databaseTextBox.Text;
                _agent.TestConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _agent = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void databaseTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void connectionStringTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void sqlAgentsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
