using System;
using System.Collections.Generic;
using System.Linq;
using Apskaita5.DAL.Common.DbSchema;

namespace DeveloperUtils.TestClasses
{
    static class DbSchemaExtensions
    {

        private const string _letters = "abcdefghijklmnoqprstuvzwx";


        public static string ToClass(this DbTableSchema table)
        {

            var fields = string.Join(Environment.NewLine, table.Fields.Select(f => f.ToFieldDefinition()).ToArray());
            var props = string.Join(Environment.NewLine + Environment.NewLine, table.Fields.Select(f => f.ToPropertyDefinition()).ToArray());
            
            return string.Format(@"
            class {0} : BaseClass<{0}>
            {{

                {1}


                {2}


                {3}                

            }}", table.Name.ToPropName(), table.ToOrmMapSection(), fields, props);

        }


        private static string ToFieldDefinition(this DbFieldSchema field)
        {
            return string.Format("private {0} {1};", field.ToFieldType(), field.Name.ToFieldName());
        }

        private static string ToPropertyDefinition(this DbFieldSchema field)
        {

            if (field.IndexType == DbIndexType.Primary || field.IndexType == DbIndexType.ForeignPrimary)
            {
                if (field.Autoincrement)
                {
                    return string.Format(@"public {0} {1} {{ get {{ return {2}; }} }}", field.ToFieldType(), 
                        field.Name.ToPropName(), field.Name.ToFieldName());
                }
                else
                {
                    return string.Format(@"public {0} {1} {{ get {{ return {2}; }} set {{ {2} = value; }} }}", 
                        field.ToFieldType(), field.Name.ToPropName(), field.Name.ToFieldName());
                }
            }
                

            return string.Format(@"public {0} {1} {{ get {{ return {2}; }} set {{ {2} = value; }} }}", 
                field.ToFieldType(), field.Name.ToPropName(), field.Name.ToFieldName());
        }

        private static string ToOrmMapSection(this DbTableSchema table)
        {
            var result = new List<string>
            {
                table.ToOrmIdentityMap()
            };
            foreach (var f in table.Fields)
                if (f.IndexType != DbIndexType.ForeignPrimary && f.IndexType != DbIndexType.Primary)
                    result.Add(f.ToOrmFieldMap(table.Name));
            return string.Join(Environment.NewLine, result.ToArray());
        }

        private static string ToOrmIdentityMap(this DbTableSchema table)
        {
            var pk = table.PrimaryKey();
            var pk_name = pk?.Name.Trim() ?? string.Empty;
            var pk_field = pk_name.ToFieldName();
            var pk_prop = pk_name.ToPropName();
            return string.Format(@"private static OrmIdentityMap<{0}> _identityMap = new OrmIdentityMap<{0}>(""{1}"", ""{2}"", 
                nameof({3}), {6}, a => a.{4}, (a, dr) => a.{5}); ",
                table.Name.ToPropName(), table.Name.Trim(), pk_name,
                pk_prop, pk_field, pk?.ToLoaderDefinition(), pk.Autoincrement ? "true" : "false");
        }

        private static string ToOrmFieldMap(this DbFieldSchema field, string tableName)
        {
           return string.Format(@"private static OrmFieldMap<{0}> {1}Map = new OrmFieldMap<{0}>(
                ""{2}"", nameof({3}), typeof({4}), v => v.{1}, (a, dr) => a.{5});",
                tableName.ToPropName(), field.Name.ToFieldName(), field.Name.Trim(),
                field.Name.ToPropName(), field.ToFieldType(), field.ToLoaderDefinition()); 
        }

        private static string ToLoaderDefinition(this DbFieldSchema field)
        {

            var result = "undefined";
            var dbType = field.DataType;

            if (field.NotNull)
            {
                if (dbType == DbDataType.Blob || dbType == DbDataType.BlobLong || dbType == DbDataType.BlobMedium
                    || dbType == DbDataType.BlobTiny) result = "{0} = dr.GetByteArray(nameof({1}))";
                if (dbType == DbDataType.Char && 32 == field.Length) result = "{0} = dr.GetGuid(nameof({1}))";
                if (dbType == DbDataType.Char || dbType == DbDataType.VarChar || dbType == DbDataType.Text
                    || dbType == DbDataType.TextLong || dbType == DbDataType.TextMedium) result = "{0} = dr.GetString(nameof({1}))";
                if (dbType == DbDataType.Date || dbType == DbDataType.DateTime || dbType == DbDataType.Time
                    || dbType == DbDataType.TimeStamp) result = "{0} = dr.GetDateTime(nameof({1}))";
                if (dbType == DbDataType.Decimal) result = "{0} = dr.GetDecimal(nameof({1}))";
                if (dbType == DbDataType.Double || dbType == DbDataType.Float || dbType == DbDataType.Real)
                    result = "{0} = dr.GetDouble(nameof({1}))";
                if (dbType == DbDataType.Integer || dbType == DbDataType.IntegerMedium || dbType == DbDataType.IntegerSmall)
                    result = "{0} = dr.GetInt32(nameof({1}))";
                if (dbType == DbDataType.IntegerTiny) result = "{0} = dr.GetBoolean(nameof({1}))";
                if (dbType == DbDataType.Enum) result = "{0} = dr.GetString(nameof({1}))";
                if (dbType == DbDataType.IntegerBig) result = "{0} = dr.GetInt64(nameof({1}))";
            }
            else
            {
                if (dbType == DbDataType.Blob || dbType == DbDataType.BlobLong || dbType == DbDataType.BlobMedium
                    || dbType == DbDataType.BlobTiny) result = "{0} = dr.GetByteArray(nameof({1}))";
                if (dbType == DbDataType.Char && 32 == field.Length) result = "{0} = dr.GetGuidNullable(nameof({1}))";
                if (dbType == DbDataType.Char || dbType == DbDataType.VarChar || dbType == DbDataType.Text
                    || dbType == DbDataType.TextLong || dbType == DbDataType.TextMedium) result = "{0} = dr.GetStringOrDefault(nameof({1}))";
                if (dbType == DbDataType.Date || dbType == DbDataType.DateTime || dbType == DbDataType.Time
                    || dbType == DbDataType.TimeStamp) result = "{0} = dr.GetDateTimeNullable(nameof({1}))";
                if (dbType == DbDataType.Decimal) result = "{0} = dr.GetDecimalNullable(nameof({1}))";
                if (dbType == DbDataType.Double || dbType == DbDataType.Float || dbType == DbDataType.Real)
                    result = "{0} = dr.GetDoubleNullable(nameof({1}))";
                if (dbType == DbDataType.Integer || dbType == DbDataType.IntegerMedium || dbType == DbDataType.IntegerSmall)
                    result = "{0} = dr.GetInt32Nullable(nameof({1}))";
                if (dbType == DbDataType.IntegerTiny) result = "{0} = dr.GetBooleanNullable(nameof({1}))";
                if (dbType == DbDataType.Enum) result = "{0} = dr.GetStringOrDefault(nameof({1}))";
                if (dbType == DbDataType.IntegerBig) result = "{0} = dr.GetInt64Nullable(nameof({1}))";
            }            

            return string.Format(result, field.Name.ToFieldName(), field.Name.ToPropName());

        }



        private static string ToFieldType(this DbFieldSchema field)
        {
            if (field.DataType == DbDataType.Blob || field.DataType == DbDataType.BlobLong 
                || field.DataType == DbDataType.BlobMedium || field.DataType == DbDataType.BlobTiny) return "byte[]";

            var res = string.Empty;

            if (field.DataType == DbDataType.Char && field.Length == 32) res = "Guid";
            if (field.DataType == DbDataType.Char || field.DataType == DbDataType.VarChar 
                || field.DataType == DbDataType.Text || field.DataType == DbDataType.TextLong 
                || field.DataType == DbDataType.TextMedium) return "string";
            if (field.DataType == DbDataType.Date || field.DataType == DbDataType.DateTime 
                || field.DataType == DbDataType.Time || field.DataType == DbDataType.TimeStamp) res = "DateTime";
            if (field.DataType == DbDataType.Decimal) res = "decimal";
            if (field.DataType == DbDataType.Double || field.DataType == DbDataType.Float 
                || field.DataType == DbDataType.Real) res = "double";
            if (field.DataType == DbDataType.Integer || field.DataType == DbDataType.IntegerMedium 
                || field.DataType == DbDataType.IntegerSmall) res = "int";
            if (field.DataType == DbDataType.Enum) res = "string";
            if (field.DataType == DbDataType.IntegerTiny) res = "bool";
            if (field.DataType == DbDataType.IntegerBig) res = "long";

            if (string.Empty == res) return "undefined";

            if (field.NotNull) return res;

            return res + "?";

        }

        private static string ToPropName(this string dbFieldName)
        {
            var result = dbFieldName.Trim();
            foreach (var letter in _letters)
            {
                var str = letter.ToString();
                result = result.Replace("_" + str, str.ToUpper());
            }
            return result.First().ToString().ToUpper() + result.Substring(1);
        }

        private static string ToFieldName(this string dbFieldName)
        {
            var result = dbFieldName.Trim();
            foreach (var letter in _letters)
            {
                var str = letter.ToString();
                result = result.Replace("_" + str, str.ToUpper());
            }
            return "_" + result;
        }

        private static DbFieldSchema PrimaryKey(this DbTableSchema table)
        {
            return table.Fields.FirstOrDefault(f => f.IndexType == DbIndexType.ForeignPrimary
                || f.IndexType == DbIndexType.Primary);
        }

    }
}
