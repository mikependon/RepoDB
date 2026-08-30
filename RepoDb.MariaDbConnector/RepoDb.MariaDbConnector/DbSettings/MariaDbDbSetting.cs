#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.MariaDbConnector;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="MariaDbConnection"/> data provider.
    /// </summary>
    public sealed class MariaDbDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="MariaDbDbSetting"/> class.
        /// </summary>
        public MariaDbDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "`";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = true;
            IsInsertAllBatchable = true;
            IsPreparable = false;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            RequiresDbTypeBeforeValue = false;
            SkipsUnreferencedParameters = false;
            MaxParameterCount = 2100 - 2;
            MultiStatementSeparator = ";";
            OpeningQuote = "`";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
