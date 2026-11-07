#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that a schema composer implements when it gives the formatter that converts the default values that come from a database of another provider.
    /// A default value that can not be converted is left out and reported as a warning, so the table is still created.
    /// </summary>
    public interface ISchemaValueSupport
    {
        #region Properties

        /// <summary>
        /// Gets the formatter of the database.
        /// </summary>
        ISchemaValueFormatter ValueFormatter { get; }

        #endregion
    }
}
