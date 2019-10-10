using System;
using Apskaita5.DAL.Common.MicroOrm;

namespace DeveloperUtils.TestClasses
{


    class Account : BaseClass<Account>
    {

        private static OrmIdentityMap<Account> _identityMap = new OrmIdentityMap<Account>("accounts", "id",
            nameof(Id), false, a => a._id, (a, dr) => a._id = dr.GetInt64(nameof(Id)));        
        private static OrmFieldMap<Account> _balanceAndIncomeLineIdMap = new OrmFieldMap<Account>(
            "balance_and_income_line_id", nameof(Balanceandincomelineid), typeof(int), 
            v => v._balanceAndIncomeLineId, (a, dr) => a._balanceAndIncomeLineId = dr.GetInt32(nameof(Balanceandincomelineid)));
        private static OrmFieldMap<Account> _accountNameMap = new OrmFieldMap<Account>("account_name", nameof(Accountname),
            typeof(string), v => v._accountName, (a, dr) => a._accountName = dr.GetString(nameof(Accountname)));
        private static OrmFieldMap<Account> _accountTypeMap = new OrmFieldMap<Account>("account_type", nameof(Accounttype),
            typeof(int), v => v._accountType, (a, dr) => a._accountType = dr.GetInt32(nameof(Accounttype)));
        private static OrmFieldMap<Account> _officialCodeMap = new OrmFieldMap<Account>("official_code", nameof(Officialcode),
            typeof(string), v => v._officialCode, (a, dr) => a._officialCode = dr.GetString(nameof(Officialcode)));
        private static OrmFieldMap<Account> _insertedAtMap = new OrmFieldMap<Account>("inserted_at", nameof(Insertedat),
            typeof(DateTime), v => v._insertedAt, (a, dr) => a._insertedAt = dr.GetDateTime(nameof(Insertedat)));
        private static OrmFieldMap<Account> _insertedByMap = new OrmFieldMap<Account>("inserted_by", nameof(Insertedby),
            typeof(string), v => v._insertedBy, (a, dr) => a._insertedBy = dr.GetString(nameof(Insertedby)));
        private static OrmFieldMap<Account> _updatedAtMap = new OrmFieldMap<Account>("updated_at", nameof(Updatedat),
            typeof(DateTime), v => v._updatedAt, (a, dr) => a._updatedAt = dr.GetDateTime(nameof(Updatedat)));
        private static OrmFieldMap<Account> _updatedByMap = new OrmFieldMap<Account>("updated_by", nameof(Updatedby),
            typeof(string), v => v._updatedBy, (a, dr) => a._updatedBy = dr.GetString(nameof(Updatedby)));


        private long _id;
        private int _balanceAndIncomeLineId;
        private string _accountName;
        private int _accountType;
        private string _officialCode;
        private DateTime _insertedAt;
        private string _insertedBy;
        private DateTime _updatedAt;
        private string _updatedBy;


        public long Id { get { return _id; } set { _id = value; } }

        public int Balanceandincomelineid { get { return _balanceAndIncomeLineId; } set { _balanceAndIncomeLineId = value; } }

        public string Accountname { get { return _accountName; } set { _accountName = value; } }

        public int Accounttype { get { return _accountType; } set { _accountType = value; } }

        public string Officialcode { get { return _officialCode; } set { _officialCode = value; } }

        public DateTime Insertedat { get { return _insertedAt; } set { _insertedAt = value; } }

        public string Insertedby { get { return _insertedBy; } set { _insertedBy = value; } }

        public DateTime Updatedat { get { return _updatedAt; } set { _updatedAt = value; } }

        public string Updatedby { get { return _updatedBy; } set { _updatedBy = value; } }

    }
}
