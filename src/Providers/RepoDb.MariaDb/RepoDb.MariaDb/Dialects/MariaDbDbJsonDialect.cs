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
    /// The JSON dialect of MariaDB, which unquotes the value returned by <c>JSON_EXTRACT</c>.
    /// </summary>
    public class MariaDbDbJsonDialect : DefaultDbJsonDialect
    {
        #region Private Methods

        /// <summary>
        /// Gets the format of the unquoted JSON extraction.
        /// </summary>
        /// <returns>The format of the JSON extraction.</returns>
        private static string GetUnquotedExtract() =>
            string.Concat("JSON_UNQUOTE(JSON_EXTRACT({0}, ", JsonConstant.PathPlaceholder, "))");

        /// <summary>
        /// Checks whether the type is a boolean (or a nullable boolean).
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns><c>true</c> if the type is a boolean.</returns>
        private static bool IsBoolean(Type valueType) =>
            valueType != null && (Nullable.GetUnderlyingType(valueType) ?? valueType) == typeof(bool);

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the <c>JSON_UNQUOTE(JSON_EXTRACT({0}, path))</c> format. A boolean is compared as the <c>'true'</c> text.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>The format of the JSON extraction.</returns>
        public override string GetExtractFormat(Type valueType) =>
            IsBoolean(valueType) ? string.Concat("(", GetUnquotedExtract(), " = 'true')") : GetUnquotedExtract();

        #endregion
    }
}
