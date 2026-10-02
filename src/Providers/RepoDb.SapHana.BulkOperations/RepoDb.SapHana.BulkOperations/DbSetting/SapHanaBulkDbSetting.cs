#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Enumerations.SapHana;
using Sap.Data.Hana;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="HanaConnection"/> data provider.
    /// </summary>
    public sealed class SapHanaBulkDbSetting : SapHanaDbSetting, ISapHanaBulkDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="SapHanaBulkDbSetting"/> class.
        /// </summary>
        public SapHanaBulkDbSetting()
            : base()
        { }

        /// <summary>
        /// Gets or sets how the asynchronous <c>Bulk*Async</c> operations write rows into the destination (or pseudo/staging)
        /// table. The default, <see cref="SapHanaWriteToServerExecution.AsyncOverSync"/>, offloads the genuine, native
        /// <see cref="HanaBulkCopy"/> load onto a background thread - by far the fastest option, since the SAP HANA ADO.NET
        /// driver has no asynchronous bulk-copy API of its own to call into directly. <see cref="SapHanaWriteToServerExecution.SapHanaCommandBatcher"/>
        /// falls back to one parameterized <c>INSERT</c> round trip per row and should only be selected for environments
        /// where <see cref="HanaBulkCopy"/> itself cannot be used.
        /// </summary>
        public SapHanaWriteToServerExecution WriteToServerExecution { get; set; } = SapHanaWriteToServerExecution.AsyncOverSync;

        /// <summary>
        /// Gets or sets the value that defines how the columns of the source are aligned with the columns of the destination
        /// table when no explicit mappings were passed to a bulk operation. Explicit mappings always take precedence over this
        /// setting. The default value is <see cref="SapHanaBulkColumnMappingsBehavior.Automatic"/>.
        /// </summary>
        public SapHanaBulkColumnMappingsBehavior BulkColumnMappingsBehavior { get; set; } = SapHanaBulkColumnMappingsBehavior.Automatic;
    }
}
