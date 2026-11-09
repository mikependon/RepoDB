#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.SqlServer.IntegrationTests.Models
{
    /// <summary>
    /// A model that maps (by convention) to the [dbo].[InsertAllOutputOrderTable] table. The table has a non-clustered
    /// identity primary key and a clustered index on [SortKey], so that SQL Server may write (and OUTPUT) the rows of a
    /// batched insert in [SortKey] order rather than in the order of the source rows (#1393).
    /// </summary>
    public class InsertAllOutputOrderTable
    {
        public System.Int64 Id { get; set; }
        public System.Int32 SortKey { get; set; }
        public System.String Name { get; set; }
    }
}
