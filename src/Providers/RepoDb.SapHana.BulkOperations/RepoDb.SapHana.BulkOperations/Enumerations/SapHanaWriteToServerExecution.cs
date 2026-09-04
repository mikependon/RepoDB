#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Enumerations.SapHana
{
    /// <summary>
    /// An enumeration that is being used to define the behavior of the <see cref="SapHanaBulkCopy"/> class when executing the bulk-import operation towards the underlying target table.
    /// </summary>
    public enum SapHanaWriteToServerExecution : short
    {
        /// <summary>
        /// A value that indicates whether the <see cref="SapHanaBulkCopy"/> class will use the <see cref="SapHanaCommandBatcher"/> class to batch the commands instead of executing them one by one.
        /// </summary>
        SapHanaCommandBatcher,

        /// <summary>
        /// A value that indicates whether the <see cref="SapHanaBulkCopy"/> class will use the asynchronous API of the <see cref="SapHanaCommandBatcher"/> class instead of the synchronous API of the <see cref="SapHanaBulkCopy"/> class.
        /// </summary>
        AsyncOverSync
    }
}
