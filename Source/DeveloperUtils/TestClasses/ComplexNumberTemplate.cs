using System;
using Apskaita5.DAL.Common.MicroOrm;

namespace DeveloperUtils.TestClasses
{
    class ComplexNumberTemplate : BaseClass<ComplexNumberTemplate>
    {

        private static OrmIdentityMap<ComplexNumberTemplate> _identityMap = new OrmIdentityMap<ComplexNumberTemplate>(
            "complex_number_templates", "id", nameof(Id), true, a => a._id, (a, dr) => a._id = dr.GetInt32(nameof(Id)), 
            (a, pk) => a._id = (int)pk);
        private static OrmFieldMap<ComplexNumberTemplate> _documentTypeMap = new OrmFieldMap<ComplexNumberTemplate>(
            "document_type", nameof(DocumentType), typeof(int), v => v._documentType, 
            (a, dr) => a._documentType = dr.GetInt32(nameof(DocumentType)));
        private static OrmFieldMap<ComplexNumberTemplate> _extendedDocumentTypeIdMap = new OrmFieldMap<ComplexNumberTemplate>(
            "extended_document_type_id", nameof(ExtendedDocumentTypeId), typeof(Guid?), v => v._extendedDocumentTypeId, 
            (a, dr) => a._extendedDocumentTypeId = dr.GetGuidNullable(nameof(ExtendedDocumentTypeId)));
        private static OrmFieldMap<ComplexNumberTemplate> _serialMap = new OrmFieldMap<ComplexNumberTemplate>(
            "serial", nameof(Serial), typeof(string), v => v._serial, (a, dr) => a._serial = dr.GetString(nameof(Serial)));
        private static OrmFieldMap<ComplexNumberTemplate> _resetPolicyMap = new OrmFieldMap<ComplexNumberTemplate>(
            "reset_policy", nameof(ResetPolicy), typeof(string), v => v._resetPolicy.ToString(), 
            (a, dr) => a._resetPolicy = dr.GetEnum<ResetPolicyType>(nameof(ResetPolicy)));
        private static OrmFieldMap<ComplexNumberTemplate> _formatStringMap = new OrmFieldMap<ComplexNumberTemplate>(
            "format_string", nameof(FormatString), typeof(string), v => v._formatString, 
            (a, dr) => a._formatString = dr.GetString(nameof(FormatString)));
        private static OrmFieldMap<ComplexNumberTemplate> _hasExternalProviderMap = new OrmFieldMap<ComplexNumberTemplate>(
            "has_external_provider", nameof(HasExternalProvider), typeof(bool), v => v._hasExternalProvider, 
            (a, dr) => a._hasExternalProvider = dr.GetBoolean(nameof(HasExternalProvider)));
        private static OrmFieldMap<ComplexNumberTemplate> _isArchivedMap = new OrmFieldMap<ComplexNumberTemplate>(
            "is_archived", nameof(IsArchived), typeof(bool), v => v._isArchived, 
            (a, dr) => a._isArchived = dr.GetBoolean(nameof(IsArchived)));
        private static OrmFieldMap<ComplexNumberTemplate> _commentsMap = new OrmFieldMap<ComplexNumberTemplate>(
            "comments", nameof(Comments), typeof(string), v => v._comments, (a, dr) => a._comments = dr.GetString(nameof(Comments)));
        private static OrmFieldMap<ComplexNumberTemplate> _insertedAtMap = new OrmFieldMap<ComplexNumberTemplate>(
            "inserted_at", nameof(InsertedAt), typeof(DateTime), v => v._insertedAt.ToUniversalTime(), 
            (a, dr) => a._insertedAt = dr.GetDateTime(nameof(InsertedAt)).ToLocalTime(), FieldPersistenceType.InsertOnly);
        private static OrmFieldMap<ComplexNumberTemplate> _insertedByMap = new OrmFieldMap<ComplexNumberTemplate>(
            "inserted_by", nameof(InsertedBy), typeof(string), v => v._insertedBy, 
            (a, dr) => a._insertedBy = dr.GetString(nameof(InsertedBy)), FieldPersistenceType.InsertOnly);
        private static OrmFieldMap<ComplexNumberTemplate> _updatedAtMap = new OrmFieldMap<ComplexNumberTemplate>(
            "updated_at", nameof(UpdatedAt), typeof(DateTime), v => v._updatedAt.ToUniversalTime(), 
            (a, dr) => a._updatedAt = dr.GetDateTime(nameof(UpdatedAt)).ToLocalTime());
        private static OrmFieldMap<ComplexNumberTemplate> _updatedByMap = new OrmFieldMap<ComplexNumberTemplate>(
            "updated_by", nameof(UpdatedBy), typeof(string), v => v._updatedBy, 
            (a, dr) => a._updatedBy = dr.GetString(nameof(UpdatedBy)));


        private int _id;
        private int _documentType;
        private Guid? _extendedDocumentTypeId;
        private string _serial;
        private ResetPolicyType _resetPolicy;
        private string _formatString;
        private bool _hasExternalProvider;
        private bool _isArchived;
        private string _comments;
        private DateTime _insertedAt;
        private string _insertedBy;
        private DateTime _updatedAt;
        private string _updatedBy;


        public int Id { get { return _id; } }

        public int DocumentType { get { return _documentType; } set { _documentType = value; } }

        public Guid?  ExtendedDocumentTypeId { get { return _extendedDocumentTypeId; } set { _extendedDocumentTypeId = value; } }

        public string Serial { get { return _serial; } set { _serial = value; } }

        public ResetPolicyType ResetPolicy { get { return _resetPolicy; } set { _resetPolicy = value; } }

        public string FormatString { get { return _formatString; } set { _formatString = value; } }

        public bool HasExternalProvider { get { return _hasExternalProvider; } set { _hasExternalProvider = value; } }

        public bool IsArchived { get { return _isArchived; } set { _isArchived = value; } }

        public string Comments { get { return _comments; } set { _comments = value; } }

        public DateTime InsertedAt { get { return _insertedAt; } }

        public string InsertedBy { get { return _insertedBy; } }

        public DateTime UpdatedAt { get { return _updatedAt; } }

        public string UpdatedBy { get { return _updatedBy; } }


        protected override void BeforeSave()
        {
            if (IsNew)
            {
                _insertedAt = DateTime.Now;
                _updatedAt = _insertedAt;
                _insertedBy = "Insert User";
                _updatedBy = "Insert User";
            }
            else
            {
                _updatedAt = DateTime.Now;
                _updatedBy = "Update User";
            }
        }

    }
}
