#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;

namespace RepoDb.Dialects
{
    /// <summary>
    /// The JSON dialect of DuckDB, which uses <c>json_extract_string</c> cast to the type of the compared value.
    /// </summary>
    public class DuckDbDbJsonDialect : DefaultDbJsonDialect
    {
        private static readonly Dictionary<Type, string> CastTypes = new()
        {
            [typeof(bool)] = "BOOLEAN",
            [typeof(DateTime)] = "TIMESTAMP",
            [typeof(decimal)] = "DECIMAL(38,10)",
            [typeof(double)] = "DOUBLE",
            [typeof(float)] = "FLOAT",
            [typeof(Guid)] = "UUID",
            [typeof(int)] = "INTEGER",
            [typeof(long)] = "BIGINT",
            [typeof(short)] = "SMALLINT"
        };

        #region Public Methods

        /// <summary>
        /// Gets the <c>json_extract_string({0}, path)</c> format, cast to the type of the compared value.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        public override string GetExtractFormat(Type valueType)
        {
            var extract = string.Concat("json_extract_string({0}, ", JsonConstant.PathPlaceholder, ")");
            var type = valueType == null ? null : Nullable.GetUnderlyingType(valueType) ?? valueType;
            return type != null && CastTypes.TryGetValue(type, out var castType) ?
                string.Concat("CAST(", extract, " AS ", castType, ")") :
                extract;
        }

        #endregion
    }
}
