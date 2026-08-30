#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="FbConnection"/> data provider.
    /// </summary>
    public sealed class FirebirdDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="FirebirdDbSetting"/> class.
        /// </summary>
        public FirebirdDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = false;
            IsInsertAllBatchable = false;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            RequiresDbTypeBeforeValue = false;
            SkipsUnreferencedParameters = false;
            MaxParameterCount = 1500;
            MultiStatementSeparator = ";";
            OpeningQuote = "\"";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
