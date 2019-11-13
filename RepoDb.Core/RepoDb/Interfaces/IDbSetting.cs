using System;
using System.Data.Common;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark the class to become a database setting object.
    /// </summary>
    public interface IDbSetting
    {
        /// <summary>
        /// Gets a value whether the multiple statement execution is supported.
        /// </summary>
        bool IsMultipleStatementExecutionSupported { get; }

        /// <summary>
        /// Gets a value that indicates whether the Insert/Update operation will be used for Merge operation.
        /// </summary>
        bool IsUseUpsertForMergeOperation { get; }

        /// <summary>
        /// Gets a value that indicates whether setting the value of <see cref="DbParameter.Direction"/> object is supported.
        /// </summary>
        bool IsDbParameterDirectionSettingSupported { get; }

        /// <summary>
        /// Gets the value that indicates whether the table hints are supported.
        /// </summary>
        bool AreTableHintsSupported { get; }

        /// <summary>
        /// Gets the character used for opening quote.
        /// </summary>
        string OpeningQuote { get; }

        /// <summary>
        /// Gets the character used for closing quote.
        /// </summary>
        string ClosingQuote { get; }

        /// <summary>
        /// Gets the character used for the database command parameter prefixing.
        /// </summary>
        string ParameterPrefix { get; }

        /// <summary>
        /// Gets the character (or string) used for dot notation.
        /// </summary>
        string SchemaSeparator { get; }

        /// <summary>
        /// Gets the default schema of the database.
        /// </summary>
        string DefaultSchema { get; }

        /// <summary>
        /// Gets the default averageable .NET CLR types for the database.
        /// </summary>
        Type DefaultAverageableType { get; }
    }
}
