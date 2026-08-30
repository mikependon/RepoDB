#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for Db2 data provider.
    /// </summary>
    public sealed class Db2DbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="Db2DbSetting"/> class.
        /// </summary>
        public Db2DbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsInsertAllBatchable = true;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            RequiresDbTypeBeforeValue = false;
            SkipsUnreferencedParameters = false;
            MaxParameterCount = 2100 - 2;
            MultiStatementSeparator = ";";
            OpeningQuote = "\"";
            ParameterPrefix = ":";
            SqlTextParameterPrefix = ":";
        }
    }
}
