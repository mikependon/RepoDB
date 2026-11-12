#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
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

namespace RepoDb.Dialects
{
    /// <summary>
    /// The JSON dialect of PostgreSQL, which uses the <c>#&gt;&gt;</c> operator, text array paths and <c>jsonb</c> parameters.
    /// </summary>
    public class PostgreSqlDbJsonDialect : DefaultDbJsonDialect
    {
        private static readonly Dictionary<Type, string> CastTypes = new()
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
        public override void ConfigureParameter(IDbDataParameter parameter)
        {
            if (parameter is NpgsqlParameter npgsqlParameter)
            {
                npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
            }
            else
            {
                base.ConfigureParameter(parameter);
            }
        }

        /// <summary>
        /// Formats the path as a PostgreSQL text array literal (i.e.: <c>'{Address,City}'</c>).
        /// </summary>
        /// <param name="segments">The property and array index segments.</param>
        /// <returns>The SQL literal of the path.</returns>
        public override string FormatPath(IReadOnlyList<string> segments) =>
            JsonPathBuilder.ToSqlLiteral(string.Concat("{", string.Join(",", segments.Select(ToElement)), "}"));

        /// <summary>
        /// Gets the <c>#&gt;&gt;</c> operator format, cast to the type of the value the extracted value is compared with.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        public override string GetExtractFormat(Type valueType)
        {
            var extract = string.Concat("{0} #>> ", JsonConstant.PathPlaceholder);
            var type = valueType == null ? null : Nullable.GetUnderlyingType(valueType) ?? valueType;
            return type != null && CastTypes.TryGetValue(type, out var castType) ?
                string.Concat("CAST(", extract, " AS ", castType, ")") :
                string.Concat("(", extract, ")");
        }

        #endregion
    }
}
