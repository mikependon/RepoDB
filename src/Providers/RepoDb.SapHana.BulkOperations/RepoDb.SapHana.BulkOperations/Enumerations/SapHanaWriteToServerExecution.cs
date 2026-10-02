#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Enumerations.SapHana
{
    /// <summary>
    /// An enumeration used to define how an asynchronous <c>Bulk*Async</c> operation writes rows into SAP HANA. The SAP
    /// HANA ADO.NET driver (<c>Sap.Data.Hana</c>) exposes no asynchronous bulk-copy API, so every option here is a
    /// trade-off between genuine throughput and true non-blocking I/O.
    /// </summary>
    public enum SapHanaWriteToServerExecution : short
    {
        /// <summary>
        /// Uses <see cref="SapHanaCommandBatcher"/> to execute one parameterized <c>INSERT</c> round trip per row. This is
        /// a compatibility fallback only - it is dramatically slower than <see cref="AsyncOverSync"/> for any non-trivial
        /// row count, since it pays one network round trip per row instead of one bulk load for the whole batch.
        /// </summary>
        SapHanaCommandBatcher,

        /// <summary>
        /// The default. Offloads the genuine, synchronous <see cref="Sap.Data.Hana.HanaBulkCopy"/> bulk-load call onto a
        /// background thread via <c>Task.Run</c>, so the calling thread isn't blocked even though the underlying driver
        /// call itself is synchronous. By far the fastest option for any non-trivial row count.
        /// </summary>
        AsyncOverSync
    }
}
