#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the Oracle bulk
    /// operations (<see cref="RepoDb.Oracle.BulkOperations"/>). Extends the core <see cref="OracleTraceKeys"/>
    /// (defined in <c>RepoDb.Core</c>) rather than replacing it.
    /// </summary>
    public static partial class OracleTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string OracleBulkDelete = "OracleBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string OracleBulkDeleteByKey = "OracleBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string OracleBulkInsert = "OracleBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string OracleBulkMerge = "OracleBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string OracleBulkUpdate = "OracleBulkUpdate";
    }
}
