#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;

namespace RepoDb.Dialects
{
    /// <summary>
    /// The JSON dialect of Firebird, which cannot query inside a JSON text. Saving and loading the JSON text still works.
    /// </summary>
    public class FirebirdDbJsonDialect : DefaultDbJsonDialect
    {
        private const string Message = "Firebird cannot query the values inside a JSON column.";

        #region Public Methods

        /// <summary>
        /// Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="segments">The property and array index segments.</param>
        /// <returns>Never returns.</returns>
        public override string FormatPath(IReadOnlyList<string> segments) =>
            throw new NotSupportedException(Message);

        /// <summary>
        /// Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="valueType">The type of the value the extracted value is compared with, or <c>null</c> when unknown.</param>
        /// <returns>Never returns.</returns>
        public override string GetExtractFormat(Type valueType) =>
            throw new NotSupportedException(Message);

        #endregion
    }
}
