#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Firebird.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the Firebird bulk
    /// operations (<see cref="RepoDb.Firebird.BulkOperations"/>).
    /// </summary>
    public static partial class FirebirdTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string FirebirdBulkDelete = "FirebirdBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string FirebirdBulkDeleteByKey = "FirebirdBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string FirebirdBulkInsert = "FirebirdBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string FirebirdBulkMerge = "FirebirdBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string FirebirdBulkUpdate = "FirebirdBulkUpdate";
    }
}
