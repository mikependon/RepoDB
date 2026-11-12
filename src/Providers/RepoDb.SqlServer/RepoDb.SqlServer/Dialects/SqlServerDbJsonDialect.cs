#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Dialects
{
    /// <summary>
    /// The JSON dialect of SQL Server, which uses <c>JSON_VALUE</c> and also works on the native <c>json</c> type of SQL Server 2025.
    /// </summary>
    public class SqlServerDbJsonDialect : DefaultDbJsonDialect
    {
        #region Public Methods

        /// <summary>
        /// Gets the <c>JSON_VALUE({0}, path)</c> format.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        public override string GetExtractFormat(Type valueType)
        {
            return string.Concat("JSON_VALUE({0}, ", JsonConstant.PathPlaceholder, ")");
        }

        #endregion
    }
}
