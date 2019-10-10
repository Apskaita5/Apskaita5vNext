using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apskaita5.DAL.Common;
using Apskaita5.DAL.Common.MicroOrm;

namespace DeveloperUtils.TestClasses
{

    class ComplexNumbers : ChildBaseClass<ComplexNumbers>
    {

        private static OrmIdentityMap<ComplexNumbers> _identityMap = new OrmIdentityMap<ComplexNumbers>(
            "complex_numbers", "document_id", nameof(DocumentId), false, a => a._documentId, 
            (a, dr) => a._documentId = dr.GetInt32(nameof(DocumentId)));
        private static OrmFieldMap<ComplexNumbers> _templateIdMap = new OrmFieldMap<ComplexNumbers>(
            "template_id", nameof(TemplateId), typeof(int), v => v._templateId, 
            (a, dr) => a._templateId = dr.GetInt32(nameof(TemplateId)), FieldPersistenceType.InsertOnly);
        private static OrmFieldMap<ComplexNumbers> _resetPolicyMap = new OrmFieldMap<ComplexNumbers>(
            "reset_policy", nameof(ResetPolicy), typeof(string), v => v._resetPolicy, 
            (a, dr) => a._resetPolicy = dr.GetEnum<ResetPolicyType>(nameof(ResetPolicy)));
        private static OrmFieldMap<ComplexNumbers> _formatStringMap = new OrmFieldMap<ComplexNumbers>(
            "format_string", nameof(FormatString), typeof(string), v => v._formatString, 
            (a, dr) => a._formatString = dr.GetString(nameof(FormatString)));
        private static OrmFieldMap<ComplexNumbers> _hasExternalProviderMap = new OrmFieldMap<ComplexNumbers>(
            "has_external_provider", nameof(HasExternalProvider), typeof(bool), v => v._hasExternalProvider, 
            (a, dr) => a._hasExternalProvider = dr.GetBoolean(nameof(HasExternalProvider)));
        private static OrmFieldMap<ComplexNumbers> _runningNoMap = new OrmFieldMap<ComplexNumbers>(
            "running_no", nameof(RunningNo), typeof(int), v => v._runningNo, 
            (a, dr) => a._runningNo = dr.GetInt32(nameof(RunningNo)));
        private static OrmFieldMap<ComplexNumbers> _fullNoMap = new OrmFieldMap<ComplexNumbers>(
            "full_no", nameof(FullNo), typeof(string), v => v._fullNo, 
            (a, dr) => a._fullNo = dr.GetString(nameof(FullNo)));


        private int _documentId;
        private int _templateId;
        private ResetPolicyType _resetPolicy;
        private string _formatString;
        private bool _hasExternalProvider;
        private int _runningNo;
        private string _fullNo;
        private string _serial;


        public ComplexNumbers() { }

        public ComplexNumbers(ComplexNumberTemplate template, DateTime docDate, int runningNo)
        {
            if (null == template) throw new ArgumentNullException(nameof(template));
            _formatString = template.FormatString;
            _hasExternalProvider = template.HasExternalProvider;
            _resetPolicy = template.ResetPolicy;
            _templateId = template.Id;
            _serial = template.Serial;
            _runningNo = runningNo;
            _fullNo = string.Format(_formatString, docDate, runningNo);
        }



        public int DocumentId { get { return _documentId; } }

        public int TemplateId { get { return _templateId; } }

        public ResetPolicyType ResetPolicy { get { return _resetPolicy; } set { _resetPolicy = value; } }

        public string FormatString { get { return _formatString; } set { _formatString = value; } }

        public bool HasExternalProvider { get { return _hasExternalProvider; } set { _hasExternalProvider = value; } }

        public int RunningNo { get { return _runningNo; } set { _runningNo = value; } }

        public string FullNo { get { return _fullNo; } set { _fullNo = value; } }



        public string GetDocumentNo() => _serial + _fullNo;

        protected override SqlParam[] GetParamsForParentId(object parentId)
        {

            _documentId = (int)parentId;
            return null;
        }

    }
}
