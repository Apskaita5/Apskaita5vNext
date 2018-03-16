using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Apskaita5.DAL.Common;
using Apskaita5.Common;

namespace DeveloperUtils
{
    static class Utilities
    {

        internal static SqlAgentBase CreateSqlAgent(string typeName)
        {

            if (typeName.IsNullOrWhiteSpace())
                throw new ArgumentNullException("typeName");

            var typeNameParts = typeName.Split(new string[] { "," },
                StringSplitOptions.RemoveEmptyEntries);

            if (typeNameParts.Length < 5)
                throw new ArgumentException("String \"{0}\" is not an assembly qualified type name.");

            var assemblyName = typeNameParts[1].Trim();

            foreach (var loadedType in from assembly in AppDomain.CurrentDomain.GetAssemblies() 
                                       from loadedType in assembly.GetTypes() 
                                       where loadedType.AssemblyQualifiedName.Trim().ToLower() ==
                                            typeName.Trim().ToLower() select loadedType)
            {
                return CreateSqlAgentInt(loadedType);
            }

            var executingAssemblyPath = Assembly.GetExecutingAssembly().GetName().CodeBase;
            var currentPath = Path.GetDirectoryName(executingAssemblyPath);
            if (currentPath.IsNullOrWhiteSpace())
                throw new InvalidOperationException("Failed to resolve app path.");
            currentPath = Path.Combine(currentPath, "Plugins");
            currentPath = (new Uri(currentPath)).LocalPath;

            foreach (var filePath in System.IO.Directory.GetFiles(currentPath,
                "*.*", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(filePath);
                if (fileName != null && fileName.Trim().ToLower() == 
                    assemblyName.Trim().ToLower() + ".dll")
                {
                    var asm = Assembly.LoadFrom(filePath);
                    var instanceType = GetType(asm, typeName);
                    return CreateSqlAgentInt(instanceType);
                }
            }

            throw new ArgumentException(string.Format("Plugin assembly {0} not found.", assemblyName));

        }

        internal static SqlAgentBase CreateSqlAgent()
        {
            return CreateSqlAgent(ConfigurationManager.AppSettings["SqlAgentType"]);
        }

        private static SqlAgentBase CreateSqlAgentInt(Type instanceType)
        {
            if (instanceType == null)
                throw new ArgumentNullException("instanceType");

            return (SqlAgentBase)Activator.CreateInstance(instanceType,
                "connString", "", false);
        }

        private static Type GetType(Assembly assembly, string typeName)
        {
            foreach (var type in assembly.GetTypes().Where(type => 
                type.AssemblyQualifiedName.Trim().ToLower() == typeName.Trim().ToLower()))
            {
                return type;
            }
            throw new InvalidOperationException(string.Format("Assembly {0} does not contain type {1}.",
                assembly.FullName, typeName));
        }


        internal static void SetErrors(DataGridViewRow row, Dictionary<string, 
            List<string>> errorDictionary)
        {

            foreach (DataGridViewCell cell in row.Cells)
            {
                if (errorDictionary.ContainsKey(cell.OwningColumn.DataPropertyName))
                {
                    cell.ErrorText = string.Join(Environment.NewLine,
                        errorDictionary[cell.OwningColumn.DataPropertyName].ToArray());
                }
                else
                {
                    cell.ErrorText = string.Empty;
                }
            }

        }

        internal static DataTable ToDataTable(this LightDataTable source)
        {
            
            if (source == null) return null;

            var result = new DataTable(source.TableName);

            try
            {
                foreach (var column in source.Columns)
                {
                    result.Columns.Add(column.ColumnName, column.DataType);
                }

                foreach (var row in source.Rows)
                {
                    var dr = result.Rows.Add();
                    for (int i = 0; i < source.Columns.Count; i++)
                    {
                        dr[i] = row.GetValue(i);
                    }
                }
            }
            catch (Exception)
            {
                result.Dispose();
                throw;
            }

            return result;

        }

    }
}
