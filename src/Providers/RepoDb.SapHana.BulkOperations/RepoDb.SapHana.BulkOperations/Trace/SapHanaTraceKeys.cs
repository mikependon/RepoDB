#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.SapHana.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the SapHana bulk
    /// operations (<see cref="RepoDb.SapHana.BulkOperations"/>).
    /// </summary>
    public static partial class SapHanaTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string SapHanaBulkDelete = "SapHanaBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string SapHanaBulkDeleteByKey = "SapHanaBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string SapHanaBulkInsert = "SapHanaBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string SapHanaBulkMerge = "SapHanaBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string SapHanaBulkUpdate = "SapHanaBulkUpdate";
    }
}
