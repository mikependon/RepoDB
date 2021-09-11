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
            // Validation
            Validate(parameterType, propertyName);

            // Set the properties
            ParameterType = parameterType;
            PropertyName = propertyName;
            Value = value;
        }

        /*
         * Properties
         */

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

        /*
         * Methods
         */

        /// <summary>
        /// Gets the string representation of the current attribute object.
        /// </summary>
        /// <returns>The represented string.</returns>
        public override string ToString() =>
            $"{ParameterType?.FullName}.{PropertyName} = {Value}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        internal void SetValue(IDbDataParameter parameter)
        {
            ThrowIfNull(parameter, "Parameter");

            if (ParameterType.IsAssignableFrom(parameter.GetType()))
            {
                PropertyInfo.SetValue(parameter, Value);
            }
        }

        /*
         * Helpers
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterType"></param>
        /// <param name="propertyName"></param>
        private void Validate(Type parameterType,
            string propertyName)
        {
            ThrowIfNull(parameterType, "ParameterType");
            ValidateParameterType(parameterType);
            ThrowIfNull(propertyName, "PropertyName");
            EnsurePropertyInfo(parameterType, propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterType"></param>
        private void ValidateParameterType(Type parameterType)
        {
            if (StaticType.IDbDataParameter.IsAssignableFrom(parameterType) == false)
            {
                throw new InvalidOperationException($"The parameter type must be deriving from the '{StaticType.IDbDataParameter.FullName}' interface. " +
                    $"The current passed parameter type is '{parameterType.FullName}'.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterType"></param>
        /// <param name="propertyName"></param>
        private void EnsurePropertyInfo(Type parameterType,
            string propertyName)
        {
            // Property
            PropertyInfo = parameterType?.GetProperty(propertyName);
            ThrowIfNull(PropertyInfo,
                $"The property '{propertyName}' is not found from type '{parameterType?.FullName}'.");
        }

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
    }
}
