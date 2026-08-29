#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.MySqlConnector.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the MySqlConnector bulk
    /// operations (<see cref="RepoDb.MySqlConnector.BulkOperations"/>).
    /// </summary>
    public static partial class MySqlConnectorTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string MySqlConnectorBulkDelete = "MySqlConnectorBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string MySqlConnectorBulkDeleteByKey = "MySqlConnectorBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string MySqlConnectorBulkInsert = "MySqlConnectorBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string MySqlConnectorBulkMerge = "MySqlConnectorBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string MySqlConnectorBulkUpdate = "MySqlConnectorBulkUpdate";
    }
}
