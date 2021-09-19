using RepoDb.Extensions;
using System;
using System.Data;
using System.Reflection;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute that is being used to set a value to any property of the <see cref="IDbDataParameter"/> object.
    /// </summary>
    public class PropertyValueAttribute : Attribute, IEquatable<PropertyValueAttribute>
    {
        private int? hashCode = null;

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="PropertyValueAttribute"/> class.
        /// </summary>
        /// <param name="parameterType">The type of the <see cref="IDbDataParameter"/> object.</param>
        /// <param name="propertyName">The name to be set to the parameter.</param>
        /// <param name="value">The value to be set to the parameter.</param>
        public PropertyValueAttribute(Type parameterType,
            string propertyName,
            object value)
            : this(parameterType, propertyName, value, true)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="PropertyValueAttribute"/> class.
        /// </summary>
        /// <param name="parameterType">The type of the <see cref="IDbDataParameter"/> object.</param>
        /// <param name="propertyName">The name to be set to the parameter.</param>
        /// <param name="value">The value to be set to the parameter.</param>
        /// <param name="includedInCompilation">
        /// The value that indicates whether this current attribute method invocation 
        /// will be included on the ahead-of-time (AOT) compilation.
        /// </param>
        internal PropertyValueAttribute(Type parameterType,
            string propertyName,
            object value,
            bool includedInCompilation)
        {
            // Validation
            Validate(parameterType, propertyName);

            // Set the properties
            ParameterType = parameterType;
            PropertyName = propertyName;
            Value = value;
            IncludedInCompilation = includedInCompilation;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the represented <see cref="Type"/> of the <see cref="IDbDataParameter"/> object.
        /// </summary>
        public Type ParameterType { get; }

        /// <summary>
        /// Gets the name of the target property to be set.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the value that is used to set in the parameter.
        /// </summary>
        protected internal object Value { get; }

        /// <summary>
        /// Gets the value that indicates whether this current attribute method invocation 
        /// will be included on the ahead-of-time (AOT) compilation.
        /// </summary>
        protected internal bool IncludedInCompilation { get; }

        /// <summary>
        /// Gets the instance of the <see cref="PropertyInfo"/> based on the target property name.
        /// </summary>
        /// <returns></returns>
        internal PropertyInfo PropertyInfo { get; private set; }

        #endregion

        #region Methods

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
            ObjectExtension.ThrowIfNull(parameter, "Parameter");

            if (ParameterType.IsAssignableFrom(parameter.GetType()))
            {
                // The reason why we use the 'GetValue()' method over the 'Value' property is because
                // of the fact that derived class should sometime customize the value.
                PropertyInfo.SetValue(parameter, GetValue());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal virtual object GetValue() => Value;

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterType"></param>
        /// <param name="propertyName"></param>
        private void Validate(Type parameterType,
            string propertyName)
        {
            ObjectExtension.ThrowIfNull(parameterType, "ParameterType");
            ValidateParameterType(parameterType);
            ObjectExtension.ThrowIfNull(propertyName, "PropertyName");
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
            ObjectExtension.ThrowIfNull(PropertyInfo,
                $"The property '{propertyName}' is not found from type '{parameterType?.FullName}'.");
        }

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="PropertyValueAttribute"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            // FullName: This is to ensure that even the user has created an identical formatting 
            //  on the derived class with the existing classes, the Type.FullName could still 
            // differentiate the instances
            var hashCode = GetType().FullName.GetHashCode();

            // Base
            hashCode += base.GetHashCode();

            // PropertyName
            if (PropertyName != null)
            {
                hashCode += PropertyName.GetHashCode();
            }

            // ParameterType
            if (ParameterType != null)
            {
                hashCode += ParameterType.GetHashCode();
            }

            // IncludedInCompilation
            hashCode += IncludedInCompilation.GetHashCode();

            // Value
            if (Value != null)
            {
                hashCode += Value.GetHashCode();
            }

            // Return
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="PropertyValueAttribute"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;

            return obj.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="PropertyValueAttribute"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(PropertyValueAttribute other)
        {
            if (other is null) return false;

            return other.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="PropertyValueAttribute"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="PropertyValueAttribute"/> object.</param>
        /// <param name="objB">The second <see cref="PropertyValueAttribute"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(PropertyValueAttribute objA,
            PropertyValueAttribute objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="PropertyValueAttribute"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="PropertyValueAttribute"/> object.</param>
        /// <param name="objB">The second <see cref="PropertyValueAttribute"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(PropertyValueAttribute objA,
            PropertyValueAttribute objB) =>
            (objA == objB) == false;

        #endregion
    }
}
