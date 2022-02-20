using RepoDb.Interfaces;
using System;
using System.Data.Common;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A base class to be used when implementing an <see cref="IDbSetting"/>-based object to support a specific RDBMS data provider.
    /// </summary>
    public abstract class BaseDbSetting : IDbSetting
    {
        #region Privates

        private int? hashCode = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="BaseDbSetting"/> class.
        /// </summary>
        public BaseDbSetting()
        {
            AreTableHintsSupported = true;
            AverageableType = StaticType.Double;
            ClosingQuote = "]";
            DefaultSchema = "dbo";
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "[";
            ParameterPrefix = "@";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value that indicates whether the table hints are supported.
        /// </summary>
        public bool AreTableHintsSupported { get; protected set; }

        /// <summary>
        /// Gets the character (or string) used for closing quote.
        /// </summary>
        public string ClosingQuote { get; protected set; }

        /// <summary>
        /// Gets the default averageable .NET CLR types for the database.
        /// </summary>
        public Type AverageableType { get; protected set; }

        /// <summary>
        /// Gets the default schema of the database.
        /// </summary>
        public string DefaultSchema { get; protected set; }

        /// <summary>
        /// Gets a value that indicates whether setting of the value of <see cref="DbParameter.Direction"/> object is supported.
        /// </summary>
        public bool IsDirectionSupported { get; protected set; }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="DbCommand"/> object must be disposed after calling the <see cref="DbCommand.ExecuteReader()"/> method.
        /// </summary>
        public bool IsExecuteReaderDisposable { get; protected set; }

        /// <summary>
        /// Gets a value whether the multiple statement execution is supported.
        /// </summary>
        public bool IsMultiStatementExecutable { get; protected set; }

        /// <summary>
        /// Gets a value that indicates whether the current DB Provider supports the <see cref="DbCommand.Prepare()"/> calls.
        /// </summary>
        public bool IsPreparable { get; protected set; }

        /// <summary>
        /// Gets a value that indicates whether the Insert/Update operation will be used for Merge operation.
        /// </summary>
        public bool IsUseUpsert { get; protected set; }

        /// <summary>
        /// Gets the character (or string) used for opening quote.
        /// </summary>
        public string OpeningQuote { get; protected set; }

        /// <summary>
        /// Gets the character (or string) used for the database command parameter quoting.
        /// </summary>
        public string ParameterPrefix { get; protected set; }

        /// <summary>
        /// Gets the character (or string) used for separating the schema.
        /// </summary>
        [Obsolete("This will be removed in the future releases. The schema separator will be defaulted to a 'period' character.")]
        public string SchemaSeparator { get; protected set; }

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="BaseDbSetting"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            // Use the non nullable for perf purposes
            var hashCode = 0;

            // AreTableHintsSupported
            hashCode = HashCode.Combine(hashCode, AreTableHintsSupported);

            // ClosingQuote
            if (!string.IsNullOrWhiteSpace(ClosingQuote))
            {
                hashCode = HashCode.Combine(hashCode, ClosingQuote);
            }

            // DefaultAverageableType
            if (AverageableType != null)
            {
                hashCode = HashCode.Combine(hashCode, AverageableType);
            }

            // DefaultSchema
            if (!string.IsNullOrWhiteSpace(DefaultSchema))
            {
                hashCode = HashCode.Combine(hashCode, DefaultSchema);
            }

            // IsDirectionSupported
            hashCode = HashCode.Combine(hashCode, IsDirectionSupported);

            // IsExecuteReaderDisposable
            hashCode = HashCode.Combine(hashCode, IsExecuteReaderDisposable);

            // IsMultiStatementExecutable
            hashCode = HashCode.Combine(hashCode, IsMultiStatementExecutable);

            // IsPreparable
            hashCode = HashCode.Combine(hashCode, IsPreparable);

            // IsUseUpsert
            hashCode = HashCode.Combine(hashCode, IsUseUpsert);

            // OpeningQuote
            if (!string.IsNullOrWhiteSpace(OpeningQuote))
            {
                hashCode = HashCode.Combine(hashCode, OpeningQuote);
            }

            // ParameterPrefix
            if (!string.IsNullOrWhiteSpace(ParameterPrefix))
            {
                hashCode = HashCode.Combine(hashCode, ParameterPrefix);
            }

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="BaseDbSetting"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            
            return obj.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="BaseDbSetting"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(BaseDbSetting other)
        {
            if (other is null) return false;
            
            return other.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="BaseDbSetting"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="BaseDbSetting"/> object.</param>
        /// <param name="objB">The second <see cref="BaseDbSetting"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(BaseDbSetting objA,
            BaseDbSetting objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="BaseDbSetting"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="BaseDbSetting"/> object.</param>
        /// <param name="objB">The second <see cref="BaseDbSetting"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(BaseDbSetting objA,
            BaseDbSetting objB) =>
            (objA == objB) == false;

        #endregion
    }
}
