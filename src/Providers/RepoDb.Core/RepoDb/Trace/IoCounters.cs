#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Runtime.InteropServices;

namespace RepoDb
{
    /// <summary>
    /// The layout of the IO_COUNTERS structure of the operating system.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct IoCounters
    {
        #region Variables

        /// <summary>
        /// The number of read operations.
        /// </summary>
        public ulong ReadOperationCount;

        /// <summary>
        /// The number of write operations.
        /// </summary>
        public ulong WriteOperationCount;

        /// <summary>
        /// The number of other operations.
        /// </summary>
        public ulong OtherOperationCount;

        /// <summary>
        /// The number of bytes read.
        /// </summary>
        public ulong ReadTransferCount;

        /// <summary>
        /// The number of bytes written.
        /// </summary>
        public ulong WriteTransferCount;

        /// <summary>
        /// The number of bytes of the other operations.
        /// </summary>
        public ulong OtherTransferCount;

        #endregion
    }
}
