#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.DbSettings;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// A fake connection whose statements use the prefix <c>$</c> for their parameters (as DuckDB does).
    /// </summary>
    public class FakeDollarDbConnection : FakeDbConnection
    {
        #region Constructors

        /// <summary>
        /// Maps the setting and the helper of the connection.
        /// </summary>
        static FakeDollarDbConnection()
        {
            DbSettingMapper.Add<FakeDollarDbConnection>(new FakeDollarDbSetting(), true);
            DbHelperMapper.Add<FakeDollarDbConnection>(new FakeDbHelper(), true);
        }

        #endregion
    }

    /// <summary>
    /// The setting of the fake connection that uses the prefix <c>$</c> for the parameters of its statements.
    /// </summary>
    public class FakeDollarDbSetting : BaseDbSetting
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="FakeDollarDbSetting"/> class.
        /// </summary>
        public FakeDollarDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = false;
            IsPreparable = true;
            IsTransactionSupported = false;
            IsUseUpsert = false;
            OpeningQuote = "\"";
            ParameterPrefix = "$";
            SqlTextParameterPrefix = "$";
            SchemaSeparator = ".";
        }

        #endregion
    }
}
