#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.DbSettings;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    public class FakeDbSetting : BaseDbSetting
    {
        public FakeDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "]";
            DefaultSchema = "dbo";
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = false;
            IsPreparable = true;
            IsTransactionSupported = false;
            IsUseUpsert = false;
            OpeningQuote = "[";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
            SchemaSeparator = ".";
        }
    }
}
