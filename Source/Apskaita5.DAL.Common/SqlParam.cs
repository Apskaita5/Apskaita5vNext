using System;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents an abstract SQL query parameter data.
    /// </summary>
    [Serializable]
    public sealed class SqlParam
    {

        private string _name = string.Empty;
        private object _value = null;
        private Type _valueType = null;
        private bool _replaceInQuery = false;


        /// <summary>
        /// Gets or sets the parameter name. 
        /// The name should be set without SQL implementation specific prefix.
        /// </summary>
        public string Name {
            get { return _name; }
            set
            {
               if (value.IsNullOrWhiteSpace()) 
                   throw new ArgumentNullException(nameof(value));
                _name = value.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the parameter value. 
        /// If the value is set to null, <see cref="ValueType">ValueType</see> property should also be set.
        /// </summary>
        public object Value {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Gets or sets the parameter value type. 
        /// Only required when the <see cref="Value">Value</see> property is set to null.
        /// </summary>
        public Type ValueType {
            get { return _valueType; }
            set
            {
                if (value==null)
                    throw new ArgumentNullException(nameof(value));
                _valueType = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether modify SQL query by replacing parameter name with value
        /// instead of using built in SQL parameter functionality.
        /// </summary>
        public bool ReplaceInQuery {
            get { return _replaceInQuery; }
            set { _replaceInQuery = value; }
        }


        /// <summary>
        /// Initializes a new SqlParam instance.
        /// </summary>
        /// <param name="name">the parameter name (should be set without SQL implementation specific prefix)</param>
        /// <param name="value">the parameter value</param>
        public SqlParam(string name, object value)
        {

            if (name.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Value = value;

            if (value == null)
            {
                ValueType = typeof (string);
            }
            else
            {
                ValueType = value.GetType();
            }

        }

        /// <summary>
        /// Initializes a new SqlParam instance.
        /// </summary>
        /// <param name="name">the parameter name (should be set without SQL implementation specific prefix)</param>
        /// <param name="value">the parameter value</param>
        /// <param name="valueType">the parameter value type (only required when 
        /// the <see cref="value">value</see> is set to null)</param>
        public SqlParam(string name, object value, Type valueType)
        {

            if (name.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(name));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));

            Name = name;
            Value = value;
            ValueType = valueType;

        }

    }
}
