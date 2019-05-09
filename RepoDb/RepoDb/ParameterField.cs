using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used when creating a <see cref="DbParameter"/> object for <see cref="DbCommand"/>.
    /// </summary>
    internal class ParameterField
    {
        private int m_hashCode = 0;

        /// <summary>
        /// Creates a new instance of <see cref="DbField"/> object.
        /// </summary>
        /// <param name="dbField">The target <see cref="DbField"/> object.</param>
        /// <param name="classProperty">The target <see cref="ClassProperty"/> object.</param>
        public ParameterField(DbField dbField,
            ClassProperty classProperty)
        {
            if (dbField != null)
            {
                m_hashCode += dbField.GetHashCode();
            }
            if (classProperty != null)
            {
                m_hashCode += classProperty.GetHashCode();
            }
        }

        /// <summary>
        /// Gets the current <see cref="DbField"/> object.
        /// </summary>
        public DbField DbField { get; }

        /// <summary>
        /// Gets the current <see cref="ClassProperty"/> object.
        /// </summary>
        public ClassProperty ClassProperty { get; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="ParameterField"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            return m_hashCode;
        }

        /// <summary>
        /// Compares the <see cref="ParameterField"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="ParameterField"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(ParameterField other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="ParameterField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ParameterField"/> object.</param>
        /// <param name="objB">The second <see cref="ParameterField"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(ParameterField objA, ParameterField objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="ParameterField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ParameterField"/> object.</param>
        /// <param name="objB">The second <see cref="ParameterField"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(ParameterField objA, ParameterField objB)
        {
            return (objA == objB) == false;
        }
    }
}
