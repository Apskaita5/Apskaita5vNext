using System;
using System.Windows.Forms;
using Apskaita5.DAL.Common;
using Apskaita5.Common;
using BrightIdeasSoftware;

namespace DeveloperUtils
{
    public partial class ApplicationRoleSchemaListForm : Form, IStandardActionForm
    {

        private ApplicationRoleSchemaList _currentSource = null;
        private string _currentFilePath = string.Empty;


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

        
        public ApplicationRoleSchemaListForm()
        {
            InitializeComponent();
        }

        public ApplicationRoleSchemaListForm(ApplicationRoleSchemaList objectToEdit, string filePath)
        {
            InitializeComponent();
            _currentSource = objectToEdit;
            _currentFilePath = filePath;            
        }


        private void ApplicationRoleSchemaListForm_Load(object sender, EventArgs e)
        {

            if (_currentSource == null)
            {
                _currentSource = new ApplicationRoleSchemaList();
                this.Text = "New Role Schema";
            }
            else
            {
                this.Text = "Role Schema: " + _currentFilePath;
            }
            
            this.applicationRoleSchemaListBindingSource.DataSource = _currentSource;

            ((SimpleDropSink) this.EditableDataListView.DropSink).AcceptExternal = false;
            ((SimpleDropSink) this.EditableDataListView.DropSink).CanDropOnBackground = false;
            ((SimpleDropSink)this.EditableDataListView.DropSink).CanDropOnItem = false;
            ((SimpleDropSink)this.EditableDataListView.DropSink).CanDropOnSubItem = false;
            ((SimpleDropSink)this.EditableDataListView.DropSink).EnableFeedback = true;
            ((SimpleDropSink)this.EditableDataListView.DropSink).CanDropBetween = true;            

        }


        private void EditableDataListView_KeyDown(object sender, KeyEventArgs e)
        {

            if (_currentSource == null) return;

            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Subtract)
            {
                if (this.EditableDataListView.SelectedObjects != null
                    && this.EditableDataListView.SelectedObjects.Count > 0)
                {
                    foreach (ApplicationRoleSchema item in this.EditableDataListView.SelectedObjects)
                    {
                        if (item != null)
                        {
                            _currentSource.Remove(item);
                        }
                    }
                    this.applicationRoleSchemaListBindingSource.ResetBindings(false);
                }
            }

            if (e.KeyData == Keys.Insert || e.KeyData == Keys.Add)
            {
                var newItem = new ApplicationRoleSchema();
                _currentSource.Add(newItem);
                this.applicationRoleSchemaListBindingSource.ResetBindings(false);
                this.EditableDataListView.EnsureVisible(this.EditableDataListView.
                    GetLastItemInDisplayOrder().Index);
            }

        }

        private void EditableDataListView_ModelCanDrop(object sender, ModelDropEventArgs e)
        {

            e.Handled = true;
            e.Effect = DragDropEffects.None;

            if (_currentSource == null || e.SourceModels == null || e.SourceModels.Count!=1
                || e.TargetModel == null || object.ReferenceEquals(e.SourceModels[0], e.TargetModel))
                return;

            var indexToMove = _currentSource.IndexOf((ApplicationRoleSchema)e.TargetModel);
            if (e.DropTargetLocation == DropTargetLocation.BelowItem)
                indexToMove += 1;

            if (_currentSource.IndexOf((ApplicationRoleSchema)e.SourceModels[0]) == indexToMove)
                return;

            e.Effect = DragDropEffects.Move;

        }

        private void EditableDataListView_ModelDropped(object sender, ModelDropEventArgs e)
        {

            if (_currentSource == null || e.SourceModels == null || e.SourceModels.Count != 1
                || e.TargetModel == null || object.ReferenceEquals(e.SourceModels[0], e.TargetModel))
                return;

            var indexToMove = _currentSource.IndexOf((ApplicationRoleSchema)e.TargetModel);
            if (e.DropTargetLocation == DropTargetLocation.BelowItem)
                indexToMove += 1;

            if (_currentSource.IndexOf((ApplicationRoleSchema)e.SourceModels[0]) == indexToMove)
                return;

            _currentSource.MoveToIndex((ApplicationRoleSchema)e.SourceModels[0], indexToMove);

            this.applicationRoleSchemaListBindingSource.ResetBindings(false);

        }


        public void Save(string filePath)
        {

            if (_currentSource == null || filePath.IsNullOrWhiteSpace()) return;

            try
            {
                _currentSource.SetSequentialVisibleIndex();
                _currentSource.SaveXmlFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _currentFilePath = filePath;

            this.Text = "Role Schema: " + _currentFilePath;

        }

        public void Create()
        {

            if (!SaveCurrentSource()) return;

            var result = new ApplicationRoleSchemaList();

            RebindDataSource(result);

            _currentFilePath = string.Empty;

            this.Text = "New Role Schema";

        }

        public void Paste(string source)
        {

            if (_currentSource == null) return;

            try
            {
                _currentSource.LoadDelimitedString(source, Environment.NewLine, "\t");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.applicationRoleSchemaListBindingSource.ResetBindings(false);

            this.EditableDataListView.EnsureVisible(this.EditableDataListView.GetLastItemInDisplayOrder().Index);

            this.EditableDataListView.Select();

        }


        private bool SaveCurrentSource()
        {

            if (_currentSource == null || _currentSource.Count < 1) return true;

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

        private void RebindDataSource(ApplicationRoleSchemaList newSource)
        {

            this.applicationRoleSchemaListBindingSource.RaiseListChangedEvents = false;
            this.applicationRoleSchemaListBindingSource.EndEdit();
            this.applicationRoleSchemaListBindingSource.DataSource = null;

            _currentSource = newSource;

            this.applicationRoleSchemaListBindingSource.DataSource = _currentSource;
            this.applicationRoleSchemaListBindingSource.RaiseListChangedEvents = true;
            this.applicationRoleSchemaListBindingSource.ResetBindings(false);

            this.EditableDataListView.Select();

        }

    }
}
