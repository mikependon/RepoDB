#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that a schema composer implements when it creates the default value of a column from its kind (see <see cref="Enumerations.ColumnDefaultValue"/>), and
    /// there are default values that the destination database can not have (i.e.: the next value of a sequence in MySQL). The column is created without its default value,
    /// and the reason is reported as a warning of the <see cref="CopyTableSchemaResult"/>, so the table is still created.
    /// </summary>
    public interface IColumnDefaultSupport
    {
        /// <summary>
        /// Gets the reason why the destination database does not create the default value of the column.
        /// </summary>
        /// <param name="column">The column of the source table.</param>
        /// <returns>The reason (i.e.: <c>its value comes from a sequence, which MySQL can not use as a default value</c>), or <c>null</c> if the column has no default value or it is created.</returns>
        string GetUnsupportedDefaultReason(ColumnInfo column);
    }
}
