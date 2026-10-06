#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the internal extension methods for <see cref="DataTable"/>.
    /// </summary>
    internal static class DataTableExtension
    {
        /// <summary>
        /// Adds a column of the given type into the <see cref="DataTable"/>.
        /// </summary>
        /// <param name="table">The target table.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="type">The type of the column.</param>
        /// <returns>The added column.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2067:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method.",
            Justification = "The column types are the CLR types of the database fields (i.e.: primitives, strings, byte arrays, date/time types), " +
                "whose members are not reflected by the DataTable.")]
        internal static DataColumn AddColumn(this DataTable table,
            string columnName,
            Type type) =>
            table.Columns.Add(columnName, type);
    }
}
