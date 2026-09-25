#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.DuckDb.BulkOperations
{
    /// <summary>
    /// The trace keys of the DuckDB bulk operations.
    /// </summary>
    public static partial class DuckDbTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string DuckDbBulkDelete = "DuckDbBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string DuckDbBulkDeleteByKey = "DuckDbBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string DuckDbBulkInsert = "DuckDbBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string DuckDbBulkMerge = "DuckDbBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string DuckDbBulkUpdate = "DuckDbBulkUpdate";
    }
}
