﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Apskaita5.DAL.SQLite.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Apskaita5.DAL.SQLite.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot add new NOT NULL column of type {0} because {0} type does not have default value..
        /// </summary>
        internal static string CannotAddNewNotNullFieldException {
            get {
                return ResourceManager.GetString("CannotAddNewNotNullFieldException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot open SQLite connection without a database specified..
        /// </summary>
        internal static string CannotOpenConnectionException {
            get {
                return ResourceManager.GetString("CannotOpenConnectionException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create a transaction while another transaction is in progress..
        /// </summary>
        internal static string CannotStartTransactionException {
            get {
                return ResourceManager.GetString("CannotStartTransactionException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create a database while there is a transaction in progress..
        /// </summary>
        internal static string CreateDatabaseExceptionTransactionInProgress {
            get {
                return ResourceManager.GetString("CreateDatabaseExceptionTransactionInProgress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database schema should contain at least one table..
        /// </summary>
        internal static string DatabaseCreateExceptionNoTables {
            get {
                return ResourceManager.GetString("DatabaseCreateExceptionNoTables", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot compare fields with diferent names..
        /// </summary>
        internal static string DbSchemaErrorExceptionFieldMismatch {
            get {
                return ResourceManager.GetString("DbSchemaErrorExceptionFieldMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot get schema errors while a transaction is in progress..
        /// </summary>
        internal static string DbSchemaErrorsExceptionCannotGetInTransaction {
            get {
                return ResourceManager.GetString("DbSchemaErrorsExceptionCannotGetInTransaction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot get schema while a transaction is in progress..
        /// </summary>
        internal static string DbSchemaExceptionCannotGetInTransaction {
            get {
                return ResourceManager.GetString("DbSchemaExceptionCannotGetInTransaction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database is not set, cannot get schema..
        /// </summary>
        internal static string DbSchemaExceptionDatabaseNull {
            get {
                return ResourceManager.GetString("DbSchemaExceptionDatabaseNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command DisableForeignKeysForCurrentTransactionAsync can only be invoked within an active transaction..
        /// </summary>
        internal static string DisableForeignKeysExceptionTransactionNull {
            get {
                return ResourceManager.GetString("DisableForeignKeysExceptionTransactionNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot drop database while transaction is in progress..
        /// </summary>
        internal static string DropDatabaseExceptionTransactionInProgress {
            get {
                return ResourceManager.GetString("DropDatabaseExceptionTransactionInProgress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enum value {0} is not implemented..
        /// </summary>
        internal static string EnumValueNotImplementedException {
            get {
                return ResourceManager.GetString("EnumValueNotImplementedException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Field {0} in table {1} missing..
        /// </summary>
        internal static string FieldMissingErrorDescription {
            get {
                return ResourceManager.GetString("FieldMissingErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Field {0} in table {1} missing. Cannot add new NOT NULL column for BLOB type in SQLite..
        /// </summary>
        internal static string FieldMissingUnreparableErrorDescription {
            get {
                return ResourceManager.GetString("FieldMissingUnreparableErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Table {0} field {1} schema definition is obsolete: current definition - {2}; gauge definition - {3}. SQLite does not support field schema changes..
        /// </summary>
        internal static string FieldObsoleteErrorDescription {
            get {
                return ResourceManager.GetString("FieldObsoleteErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Field {0} in table {1} is redundant. Change is not supported by SQLite..
        /// </summary>
        internal static string FieldRedundantUnreparableErrorDescription {
            get {
                return ResourceManager.GetString("FieldRedundantUnreparableErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fields cannot be empty..
        /// </summary>
        internal static string FieldsEmptyException {
            get {
                return ResourceManager.GetString("FieldsEmptyException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Index {0} missing on table {1} field {2}..
        /// </summary>
        internal static string IndexMissingErrorDescription {
            get {
                return ResourceManager.GetString("IndexMissingErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Index {0} type is obsolete on table {1} field {2}..
        /// </summary>
        internal static string IndexObsoleteErrorDescription {
            get {
                return ResourceManager.GetString("IndexObsoleteErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Index {0} type is obsolete on table {1} field {2}. Change is not supported by SQLite..
        /// </summary>
        internal static string IndexObsoleteUnreparableErrorDescription {
            get {
                return ResourceManager.GetString("IndexObsoleteUnreparableErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Index {0} type is redundant on table {1} field {2}..
        /// </summary>
        internal static string IndexRedundantErrorDescription {
            get {
                return ResourceManager.GetString("IndexRedundantErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generic parameter type {0} is not supported by SQLiteAgent.ExecuteCommandInt..
        /// </summary>
        internal static string InvalidInternalExecuteParamException {
            get {
                return ResourceManager.GetString("InvalidInternalExecuteParamException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type {0} is not a valid SQLite transaction..
        /// </summary>
        internal static string InvalidTransactionTypeException {
            get {
                return ResourceManager.GetString("InvalidTransactionTypeException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Manual rollback..
        /// </summary>
        internal static string ManualRollbackException {
            get {
                return ResourceManager.GetString("ManualRollbackException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SQLite database data type {0} is unknown..
        /// </summary>
        internal static string NativeTypeNotImplementedException {
            get {
                return ResourceManager.GetString("NativeTypeNotImplementedException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot execute batch while a transaction is in progress..
        /// </summary>
        internal static string NoBatchInTransactionException {
            get {
                return ResourceManager.GetString("NoBatchInTransactionException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to find the default enum value..
        /// </summary>
        internal static string NoDefaultEnumValueException {
            get {
                return ResourceManager.GetString("NoDefaultEnumValueException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception of type &apos;{0}&apos;, message: {1}.
        /// </summary>
        internal static string NonSqlExceptionDescription {
            get {
                return ResourceManager.GetString("NonSqlExceptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No transactiion to commit..
        /// </summary>
        internal static string NoTransactionToCommitException {
            get {
                return ResourceManager.GetString("NoTransactionToCommitException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Query token cannot be empty..
        /// </summary>
        internal static string QueryTokenEmptyException {
            get {
                return ResourceManager.GetString("QueryTokenEmptyException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SQLite database file {0} not found: {1}.
        /// </summary>
        internal static string SqlExceptionDatabaseNotFound {
            get {
                return ResourceManager.GetString("SqlExceptionDatabaseNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SQLite returned an exception: ErrorCode={0}; HResult={1}; Message=&apos;{2}&apos;.
        /// </summary>
        internal static string SqlExceptionMessage {
            get {
                return ResourceManager.GetString("SqlExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WARNING. Critical SQL transaction error, failed to rollback the transaction.{4}SQLite returned an exception: ErrorCode={0}; HResult={1}; Message=&apos;{2}&apos;{3}Initial exception that caused the rollback:{3}{4}{3}SQL statement/query that caused the exception:{3}{5}.
        /// </summary>
        internal static string SqlExceptionMessageRollbackFailed {
            get {
                return ResourceManager.GetString("SqlExceptionMessageRollbackFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SQLite returned an exception: ErrorCode={0}; HResult={1}; Message=&apos;{2}&apos;{3}SQL statement/query that caused the exception:{3}{4} .
        /// </summary>
        internal static string SqlExceptionMessageWithStatement {
            get {
                return ResourceManager.GetString("SqlExceptionMessageWithStatement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password is invalid or SQLite file corrupted..
        /// </summary>
        internal static string SqlExceptionPasswordInvalid {
            get {
                return ResourceManager.GetString("SqlExceptionPasswordInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown exception occured while opening SQLite connection: {0}.
        /// </summary>
        internal static string SqlExceptionUnknownException {
            get {
                return ResourceManager.GetString("SqlExceptionUnknownException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to At least one statement should be non empty..
        /// </summary>
        internal static string StatementsEmptyException {
            get {
                return ResourceManager.GetString("StatementsEmptyException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Table {0} missing..
        /// </summary>
        internal static string TableMissingErrorDescription {
            get {
                return ResourceManager.GetString("TableMissingErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Table {0} is obsolete..
        /// </summary>
        internal static string TableObsoleteErrorDescription {
            get {
                return ResourceManager.GetString("TableObsoleteErrorDescription", resourceCulture);
            }
        }
    }
}
