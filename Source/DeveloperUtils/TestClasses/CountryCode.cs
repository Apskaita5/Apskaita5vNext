using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apskaita5.DAL.Common.MicroOrm;

namespace DeveloperUtils.TestClasses
{
    class Countrycodes : BaseClass<Countrycodes>
    {

        private static OrmIdentityMap<Countrycodes> _identityMap = new OrmIdentityMap<Countrycodes>("country_codes", "id",
        nameof(Id), false, a => a._id, (a, dr) => a._id = dr.GetString(nameof(Id)));
        private static OrmFieldMap<Countrycodes> _idMap = new OrmFieldMap<Countrycodes>("id", nameof(Id),
                        typeof(string), v => v._id, (a, dr) => a._id = dr.GetString(nameof(Id)));
        private static OrmFieldMap<Countrycodes> _isArchivedMap = new OrmFieldMap<Countrycodes>("is_archived", nameof(Isarchived),
                        typeof(byte), v => v._isArchived, (a, dr) => a._isArchived = dr.GetByte(nameof(Isarchived)));
        private static OrmFieldMap<Countrycodes> _insertedAtMap = new OrmFieldMap<Countrycodes>("inserted_at", nameof(Insertedat),
                        typeof(DateTime), v => v._insertedAt, (a, dr) => a._insertedAt = dr.GetDateTime(nameof(Insertedat)));
        private static OrmFieldMap<Countrycodes> _insertedByMap = new OrmFieldMap<Countrycodes>("inserted_by", nameof(Insertedby),
                        typeof(string), v => v._insertedBy, (a, dr) => a._insertedBy = dr.GetString(nameof(Insertedby)));
        private static OrmFieldMap<Countrycodes> _updatedAtMap = new OrmFieldMap<Countrycodes>("updated_at", nameof(Updatedat),
                        typeof(DateTime), v => v._updatedAt, (a, dr) => a._updatedAt = dr.GetDateTime(nameof(Updatedat)));
        private static OrmFieldMap<Countrycodes> _updatedByMap = new OrmFieldMap<Countrycodes>("updated_by", nameof(Updatedby),
                        typeof(string), v => v._updatedBy, (a, dr) => a._updatedBy = dr.GetString(nameof(Updatedby)));


        private string _id;
        private byte _isArchived;
        private DateTime _insertedAt;
        private string _insertedBy;
        private DateTime _updatedAt;
        private string _updatedBy;


        public string Id { get { return _id; } set { _id = value; } }

        public byte Isarchived { get { return _isArchived; } set { _isArchived = value; } }

        public DateTime Insertedat { get { return _insertedAt; } set { _insertedAt = value; } }

        public string Insertedby { get { return _insertedBy; } set { _insertedBy = value; } }

        public DateTime Updatedat { get { return _updatedAt; } set { _updatedAt = value; } }

        public string Updatedby { get { return _updatedBy; } set { _updatedBy = value; } }

    }
}
