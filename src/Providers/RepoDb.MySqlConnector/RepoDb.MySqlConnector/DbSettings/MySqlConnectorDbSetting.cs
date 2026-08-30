#region Copyright Attributions

// Copyright (c) 2019 Bradley Graigner and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySqlConnector;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="MySqlConnection"/> data provider.
    /// </summary>
    public sealed class MySqlConnectorDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlConnectorDbSetting"/> class.
        /// </summary>
        public MySqlConnectorDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "`";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
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
            OpeningQuote = "`";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
