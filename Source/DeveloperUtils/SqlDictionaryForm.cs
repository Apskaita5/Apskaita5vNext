using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Apskaita5.DAL.Common;
using Apskaita5.Common;

namespace DeveloperUtils
{
    public partial class SqlDictionaryForm : Form, IStandardActionForm
    {

        private const string ParamRegExString = @"[\?][A-Za-z_]*";
        private const string ParamWithValueRegExString = @"\b[A-Za-z0-9_]*[=][ ]*[/?][A-Za-z]*";

        private SqlRepository _currentSource = null;
        private string _currentFilePath = string.Empty;
        private string _currentNamespace = string.Empty;
        private string _currentClass = string.Empty;


        public bool CanSave
        {
            get { return true; }
        }

        public bool CanOpen
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

        public string DefaultExtension
        {
            get { return "xml"; }
        }

        public string DefaultExtensionDescription
        {
            get { return "XML Files"; }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
        }


        public SqlDictionaryForm()
        {
            InitializeComponent();
        }


        private void SqlDictionaryForm_Load(object sender, EventArgs e)
        {
            _currentSource = new SqlRepository();
            this.sqlRepositoryBindingSource.DataSource = _currentSource;
            this.Text = "New SQL Dictionary";
        }


        private void sqlRepositoryDataGridView_CellValidating(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (this.sqlRepositoryDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()
                                                                                    .IsNullOrWhiteSpace())
            {
                this.sqlRepositoryDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText =
                    this.sqlRepositoryDataGridView.Columns[e.ColumnIndex].HeaderText + " not specified.";
            }
            else
            {
                this.sqlRepositoryDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
            }

        }

        private void sqlRepositoryDataGridView_KeyDown(object sender, KeyEventArgs e)
        {

            if (_currentSource == null) return;

            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Subtract)
            {
                if (this.sqlRepositoryDataGridView.SelectedRows != null 
                    && this.sqlRepositoryDataGridView.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in this.sqlRepositoryDataGridView.SelectedRows)
                    {
                        var item = row.DataBoundItem as SqlRepositoryItem;
                        if (item != null)
                        {
                            _currentSource.Remove(item);
                        }
                    }
                    RefreshDataSource();
                }
            }

            if (e.KeyData == Keys.Insert || e.KeyData == Keys.Add)
            {
                var newItem = new SqlRepositoryItem();
                if (!_currentNamespace.IsNullOrWhiteSpace() && !_currentClass.IsNullOrWhiteSpace())
                {
                    newItem.UsedByTypes = _currentNamespace + "." + _currentClass;
                }
                _currentSource.Add(newItem);
                RefreshDataSource();
                this.sqlRepositoryDataGridView.FirstDisplayedScrollingRowIndex = 
                    this.sqlRepositoryDataGridView.RowCount - 1;
            }

        }


        private void RefreshNamespacesButton_Click(object sender, EventArgs e)
        {
            if (_currentSource == null) return;
            NamespaceComboBox.DataSource = _currentSource.GetNamespaces();
            NamespaceComboBox.SelectedIndex = -1;
        }

        private void NamespaceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NamespaceComboBox.SelectedIndex < 0) return;
            ClassComboBox.DataSource = _currentSource.GetTypes(NamespaceComboBox.SelectedValue.ToString());
            ClassComboBox.SelectedIndex = -1;
        }

        private void filterByClassButton_Click(object sender, EventArgs e)
        {

            if (_currentSource == null) return;

            var filterExists = (!_currentNamespace.IsNullOrWhiteSpace() && !_currentClass.IsNullOrWhiteSpace());

            if (this.NamespaceComboBox.SelectedValue == null)
            {
                _currentNamespace = string.Empty;
            }
            else
            {
                _currentNamespace = this.NamespaceComboBox.SelectedValue.ToString().Trim();
            }

            if (this.ClassComboBox.SelectedValue == null)
            {
                _currentClass = string.Empty;
            }
            else
            {
                _currentClass = this.ClassComboBox.SelectedValue.ToString().Trim();
            }

            if ((!_currentNamespace.IsNullOrWhiteSpace() && !_currentClass.IsNullOrWhiteSpace()) 
                || filterExists) RefreshDataSource();

        }

        private void doReplaceButton_Click(object sender, EventArgs e)
        {

            if (this.sqlRepositoryDataGridView.SelectedCells == null || 
                this.sqlRepositoryDataGridView.SelectedCells.Count < 1 ||
                replaceOldTextBox.Text.IsNullOrWhiteSpace() ||
                this.sqlRepositoryDataGridView.SelectedCells[0].OwningColumn.DataPropertyName != "Query") return;

            if (this.sqlRepositoryDataGridView.SelectedCells.Count > 1)
            {
                MessageBox.Show("Can do replace only on single cell.", "", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            if (!this.ReplaceAllCheckBox.Checked)
            {
                this.sqlRepositoryDataGridView.SelectedCells[0].Value =
                    this.sqlRepositoryDataGridView.SelectedCells[0].Value.ToString().Replace(
                        this.replaceOldTextBox.Text, this.replaceNewTextBox.Text);
                return;
            }

            if (MessageBox.Show("Are you sure you want to do global replace?", "", 
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;

            if (_currentNamespace.IsNullOrWhiteSpace() || _currentClass.IsNullOrWhiteSpace())
            {
                foreach (var item in _currentSource)
                {
                    item.Query = item.Query.Replace(this.replaceOldTextBox.Text, this.replaceNewTextBox.Text);
                }
            }
            else
            {
                var visibleItems = _currentSource.Where(item => item.UsedByTypes.IndexOf(
                    _currentNamespace + "." + _currentClass + ",", StringComparison.OrdinalIgnoreCase) > -1 
                    || item.UsedByTypes.Trim().EndsWith(_currentNamespace + "." + _currentClass, 
                    StringComparison.OrdinalIgnoreCase));
                foreach (var item in visibleItems)
                {
                    item.Query = item.Query.Replace(this.replaceOldTextBox.Text, this.replaceNewTextBox.Text);
                }
            }

            RefreshDataSource();

        }

        private void showErrorsButton_Click(object sender, EventArgs e)
        {
            
            if (_currentSource == null) return;

            var duplicateTokens = _currentSource.GetDuplicateTokens();
            var notUsedTokens = _currentSource.GetNotUsedTokens();

            if (duplicateTokens.Count < 1 && notUsedTokens.Count < 1)
            {
                MessageBox.Show("Ok", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = string.Empty;
            if (duplicateTokens.Count > 0)
                result = result.AddNewLine(string.Format("Duplicate tokens found: {0}",
                    string.Join(", ", duplicateTokens.ToArray())), false);
            if (notUsedTokens.Count > 0)
                result = result.AddNewLine(string.Format("Not used tokens found: {0}",
                    string.Join(", ", notUsedTokens.ToArray())), false);

            MessageBox.Show(result, "", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void GetParametersButton_Click(object sender, EventArgs e)
        {

            if (_currentSource == null) return;

            if (this.sqlRepositoryDataGridView.SelectedRows == null 
                || this.sqlRepositoryDataGridView.SelectedRows.Count < 1) return;

            if (this.sqlRepositoryDataGridView.SelectedRows.Count > 1)
            {
                MessageBox.Show("Cannot get params for multiple queries.", "", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var item = this.sqlRepositoryDataGridView.SelectedRows[0].DataBoundItem as SqlRepositoryItem;
            if (item == null) return;

            var paramRegex = new Regex(ParamWithValueRegExString);

            var paramsMatches = paramRegex.Matches(item.Query.Trim());

            var result = string.Empty;
            var resultlist = new List<string>();

            foreach (Match paramsMatch in paramsMatches)
            {
                resultlist.Add(paramsMatch.Value.Substring(0, paramsMatch.Value.IndexOf("=")).Trim());
                result.AddNewLine(string.Format("MyComm.AddParam(\"{0}\", _{1})",
                    paramsMatch.Value.Substring(paramsMatch.Value.IndexOf("=") + 1, 3).Trim(),
                    paramsMatch.Value.Substring(0, paramsMatch.Value.IndexOf("="))), false);
            }

            Clipboard.SetText(string.Join(", ", resultlist.ToArray()) + Environment.NewLine
                + Environment.NewLine + result, TextDataFormat.UnicodeText);

            MessageBox.Show("Parameter string has been copied to the clipboard.", "", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        }


        public void Save(string filePath)
        {

            if (_currentSource == null || filePath.IsNullOrWhiteSpace()) return;

            try
            {
                _currentSource.SaveToFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _currentFilePath = filePath;

            this.Text = "SQL Dictionary: " + _currentFilePath;

        }

        public void Open(string filePath)
        {

            if (filePath.IsNullOrWhiteSpace()) return;

            if (!SaveCurrentSource()) return;

            var result = new SqlRepository();
            try
            {
                result.LoadFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            RebindDataSource(result);

            _currentFilePath = filePath;

            this.Text = "SQL Dictionary: " + _currentFilePath;

        }

        public void Create()
        {

            if (!SaveCurrentSource()) return;

            var result = new SqlRepository();
            
            RebindDataSource(result);

            _currentFilePath = string.Empty;

            this.Text = "New SQL Dictionary";

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

            RefreshDataSource();
            
            this.sqlRepositoryDataGridView.FirstDisplayedScrollingRowIndex = 
                this.sqlRepositoryDataGridView.RowCount - 1;

            this.sqlRepositoryDataGridView.Select();

        }


        private void RefreshDataSource()
        {

            if (_currentSource == null) return;

            if (_currentNamespace.IsNullOrWhiteSpace() || _currentClass.IsNullOrWhiteSpace())
            {
                this.sqlRepositoryBindingSource.DataSource = null;
                this.sqlRepositoryBindingSource.DataSource = _currentSource;
                this.sqlRepositoryBindingSource.ResetBindings(false);
            }
            else
            {
                this.sqlRepositoryBindingSource.DataSource = null;
                this.sqlRepositoryBindingSource.DataSource = _currentSource.Where(
                    item => item.UsedByTypes.IndexOf(_currentNamespace + "." + _currentClass + ",", 
                        StringComparison.OrdinalIgnoreCase) > -1 || item.UsedByTypes.Trim().
                        EndsWith(_currentNamespace + "." + _currentClass, StringComparison.OrdinalIgnoreCase));
                this.sqlRepositoryBindingSource.ResetBindings(false);
            }
            this.sqlRepositoryDataGridView.Select();

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
                _currentSource.SaveToFile(filePath.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;

        }

        private void RebindDataSource(SqlRepository newSource)
        {

            this.sqlRepositoryBindingSource.RaiseListChangedEvents = false;
            this.sqlRepositoryBindingSource.EndEdit();
            this.sqlRepositoryBindingSource.DataSource = null;

            _currentSource = newSource;

            ClassComboBox.SelectedIndex = -1;
            ClassComboBox.DataSource = null;

            NamespaceComboBox.SelectedIndex = -1;
            if (_currentSource != null)
            {
                NamespaceComboBox.DataSource = _currentSource.GetNamespaces();
            }
            else
            {
                NamespaceComboBox.DataSource = null;
            }            

            this.sqlRepositoryBindingSource.DataSource = _currentSource;
            this.sqlRepositoryBindingSource.RaiseListChangedEvents = true;
            this.sqlRepositoryBindingSource.ResetBindings(false);

            this.sqlRepositoryDataGridView.Select();

        }

    }

}
