#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySql.Data.MySqlClient;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="MySqlConnection"/> data provider.
    /// </summary>
    public sealed class MySqlDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlDbSetting"/> class.
        /// </summary>
        public MySqlDbSetting()
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
