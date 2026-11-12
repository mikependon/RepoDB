#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Helpers;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Dialects
{
    /// <summary>
    /// The default <see cref="IDbJsonDialect"/>, which uses <c>JSON_EXTRACT</c> with a quoted JSON path and a string parameter.
    /// </summary>
    public class DefaultDbJsonDialect : IDbJsonDialect
    {
        #region Properties

        /// <summary>
        /// Gets the shared instance of the <see cref="DefaultDbJsonDialect"/>.
        /// </summary>
        public static DefaultDbJsonDialect Instance { get; } = new DefaultDbJsonDialect();

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the parameter as a string parameter.
        /// </summary>
        /// <param name="parameter">The parameter to configure.</param>
        public virtual void ConfigureParameter(IDbDataParameter parameter)
        {
            parameter.DbType = DbType.String;
        }

        /// <summary>
        /// Formats the segments as a quoted JSON path (i.e.: <c>'$.Address.City'</c>).
        /// </summary>
        /// <param name="segments">The property and array index segments.</param>
        /// <returns>The SQL literal of the path.</returns>
        public virtual string FormatPath(IReadOnlyList<string> segments)
        {
            return JsonPathBuilder.ToSqlLiteral(JsonPathBuilder.ToJsonPath(segments));
        }

        /// <summary>
        /// Gets the <c>JSON_EXTRACT({0}, path)</c> format.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        public virtual string GetExtractFormat(Type valueType)
        {
            return string.Concat("JSON_EXTRACT({0}, ", JsonConstant.PathPlaceholder, ")");
        }

        #endregion
    }
}
