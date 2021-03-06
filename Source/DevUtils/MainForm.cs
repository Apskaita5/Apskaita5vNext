﻿using System;
using System.Linq;
using System.Windows.Forms;
using Apskaita5.Common;
using Apskaita5.DAL.Common;
using Apskaita5.DAL.Common.DbSchema;

namespace DeveloperUtils
{
    public partial class MainForm : Form
    {

        private SqlAgentBase _agent = null;


        public MainForm()
        {
            InitializeComponent();
        }


        private void ShowNewForm(object sender, EventArgs e)
        {
            
            if (this.ActiveMdiChild == null || !this.ActiveMdiChild.GetType().GetInterfaces().
                Contains(typeof(IStandardActionForm))) return;

            var provider = (IStandardActionForm)this.ActiveMdiChild;

            provider.Create();

        }

        private void OpenFile(object sender, EventArgs e)
        {

            string filePath = string.Empty;

            using (var openFileDialog = new OpenFileDialog())
            {

                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                openFileDialog.Filter = string.Format("{0} (*.{1})|*.{1}|All Files (*.*)|*.*", 
                    "xml files", "xml");
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;                    
                }

            }

            if (filePath.IsNullOrWhiteSpace()) return;

            foreach (var child in MdiChildren)
            {
                if (child is IStandardActionForm && ((IStandardActionForm)child).CurrentFilePath.Trim().ToLower() 
                    == filePath.Trim().ToLower())
                {
                    child.Activate();
                    return;
                }
            }

            Form childForm = null;

            try
            {
                var result = new ApplicationRoleSchemaList();
                result.LoadXmlFile(filePath);
                childForm = new ApplicationRoleSchemaListForm(result, filePath);

            }
            catch (Exception) { }

            if (childForm == null)
            {
                try
                {
                    var result = new DbSchema();
                    result.LoadXmlFile(filePath);
                    childForm = new DbSchemaForm(filePath, result);

                }
                catch (Exception) { }
            }

            if (childForm == null)
            {
                try
                {
                    var result = new SqlRepository();
                    result.LoadFile(filePath);
                    childForm = new SqlDictionaryForm(result, filePath);

                }
                catch (Exception) { }
            }

            if (childForm == null)
            {
                MessageBox.Show("File is not recognized as any Apskaita5 cofig file.", "", 
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            childForm.MdiParent = this;
            childForm.Show();

        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (this.ActiveMdiChild == null || !this.ActiveMdiChild.GetType().GetInterfaces().
                Contains(typeof(IStandardActionForm))) return;

            var provider = (IStandardActionForm)this.ActiveMdiChild;

            if (!provider.CanSave) return;

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveFileDialog.Filter = string.Format("{0} (*.{1})|*.{1}|All Files (*.*)|*.*", "xml file", "xml");
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    if (!filePath.IsNullOrWhiteSpace()) provider.Save(filePath);
                }
            }
       
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (this.ActiveMdiChild == null || !this.ActiveMdiChild.GetType().GetInterfaces().
                Contains(typeof(IStandardActionForm))) return;

            var provider = (IStandardActionForm)this.ActiveMdiChild;

            if (!provider.CanSave) return;

            if (!provider.CurrentFilePath.IsNullOrWhiteSpace())
            {
                provider.Save(provider.CurrentFilePath);
                return;
            }

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveFileDialog.Filter = string.Format("{0} (*.{1})|*.{1}|All Files (*.*)|*.*",
                    "xml file", "xml");
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    if (!filePath.IsNullOrWhiteSpace()) provider.Save(filePath);
                }
            }

        }
        
        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null || !this.ActiveMdiChild.GetType().GetInterfaces().
                Contains(typeof(IStandardActionForm))) return;

            var provider = (IStandardActionForm)this.ActiveMdiChild;

            if (!provider.CanPaste) return;

            provider.Paste(Clipboard.GetText(TextDataFormat.UnicodeText));

        }


        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }


        private void xsdValidation_MenuItem_Click(object sender, EventArgs e)
        {
            Form childForm = new XsdValidationForm
            {
                MdiParent = this
            };
            childForm.Show();
        }

        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null || !this.ActiveMdiChild.GetType().GetInterfaces().
                Contains(typeof (IStandardActionForm)))
            {
                newToolStripButton.Enabled = false;
                saveToolStripButton.Enabled = false;
                saveAsToolStripButton.Enabled = false;
                pasteToolStripButton.Enabled = false;
                return;
            }

            var provider = (IStandardActionForm) this.ActiveMdiChild;

            newToolStripButton.Enabled = provider.CanCreate;
            saveToolStripButton.Enabled = provider.CanSave;
            saveAsToolStripButton.Enabled = provider.CanSave;
            pasteToolStripButton.Enabled = provider.CanPaste;

        }

        private void sqlDictionaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form childForm = new SqlDictionaryForm
            {
                MdiParent = this
            };
            childForm.Show();
        }

        private void roleSchemaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form childForm = new ApplicationRoleSchemaListForm
            {
                MdiParent = this
            };
            childForm.Show();
        }

        private void sqlGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form childForm = new SqlGeneratorForm
            {
                MdiParent = this
            };
            childForm.Show();
        }

        private void databaseSchemaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form childForm = new DbSchemaForm
            {
                MdiParent = this
            };
            childForm.Show();
        }

        private void loginSQLServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var childForm = new InitSqlAgentForm())
            {
                if (childForm.ShowDialog(this) == DialogResult.OK) _agent = childForm.Agent;
            }
        }

        private void sQLBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_agent == null)
            {
                MessageBox.Show("No SQL server connected.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Form childForm = new SqlBrowserForm(_agent)
            {
                MdiParent = this
            };
            childForm.Show();
        }

        private void cloneDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form childForm = new CloneDatabaseForm
            {
                MdiParent = this
            };
            childForm.Show();
        }

        private void checkStructureErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_agent == null)
            {
                MessageBox.Show("No SQL server connected.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Form childForm = new DbSchemaErrorsForm(_agent)
            {
                MdiParent = this
            };
            childForm.Show();
        }

        private void dBBatchInsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form childForm = new Form1
            {
                MdiParent = this
            };
            childForm.Show();
        }

        private void createDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form childForm = new CreateDatabaseForm
            {
                MdiParent = this
            };
            childForm.Show();
        }
    }
}
