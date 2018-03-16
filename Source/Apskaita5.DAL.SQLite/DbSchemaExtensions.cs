using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Apskaita5.DAL.Common;

namespace Apskaita5.DAL.Sqlite
{
    internal static class DbSchemaExtensions
    {

        private const string DateTimeFormatString = "yyyy-MM-dd HH:mm:ss.fff" ;


        /// <summary>
        /// Returns true if the canonical DbDataTypes specified are equivalent for the SQLite engine,
        /// otherwise - returns false.
        /// </summary>
        /// <param name="type1">the first type to compare</param>
        /// <param name="type2">the second type to compare</param>
        internal static bool IsEquivalentTo(this DbDataType type1, DbDataType type2)
        {
            return (ToBaseType(type1) == ToBaseType(type2));
        }

        internal static DbDataType ToBaseType(this DbDataType fieldType)
        {

            switch (fieldType)
            {
                case DbDataType.Blob:
                    return DbDataType.Blob;
                case DbDataType.BlobLong:
                    return DbDataType.Blob;
                case DbDataType.BlobMedium:
                    return DbDataType.Blob;
                case DbDataType.BlobTiny:
                    return DbDataType.Blob;
                case DbDataType.Char:
                    return DbDataType.Text;
                case DbDataType.Date:
                    return DbDataType.Date;
                case DbDataType.DateTime:
                    return DbDataType.Date;
                case DbDataType.Decimal:
                    return DbDataType.Decimal;
                case DbDataType.Double:
                    return DbDataType.Double;
                case DbDataType.Enum:
                    return DbDataType.Text;
                case DbDataType.Float:
                    return DbDataType.Double;
                case DbDataType.Integer:
                    return DbDataType.Integer;
                case DbDataType.IntegerBig:
                    return DbDataType.Integer;
                case DbDataType.IntegerMedium:
                    return DbDataType.Integer;
                case DbDataType.IntegerSmall:
                    return DbDataType.Integer;
                case DbDataType.IntegerTiny:
                    return DbDataType.Integer;
                case DbDataType.Real:
                    return DbDataType.Double;
                case DbDataType.Text:
                    return DbDataType.Text;
                case DbDataType.TextLong:
                    return DbDataType.Text;
                case DbDataType.TextMedium:
                    return DbDataType.Text;
                case DbDataType.Time:
                    return DbDataType.Date;
                case DbDataType.TimeStamp:
                    return DbDataType.Date;
                case DbDataType.VarChar:
                    return DbDataType.Text;
                default:
                    return DbDataType.Blob;
            }


        }

        /// <summary>
        /// Returns true if the schemas of the specified fields are equivalent for the SQLite engine,
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
            if (field1.DataType.IsDbDataTypeInteger() && field1.Autoincrement != field2.Autoincrement)
                return false;
            if ((field1.IndexType == DbIndexType.Primary && field2.IndexType != DbIndexType.Primary)
                || (field1.IndexType != DbIndexType.Primary && field2.IndexType == DbIndexType.Primary))
                return false;
            
            return true;

        }

        /// <summary>
        /// Returns true if the indexes of the specified fields are equivalent for the SQLite engine,
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
        /// Gets the SQLite native name for the canonical data type for the field schema specified.
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
                    return "INTEGER";
                case DbDataType.IntegerBig:
                    return "BIGINT";
                case DbDataType.TimeStamp:
                    return "DATETIME";
                case DbDataType.Date:
                    return "DATE";
                case DbDataType.DateTime:
                    return "DATETIME";
                case DbDataType.Time:
                    return "DATETIME";
                case DbDataType.Char:
                    return "VARCHAR";
                case DbDataType.VarChar:
                    return "VARCHAR";
                case DbDataType.Text:
                    return "TEXT";
                case DbDataType.TextMedium:
                    return "TEXT";
                case DbDataType.TextLong:
                    return "TEXT";
                case DbDataType.BlobTiny:
                    return "BLOB";
                case DbDataType.Blob:
                    return "BLOB";
                case DbDataType.BlobMedium:
                    return "BLOB";
                case DbDataType.BlobLong:
                    return "BLOB";
                case DbDataType.Enum:
                    return "VARCHAR";
                default:
                    throw new NotImplementedException(string.Format("Enum value {0} is not implemented.",
                        schema.DataType.ToString()));
            }

        }

        /// <summary>
        /// Gets the SQLite native foreign index change action type.
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

        private static string GetDefaultValueForFieldType(DbDataType fieldType, string enumValues)
        {

            switch (fieldType)
            {
                case DbDataType.Blob:
                    throw new NotSupportedException("Cannot add new NOT NULL column of type " + "BLOB because BLOB type does not have default value.");
                case DbDataType.BlobLong:
                    throw new NotSupportedException("Cannot add new NOT NULL column of type " + "BLOB because BLOB type does not have default value.");
                case DbDataType.BlobMedium:
                    throw new NotSupportedException("Cannot add new NOT NULL column of type " + "BLOB because BLOB type does not have default value.");
                case DbDataType.BlobTiny:
                    throw new NotSupportedException("Cannot add new NOT NULL column of type " + "BLOB because BLOB type does not have default value.");
                case DbDataType.Char:
                    return "''";
                case DbDataType.Date:
                    return "'" + DateTime.Now.Date.ToString(DateTimeFormatString) + "'";
                case DbDataType.DateTime:
                    return "'" + DateTime.Now.ToString(DateTimeFormatString) + "'";
                case DbDataType.Decimal:
                    return "0";
                case DbDataType.Double:
                    return "0";
                case DbDataType.Enum:
                    try
                    {
                        return "'" + enumValues.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0] + "'";
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to find the default enum value.", ex);
                    }
                case DbDataType.Float:
                    return "0";
                case DbDataType.Integer:
                    return "0";
                case DbDataType.IntegerBig:
                    return "0";
                case DbDataType.IntegerMedium:
                    return "0";
                case DbDataType.IntegerSmall:
                    return "0";
                case DbDataType.IntegerTiny:
                    return "0";
                case DbDataType.Real:
                    return "0";
                case DbDataType.Text:
                    return "''";
                case DbDataType.TextLong:
                    return "''";
                case DbDataType.TextMedium:
                    return "''";
                case DbDataType.Time:
                    return "'" + DateTime.Now.ToString(DateTimeFormatString) + "'";
                case DbDataType.TimeStamp:
                    return "'" + DateTime.Now.ToString(DateTimeFormatString) + "'";
                case DbDataType.VarChar:
                    return "''";
                default:
                    throw new NotSupportedException(string.Format("Cannot add new NOT NULL column of type {0} because this type does not have a default value.",
                        fieldType.ToString()));
            }

        }

        /// <summary>
        /// Gets the SQLite native field schema definition.
        /// </summary>
        /// <param name="schema">the canonical field schema to translate</param>
        /// <param name="addSafe">whether the definition should be safe for add field,
        /// i.e. dto add default value for not null fields</param>
        internal static string GetFieldDefinition(this DbFieldSchema schema, bool addSafe)
        {

            if (schema == null) throw new ArgumentNullException("schema");

            if (schema.Autoincrement) schema.DataType = DbDataType.Integer;

            var result = schema.Name.Trim().ToLower() + " " + schema.GetNativeDataType();

            if ((schema.DataType == DbDataType.Char || schema.DataType == DbDataType.VarChar)
                && schema.Length > 0)
            {
                result = result + "(" + schema.Length.ToString(CultureInfo.InvariantCulture) + ")";
            }
            

            if (schema.NotNull)
            {
                if (addSafe)
                {
                    result = result + " DEFAULT " + GetDefaultValueForFieldType(schema.DataType, 
                        schema.EnumValues) + " NOT NULL";
                }
                else
                {
                    result = result + " NOT NULL";
                }
            }

            if (schema.IndexType == DbIndexType.Primary)
            {
                if (schema.Autoincrement)
                {
                    result = result + " PRIMARY KEY AUTOINCREMENT";
                }
                else
                {
                    result = result + " PRIMARY KEY";
                }
            }

            return result;

        }

        /// <summary>
        /// Gets a list of statements required to add a new database table field using the field schema specified.
        /// (also fixes datetime defaults and adds index if required)
        /// </summary>
        /// <param name="schema">a canonical schema of the new database table field</param>
        /// <param name="tblName">the database table to add the field for</param>
        internal static List<string> GetAddFieldStatements(this DbFieldSchema schema, string tblName)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            if (tblName == null || string.IsNullOrEmpty(tblName.Trim())) throw new ArgumentNullException("tblName");

            var alterStatement = string.Format("ALTER TABLE {0} ADD COLUMN {1}",
                tblName, schema.GetFieldDefinition(true));
            if (schema.IndexType == DbIndexType.ForeignKey)
                alterStatement = alterStatement + string.Format(" CONSTRAINT {0} REFERENCES {1}({2}) ON UPDATE {3} ON DELETE {4}", 
                    schema.IndexName.Trim().ToLower(), schema.RefTable.Trim().ToLower(), 
                    schema.RefField.Trim().ToLower(), schema.OnUpdateForeignKey.GetNativeActionType(), 
                    schema.OnDeleteForeignKey.GetNativeActionType());
            alterStatement = alterStatement + ";";

            var result = new List<string>(){ alterStatement };

            result.AddRange(schema.GetAddIndexStatements(tblName));

            return result;

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

            return string.Format("CONSTRAINT {0} FOREIGN KEY({1}) REFERENCES {2}({3}) ON DELETE {4} ON UPDATE {5}",
                schema.IndexName.Trim().ToLower(), schema.Name.Trim().ToLower(),
                schema.RefTable.Trim().ToLower(), schema.RefField.Trim().ToLower(),
                schema.OnDeleteForeignKey.GetNativeActionType(), schema.OnUpdateForeignKey.GetNativeActionType());

        }

        /// <summary>
        /// Gets a list of statements required to add a new index using the field schema specified.
        /// (also adds a foreign key if required)
        /// </summary>
        /// <param name="schema">a canonical database table field schema to apply</param>
        /// <param name="tblName">the database table to add the index for</param>
        internal static List<string> GetAddIndexStatements(this DbFieldSchema schema, string tblName)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            if (tblName == null || string.IsNullOrEmpty(tblName.Trim())) 
                throw new ArgumentNullException("tblName");

            if (schema.IndexType == DbIndexType.None || schema.IndexType == DbIndexType.Primary)
                return new List<string>();

            string result;

            if (schema.IndexType == DbIndexType.Simple)
            {
                result = string.Format("CREATE INDEX {0} ON {1}({2});", 
                    schema.IndexName.Trim().ToLower(), tblName.Trim().ToLower(), schema.Name.Trim().ToLower());
            }
            else if (schema.IndexType == DbIndexType.Unique)
            {
                result = string.Format("CREATE UNIQUE INDEX {0} ON {1}({2});",
                    schema.IndexName.Trim().ToLower(), tblName.Trim().ToLower(), schema.Name.Trim().ToLower());
            }
            else // backing foreign key index
            {
                result = string.Format("CREATE INDEX {0} ON {1}({2});",
                    schema.IndexName.Trim().ToLower() + "_fkindex", 
                    tblName.Trim().ToLower(), schema.Name.Trim().ToLower());
            }

            return new List<string>() { result };

        }

        /// <summary>
        /// Gets a list of statements required to drop the index.(also drops a foreign key if required)
        /// </summary>
        /// <param name="schema">the field schema containing the index to drop</param>
        internal static List<string> GetDropIndexStatements(this DbFieldSchema schema)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            
            if (schema.IndexType != DbIndexType.Simple && schema.IndexType != DbIndexType.Unique)
                return new List<string>();

            return new List<string>() { string.Format("DROP INDEX {0};", schema.IndexName.Trim().ToLower()) };

        }


        /// <summary>
        /// Gets a list of statements required to add a new database table using the schema specified.
        /// </summary>
        /// <param name="schema">a canonical schema of the new database table</param>
        internal static List<string> GetCreateTableStatements(this DbTableSchema schema)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            
            var lines = schema.Fields.Select(field => field.GetFieldDefinition(false)).ToList();

            lines.AddRange(from field in schema.Fields
                           where field.IndexType == DbIndexType.ForeignKey
                           select field.GetForeignKeyDefinition());

            var result = new List<string>()
                {
                    string.Format("CREATE TABLE {0}({1});",
                        schema.Name.Trim().ToLower(), string.Join(", ", lines.ToArray()))
                };

            foreach (var field in schema.Fields)
            {
                if (field.IndexType == DbIndexType.Simple || field.IndexType == DbIndexType.Unique)
                    result.AddRange(field.GetAddIndexStatements(schema.Name));
            }

            return result;

        }

        /// <summary>
        /// Gets a list of statements required to drop the database table.
        /// </summary>
        /// <param name="schema">the table schema to drop</param>
        internal static List<string> GetDropTableStatements(this DbTableSchema schema)
        {

            if (schema == null) throw new ArgumentNullException("schema");
            
            return new List<string>() { string.Format("DROP TABLE {0};", 
                schema.Name.Trim().ToLower()) };

        }

    }
}
