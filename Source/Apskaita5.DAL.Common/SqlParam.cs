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
            set { SetValue(value, null); }
        }

        /// <summary>
        /// Gets the parameter value type.
        /// </summary>
        public Type ValueType  => _valueType;
        
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

            _name = name;
            SetValue(value, null);

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

            _name = name;
            SetValue(value, valueType);

        }

        private void SetValue(object value, Type valueType)
        {   
            
            if (value != null)
            {
                if (value.GetType() == typeof(Nullable<byte>))
                {
                    var c = (Nullable<byte>)value;
                    _valueType = typeof(byte);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else if (value.GetType() == typeof(Nullable<sbyte>))
                {
                    var c = (Nullable<sbyte>)value;
                    _valueType = typeof(sbyte);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else if (value.GetType() == typeof(Nullable<Int16>))
                {
                    var c = (Nullable<Int16>)value;
                    _valueType = typeof(Int16);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else if (value.GetType() == typeof(Nullable<Int32>))
                {
                    var c = (Nullable<Int32>)value;
                    _valueType = typeof(Int32);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else if (value.GetType() == typeof(Nullable<Int64>))
                {
                    var c = (Nullable<Int64>)value;
                    _valueType = typeof(Int64);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else if (value.GetType() == typeof(Nullable<char>))
                {
                    var c = (Nullable<char>)value;
                    _valueType = typeof(char);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else if (value.GetType() == typeof(Nullable<DateTime>))
                {
                    var c = (Nullable<DateTime>)value;
                    _valueType = typeof(DateTime);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else if (value.GetType() == typeof(Nullable<bool>))
                {
                    var c = (Nullable<bool>)value;
                    _valueType = typeof(bool);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else if (value.GetType() == typeof(Nullable<double>))
                {
                    var c = (Nullable<double>)value;
                    _valueType = typeof(double);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else if (value.GetType() == typeof(Nullable<float>))
                {
                    var c = (Nullable<float>)value;
                    _valueType = typeof(float);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else if (value.GetType() == typeof(Nullable<decimal>))
                {
                    var c = (Nullable<decimal>)value;
                    _valueType = typeof(decimal);
                    if (c.HasValue) _value = c.Value;
                    else _value = null;
                }
                else
                {
                    _value = value;
                    _valueType = value.GetType();
                }

            }
            else if (valueType != null)
            {
                _value = null;
                _valueType = valueType;
            }
            else
            {
                _value = null;
                _valueType = typeof(string);
            }

        }

    }

}
