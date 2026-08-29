using System;
using System.Data.Common;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface that is used to mark a class to be a database setting object.
    /// </summary>
    public interface IDbSetting
    {
        /// <summary>
        /// Gets the value that indicates whether the table hints are supported.
        /// </summary>
        bool AreTableHintsSupported { get; }

        /// <summary>
        /// Gets the default averageable .NET CLR types for the database.
        /// </summary>
        [Obsolete("This will be removed in the future releases.")]
        Type AverageableType { get; }

        /// <summary>
        /// Gets the character used for closing quote.
        /// </summary>
        string ClosingQuote { get; }

        /// <summary>
        /// Gets the default schema of the database.
        /// </summary>
        string DefaultSchema { get; }

        /// <summary>
        /// Gets a value that indicates whether the current DB Provider's <see cref="DbCommand.ExecuteNonQuery()"/>
        /// (or its <see cref="DbCommand.ExecuteNonQueryAsync()"/> equivalent) reliably reports the number of rows
        /// affected by a DML statement (e.g. <c>DELETE</c>, <c>UPDATE</c>). When <see langword="false"/>, operations
        /// that need a precise affected-row count (e.g. <c>DeleteAll</c>) fall back to a separate <c>COUNT</c> query
        /// instead of trusting the driver's return value. Defaults to <see langword="true"/>.
        /// </summary>
        bool IsAffectedRowsSupported { get; }

        /// <summary>
        /// Gets a value that indicates whether setting the value of <see cref="DbParameter.Direction"/> object is supported.
        /// </summary>
        bool IsDirectionSupported { get; }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="DbCommand"/> object must be disposed after calling the <see cref="DbCommand.ExecuteReader()"/> method.
        /// </summary>
        bool IsExecuteReaderDisposable { get; }

        /// <summary>
        /// Gets a value whether the multiple statement execution is supported.
        /// </summary>
        bool IsMultiStatementExecutable { get; }

        /// <summary>
        /// Gets a value that indicates whether the current DB Provider supports the <see cref="DbCommand.Prepare()"/> calls.
        /// </summary>
        bool IsPreparable { get; }

        /// <summary>
        /// Gets a value that indicates whether the current DB Provider's <see cref="System.Data.IDbConnection.BeginTransaction()"/>
        /// (or its <see cref="DbConnection.BeginDbTransaction(System.Data.IsolationLevel)"/> equivalent) is supported.
        /// </summary>
        bool IsTransactionSupported { get; }

        /// <summary>
        /// Gets a value that indicates whether the Insert/Update operation will be used for Merge operation.
        /// </summary>
        bool IsUseUpsert { get; }

        /// <summary>
        /// Gets the maximum number of parameters/members the current DB provider allows in a single generated
        /// command text.
        /// </summary>
        int MaxParameterCount { get; }

        /// <summary>
        /// Gets the character used for opening quote.
        /// </summary>
        string OpeningQuote { get; }

        /// <summary>
        /// Gets the character (or string) used to prefix an actual <see cref="System.Data.Common.DbParameter.ParameterName"/>
        /// value when a parameter object is created.
        /// </summary>
        string ParameterPrefix { get; }

        /// <summary>
        /// Gets the character (or string) used to prefix a parameter placeholder token embedded directly into the
        /// generated SQL command text (e.g. the "@Name" in <c>INSERT INTO Table (Name) VALUES (@Name)</c>).
        /// </summary>
        string SqlTextParameterPrefix { get; }

        /// <summary>
        /// Gets the string used to join the individual per-type command texts generated for a <c>QueryMultiple</c>/<c>QueryMultipleAsync</c> call into one combined command text.
        /// </summary>
        public string MultiStatementSeparator { get; }

        /// <summary>
        /// Gets the character (or string) used for dot notation.
        /// </summary>
        string SchemaSeparator { get; }
    }
}
