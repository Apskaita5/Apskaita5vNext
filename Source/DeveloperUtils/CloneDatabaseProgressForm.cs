using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apskaita5.DAL.Common;
using Apskaita5.DAL.Common.DbSchema;

namespace DeveloperUtils
{
    public partial class CloneDatabaseProgressForm : Form
    {

        private SqlAgentBase _sourceAgent;
        private SqlAgentBase _targetAgent;
        private DbSchema _schema;
        private CancellationTokenSource cts = new CancellationTokenSource();


        public CloneDatabaseProgressForm(SqlAgentBase sourceAgent, SqlAgentBase targetAgent, DbSchema schema)
        {
            InitializeComponent();
            _sourceAgent = sourceAgent;
            _targetAgent = targetAgent;
            _schema = schema;
        }


        private async void CloneDatabaseProgressForm_Load(object sender, EventArgs e)
        {

            this.progressLabel.Text = "Initializing...";

            var progress = new Progress<DbCloneProgressArgs>(ReportProgress);

            try
            {
                var sourceManager = (SchemaManagerBase)_sourceAgent.GetDefaultSchemaManager();
                var targetManager = (SchemaManagerBase)_targetAgent.GetDefaultSchemaManager();
                await Task.Run(async () => await sourceManager.CloneDatabase(targetManager, _schema, progress, cts.Token));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            

        }

        private void ReportProgress(DbCloneProgressArgs args)
        {
            if (args.CurrentStage == DbCloneProgressArgs.Stage.Canceled)
            {
                this.progressLabel.Text = "Database clone operation has been canceled by the user.";
                MessageBox.Show("Database clone operation has been canceled by the user.", "Canceled", 
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
            else if (args.CurrentStage == DbCloneProgressArgs.Stage.Completed)
            {
                this.progressLabel.Text = "Database clone operation has completed succesfully.";
                MessageBox.Show("Database clone operation has completed succesfully.", "Completed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else if (args.CurrentStage == DbCloneProgressArgs.Stage.CopyingData)
            {
                this.progressLabel.Text = string.Format("Copying data for table {0}.", args.CurrentTable);
            }
            else if (args.CurrentStage == DbCloneProgressArgs.Stage.CreatingSchema)
            {
                this.progressLabel.Text = "Creating new schema...";
            }
            else if (args.CurrentStage == DbCloneProgressArgs.Stage.FetchingRowCount)
            {
                this.progressLabel.Text = "Fetching total row count...";
            }
            else
            {
                this.progressLabel.Text = "Fetching schema...";
            }
            this.progressBar1.Value = args.RowProgress;
        }
        
        private void cancelCloneButton_Click(object sender, EventArgs e)
        {
            cts.Cancel(); 
            this.cancelCloneButton.Enabled = false;
            this.progressLabel.Text = "Canceling database clone operation...";
        }

    }
}
