using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apskaita5.DAL.Common.MicroOrm;

namespace DeveloperUtils.TestClasses
{
    class Costcentres : BaseClass<Costcentres>
    {

        private static OrmIdentityMap<Costcentres> _identityMap = new OrmIdentityMap<Costcentres>("cost_centres", "id",
        nameof(Id), false, a => a._id, (a, dr) => a._id = dr.GetInt32(nameof(Id)));
        private static OrmFieldMap<Costcentres> _idMap = new OrmFieldMap<Costcentres>("id", nameof(Id),
                        typeof(int), v => v._id, (a, dr) => a._id = dr.GetInt32(nameof(Id)));
        private static OrmFieldMap<Costcentres> _costCentreGroupIdMap = new OrmFieldMap<Costcentres>("cost_centre_group_id", nameof(Costcentregroupid),
                        typeof(int), v => v._costCentreGroupId, (a, dr) => a._costCentreGroupId = dr.GetInt32(nameof(Costcentregroupid)));
        private static OrmFieldMap<Costcentres> _costCentreNameMap = new OrmFieldMap<Costcentres>("cost_centre_name", nameof(Costcentrename),
                        typeof(string), v => v._costCentreName, (a, dr) => a._costCentreName = dr.GetString(nameof(Costcentrename)));
        private static OrmFieldMap<Costcentres> _descriptionMap = new OrmFieldMap<Costcentres>("description", nameof(Description),
                        typeof(string), v => v._description, (a, dr) => a._description = dr.GetString(nameof(Description)));
        private static OrmFieldMap<Costcentres> _isArchivedMap = new OrmFieldMap<Costcentres>("is_archived", nameof(Isarchived),
                        typeof(byte), v => v._isArchived, (a, dr) => a._isArchived = dr.GetByte(nameof(Isarchived)));
        private static OrmFieldMap<Costcentres> _insertedAtMap = new OrmFieldMap<Costcentres>("inserted_at", nameof(Insertedat),
                        typeof(DateTime), v => v._insertedAt, (a, dr) => a._insertedAt = dr.GetDateTime(nameof(Insertedat)));
        private static OrmFieldMap<Costcentres> _insertedByMap = new OrmFieldMap<Costcentres>("inserted_by", nameof(Insertedby),
                        typeof(string), v => v._insertedBy, (a, dr) => a._insertedBy = dr.GetString(nameof(Insertedby)));
        private static OrmFieldMap<Costcentres> _updatedAtMap = new OrmFieldMap<Costcentres>("updated_at", nameof(Updatedat),
                        typeof(DateTime), v => v._updatedAt, (a, dr) => a._updatedAt = dr.GetDateTime(nameof(Updatedat)));
        private static OrmFieldMap<Costcentres> _updatedByMap = new OrmFieldMap<Costcentres>("updated_by", nameof(Updatedby),
                        typeof(string), v => v._updatedBy, (a, dr) => a._updatedBy = dr.GetString(nameof(Updatedby)));


        private int _id;
        private int _costCentreGroupId;
        private string _costCentreName;
        private string _description;
        private byte _isArchived;
        private DateTime _insertedAt;
        private string _insertedBy;
        private DateTime _updatedAt;
        private string _updatedBy;


        public int Id { get { return _id; } }

        public int Costcentregroupid { get { return _costCentreGroupId; } set { _costCentreGroupId = value; } }

        public string Costcentrename { get { return _costCentreName; } set { _costCentreName = value; } }

        public string Description { get { return _description; } set { _description = value; } }

        public byte Isarchived { get { return _isArchived; } set { _isArchived = value; } }

        public DateTime Insertedat { get { return _insertedAt; } set { _insertedAt = value; } }

        public string Insertedby { get { return _insertedBy; } set { _insertedBy = value; } }

        public DateTime Updatedat { get { return _updatedAt; } set { _updatedAt = value; } }

        public string Updatedby { get { return _updatedBy; } set { _updatedBy = value; } }

    }

}
