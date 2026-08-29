#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.SqlServer.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the SqlServer bulk
    /// operations (<see cref="RepoDb.SqlServer.BulkOperations"/>). Extends the core <see cref="TraceKeys"/>
    /// (defined in <c>RepoDb.Core</c>) rather than replacing it.
    /// </summary>
    public static partial class SqlServerTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string SqlServerBulkDelete = "SqlServerBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string SqlServerBulkDeleteByKey = "SqlServerBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string SqlServerBulkInsert = "SqlServerBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string SqlServerBulkMerge = "SqlServerBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string SqlServerBulkUpdate = "SqlServerBulkUpdate";
    }
}
