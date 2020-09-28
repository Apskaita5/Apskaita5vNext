using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apskaita5.DAL.Common.MicroOrm;

namespace DevUtils.TestClasses
{



    //public class ClaimTemplate 
    //{

    //    private static OrmIdentityMapParentInt32Autoincrement<ClaimTemplate> _identityMap = new OrmIdentityMapParentInt32Autoincrement<ClaimTemplate>(
    //    "claim_templates", "id", nameof(Id), () => new ClaimTemplate(), a => a._id, (a, v) => a._id = v);
    //    private static OrmFieldMapString<ClaimTemplate> _claimTemplateNameMap = new OrmFieldMapString<ClaimTemplate>(
    //                    "claim_template_name", nameof(ClaimTemplateName), (a, v) => a._claimTemplateName = v, v => v._claimTemplateName, FieldPersistenceType.InsertAndUpdate);
    //    private static OrmFieldMapString<ClaimTemplate> _claimTemplateMap = new OrmFieldMapString<ClaimTemplate>(
    //                    "claim_template", nameof(ClaimTemplateBody), (a, v) => a._claimTemplate = v, v => v._claimTemplate, FieldPersistenceType.InsertAndUpdate);
    //    private static OrmFieldMapBool<ClaimTemplate> _isArchivedMap = new OrmFieldMapBool<ClaimTemplate>(
    //                    "is_archived", nameof(IsArchived), (a, v) => a._isArchived = v.Value, v => v._isArchived, FieldPersistenceType.InsertAndUpdate);
    //    private static OrmFieldMapInsertedAt<ClaimTemplate> _insertedAtMap = new OrmFieldMapInsertedAt<ClaimTemplate>(
    //                    "inserted_at", nameof(InsertedAt), (a, v) => a._insertedAt = v, v => v._insertedAt);
    //    private static OrmFieldMapInsertedBy<ClaimTemplate> _insertedByMap = new OrmFieldMapInsertedBy<ClaimTemplate>(
    //                    "inserted_by", nameof(InsertedBy), (a, v) => a._insertedBy = v, v => v._insertedBy);
    //    private static OrmFieldMapUpdatedAt<ClaimTemplate> _updatedAtMap = new OrmFieldMapUpdatedAt<ClaimTemplate>(
    //                    "updated_at", nameof(UpdatedAt), (a, v) => a._updatedAt = v, v => v._updatedAt);
    //    private static OrmFieldMapUpdatedBy<ClaimTemplate> _updatedByMap = new OrmFieldMapUpdatedBy<ClaimTemplate>(
    //                    "updated_by", nameof(UpdatedBy), (a, v) => a._updatedBy = v, v => v._updatedBy);


    //    private int? _id;
    //    private string _claimTemplateName;
    //    private string _claimTemplate;
    //    private bool _isArchived;
    //    private DateTime _insertedAt;
    //    private string _insertedBy;
    //    private DateTime _updatedAt;
    //    private string _updatedBy;


    //    public int? Id { get { return _id; } }

    //    public string ClaimTemplateName { get { return _claimTemplateName; } set { _claimTemplateName = ApplyPropValue(value, _claimTemplateName); } }

    //    public string ClaimTemplateBody { get { return _claimTemplate; } set { _claimTemplate = ApplyPropValue(value, _claimTemplate); } }

    //    public bool IsArchived { get { return _isArchived; } set { _isArchived = ApplyPropValue(value, _isArchived); } }

    //    public DateTime InsertedAt { get { return _insertedAt; } set { _insertedAt = ApplyPropValue(value, _insertedAt); } }

    //    public string InsertedBy { get { return _insertedBy; } set { _insertedBy = ApplyPropValue(value, _insertedBy); } }

    //    public DateTime UpdatedAt { get { return _updatedAt; } set { _updatedAt = ApplyPropValue(value, _updatedAt); } }

    //    public string UpdatedBy { get { return _updatedBy; } set { _updatedBy = ApplyPropValue(value, _updatedBy); } }

    //}
}
