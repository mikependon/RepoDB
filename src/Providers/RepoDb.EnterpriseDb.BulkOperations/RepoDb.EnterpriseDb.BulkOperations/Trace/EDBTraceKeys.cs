#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.EnterpriseDb.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the EnterpriseDB bulk
    /// operations (<see cref="RepoDb.EnterpriseDb.BulkOperations"/>).
    /// </summary>
    public static partial class EDBTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string EDBBulkDelete = "EDBBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string EDBBulkDeleteByKey = "EDBBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string EDBBulkInsert = "EDBBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string EDBBulkMerge = "EDBBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string EDBBulkUpdate = "EDBBulkUpdate";
    }
}
