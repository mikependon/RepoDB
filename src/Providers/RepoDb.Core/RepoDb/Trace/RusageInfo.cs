#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Runtime.InteropServices;

namespace RepoDb
{
    /// <summary>
    /// The layout of the rusage_info_v2 structure of macOS, which is filled by proc_pid_rusage.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RusageInfo
    {
        #region Variables

        /// <summary>
        /// The first half of the 16 bytes of the unique identifier of the executable.
        /// </summary>
        public ulong UuidHigh;

        /// <summary>
        /// The second half of the 16 bytes of the unique identifier of the executable.
        /// </summary>
        public ulong UuidLow;

        /// <summary>
        /// The first of the 16 values before the disk I/O fields (times, wakeups, page-ins, sizes and the child fields), which are not used.
        /// </summary>
        public ulong Reserved0;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved1;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved2;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved3;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved4;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved5;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved6;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved7;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved8;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved9;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved10;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved11;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved12;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved13;

        /// <summary>
        /// Reserved (the 15th and 16th values: the child page-ins and the child elapsed time).
        /// </summary>
        public ulong Reserved14;

        /// <summary>
        /// Reserved.
        /// </summary>
        public ulong Reserved15;

        /// <summary>
        /// The number of bytes read from the disk.
        /// </summary>
        public ulong DiskIoBytesRead;

        /// <summary>
        /// The number of bytes written to the disk.
        /// </summary>
        public ulong DiskIoBytesWritten;

        #endregion
    }
}
