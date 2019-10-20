using RepoDb.Interfaces;

namespace RepoDb.SqLite.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="SqlConnection"/> data provider.
    /// </summary>
    internal sealed class SqLiteDbSetting : IDbSetting
    {
        private int? m_hashCode = null;

        #region Properties

        /// <summary>
        /// Gets a value whether the multiple statement execution is supported.
        /// </summary>
        public bool IsMultipleStatementExecutionSupported { get; } = false;

        /// <summary>
        /// Gets the character (or string) used for opening quote.
        /// </summary>
        public string OpeningQuote { get; } = "(";

        /// <summary>
        /// Gets the character (or string) used for closing quote.
        /// </summary>
        public string ClosingQuote { get; } = ")";

        /// <summary>
        /// Gets the character (or string) used for the database command parameter quoting.
        /// </summary>
        public string ParameterPrefix { get; } = "@";

        /// <summary>
        /// Gets the character (or string) used for separating the schema.
        /// </summary>
        public string SchemaSeparator { get; } = ".";

        /// <summary>
        /// Gets the default schema of the database.
        /// </summary>
        public string DefaultSchema { get; } = null; // Research the SQLite default schema

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="SqLiteDbSetting"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (m_hashCode != null)
            {
                return m_hashCode.Value;
            }

            // Use the non nullable for perf purposes
            var hashCode = 0;

            // OpeningQuote
            if (!string.IsNullOrEmpty(OpeningQuote))
            {
                hashCode += OpeningQuote.GetHashCode();
            }

            // ClosingQuote
            if (!string.IsNullOrEmpty(ClosingQuote))
            {
                hashCode += ClosingQuote.GetHashCode();
            }

            // ParameterPrefix
            if (!string.IsNullOrEmpty(ParameterPrefix))
            {
                hashCode += ParameterPrefix.GetHashCode();
            }

            // SchemaSeparator
            if (!string.IsNullOrEmpty(SchemaSeparator))
            {
                hashCode += SchemaSeparator.GetHashCode();
            }

            // DefaultSchema
            if (!string.IsNullOrEmpty(DefaultSchema))
            {
                hashCode += DefaultSchema.GetHashCode();
            }

            // Set and return the hashcode
            return (m_hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="SqLiteDbSetting"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="SqLiteDbSetting"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(SqLiteDbSetting other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="SqLiteDbSetting"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="SqLiteDbSetting"/> object.</param>
        /// <param name="objB">The second <see cref="SqLiteDbSetting"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(SqLiteDbSetting objA, SqLiteDbSetting objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="SqLiteDbSetting"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="SqLiteDbSetting"/> object.</param>
        /// <param name="objB">The second <see cref="SqLiteDbSetting"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(SqLiteDbSetting objA, SqLiteDbSetting objB)
        {
            return (objA == objB) == false;
        }

        #endregion
    }
}
