#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="VerticaConnection"/> data provider.
    /// </summary>
    public sealed class VerticaDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="VerticaDbSetting"/> class.
        /// </summary>
        public VerticaDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = false;
            IsInsertAllBatchable = true;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = true;
            RequiresDbTypeBeforeValue = true;
            SkipsUnreferencedParameters = true;
            MaxParameterCount = 1500;
            MultiStatementSeparator = ";";
            OpeningQuote = "\"";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
