#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.CockroachDb.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the CockroachDB bulk
    /// operations (<see cref="RepoDb.CockroachDb.BulkOperations"/>).
    /// </summary>
    public static partial class CockroachDbTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string CockroachDbBulkDelete = "CockroachDbBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string CockroachDbBulkDeleteByKey = "CockroachDbBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string CockroachDbBulkInsert = "CockroachDbBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string CockroachDbBulkMerge = "CockroachDbBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string CockroachDbBulkUpdate = "CockroachDbBulkUpdate";
    }
}
