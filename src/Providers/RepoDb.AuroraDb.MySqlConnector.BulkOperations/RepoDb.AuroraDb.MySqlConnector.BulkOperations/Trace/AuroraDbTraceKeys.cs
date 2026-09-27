#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.AuroraDb.MySqlConnector.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the AuroraDb bulk
    /// operations (<see cref="RepoDb.AuroraDb.MySqlConnector.BulkOperations"/>).
    /// </summary>
    public static partial class AuroraDbTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string AuroraDbBulkDelete = "AuroraDbBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string AuroraDbBulkDeleteByKey = "AuroraDbBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string AuroraDbBulkInsert = "AuroraDbBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string AuroraDbBulkMerge = "AuroraDbBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string AuroraDbBulkUpdate = "AuroraDbBulkUpdate";
    }
}
