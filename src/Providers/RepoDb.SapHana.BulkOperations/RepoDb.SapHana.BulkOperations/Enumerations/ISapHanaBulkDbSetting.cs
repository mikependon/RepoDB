#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;

namespace RepoDb.Enumerations.SapHana
{
    /// <summary>
    /// Defines a contract for the SAP HANA database setting used for bulk operations.
    /// </summary>
    public interface ISapHanaBulkDbSetting : IDbSetting
    {
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="SapHanaBulkCopy"/> class should use the <see cref="SapHanaCommandBatcher"/> class to batch the commands instead of executing them one by one.
        /// </summary>
        SapHanaWriteToServerExecution WriteToServerExecution { get; set; }
    }
}
