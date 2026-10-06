#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that holds a statement of a schema copy and the table that it belongs to.
    /// </summary>
    internal sealed class CopySchemaStep
    {
        #region Constructors

        public CopySchemaStep(string statement,
            CopySchemaTable owner)
        {
            Statement = statement;
            Owner = owner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SQL statement.
        /// </summary>
        public string Statement { get; }

        /// <summary>
        /// Gets the table that the statement belongs to (<c>null</c> if it is not known).
        /// </summary>
        public CopySchemaTable Owner { get; }

        #endregion
    }
}
