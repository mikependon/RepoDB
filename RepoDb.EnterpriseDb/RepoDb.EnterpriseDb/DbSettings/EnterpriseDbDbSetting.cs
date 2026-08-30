#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using EnterpriseDB.EDBClient;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="EDBConnection"/> data provider.
    /// </summary>
    public sealed class EnterpriseDbDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="EnterpriseDbDbSetting"/> class.
        /// </summary>
        public EnterpriseDbDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = "public";
            IsAffectedRowsSupported = true;
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            OpeningQuote = "\"";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
