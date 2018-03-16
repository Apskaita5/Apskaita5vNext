using System;
using System.ComponentModel;
using System.Windows.Forms;
using Apskaita5.DAL.Common;

namespace DeveloperUtils
{
    public partial class CloneDatabaseProgressForm : Form
    {

        private SqlAgentBase _sourceAgent;
        private SqlAgentBase _targetAgent;
        private DbSchema _schema;
        

        public CloneDatabaseProgressForm(SqlAgentBase sourceAgent, SqlAgentBase targetAgent, DbSchema schema)
        {
            InitializeComponent();
            _sourceAgent = sourceAgent;
            _targetAgent = targetAgent;
            _schema = schema;
        }


        private void CloneDatabaseProgressForm_Load(object sender, EventArgs e)
        {
            var param = new object[]{_sourceAgent, _targetAgent, _schema};
            this.backgroundWorker1.RunWorkerAsync(param);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var targetAgent = (SqlAgentBase) (((object[]) e.Argument)[1]);
            var schema = (DbSchema) (((object[]) e.Argument)[2]);
            _sourceAgent.CloneDatabase(_targetAgent.CurrentDatabase, _targetAgent, schema, this.backgroundWorker1);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
            this.progressLabel.Text = e.UserState.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            if (e.Cancelled)
            {
                MessageBox.Show("Database cloning has been canceled by the user.", "", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            MessageBox.Show("Database has been succesfully cloned.", "", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

            this.Close();
        }

        private void cancelCloneButton_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.CancelAsync();
            this.cancelCloneButton.Enabled = false;
        }

    }
}
