#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.DbSettings;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbSetting : BaseDbSetting
    {
        public CustomDbSetting()
        {
            AreTableHintsSupported = true;
            ClosingQuote = "]";
            DefaultSchema = "dbo";
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "[";
            ParameterPrefix = "@";
        }
    }

    public class CustomNonHintsSupportingDbSetting : BaseDbSetting
    {
        public CustomNonHintsSupportingDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "]";
            DefaultSchema = "dbo";
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "[";
            ParameterPrefix = "@";

        }
    }

    public class CustomSingleStatementSupportDbSetting : BaseDbSetting
    {
        public CustomSingleStatementSupportDbSetting()
        {
            AreTableHintsSupported = true;
            ClosingQuote = "]";
            DefaultSchema = "dbo";
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = false;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "[";
            ParameterPrefix = "@";
        }
    }
}
