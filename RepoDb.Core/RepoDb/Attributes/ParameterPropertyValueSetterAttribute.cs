using System;
using System.Data;
using System.Reflection;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is being used to set a value to any property of the <see cref="IDbDataParameter"/> object.
    /// </summary>
    public class ParameterPropertyValueSetterAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ParameterPropertyValueSetterAttribute"/> class.
        /// </summary>
        /// <param name="parameterType">The type of the parameter.</param>
        /// <param name="propertyName">The name of the property to be set.</param>
        /// <param name="value">The value to be set.</param>
        public ParameterPropertyValueSetterAttribute(Type parameterType,
            string propertyName,
            object value)
        {
            // ParameterType
            ThrowIfNull(parameterType, "ParameterType");
            ParameterType = parameterType;

            // PropertyName
            ThrowIfNull(propertyName, "PropertyName");
            PropertyName = propertyName;

            // PropertyInfo
            EnsurePropertyInfo($"The property '{propertyName}' is not found from type '{parameterType.FullName}'.");

            // Value
            Value = value;
        }

        // Properties

        /// <summary>
        /// Gets the represented <see cref="Type"/> of the <see cref="IDbDataParameter"/> object.
        /// </summary>
        public Type ParameterType { get; }

        /// <summary>
        /// Gets the name of the target property to be set.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the value that is used to set property.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the instance of the <see cref="PropertyInfo"/> based on the target property name.
        /// </summary>
        /// <returns></returns>
        internal PropertyInfo PropertyInfo { get; private set; }

        // Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        private void ThrowIfNull(object obj,
            string message)
        {
            if (obj == null)
            {
                throw new NullReferenceException(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void EnsurePropertyInfo(string message)
        {
            PropertyInfo = ParameterType?.GetProperty(PropertyName);

            if (PropertyInfo == null)
            {
                ThrowIfNull(PropertyInfo, message);
            }
        }

        /// <summary>
        /// Gets the string representation of the current attribute object.
        /// </summary>
        /// <returns>The represented string.</returns>
        public override string ToString() =>
            $"{ParameterType?.FullName}.{PropertyName} = {Value}";

        /// <summary>
        /// Sets the value of the <see cref="IDbDataParameter"/> object.
        /// </summary>
        /// <param name="parameter">The instance of the <see cref="IDbDataParameter"/> object.</param>
        /// <param name="throwError">
        /// Throw an error if the parameter instance is null or the type is different from the 
        /// <see cref="ParameterType"/> property.
        /// </param>
        internal void SetValue(IDbDataParameter parameter,
            bool throwError = true)
        {
            if (parameter == null)
            {
                if (throwError)
                {
                    throw new NullReferenceException("Parameter");
                }
                return;
            }

            if (ParameterType != parameter?.GetType())
            {
                if (throwError)
                {
                    throw new InvalidOperationException($"The instance of the given parameter must be of type '{ParameterType.FullName}'.");
                }
                return;
            }

            // TODO: Minor culprit. (NTH) Pre-compile this (if necessary)
            PropertyInfo.SetValue(parameter, Value);
        }
    }
}
