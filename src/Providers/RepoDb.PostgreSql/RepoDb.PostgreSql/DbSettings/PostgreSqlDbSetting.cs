#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Npgsql;
using NpgsqlTypes;
using RepoDb.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="NpgsqlConnection"/> data provider. It can be inherited by the settings of the
    /// extension libraries (i.e. the PostgreSqlBulkOperationsDbSetting of RepoDb.PostgreSql.BulkOperations).
    /// </summary>
    public class PostgreSqlDbSetting : BaseDbSetting
    {
        private static readonly Dictionary<Type, string> JsonCastTypes = new()
        {
            [typeof(bool)] = "boolean",
            [typeof(DateTime)] = "timestamp",
            [typeof(decimal)] = "numeric",
            [typeof(double)] = "double precision",
            [typeof(float)] = "real",
            [typeof(Guid)] = "uuid",
            [typeof(int)] = "integer",
            [typeof(long)] = "bigint",
            [typeof(short)] = "smallint"
        };

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="PostgreSqlDbSetting"/> class.
        /// </summary>
        public PostgreSqlDbSetting()
        {
            AreIndexNamesUniquePerSchema = true;
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = "public";
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
            OpeningQuote = "\"";
            ParameterPrefix = "@";
            Provider = "PGSQL";
            SqlTextParameterPrefix = "@";
        }

        #endregion

        #region Private Methods

        private static string QuoteElement(string segment) =>
            segment.Any(c => !char.IsLetterOrDigit(c) && c != '_') ?
                string.Concat("\"", segment.Replace("\\", "\\\\").Replace("\"", "\\\""), "\"") :
                segment;

        private static string ToElement(string segment) =>
            JsonPathBuilder.IsIndex(segment) ? segment.Substring(1, segment.Length - 2) : QuoteElement(segment);

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the parameter as a <c>jsonb</c> parameter.
        /// </summary>
        /// <param name="parameter">The parameter to configure.</param>
        public override void ConfigureJsonParameter(IDbDataParameter parameter)
        {
            if (parameter is NpgsqlParameter npgsqlParameter)
            {
                npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
            }
            else
            {
                base.ConfigureJsonParameter(parameter);
            }
        }

        /// <summary>
        /// Formats the path as a PostgreSQL text array literal (i.e.: <c>'{Address,City}'</c>).
        /// </summary>
        /// <param name="segments">The property and array index segments.</param>
        /// <returns>The SQL literal of the path.</returns>
        public override string FormatJsonPath(IReadOnlyList<string> segments) =>
            JsonPathBuilder.ToSqlLiteral(string.Concat("{", string.Join(",", segments.Select(ToElement)), "}"));

        /// <summary>
        /// Gets the <c>#&gt;&gt;</c> operator format, cast to the type of the value the extracted value is compared with.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        public override string GetJsonExtractFormat(Type valueType)
        {
            var extract = string.Concat("{0} #>> ", JsonPathPlaceholder);
            var type = Nullable.GetUnderlyingType(valueType) ?? valueType;
            return type != null && JsonCastTypes.TryGetValue(type, out var castType) ?
                string.Concat("CAST(", extract, " AS ", castType, ")") :
                string.Concat("(", extract, ")");
        }

        #endregion
    }
}
