#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RepoDb.Dialects
{
    /// <summary>
    /// The JSON dialect of ClickHouse, which uses the typed <c>JSONExtract</c> functions with the path as separate arguments.
    /// </summary>
    public class ClickHouseDbJsonDialect : DefaultDbJsonDialect
    {
        private static readonly Dictionary<Type, string> Functions = new()
        {
            [typeof(bool)] = "JSONExtractBool",
            [typeof(byte)] = "JSONExtractUInt",
            [typeof(decimal)] = "JSONExtractFloat",
            [typeof(double)] = "JSONExtractFloat",
            [typeof(float)] = "JSONExtractFloat",
            [typeof(int)] = "JSONExtractInt",
            [typeof(long)] = "JSONExtractInt",
            [typeof(sbyte)] = "JSONExtractInt",
            [typeof(short)] = "JSONExtractInt",
            [typeof(uint)] = "JSONExtractUInt",
            [typeof(ulong)] = "JSONExtractUInt",
            [typeof(ushort)] = "JSONExtractUInt"
        };

        #region Private Methods

        /// <summary>
        /// Gets the <c>JSONExtract</c> function that matches the type of the compared value.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The name of the function.</returns>
        private static string GetFunction(Type valueType)
        {
            var type = valueType == null ? null : Nullable.GetUnderlyingType(valueType) ?? valueType;
            return type != null && Functions.TryGetValue(type, out var function) ? function : "JSONExtractString";
        }

        /// <summary>
        /// Formats a path segment as an argument of the function.
        /// </summary>
        /// <param name="segment">The property or array index segment.</param>
        /// <returns>The quoted property name, or the 1-based array position.</returns>
        private static string ToArgument(string segment) =>
            JsonPathBuilder.IsIndex(segment) ? ToPosition(segment) : JsonPathBuilder.ToSqlLiteral(segment);

        /// <summary>
        /// Converts an array index segment into its 1-based position.
        /// </summary>
        /// <param name="segment">The property or array index segment.</param>
        /// <returns>The position as text.</returns>
        private static string ToPosition(string segment) =>
            (int.Parse(segment.Substring(1, segment.Length - 2), CultureInfo.InvariantCulture) + 1).ToString(CultureInfo.InvariantCulture);

        #endregion

        #region Public Methods

        /// <summary>
        /// Formats the path as the arguments of the function, where the array indexes are 1-based (i.e.: <c>'Tags',1</c>).
        /// </summary>
        /// <param name="segments">The property and array index segments.</param>
        /// <returns>The arguments of the path.</returns>
        public override string FormatPath(IReadOnlyList<string> segments) =>
            string.Join(",", segments.Select(ToArgument));

        /// <summary>
        /// Gets the <c>JSONExtract</c> function format that matches the type of the compared value.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        public override string GetExtractFormat(Type valueType) =>
            string.Concat(GetFunction(valueType), "({0}, ", JsonConstant.PathPlaceholder, ")");

        #endregion
    }
}
