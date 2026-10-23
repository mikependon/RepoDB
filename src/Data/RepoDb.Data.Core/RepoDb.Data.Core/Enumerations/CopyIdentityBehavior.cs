#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define the behavior of the identity (and auto-increment) columns of the target table when the rows of the source table are copied. It is passed on to the
    /// bulk insert of the provider of the destination connection (i.e.: the <c>KeepIdentity</c> option of <c>SqlBulkCopyOptions</c>, or the <c>identityBehavior</c> argument of PostgreSQL),
    /// and it is honored by the batch insert as well.
    /// </summary>
    public enum CopyIdentityBehavior : short
    {
        /// <summary>
        /// The values of the identity columns of the source table are copied as they are, so the rows of the related tables keep referencing them. This is the default behavior.
        /// The database of the target table must allow explicit values for the identity columns (i.e.: SQL Server, and PostgreSQL <c>GENERATED ALWAYS</c> columns, may not).
        /// </summary>
        KeepIdentity,

        /// <summary>
        /// The values of the identity columns of the source table are not copied, and the database of the target table generates new ones. The generated values are not set back to anything:
        /// they can be read from the target table. As the rows of the related tables would no longer reference the rows that they referenced, it can only be used with
        /// <see cref="CopyRelationshipBehavior.TableOnly"/>.
        /// </summary>
        ReturnIdentity
    }
}
