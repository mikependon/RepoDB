#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for SQLite data provider.
    /// </summary>
    public sealed class SqLiteDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqLiteDbSetting"/> class.
        /// </summary>
        public SqLiteDbSetting()
            : this(true)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="SqLiteDbSetting"/> class.
        /// </summary>
        public SqLiteDbSetting(bool isExecuteReaderDisposable)
        {
            AreTableHintsSupported = false;
            ClosingQuote = "]";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = isExecuteReaderDisposable;
            IsMultiStatementExecutable = true;
            IsInsertAllBatchable = true;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = true;
            RequiresDbTypeBeforeValue = false;
            SkipsUnreferencedParameters = false;
            MaxParameterCount = 999;
            MultiStatementSeparator = ";";
            OpeningQuote = "[";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
