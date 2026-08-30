#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for Oracle data provider.
    /// </summary>
    public sealed class OracleDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="OracleDbSetting"/> class.
        /// </summary>
        public OracleDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = false;
            IsInsertAllBatchable = false;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            RequiresDbTypeBeforeValue = false;
            SkipsUnreferencedParameters = false;
            MaxParameterCount = 1000;
            MultiStatementSeparator = ";";
            OpeningQuote = "\"";
            ParameterPrefix = ":";
            SqlTextParameterPrefix = ":";
        }
    }
}
