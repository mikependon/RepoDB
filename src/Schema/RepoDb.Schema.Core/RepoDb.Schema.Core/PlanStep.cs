#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that holds a statement of a schema copy and the table that it belongs to.
    /// </summary>
    internal sealed class PlanStep
    {
        #region Constructors

        public PlanStep(string statement,
            PlanTable owner)
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
        public PlanTable Owner { get; }

        #endregion
    }
}
