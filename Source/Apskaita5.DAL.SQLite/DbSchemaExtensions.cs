using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Apskaita5.DAL.Common.DbSchema;
using System.Data.SQLite;
using Apskaita5.DAL.Common;

namespace Apskaita5.DAL.SQLite
{
    internal static class DbSchemaExtensions
    {

        private const string DateTimeFormatString = "yyyy-MM-dd HH:mm:ss.fff" ;
        internal const string NativeActionType_Cascade = "CASCADE";
        internal const string NativeActionType_SetNull = "SET NULL";
        internal const string NativeActionType_Restrict = "RESTRICT";
        private const string NativeType_Float = "FLOAT";

        private const string NativeType_Real = "REAL";
        private const string NativeType_Double = "DOUBLE";
        private const string NativeType_Decimal = "DECIMAL";
        private const string NativeType_TinyInt = "TINYINT";
        private const string NativeType_SmallInt = "SMALLINT";
        private const string NativeType_MediumInt = "MEDIUMINT";
        private const string NativeType_Int = "INTEGER";
        private const string NativeType_Int_Alt = "INT";
        private const string NativeType_BigInt = "BIGINT";
        private const string NativeType_DateTime = "DATETIME";
        private const string NativeType_Date = "DATE";
        private const string NativeType_VarChar = "VARCHAR";
        private const string NativeType_Text = "TEXT";
        private const string NativeType_Blob = "BLOB";


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

            if (field1.IsNull()) throw new ArgumentNullException(nameof(field1));
            if (field2.IsNull()) throw new ArgumentNullException(nameof(field2));

            if (!field1.Name.EqualsByConvention(field2.Name))
                throw new ArgumentException(Properties.Resources.DbSchemaErrorExceptionFieldMismatch);

            if (!field1.DataType.IsEquivalentTo(field2.DataType))
                return false;
            if (field1.NotNull != field2.NotNull)
                return false;
            if (field1.DataType.IsDbDataTypeInteger() && field1.Autoincrement != field2.Autoincrement)
                return false;
            
            // primary and foreign keys are part of field signature in sqlite
            if (field1.IndexType != field2.IndexType && (field1.IndexType == DbIndexType.ForeignKey
                || field1.IndexType == DbIndexType.ForeignPrimary || field1.IndexType == DbIndexType.Primary
                || field2.IndexType == DbIndexType.ForeignKey || field2.IndexType == DbIndexType.ForeignPrimary
                || field2.IndexType == DbIndexType.Primary))
                return false;

            if ((field1.IndexType == DbIndexType.ForeignKey || field1.IndexType == DbIndexType.ForeignPrimary)
                && (!field1.RefField.EqualsByConvention(field2.RefField)
                || !field1.RefTable.EqualsByConvention(field2.RefTable)
                || field1.OnDeleteForeignKey != field2.OnDeleteForeignKey
                || field1.OnUpdateForeignKey != field2.OnUpdateForeignKey))
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

            if (field1.IsNull()) throw new ArgumentNullException(nameof(field1));
            if (field2.IsNull()) throw new ArgumentNullException(nameof(field2));

            if (!field1.Name.EqualsByConvention(field2.Name))
                throw new ArgumentException(Properties.Resources.DbSchemaErrorExceptionFieldMismatch);

            // primary and foreign keys are part of field signature in sqlite, i.e. checked with signature not with indexes
            if (field1.IndexType != field2.IndexType && (field1.IndexType == DbIndexType.ForeignKey
                || field1.IndexType == DbIndexType.ForeignPrimary || field1.IndexType == DbIndexType.Primary
                || field2.IndexType == DbIndexType.ForeignKey || field2.IndexType == DbIndexType.ForeignPrimary
                || field2.IndexType == DbIndexType.Primary))
                return true;

            return (field1.IndexType == field2.IndexType);

        }

        /// <summary>
        /// Gets the SQLite native name for the canonical data type for the field schema specified.
        /// </summary>
        /// <param name="schema">the field schema to return the native data type for</param>
        internal static string GetNativeDataType(this DbFieldSchema schema)
        {

            if (schema.IsNull()) throw new ArgumentNullException(nameof(schema));

            switch (schema.DataType)
            {
                case DbDataType.Float:
                    return NativeType_Float;
                case DbDataType.Real:
                    return NativeType_Real;
                case DbDataType.Double:
                    return NativeType_Double;
                case DbDataType.Decimal:
                    return NativeType_Decimal;
                case DbDataType.IntegerTiny:
                    return NativeType_TinyInt;
                case DbDataType.IntegerSmall:
                    return NativeType_SmallInt;
                case DbDataType.IntegerMedium:
                    return NativeType_MediumInt;
                case DbDataType.Integer:
                    return NativeType_Int;
                case DbDataType.IntegerBig:
                    return NativeType_BigInt;
                case DbDataType.TimeStamp:
                    return NativeType_DateTime;
                case DbDataType.Date:
                    return NativeType_Date;
                case DbDataType.DateTime:
                    return NativeType_DateTime;
                case DbDataType.Time:
                    return NativeType_DateTime;
                case DbDataType.Char:
                    return NativeType_VarChar;
                case DbDataType.VarChar:
                    return NativeType_VarChar;
                case DbDataType.Text:
                    return NativeType_Text;
                case DbDataType.TextMedium:
                    return NativeType_Text;
                case DbDataType.TextLong:
                    return NativeType_Text;
                case DbDataType.BlobTiny:
                    return NativeType_Blob;
                case DbDataType.Blob:
                    return NativeType_Blob;
                case DbDataType.BlobMedium:
                    return NativeType_Blob;
                case DbDataType.BlobLong:
                    return NativeType_Blob;
                case DbDataType.Enum:
                    return NativeType_Blob;
                default:
                    throw new NotImplementedException(string.Format(Properties.Resources.EnumValueNotImplementedException,
                        schema.DataType.ToString()));
            }

        }

        /// <summary>
        /// Gets a base field data type by it's native definition.
        /// </summary>
        /// <param name="definition">native definition of the data type</param>
        internal static DbDataType GetBaseDataType(this string definition)
        {

            var nativeName = definition ?? throw new ArgumentNullException(nameof(definition));
            if (nativeName.Contains("("))
                nativeName = nativeName.Substring(0, nativeName.IndexOf("(", StringComparison.Ordinal));
            if (nativeName.Contains(" "))
                nativeName = nativeName.Substring(0, nativeName.IndexOf(" ", StringComparison.Ordinal));
            nativeName = nativeName.Trim().ToUpper();

            switch (nativeName)
            {
                case NativeType_Blob:
                    return DbDataType.Blob;
                case NativeType_Date:
                    return DbDataType.Date;
                case NativeType_DateTime:
                    return DbDataType.DateTime;
                case NativeType_Decimal:
                    return DbDataType.Decimal;
                case NativeType_Double:
                    return DbDataType.Double;
                case NativeType_Float:
                    return DbDataType.Float;
                case NativeType_Int_Alt:
                    return DbDataType.Integer;
                case NativeType_Int:
                    return DbDataType.Integer;
                case NativeType_BigInt:
                    return DbDataType.IntegerBig;
                case NativeType_MediumInt:
                    return DbDataType.IntegerMedium;
                case NativeType_SmallInt:
                    return DbDataType.IntegerSmall;
                case NativeType_TinyInt:
                    return DbDataType.IntegerTiny;
                case NativeType_Real:
                    return DbDataType.Real;
                case NativeType_Text:
                    return DbDataType.Text;
                case NativeType_VarChar:
                    return DbDataType.VarChar;
                default:
                    throw new NotImplementedException(string.Format(
                        Properties.Resources.NativeTypeNotImplementedException, definition));
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
                    return NativeActionType_Restrict;
                case DbForeignKeyActionType.Cascade:
                    return NativeActionType_Cascade;
                case DbForeignKeyActionType.SetNull:
                    return NativeActionType_SetNull;
                default:
                    throw new NotImplementedException(string.Format(Properties.Resources.EnumValueNotImplementedException,
                        actionType.ToString()));
            }
        }

        private static string GetDefaultValueForFieldType(DbDataType fieldType, string enumValues)
        {

            switch (fieldType)
            {
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
                        throw new Exception(Properties.Resources.NoDefaultEnumValueException, ex);
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
                    throw new NotSupportedException(string.Format(Properties.Resources.CannotAddNewNotNullFieldException,
                        fieldType.ToString()));
            }

        }

        /// <summary>
        /// Gets the SQLite native field schema definition.
        /// </summary>
        /// <param name="schema">the canonical field schema to translate</param>
        /// <param name="addSafe">whether the definition should be safe for add field,
        /// i.e. dto add default value for not null fields</param>
        internal static string GetFieldDefinition(this DbFieldSchema schema, bool addSafe, SqlAgentBase agent)
        {

            if (schema.IsNull()) throw new ArgumentNullException(nameof(schema));
            if (agent.IsNull()) throw new ArgumentNullException(nameof(agent));

            if (schema.Autoincrement) schema.DataType = DbDataType.Integer;

            var result = string.Format("{0} {1}", schema.Name.ToConventional(agent), schema.GetNativeDataType());
            
            if ((schema.DataType == DbDataType.Char || schema.DataType == DbDataType.VarChar)
                && schema.Length > 0)
            {
                result = string.Format("{0}({1})", result, schema.Length.ToString(CultureInfo.InvariantCulture));
            }            

            if (schema.NotNull)
            {
                if (addSafe)
                {
                    result = string.Format("{0} DEFAULT {1} NOT NULL", result, 
                        GetDefaultValueForFieldType(schema.DataType, schema.EnumValues));
                }
                else
                {
                    result = string.Format("{0} NOT NULL", result);
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

            if (schema.IndexType == DbIndexType.ForeignKey || schema.IndexType == DbIndexType.ForeignPrimary)
                result = result + string.Format(" REFERENCES {0}({1}) ON UPDATE {2} ON DELETE {3}",
                    schema.RefTable.ToConventional(agent), schema.RefField.ToConventional(agent), 
                    schema.OnUpdateForeignKey.GetNativeActionType(),
                    schema.OnDeleteForeignKey.GetNativeActionType());

            return result;

        }

        /// <summary>
        /// Gets a list of statements required to add a new database table field using the field schema specified.
        /// (also fixes datetime defaults and adds index if required)
        /// </summary>
        /// <param name="schema">a canonical schema of the new database table field</param>
        /// <param name="tblName">the database table to add the field for</param>
        internal static List<string> GetAddFieldStatements(this DbFieldSchema schema, string tblName, SqlAgentBase agent)
        {

            if (schema.IsNull()) throw new ArgumentNullException(nameof(schema));
            if (agent.IsNull()) throw new ArgumentNullException(nameof(agent));
            if (tblName.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(tblName));

            var alterStatement = string.Format("ALTER TABLE {0} ADD COLUMN {1};",
                tblName.ToConventional(agent), schema.GetFieldDefinition(true, agent));

            var result = new List<string>(){ alterStatement };

            result.AddRange(schema.GetAddIndexStatements(tblName, agent));

            return result;

        }
                
        /// <summary>
        /// Gets a list of statements required to add a new index using the field schema specified.
        /// (also adds a foreign key if required)
        /// </summary>
        /// <param name="schema">a canonical database table field schema to apply</param>
        /// <param name="tblName">the database table to add the index for</param>
        internal static List<string> GetAddIndexStatements(this DbFieldSchema schema, string tblName, SqlAgentBase agent)
        {

            if (schema.IsNull()) throw new ArgumentNullException(nameof(schema));
            if (tblName.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(tblName));
            if (agent.IsNull()) throw new ArgumentNullException(nameof(agent));

            // No backing index for foreign key in sqlite
            if (schema.IndexType == DbIndexType.None || schema.IndexType == DbIndexType.Primary
                || schema.IndexType == DbIndexType.ForeignKey || schema.IndexType == DbIndexType.ForeignPrimary)
                return new List<string>();

            string result;

            if (schema.IndexType == DbIndexType.Simple)
            {
                result = string.Format("CREATE INDEX {0} ON {1}({2});", 
                    schema.IndexName.ToConventional(agent), tblName.ToConventional(agent), 
                    schema.Name.ToConventional(agent));
            }
            else
            {
                result = string.Format("CREATE UNIQUE INDEX {0} ON {1}({2});",
                    schema.IndexName.ToConventional(agent), tblName.ToConventional(agent), schema.Name.ToConventional(agent));
            }

            return new List<string>() { result };

        }

        /// <summary>
        /// Gets a list of statements required to drop the index.(also drops a foreign key if required)
        /// </summary>
        /// <param name="schema">the field schema containing the index to drop</param>
        internal static List<string> GetDropIndexStatements(this DbFieldSchema schema, SqlAgentBase agent)
        {

            if (schema.IsNull()) throw new ArgumentNullException(nameof(schema));
            if (agent.IsNull()) throw new ArgumentNullException(nameof(agent));

            if (schema.IndexType != DbIndexType.Simple && schema.IndexType != DbIndexType.Unique)
                return new List<string>();

            return new List<string>() { string.Format("DROP INDEX {0};", schema.IndexName.ToConventional(agent)) };

        }


        /// <summary>
        /// Gets a list of statements required to add a new database table using the schema specified.
        /// </summary>
        /// <param name="schema">a canonical schema of the new database table</param>
        internal static List<string> GetCreateTableStatements(this DbTableSchema schema, SqlAgentBase agent)
        {

            if (schema.IsNull()) throw new ArgumentNullException(nameof(schema));
            if (agent.IsNull()) throw new ArgumentNullException(nameof(agent));

            var lines = schema.Fields.Select(field => field.GetFieldDefinition(false, agent)).ToArray();
            
            var result = new List<string>()
                {
                    string.Format("CREATE TABLE {0}({1});",
                        schema.Name.ToConventional(agent), string.Join(", ", lines))
                };

            foreach (var field in schema.Fields)
            {
                if (field.IndexType == DbIndexType.Simple || field.IndexType == DbIndexType.Unique)
                    result.AddRange(field.GetAddIndexStatements(schema.Name, agent));
            }

            return result;

        }

        /// <summary>
        /// Gets a list of statements required to drop the database table.
        /// </summary>
        /// <param name="schema">the table schema to drop</param>
        internal static List<string> GetDropTableStatements(this DbTableSchema schema, SqlAgentBase agent)
        {

            if (schema.IsNull()) throw new ArgumentNullException(nameof(schema));
            if (agent.IsNull()) throw new ArgumentNullException(nameof(agent));

            return new List<string>() { string.Format("DROP TABLE {0};", schema.Name.ToConventional(agent)) };

        }

    }
}
