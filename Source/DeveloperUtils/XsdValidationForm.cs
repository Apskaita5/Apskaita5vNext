using System;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace DeveloperUtils
{
    public partial class XsdValidationForm : Form
    {

        private readonly XmlReaderSettings settings = new XmlReaderSettings();


        public XsdValidationForm()
        {
            InitializeComponent();

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += ValidationCallBack;
        }


        private void validateButton_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(xmlFilePathTextBox.Text.Trim()))
            {
                MessageBox.Show("XML file is not specified.", "Error", MessageBoxButtons.OK);
                return;
            }
            if (string.IsNullOrEmpty(xsdFilePathTextBox.Text.Trim()))
            {
                MessageBox.Show("XSD file is not specified.", "Error", MessageBoxButtons.OK);
                return;
            }

            var schema = XmlSchema.Read(new XmlTextReader(xsdFilePathTextBox.Text), null);
            
            settings.Schemas.Add(schema);

            // Create the XmlReader object.
            XmlReader reader = XmlReader.Create(xmlFilePathTextBox.Text, settings);

            resultTextBox.Text = "";

            var oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                
                // Parse the file. 
                while (reader.Read());

                settings.Schemas.Remove(schema);

            }
            finally
            {
                Cursor.Current = oldCursor;
            }  

            if (string.IsNullOrEmpty(resultTextBox.Text)) resultTextBox.Text = "Schema Ok.";

        }

        // Display any warnings or errors.
        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            
            if (args.Severity == XmlSeverityType.Warning)
                AddResultLine("Warning: " + args.Message);
            else
                AddResultLine("Error: " + args.Message);

        }

        private void AddResultLine(string line)
        {
            if (line == null || string.IsNullOrEmpty(line.Trim())) return;
            if (string.IsNullOrEmpty(resultTextBox.Text))
            {
                resultTextBox.Text = line.Trim();
            }
            else
            {
                resultTextBox.Text = resultTextBox.Text + Environment.NewLine + line.Trim();
            }
        }


        private void OpenXmlFileButton_Click(object sender, EventArgs e)
        {
            var result = RequestFilePath("XML Files", "xml");
            if (!string.IsNullOrEmpty(result.Trim())) xmlFilePathTextBox.Text = result;
        }

        private void OpenXsdFileButton_Click(object sender, EventArgs e)
        {
            var result = RequestFilePath("XSD Files", "xsd");
            if (!string.IsNullOrEmpty(result.Trim())) xsdFilePathTextBox.Text = result;
        }

        private string RequestFilePath(string defaultTypeName, string defaultExtension)
        {

            using (var dlg = new OpenFileDialog())
            {
                
                dlg.CheckFileExists = true;
                dlg.Multiselect = false;
                if (defaultExtension != null && !string.IsNullOrEmpty(defaultExtension.Trim()))
                {
                    if (defaultTypeName != null && !string.IsNullOrEmpty(defaultTypeName.Trim()))
                    {
                        dlg.Filter = string.Format("{0} (*.{1})|*.{1}|All files|*.*", defaultTypeName, defaultExtension);
                    }
                    else
                    {
                        dlg.Filter = string.Format("*.{0}|*.{0}|All files|*.*", defaultExtension);
                    }
                }
            
                if (dlg.ShowDialog() != DialogResult.OK)
                    return string.Empty;

                if (dlg.FileName == null) return string.Empty;

                return dlg.FileName.Trim();

            }

        }

    }
}
