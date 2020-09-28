using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Apskaita5.Common;
using Apskaita5.Common.FormattingExtensions;

namespace DeveloperUtils
{
    public partial class SqlGeneratorForm : Form
    {

        private static readonly string[] Letters = new string[]
            {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", 
                "N", "O", "Q", "P", "R", "S", "T", "U", "V", "Z", "W"};
        private static List<string> ParamDictionary = null;


        public SqlGeneratorForm()
        {
            InitializeComponent();
        }

        
        private void generateButton_Click(object sender, EventArgs e)
        {
            outputTextBox.Text = GetInsertStatement() + Environment.NewLine
                + Environment.NewLine + Environment.NewLine + GetUpdateStatement();
        }


        private string GetInsertStatement()
        {
            if (string.IsNullOrEmpty(columnsTextBox.Text.Trim()) ||
                string.IsNullOrEmpty(tableTextBox.Text.Trim()) ||
                string.IsNullOrEmpty(paramPrefixTextBox.Text.Trim())) return "";

            columnsTextBox.Text = columnsTextBox.Text.Trim().Replace("\"", "").
                Replace("'", "").Replace("`", "").Replace(" ", "");

            var columnsList = columnsTextBox.Text.Split(new string[] { "," },
                StringSplitOptions.RemoveEmptyEntries);

            var paramList = new List<string>();
            for (int i = 0; i < columnsList.Length; i++)
            {
                paramList.Add(GetParamId(i));
            }

            string result = string.Format("INSERT INTO {0}({1}) VALUES({2});", 
                tableTextBox.Text.Trim(), string.Join(", ", columnsList), 
                string.Join(", ", paramList.ToArray()));

            var result2 = string.Empty;

            for (int i = 0; i < columnsList.Length; i++)
            {
                result2 = result2.AddNewLine(string.Format("MyComm.AddParam(\"{0}\", _{1});",
                    GetParamId(i), columnsList[i].Trim()), false);
            }

            return result + Environment.NewLine + Environment.NewLine + result2;

        }

        private string GetUpdateStatement()
        {
            if (string.IsNullOrEmpty(columnsTextBox.Text.Trim()) || 
                string.IsNullOrEmpty(tableTextBox.Text.Trim()) || 
                string.IsNullOrEmpty(paramPrefixTextBox.Text.Trim())) return "";

            columnsTextBox.Text = columnsTextBox.Text.Trim().Replace("\"", "").
                Replace("'", "").Replace("`", "");

            var columnsList = columnsTextBox.Text.Split(new string[] { "," }, 
                StringSplitOptions.RemoveEmptyEntries);

            var tableName = tableTextBox.Text.Trim();

            string result = "";
            string result2 = "";

            for (int i = 0; i < columnsList.Length; i++)
            {
                result2 = result2.AddNewLine(string.Format("MyComm.AddParam(\"{0}\", _{1});", 
                    GetParamId(i), columnsList[i].Trim()), false);
                columnsList[i] = columnsList[i].Trim() + "=" + GetParamId(i);
            }
            
            result = string.Format("UPDATE {0} SET {1} WHERE ID={2}OD ;", tableName, 
                string.Join(", ", columnsList), paramPrefixTextBox.Text.Trim());

            return result + Environment.NewLine + Environment.NewLine + result2;

        }

        private string GetParamId(int i)
        {
            return paramPrefixTextBox.Text.Trim() + GetParamDictionary()[i];
        }

        private List<string> GetParamDictionary()
        {
            if (ParamDictionary != null) return ParamDictionary;

            ParamDictionary = new List<string>();
            for (int i = 1; i <= 400; i++)
            {
                ParamDictionary.Add(Letters[(int)Math.Ceiling((i / 24.0) - 1)] 
                    + Letters[i - (int)(Math.Ceiling((i / 24.0) - 1) * 24 + 1)]);
            }
            ParamDictionary.Remove("AS");
            ParamDictionary.Remove("BY");
            ParamDictionary.Remove("IF");
            ParamDictionary.Remove("IN");
            ParamDictionary.Remove("IS");
            ParamDictionary.Remove("ON");
            ParamDictionary.Remove("OR");
            ParamDictionary.Remove("TO");

            return ParamDictionary;

        }

    }
}
