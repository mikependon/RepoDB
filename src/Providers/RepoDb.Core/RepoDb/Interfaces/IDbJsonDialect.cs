#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// The dialect of a database for its JSON columns.
    /// </summary>
    public interface IDbJsonDialect
    {
        #region Methods

        /// <summary>
        /// Configures the parameter that carries the JSON text of a <see cref="IDbJsonValue"/>.
        /// </summary>
        /// <param name="parameter">The parameter to configure.</param>
        void ConfigureParameter(IDbDataParameter parameter);

        /// <summary>
        /// Formats the segments of a JSON path as the path argument of the JSON function of the database.
        /// </summary>
        /// <param name="segments">The property and array index segments.</param>
        /// <returns>The SQL literal of the path.</returns>
        string FormatPath(IReadOnlyList<string> segments);

        /// <summary>
        /// Gets the format that extracts a value from a JSON column. The <c>{0}</c> is the column
        /// and <see cref="JsonConstant.PathPlaceholder"/> is the path.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        string GetExtractFormat(Type valueType);

        #endregion
    }
}
