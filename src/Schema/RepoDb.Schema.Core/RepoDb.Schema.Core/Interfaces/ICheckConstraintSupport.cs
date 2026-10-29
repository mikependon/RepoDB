#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that a schema composer implements when the destination database refuses some check constraints (i.e.: MySQL does not allow a function
    /// that is not deterministic, like <c>NOW()</c>, in a <c>CHECK</c>). The check constraints that are refused are left out when the table is created,
    /// and reported as a warning of the <see cref="CopyTableSchemaResult"/>, so the table is created with its other constraints.
    /// </summary>
    public interface ICheckConstraintSupport
    {
        #region Public Methods

        /// <summary>
        /// Gets the reason why the destination database refuses the check constraint.
        /// </summary>
        /// <param name="checkConstraint">The check constraint of the source table.</param>
        /// <returns>The reason (i.e.: <c>its expression uses NOW(), which MySQL does not allow in a CHECK constraint</c>), or <c>null</c> if the check constraint can be created.</returns>
        string GetUnsupportedReason(CheckConstraintInfo checkConstraint);

        #endregion
    }
}
