using System;
using System.Collections.Generic;
using System.Linq;
using Apskaita5.DAL.Common;

namespace Apskaita5.DAL.MySql
{
    /// <summary>
    /// Provides extension methods for the canonical DbSchema objects to translate them
    /// into the native form.
    /// </summary>
    internal static class DbSchemaExtensions
    {

        /// <summary>
        /// Returns true if the canonical DbDataTypes specified are equivalent for the MySQL engine,
        /// otherwise - returns false.
        /// </summary>
        /// <param name="type1">the first type to compare</param>
        /// <param name="type2">the second type to compare</param>
        internal static bool IsEquivalentTo(this DbDataType type1, DbDataType type2)
        {
            if ((type1 == DbDataType.Double || type1 == DbDataType.Float)
                && (type2 == DbDataType.Double || type2 == DbDataType.Double))
                return true;
            return (type1 == type2);
        }

        /// <summary>
        /// Returns true if the schemas of the specified fields are equivalent for the MySQL engine,
        /// otherwise - returns false. Does not compare fields indexes.
        /// </summary>
        /// <param name="field1">the first field to compare</param>
        /// <param name="field2">the second field to compare</param>
        internal static bool FieldSchemaMatch(this DbFieldSchema field1, DbFieldSchema field2)
        {
            
            if (field1 == null) throw new ArgumentNullException("field1");
            if (field2 == null) throw new ArgumentNullException("field2");

            if (field1.Name.Trim().ToLower() != field2.Name.Trim().ToLower())
                throw new ArgumentException("Cannot compare columns with diferent names.");

            if (!field1.DataType.IsEquivalentTo(field2.DataType))
                return false;
            if (field1.NotNull != field2.NotNull)
                return false;
            if ((field1.DataType.IsDbDataTypeInteger() || field1.DataType.IsDbDataTypeInteger()
                 || field1.DataType == DbDataType.Decimal) && field1.Unsigned != field2.Unsigned)
                return false;
            if (field1.DataType.IsDbDataTypeInteger() && field1.Autoincrement != field2.Autoincrement)
                return false;
            if ((field1.DataType == DbDataType.Char || field1.DataType == DbDataType.VarChar
                 || field1.DataType == DbDataType.Decimal) && field1.Length != field2.Length)
                return false;
            if ((field1.IndexType == DbIndexType.Primary && field2.IndexType != DbIndexType.Primary)
                || (field1.IndexType != DbIndexType.Primary && field2.IndexType == DbIndexType.Primary))
                return false;
            if (field1.DataType == DbDataType.Enum && field1.EnumValues.Trim().ToLower().Replace(" ", "")
                != field2.EnumValues.Trim().ToLower().Replace(" ", ""))
                return false;

            return true;

        }

        /// <summary>
        /// Returns true if the indexes of the specified fields are equivalent for the MySQL engine,
        /// otherwise - returns false.
        /// </summary>
        /// <param name="field1">the first field to compare</param>
        /// <param name="field2">the second field to compare</param>
        internal static bool FieldIndexMatch(this DbFieldSchema field1, DbFieldSchema field2)
        {

            if (field1 == null) throw new ArgumentNullException("field1");
            if (field2 == null) throw new ArgumentNullException("field2");

            if (field1.Name.Trim().ToLower() != field2.Name.Trim().ToLower())
                throw new ArgumentException("Cannot compare columns with diferent names.");

            if (field1.IndexType != field2.IndexType)
                return false;
            if (field1.IndexType == DbIndexType.ForeignKey && (field1.RefField.Trim().ToLower()
                != field2.RefField.Trim().ToLower() || field1.RefTable.Trim().ToLower()
                != field2.RefTable.Trim().ToLower() || field1.OnDeleteForeignKey != field2.OnDeleteForeignKey
                || field1.OnUpdateForeignKey != field2.OnUpdateForeignKey))
                return false;
            
            return true;

        }

        /// <summary>
        /// Gets the mysql native name for the canonical data type for the field schema specified.
        /// </summary>
        /// <param name="schema">the field schema to return the native data type for</param>
        internal static string GetNativeDataType(this DbFieldSchema schema)
        {

            if (schema == null) throw new ArgumentNullException("schema");

            switch (schema.DataType)
            {
                case DbDataType.Float:
                    return "FLOAT";
                case DbDataType.Real:
                    return "REAL";
                case DbDataType.Double:
                    return "DOUBLE";
                case DbDataType.Decimal:
                    return "DECIMAL";
                case DbDataType.IntegerTiny:
                    return "TINYINT";
                case DbDataType.IntegerSmall:
                    return "SMALLINT";
                case DbDataType.IntegerMedium:
                    return "MEDIUMINT";
                case DbDataType.Integer:
                    return "INT";
                case DbDataType.IntegerBig:
                    return "BIGINT";
                case DbDataType.TimeStamp:
                    return "TIMESTAMP";
                case DbDataType.Date:
                    return "DATE";
                case DbDataType.DateTime:
                    return "DATETIME";
                case DbDataType.Time:
                    return "TIME";
                case DbDataType.Char:
                    return "CHAR";
                case DbDataType.VarChar:
                    return "VARCHAR";
                case DbDataType.Text:
                    return "TEXT";
                case DbDataType.TextMedium:
                    return "MEDIUMTEXT";
                case DbDataType.TextLong:
                    return "LONGTEXT";
                case DbDataType.BlobTiny:
                    return "TINYBLOB";
                case DbDataType.Blob:
                    return "BLOB";
                case DbDataType.BlobMedium:
                    return "MEDIUMBLOB";
                case DbDataType.BlobLong:
                    return "LONGBLOB";
                case DbDataType.Enum:
                    return "ENUM";
                default:
                    throw new NotImplementedException(string.Format("Enum value {0} is not implemented.", 
                        schema.DataType.ToString()));
            }

        }

        /// <summary>
        /// Gets the mysql native foreign index change action type.
        /// </summary>
        /// <param name="actionType">the canonical foreign index change action type to translate</param>
        internal static string GetNativeActionType(this DbForeignKeyActionType actionType)
        {
            switch (actionType)
            {
                case DbForeignKeyActionType.Restrict:
                    return "RESTRICT";
                case DbForeignKeyActionType.Cascade:
                    return "CASCADE";
                case DbForeignKeyActionType.SetNull:
                    return "SET NULL";
                default:
                    throw new NotImplementedException(string.Format("Enum value {0} is not implemented.",
                        actionType.ToString()));
            }
        }

        /// <summary>
        /// Gets the mysql native field schema definition.
        /// </summary>
        /// <param name="schema">the canonical field schema to translate</param>
        internal static string GetFieldDefinition(this DbFieldSchema schema)
        {

            if (schema == null) throw new ArgumentNullException("schema");

            var result = schema.Name.Trim().ToLower() + " " + schema.GetNativeDataType();

            if (schema.DataType == DbDataType.Char || schema.DataType == DbDataType.VarChar)
            {
                result = result + "(" + schema.Length.ToString() + ")";
            }
            if (schema.DataType == DbDataType.Decimal)
            {
                result = result + "(" + (schema.Length + 15).ToString() + ", " + schema.Length.ToString() + ")";
            }
            if (schema.DataType == DbDataType.Enum)
            {
                var enumValues = schema.EnumValues.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                result = result + "(" + string.Join(", ", enumValues.Select(
                    enumValue => "'" + enumValue + "'").ToArray()) + ")";
            }

            if (schema.Unsigned && (schema.DataType.IsDbDataTypeInteger() ||
                schema.DataType.IsDbDataTypeFloat() || schema.DataType == DbDataType.Decimal))
            {
                result = result + " UNSIGNED";
            }

            if (schema.NotNull)
            {
                result = result + " NOT NULL";
            }

            if (schema.Autoincrement && schema.DataType.IsDbDataTypeInteger())
            {
                result = result + " AUTO_INCREMENT";
            }

            if (schema.IndexType == DbIndexType.Primary)
            {
                result = result + " PRIMARY KEY";
            }

            return result;

        }

        /// <summary>
        /// Gets a list of statements required to add a new database table field using the field schema specified.
        /// (also fixes datetime defaults and adds index if required)
        /// </summary>
        /// <param name="schema">a canonical schema of the new database table field</param>
        /// <param name="dbName">the database to add the field for</param>
        /// <param name="tblName">the database table to add the field for</param>
        internal static List<string> GetAddFieldStatements(this DbFieldSchema schema,
            string dbName, string tblName)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            if (dbName == null || string.IsNullOrEmpty(dbName.Trim())) throw new ArgumentNullException("dbName");
            if (tblName == null || string.IsNullOrEmpty(tblName.Trim())) throw new ArgumentNullException("tblName");

            var result = new List<string>()
                {
                    string.Format("ALTER TABLE `{0}`.`{1}` ADD COLUMN {2};",
                        dbName, tblName, schema.GetFieldDefinition())
                };

            if (schema.NotNull && (schema.DataType == DbDataType.Date))
            {
                result.Add(string.Format("UPDATE `{0}`.`{1}` SET {2}='{3}';", 
                    dbName, tblName.Trim().ToLower(), schema.Name.Trim().ToLower(), 
                    DateTime.UtcNow.ToString("yyyy'-'MM'-'dd")));
            }
            if (schema.NotNull && (schema.DataType == DbDataType.Time ||
                schema.DataType == DbDataType.DateTime))
            {
                result.Add(string.Format("UPDATE `{0}`.`{1}` SET {2}='{3}';", 
                    dbName, tblName.Trim().ToLower(), schema.Name.Trim().ToLower(), 
                    DateTime.UtcNow.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")));
            }

            result.AddRange(schema.GetAddIndexStatements(dbName, tblName));

            return result;
            
        }

        /// <summary>
        /// Gets a list of statements required to alter the database table field schema 
        /// to match the specified gauge schema.(does not fixes indexes)
        /// </summary>
        /// <param name="schema">the gauge field schema to apply</param>
        /// <param name="dbName">the database to alter the field for</param>
        /// <param name="tblName">the database table to alter the field for</param>
        internal static List<string> GetAlterFieldStatements(this DbFieldSchema schema,
            string dbName, string tblName)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            if (dbName == null || string.IsNullOrEmpty(dbName.Trim())) throw new ArgumentNullException("dbName");
            if (tblName == null || string.IsNullOrEmpty(tblName.Trim())) throw new ArgumentNullException("tblName");

            return new List<string>()
                {
                    string.Format("ALTER TABLE `{0}`.`{1}` MODIFY COLUMN {2};",
                        dbName, tblName, schema.GetFieldDefinition().Replace("PRIMARY KEY", ""))
                };
        }

        /// <summary>
        /// Gets a list of statements required to drop the database table field.
        /// </summary>
        /// <param name="schema">the field schema to drop</param>
        /// <param name="dbName">the database to drop the field for</param>
        /// <param name="tblName">the database table to drop the field for</param>
        internal static List<string> GetDropFieldStatements(this DbFieldSchema schema,
            string dbName, string tblName)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            if (dbName == null || string.IsNullOrEmpty(dbName.Trim())) throw new ArgumentNullException("dbName");
            if (tblName == null || string.IsNullOrEmpty(tblName.Trim())) throw new ArgumentNullException("tblName");

            return new List<string>()
                {
                    string.Format("ALTER TABLE `{0}`.`{1}` DROP COLUMN {2};", 
                        dbName, tblName.Trim().ToLower(), schema.Name.Trim().ToLower())
                };

        }

        /// <summary>
        /// Gets the mysql native index schema definition.
        /// </summary>
        /// <param name="schema">the canonical field schema to translate</param>
        /// <remarks>MySql creates an index for each foreign key.</remarks>
        internal static string GetIndexDefinition(this DbFieldSchema schema)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            
            if (schema.IndexType == DbIndexType.None || schema.IndexType == DbIndexType.Primary)
                return string.Empty;

            if (schema.IndexType == DbIndexType.Unique)
            {
                return string.Format("UNIQUE KEY `{0}` (`{1}`)", schema.IndexName.Trim().ToLower(), 
                    schema.Name.Trim().ToLower());
            }

            return string.Format("KEY `{0}` (`{1}`)", schema.IndexName.Trim().ToLower(), 
                schema.Name.Trim().ToLower());
            
        }

        /// <summary>
        /// Gets the mysql native foreign key schema definition.
        /// </summary>
        /// <param name="schema">the canonical field schema to translate</param>
        internal static string GetForeignKeyDefinition(this DbFieldSchema schema)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            
            if (schema.IndexType != DbIndexType.ForeignKey)
                return string.Empty;

            return string.Format("CONSTRAINT `{0}` FOREIGN KEY `{0}`(`{1}`) REFERENCES `{2}`(`{3}`) ON DELETE {4} ON UPDATE {5}",
                schema.IndexName.Trim().ToLower(), schema.Name.Trim().ToLower(),
                schema.RefTable.Trim().ToLower(), schema.RefField.Trim().ToLower(),
                schema.OnDeleteForeignKey.GetNativeActionType(), schema.OnUpdateForeignKey.GetNativeActionType());

        }

        /// <summary>
        /// Gets a list of statements required to add a new index using the field schema specified.
        /// (also adds a foreign key if required)
        /// </summary>
        /// <param name="schema">a canonical database table field schema to apply</param>
        /// <param name="dbName">the database to add the index for</param>
        /// <param name="tblName">the database table to add the index for</param>
        internal static List<string> GetAddIndexStatements(this DbFieldSchema schema,
            string dbName, string tblName)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            if (dbName == null || string.IsNullOrEmpty(dbName.Trim())) throw new ArgumentNullException("dbName");
            if (tblName == null || string.IsNullOrEmpty(tblName.Trim())) throw new ArgumentNullException("tblName");

            if (schema.IndexType == DbIndexType.None || schema.IndexType == DbIndexType.Primary)
                return new List<string>();

            string result;

            if (schema.IndexType == DbIndexType.ForeignKey)
            {
                result = string.Format("ALTER TABLE `{0}`.`{1}` ADD CONSTRAINT `{2}` FOREIGN KEY `{2}`(`{3}`) REFERENCES `{4}`(`{5}`) ON DELETE {6} ON UPDATE {7};",
                    dbName, tblName.Trim().ToLower(), schema.IndexName.Trim().ToLower(), schema.Name.Trim().ToLower(),
                    schema.RefTable.Trim().ToLower(), schema.RefField.Trim().ToLower(),
                    schema.OnDeleteForeignKey.GetNativeActionType(), schema.OnUpdateForeignKey.GetNativeActionType());
            }
            else
            {
                var uniqueStr = "";
                if (schema.IndexType == DbIndexType.Unique)
                    uniqueStr = "UNIQUE ";

                result = string.Format("CREATE {0}INDEX `{1}` ON `{2}`.`{3}` (`{4}`);",
                    uniqueStr, schema.IndexName.Trim().ToLower(), dbName, tblName.Trim().ToLower(),
                    schema.Name.Trim().ToLower());

            } 

            return new List<string>(){result};

        }

        /// <summary>
        /// Gets a list of statements required to drop the index.(also drops a foreign key if required)
        /// </summary>
        /// <param name="schema">the field schema containing the index to drop</param>
        /// <param name="dbName">the database to drop the index for</param>
        /// <param name="tblName">the database table to drop the index for</param>
        internal static List<string> GetDropIndexStatements(this DbFieldSchema schema,
            string dbName, string tblName)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            if (dbName == null || string.IsNullOrEmpty(dbName.Trim())) throw new ArgumentNullException("dbName");
            if (tblName == null || string.IsNullOrEmpty(tblName.Trim())) throw new ArgumentNullException("tblName");

            if (schema.IndexType == DbIndexType.None || schema.IndexType == DbIndexType.Primary)
                return new List<string>();

            string result;

            if (schema.IndexType == DbIndexType.ForeignKey)
            {
                result = string.Format("ALTER TABLE `{0}`.`{1}` DROP FOREIGN KEY `{2}`;",
                    dbName, tblName.Trim().ToLower(), schema.IndexName.Trim().ToLower());
            }
            else
            {
                result = string.Format("DROP INDEX `{0}` ON `{1}`.`{2}`;",
                    schema.IndexName.Trim().ToLower(), dbName, tblName.Trim().ToLower());
            }

            return new List<string>(){result};

        }


        /// <summary>
        /// Gets a list of statements required to add a new database table using the schema specified.
        /// </summary>
        /// <param name="schema">a canonical schema of the new database table</param>
        /// <param name="dbName">the database to add the table for</param>
        /// <param name="engine">the SQL engine to use for the table</param>
        /// <param name="charset">the default charset for the table</param>
        internal static List<string> GetCreateTableStatements(this DbTableSchema schema, string dbName,
            string engine, string charset)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            if (dbName == null || string.IsNullOrEmpty(dbName.Trim())) throw new ArgumentNullException("dbName");

            var lines = schema.Fields.Select(field => field.GetFieldDefinition()).ToList();
            
            lines.AddRange(from field in schema.Fields 
                           where field.IndexType == DbIndexType.Simple 
                           || field.IndexType == DbIndexType.Unique 
                           || field.IndexType == DbIndexType.ForeignKey 
                           select field.GetIndexDefinition());

            lines.AddRange(from field in schema.Fields
                           where field.IndexType == DbIndexType.ForeignKey
                           select field.GetForeignKeyDefinition());

            return new List<string>()
                {
                    string.Format("CREATE TABLE {0}.{1}({2}) ENGINE={3}  DEFAULT CHARSET={4};",
                        dbName, schema.Name.Trim().ToLower(), string.Join(", ", lines.ToArray()),
                        engine, charset)
                };

        }

        /// <summary>
        /// Gets a list of statements required to drop the database table.
        /// </summary>
        /// <param name="schema">the table schema to drop</param>
        /// <param name="dbName">the database to drop the table for</param>
        internal static List<string> GetDropTableStatements(this DbTableSchema schema, string dbName)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            if (dbName == null || string.IsNullOrEmpty(dbName.Trim())) throw new ArgumentNullException("dbName");

            return new List<string>() { string.Format("DROP TABLE {0}.{1};", 
                dbName, schema.Name.Trim().ToLower()) };

        }

    }
}
