#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that a schema composer implements when the destination database can stop checking the foreign keys for the session of a connection,
    /// so rows can be loaded in any order, even into a table that references itself.
    /// </summary>
    public interface IConstraintCheckSupport
    {
        #region Public Methods

        /// <summary>
        /// Composes the statement that stops the foreign keys from being checked for the session of the connection that executes it.
        /// </summary>
        /// <param name="hasTransaction">Whether the statement is executed in a transaction, as the destination may need another statement then.</param>
        /// <returns>The SQL statement.</returns>
        string ComposeDisableConstraintChecks(bool hasTransaction);

        /// <summary>
        /// Composes the statement that makes the foreign keys be checked again for the session of the connection that executes it.
        /// </summary>
        /// <param name="hasTransaction">Whether the statement is executed in a transaction.</param>
        /// <returns>The SQL statement.</returns>
        string ComposeEnableConstraintChecks(bool hasTransaction);

        #endregion
    }
}
