#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Data;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="DuckDBConnection"/> data provider.
    /// </summary>
    public class DuckDbDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuckDbDbSetting"/> class.
        /// </summary>
        public DuckDbDbSetting()
        {
            AreIndexNamesUniquePerSchema = true;
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = "main";
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsInsertAllBatchable = true;
            IsPreparable = true;
            IsSchemaSupported = true;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            RequiresDbTypeBeforeValue = false;
            SkipsUnreferencedParameters = false;
            MaxParameterCount = 2100 - 2;
            MultiStatementSeparator = ";";
            OpeningQuote = "\"";
            ParameterPrefix = "$";
            Provider = "DuckDb";
            SqlTextParameterPrefix = "$";
        }
    }
}
