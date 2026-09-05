#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using ClickHouse.Driver.ADO;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="ClickHouseConnection"/> data provider.
    /// </summary>
    public class ClickHouseDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseDbSetting"/> class.
        /// </summary>
        public ClickHouseDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "`";
            DefaultSchema = null;
            IsAffectedRowsSupported = false;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = false;
            IsInsertAllBatchable = false;
            IsPreparable = false;
            IsTransactionSupported = false;
            IsUseUpsert = false;
            RequiresDbTypeBeforeValue = false;
            SkipsUnreferencedParameters = false;
            MaxParameterCount = 2100 - 2;
            MultiStatementSeparator = ";";
            OpeningQuote = "`";
            ParameterPrefix = string.Empty;
            SqlTextParameterPrefix = "@";
        }
    }
}
