#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define the behavior of the identity (and auto-increment) columns when the rows of a JSON file are loaded into a table. It is passed on to the
    /// bulk insert of the provider of the connection (i.e.: the <c>KeepIdentity</c> option of <c>SqlBulkCopyOptions</c>, or the <c>identityBehavior</c> argument of PostgreSQL), and it is
    /// honored by the batch insert as well.
    /// </summary>
    public enum LoadIdentityBehavior : short
    {
        /// <summary>
        /// The values of the identity columns that the file has are kept and used as they are. This is the default behavior. The database must allow explicit values for the
        /// identity columns (i.e.: SQL Server, and PostgreSQL <c>GENERATED ALWAYS</c> columns, may not).
        /// </summary>
        KeepIdentity,

        /// <summary>
        /// The values of the identity columns that the file has are ignored, and the database generates new ones. As the rows come from a file (there are no entities), the generated
        /// values are not set back to anything: they can be read from the table.
        /// </summary>
        ReturnIdentity
    }
}
