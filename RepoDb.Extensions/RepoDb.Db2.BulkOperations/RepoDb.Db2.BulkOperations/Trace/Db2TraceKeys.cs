#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Db2.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the Db2 bulk
    /// operations (<see cref="RepoDb.Db2.BulkOperations"/>).
    /// </summary>
    public static partial class Db2TraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string Db2BulkDelete = "Db2BulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string Db2BulkDeleteByKey = "Db2BulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string Db2BulkInsert = "Db2BulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string Db2BulkMerge = "Db2BulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string Db2BulkUpdate = "Db2BulkUpdate";
    }
}
