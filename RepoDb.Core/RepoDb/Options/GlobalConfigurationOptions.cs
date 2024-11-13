using System.Data;
using System.Data.Common;
using RepoDb.Enumerations;

namespace RepoDb.Options
{
    /// <summary>
    /// A class that is being used to define the globalized configurations for the application.
    /// </summary>
    public record GlobalConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the value that defines the conversion logic when converting an instance of <see cref="DbDataReader"/> into a .NET CLR class.
        /// </summary>
        public ConversionType ConversionType { get; init; } = ConversionType.Default;

        /// <summary>
        /// Gets or sets the handling of invalid enum values when converting an instance of <see cref="DbDataReader"/> into .NET enum values
        /// </summary>
        public EnumHandling EnumHandling { get; init; } = EnumHandling.ThrowError;

        /// <summary>
        /// Gets or sets the value that defines how <c>null</c> is handled in linq style expressions
        /// </summary>
        public ExpressionNullSemantics ExpressionNullSemantics { get; init; } = ExpressionNullSemantics.SqlNull;

        /// <summary>
        /// Gets or sets the default value of the batch operation size. The value defines on this property mainly affects the batch size of the InsertAll, MergeAll and UpdateAll operations.
        /// </summary>
        public int DefaultBatchOperationSize { get; init; } = Constant.DefaultBatchOperationSize;

        /// <summary>
        /// Gets of sets the default value of the cache expiration in minutes.
        /// </summary>
        public int DefaultCacheItemExpirationInMinutes { get; init; } = Constant.DefaultCacheItemExpirationInMinutes;

        /// <summary>
        /// Gets or sets the default equivalent <see cref="DbType"/> of an enumeration if it is being used as a parameter to the execution of any non-entity-based operations.
        /// </summary>
        public DbType EnumDefaultDatabaseType { get; init; } = DbType.String;

        /// <summary>
        /// Gets or sets the default value of how the push operations (i.e.: Insert, InsertAll, Merge and MergeAll) behaves when returning the value from the key columns (i.e.: Primary and Identity).
        /// </summary>
        public KeyColumnReturnBehavior KeyColumnReturnBehavior { get; init; } = KeyColumnReturnBehavior.IdentityOrElsePrimary;
    }
}
