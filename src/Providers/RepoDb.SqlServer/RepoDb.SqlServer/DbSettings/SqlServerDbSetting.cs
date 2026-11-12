#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for SQL Server data provider. It can be inherited by the settings of the extension
    /// libraries (i.e. the SqlServerBulkOperationsDbSetting of RepoDb.SqlServer.BulkOperations).
    /// </summary>
    public class SqlServerDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlServerDbSetting"/> class.
        /// </summary>
        public SqlServerDbSetting()
        {
            AreIndexNamesUniquePerSchema = false;
            AreTableHintsSupported = true;
            ClosingQuote = "]";
            DefaultSchema = "dbo";
            IsAffectedRowsSupported = true;
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsInsertAllBatchable = true;
            IsPreparable = true;
            IsSchemaSupported = true;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            RequiresDbTypeBeforeValue = false;
            SkipsUnreferencedParameters = false;
            MaxParameterCount = 2100 - 2;
            MultiStatementSeparator = ";";
            OpeningQuote = "[";
            ParameterPrefix = "@";
            Provider = "MSSQL";
            SqlTextParameterPrefix = "@";
        }

        #region Public Methods

        /// <summary>
        /// Gets the <c>JSON_VALUE</c> function format, which also works on the native <c>json</c> type of SQL Server 2025.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        public override string GetJsonExtractFormat(Type valueType)
        {
            return string.Concat("JSON_VALUE({0}, ", JsonPathPlaceholder, ")");
        }

        #endregion
    }
}
