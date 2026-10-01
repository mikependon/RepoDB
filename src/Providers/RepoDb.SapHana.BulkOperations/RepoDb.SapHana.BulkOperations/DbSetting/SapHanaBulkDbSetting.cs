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
        /// Gets or sets a value indicating whether the <see cref="SapHanaBulkCopy"/> class should use the <see cref="SapHanaCommandBatcher"/> class to batch the commands instead of executing them one by one.
        /// </summary>
        public SapHanaWriteToServerExecution WriteToServerExecution { get; set; } = SapHanaWriteToServerExecution.SapHanaCommandBatcher;

        /// <summary>
        /// Gets or sets the value that defines how the columns of the source are aligned with the columns of the destination
        /// table when no explicit mappings were passed to a bulk operation. Explicit mappings always take precedence over this
        /// setting. The default value is <see cref="SapHanaBulkColumnMappingsBehavior.Automatic"/>.
        /// </summary>
        public SapHanaBulkColumnMappingsBehavior BulkColumnMappingsBehavior { get; set; } = SapHanaBulkColumnMappingsBehavior.Automatic;
    }
}
