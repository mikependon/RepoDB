#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;

namespace RepoDb.SqlServer.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps (by convention) to the [dbo].[StringKeyOutputTable] table, whose
    /// primary key is a string. The key is the column that is read back through the OUTPUT table
    /// of the Insert/InsertAll/Merge/MergeAll statements (#1385).
    /// </summary>
    public class StringKeyOutputTable
    {
        /// <summary>
        /// Gets or sets the string primary key.
        /// </summary>
        [Primary]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the group of the row. Together with the code, it is the qualifier of the merge.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the name of the row.
        /// </summary>
        public string Name { get; set; }
    }
}
