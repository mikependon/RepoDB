#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoDb.Dialects
{
    /// <summary>
    /// The JSON dialect of Vertica, which looks up the flattened keys of <c>MAPJSONEXTRACTOR</c> (i.e.: <c>Address.City</c> or <c>Tags.0</c>).
    /// </summary>
    public class VerticaDbJsonDialect : DefaultDbJsonDialect
    {
        #region Private Methods

        /// <summary>
        /// Appends the segment to the flattened key.
        /// </summary>
        /// <param name="builder">The builder to append to.</param>
        /// <param name="segment">The property or array index segment.</param>
        private static void Append(StringBuilder builder,
            string segment)
        {
            if (JsonPathBuilder.IsIndex(segment))
            {
                builder.Append(builder.Length > 0 ? "." : string.Empty).Append(segment, 1, segment.Length - 2);
            }
            else
            {
                builder.Append(builder.Length > 0 ? "." : string.Empty).Append(segment);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Formats the path as the flattened key literal (i.e.: <c>'Address.City'</c>).
        /// </summary>
        /// <param name="segments">The property and array index segments.</param>
        /// <returns>The SQL literal of the key.</returns>
        public override string FormatPath(IReadOnlyList<string> segments)
        {
            var builder = new StringBuilder();
            foreach (var segment in segments)
            {
                Append(builder, segment);
            }
            return JsonPathBuilder.ToSqlLiteral(builder.ToString());
        }

        /// <summary>
        /// Gets the <c>MAPLOOKUP(MAPJSONEXTRACTOR({0}), key)</c> format.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        public override string GetExtractFormat(Type valueType) =>
            string.Concat("MAPLOOKUP(MAPJSONEXTRACTOR({0}), ", JsonConstant.PathPlaceholder, ")");

        #endregion
    }
}
