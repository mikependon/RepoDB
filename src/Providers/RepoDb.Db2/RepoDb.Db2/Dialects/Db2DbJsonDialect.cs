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
    /// The JSON dialect of Db2, which uses <c>JSON_VALUE</c> with a <c>RETURNING</c> type that matches the compared value.
    /// </summary>
    public class Db2DbJsonDialect : DefaultDbJsonDialect
    {
        private static readonly Dictionary<Type, string> ReturnTypes = new()
        {
            [typeof(decimal)] = "DECIMAL(31,10)",
            [typeof(double)] = "DOUBLE",
            [typeof(float)] = "REAL",
            [typeof(int)] = "INTEGER",
            [typeof(long)] = "BIGINT",
            [typeof(short)] = "SMALLINT"
        };

        #region Private Methods

        /// <summary>
        /// Gets the <c>RETURNING</c> type of the compared value.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The type name, or <c>null</c> when the text value is used.</returns>
        private static string GetReturnType(Type valueType)
        {
            var type = valueType == null ? null : Nullable.GetUnderlyingType(valueType) ?? valueType;
            return type != null && ReturnTypes.TryGetValue(type, out var returnType) ? returnType : null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the <c>JSON_VALUE({0}, path RETURNING type)</c> format, so that numbers are compared as numbers.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        public override string GetExtractFormat(Type valueType)
        {
            var returnType = GetReturnType(valueType);
            return returnType == null ?
                string.Concat("JSON_VALUE({0}, ", JsonConstant.PathPlaceholder, ")") :
                string.Concat("JSON_VALUE({0}, ", JsonConstant.PathPlaceholder, " RETURNING ", returnType, ")");
        }

        #endregion
    }
}
