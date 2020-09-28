using System;
using System.Collections.Generic;
using System.Text;

namespace Apskaita5.Domain.Core.Rules
{
    /// <summary>
    /// Stores details about a specific broken business rule.
    /// </summary>
    [Serializable]
    public class BrokenRule
    {

        internal BrokenRule() { }


        private string _ruleName = string.Empty;
        private string _description = string.Empty;
        private string _property = string.Empty;
        private RuleSeverity _severity = RuleSeverity.Error;
        private string _originProperty = string.Empty;
        

        /// <summary>
        /// Provides access to the name of the broken rule.
        /// </summary>
        /// <value>The name of the rule.</value>
        public string RuleName
        {
            get { return _ruleName; }
            internal set { _ruleName = value ?? string.Empty; }
        }

        /// <summary>
        /// Provides access to the description of the broken rule.
        /// </summary>
        /// <value>The description of the rule.</value>
        public string Description
        {
            get { return _description; }
            internal set { _description = value ?? string.Empty; }
        }

        /// <summary>
        /// Provides access to the property affected by the broken rule.
        /// </summary>
        /// <value>The property affected by the rule.</value>
        public string Property
        {
            get { return _property; }
            internal set { _property = value ?? string.Empty; }
        }

        /// <summary>
        /// Gets the severity of the broken rule.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public RuleSeverity Severity
        {
            get { return _severity; }
            internal set { _severity = value; }
        }

        /// <summary>
        /// Gets or sets the origin property.
        /// </summary>
        /// <value>The origin property.</value>
        public string OriginProperty
        {
            get { return _originProperty; }
            internal set { _originProperty = value ?? string.Empty; }
        }


        /// <summary>
        /// Gets a string representation for this object.
        /// </summary>
        public override string ToString()
        {
            return Description;
        }

    }
}
