#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="HanaConnection"/> data provider.
    /// </summary>
    public sealed class SapHanaDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="SapHanaDbSetting"/> class.
        /// </summary>
        public SapHanaDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            // HANA's ADO.NET client rejects a command text containing more than one SQL statement - see
            // the remark on SapHanaStatementBuilder.CreateInsert - so RepoDb.Core must be told not to
            // batch multiple statements into one round-trip.
            IsMultiStatementExecutable = false;
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
