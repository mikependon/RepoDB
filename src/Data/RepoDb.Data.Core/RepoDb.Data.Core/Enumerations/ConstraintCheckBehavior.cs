#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define whether the destination database checks the foreign keys while the rows are copied.
    /// </summary>
    public enum ConstraintCheckBehavior : short
    {
        /// <summary>
        /// The destination database checks the foreign keys as it always does. This is the default behavior.
        /// </summary>
        Enforce,

        /// <summary>
        /// The foreign keys are not checked while the rows are copied, and they are checked again when the copy ends (even if it fails), so the rows can be copied in any order,
        /// even into a table that references itself. The rows are not verified against the foreign keys, and the destination must support it, otherwise a <see cref="System.NotSupportedException"/> is thrown before anything is copied.
        /// </summary>
        Defer
    }
}
