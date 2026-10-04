#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the full definition of a column of a table.
    /// </summary>
    public class ColumnInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base field information (name, type, size, precision, scale, nullability, primary and identity flags).
        /// </summary>
        public DbField Field { get; set; }

        /// <summary>
        /// Gets or sets the position of the column within the table (starting at 1).
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the default expression of the column, or <c>null</c> if there is none.
        /// </summary>
        public string DefaultExpression { get; set; }

        /// <summary>
        /// Gets or sets the seed of the identity column, or <c>null</c> if the column is not an identity.
        /// </summary>
        public long? IdentitySeed { get; set; }

        /// <summary>
        /// Gets or sets the increment of the identity column, or <c>null</c> if the column is not an identity.
        /// </summary>
        public long? IdentityIncrement { get; set; }

        /// <summary>
        /// Gets or sets the expression of a computed or generated column, or <c>null</c> if the column is not computed.
        /// </summary>
        public string ComputedExpression { get; set; }

        /// <summary>
        /// Gets or sets the collation of the column, or <c>null</c> if it is not applicable.
        /// </summary>
        public string Collation { get; set; }

        /// <summary>
        /// Gets or sets the comment of the column, or <c>null</c> if there is none.
        /// </summary>
        public string Comment { get; set; }

        #endregion
    }
}
