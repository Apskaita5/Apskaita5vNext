using System;
using System.Collections.Generic;
using System.Linq;
using Apskaita5.DAL.Common.TypeConverters;

namespace Apskaita5.DAL.Common
{
    
    /// <summary>
    /// Defines a schema for an application user role.
    /// </summary>
    [Serializable]
    public sealed class ApplicationRoleSchema
    {

        private readonly Guid _guid = Guid.NewGuid();
        private int _visibleIndex = 0;
        private string _name = string.Empty;
        private string _description = "";
        private bool _isLookUpRole = false;
        private bool _hasSelectSubrole = true;
        private bool _hasInsertSubrole = true;
        private bool _hasUpdateSubrole = true;
        private bool _hasExecuteSubrole = false;
        private string _requiredLookUpRoles = string.Empty;


        /// <summary>
        /// Gets or sets the visible index of the role, i.e. defines order in which the roles 
        /// are displayed in the user form.
        /// </summary>
        public int VisibleIndex
        {
            get { return _visibleIndex; }
            set { _visibleIndex = value; }
        }

        /// <summary>
        /// Gets or sets the role name. Recommended usage: Product.Namespace.TypeName.
        /// </summary>
        public string Name {
            get { return _name; }
            set { _name = value?.Trim() ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets a description of the role.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value?.Trim() ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the role is ment for access to lookup lists
        /// (not editable business objects or reports). The lookup roles does not appear in the
        /// role list for a user by itself and are only used by other roles that declare the usage
        /// of lookup lists in the <see cref="RequiredLookUpRoles">RequiredLookUpRoles</see> property.
        /// </summary>
        public bool IsLookUpSubrole
        {
            get { return _isLookUpRole; }
            set { _isLookUpRole = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the role has a Select subrole internally,
        /// e.g. Product.Namespace.TypeName.1
        /// </summary>
        public bool HasSelectSubrole
        {
            get { return _hasSelectSubrole; }
            set { _hasSelectSubrole = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the role has an Insert subrole internally,
        /// e.g. Product.Namespace.TypeName.2
        /// </summary>
        public bool HasInsertSubrole
        {
            get { return _hasInsertSubrole; }
            set { _hasInsertSubrole = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the role has an Update subrole internally,
        /// e.g. Product.Namespace.TypeName.3
        /// </summary>
        public bool HasUpdateSubrole
        {
            get { return _hasUpdateSubrole; }
            set { _hasUpdateSubrole = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the role has an Execute subrole internally,
        /// e.g. Product.Namespace.TypeName.4
        /// </summary>
        public bool HasExecuteSubrole
        {
            get { return _hasExecuteSubrole; }
            set { _hasExecuteSubrole = value; }
        }

        /// <summary>
        /// Gets or sets the (comma separated) roles, that are required to fetch the lookup
        /// lists. E.g. in order to use InvoiceMade object one needs access to the lookup
        /// lists PersonInfoList, AccountInfoList etc.
        /// </summary>
        public string RequiredLookUpRoles
        {
            get { return _requiredLookUpRoles; }
            set { _requiredLookUpRoles = value?.Trim() ?? string.Empty; }
        }


        public ApplicationRoleSchema()
        {
        }

        internal ApplicationRoleSchema(string source, string fieldDelimiter)
        {

            if (source.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(source));
            if (null == fieldDelimiter || fieldDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(fieldDelimiter));

            _name = source.GetDelimitedField(0, fieldDelimiter);
            _description = source.GetDelimitedField(1, fieldDelimiter);
            _isLookUpRole = source.GetDelimitedField(2, fieldDelimiter).GetBooleanOrDefault(false);
            _hasSelectSubrole = source.GetDelimitedField(3, fieldDelimiter).GetBooleanOrDefault(true);
            _hasInsertSubrole = source.GetDelimitedField(4, fieldDelimiter).GetBooleanOrDefault(true);
            _hasUpdateSubrole = source.GetDelimitedField(5, fieldDelimiter).GetBooleanOrDefault(true);
            _hasExecuteSubrole = source.GetDelimitedField(6, fieldDelimiter).GetBooleanOrDefault(false);
            _requiredLookUpRoles = source.GetDelimitedField(7, fieldDelimiter);

        }


        /// <summary>
        /// Gets the list of all the data errors for the ApplicationRoleSchema instance as a per property dictionary.
        /// </summary>
        public Dictionary<string, List<string>> GetDataErrors()
        {

            var result = new Dictionary<string, List<string>>();

            if (_name.IsNullOrWhiteSpace())
                GetOrCreateErrorList(nameof(Name), result).Add(Apskaita5.DAL.Common.Properties.Resources.ApplicationRoleSchema_RoleNameNull);

            if (_name.IndexOf(" ", StringComparison.OrdinalIgnoreCase) >= 0)
                GetOrCreateErrorList(nameof(Name), result).Add(Apskaita5.DAL.Common.Properties.Resources.ApplicationRoleSchema_RoleNameContainsBlankSpaces);

            return result;

        }

        private List<string> GetOrCreateErrorList(string key, Dictionary<string, List<string>> dict)
        {
            if (!dict.ContainsKey(key)) dict.Add(key, new List<string>()); ;
            return dict[key];
        }

        /// <summary>
        /// Gets the description of all the data errors for the ApplicationRoleSchema instance.
        /// </summary>
        public string GetDataErrorsString()
        {

            var dict = GetDataErrors();

            if (dict.Count() < 1) return string.Empty;

            var result = new List<string> {string.Format(Properties.Resources.ApplicationRoleSchema_DataErrorsHeader, _name)};

            result.AddRange(dict.SelectMany(entry => entry.Value));

            return string.Join(Environment.NewLine, result.ToArray());

        }


        /// <summary>
        /// Technical method to support databinding. Returns a Guid 
        /// that is created for every new ApplicationRoleSchema instance (not persisted).
        /// </summary>
        /// <returns></returns>
        public object GetIdValue()
        {
            return _guid;
        }

    }
}
