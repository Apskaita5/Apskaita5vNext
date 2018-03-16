namespace DeveloperUtils
{
    partial class XsdValidationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.validateButton = new System.Windows.Forms.Button();
            this.xmlFilePathTextBox = new System.Windows.Forms.TextBox();
            this.OpenXmlFileButton = new System.Windows.Forms.Button();
            this.OpenXsdFileButton = new System.Windows.Forms.Button();
            this.xsdFilePathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.resultTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // validateButton
            // 
            this.validateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.validateButton.Location = new System.Drawing.Point(711, 434);
            this.validateButton.Name = "validateButton";
            this.validateButton.Size = new System.Drawing.Size(75, 23);
            this.validateButton.TabIndex = 0;
            this.validateButton.Text = "Validate";
            this.validateButton.UseVisualStyleBackColor = true;
            this.validateButton.Click += new System.EventHandler(this.validateButton_Click);
            // 
            // xmlFilePathTextBox
            // 
            this.xmlFilePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.xmlFilePathTextBox.Location = new System.Drawing.Point(75, 12);
            this.xmlFilePathTextBox.Name = "xmlFilePathTextBox";
            this.xmlFilePathTextBox.ReadOnly = true;
            this.xmlFilePathTextBox.Size = new System.Drawing.Size(679, 20);
            this.xmlFilePathTextBox.TabIndex = 1;
            // 
            // OpenXmlFileButton
            // 
            this.OpenXmlFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenXmlFileButton.Location = new System.Drawing.Point(760, 12);
            this.OpenXmlFileButton.Name = "OpenXmlFileButton";
            this.OpenXmlFileButton.Size = new System.Drawing.Size(26, 20);
            this.OpenXmlFileButton.TabIndex = 2;
            this.OpenXmlFileButton.Text = "...";
            this.OpenXmlFileButton.UseVisualStyleBackColor = true;
            this.OpenXmlFileButton.Click += new System.EventHandler(this.OpenXmlFileButton_Click);
            // 
            // OpenXsdFileButton
            // 
            this.OpenXsdFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenXsdFileButton.Location = new System.Drawing.Point(760, 38);
            this.OpenXsdFileButton.Name = "OpenXsdFileButton";
            this.OpenXsdFileButton.Size = new System.Drawing.Size(26, 20);
            this.OpenXsdFileButton.TabIndex = 4;
            this.OpenXsdFileButton.Text = "...";
            this.OpenXsdFileButton.UseVisualStyleBackColor = true;
            this.OpenXsdFileButton.Click += new System.EventHandler(this.OpenXsdFileButton_Click);
            // 
            // xsdFilePathTextBox
            // 
            this.xsdFilePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.xsdFilePathTextBox.Location = new System.Drawing.Point(75, 38);
            this.xsdFilePathTextBox.Name = "xsdFilePathTextBox";
            this.xsdFilePathTextBox.ReadOnly = true;
            this.xsdFilePathTextBox.Size = new System.Drawing.Size(679, 20);
            this.xsdFilePathTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "XML Path:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "XSD Path:";
            // 
            // resultTextBox
            // 
            this.resultTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultTextBox.Location = new System.Drawing.Point(15, 64);
            this.resultTextBox.Multiline = true;
            this.resultTextBox.Name = "resultTextBox";
            this.resultTextBox.ReadOnly = true;
            this.resultTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.resultTextBox.Size = new System.Drawing.Size(771, 364);
            this.resultTextBox.TabIndex = 7;
            // 
            // XsdValidationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 469);
            this.Controls.Add(this.resultTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenXsdFileButton);
            this.Controls.Add(this.xsdFilePathTextBox);
            this.Controls.Add(this.OpenXmlFileButton);
            this.Controls.Add(this.xmlFilePathTextBox);
            this.Controls.Add(this.validateButton);
            this.Name = "XsdValidationForm";
            this.Text = "XSD Validation";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button validateButton;
        private System.Windows.Forms.TextBox xmlFilePathTextBox;
        private System.Windows.Forms.Button OpenXmlFileButton;
        private System.Windows.Forms.Button OpenXsdFileButton;
        private System.Windows.Forms.TextBox xsdFilePathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox resultTextBox;
    }
}

