#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Interfaces
{
    /// <summary>
    /// A value that is stored in a JSON column of the database.
    /// </summary>
    public interface IDbJsonValue
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the value has no JSON content.
        /// </summary>
        bool IsEmpty { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the JSON text of the value.
        /// </summary>
        /// <returns>The JSON text, or <c>null</c> when the value is empty.</returns>
        string ToJsonString();

        #endregion
    }
}
