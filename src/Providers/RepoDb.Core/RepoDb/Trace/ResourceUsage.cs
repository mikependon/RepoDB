#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb
{
    /// <summary>
    /// The memory and the process I/O that were used while an operation was executing. The values are of the whole process, so they
    /// include the work of anything else that ran at the same time.
    /// </summary>
    public sealed class ResourceUsage
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ResourceUsage"/> class.
        /// </summary>
        /// <param name="memoryBytes">The change of the memory, or <c>null</c> if it is not available.</param>
        /// <param name="readBytes">The bytes that were read, or <c>null</c> if it is not available.</param>
        /// <param name="writeBytes">The bytes that were written, or <c>null</c> if it is not available.</param>
        public ResourceUsage(long? memoryBytes,
            long? readBytes,
            long? writeBytes)
        {
            MemoryBytes = memoryBytes;
            ReadBytes = readBytes;
            WriteBytes = writeBytes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the change of the memory of the process (its working set, as Desktop Studio reads it) during the execution. It is negative if the
        /// working set shrank, or <c>null</c> if it is not available.
        /// </summary>
        public long? MemoryBytes { get; }

        /// <summary>
        /// Gets the bytes that were read by the process, or <c>null</c> if it is not available on the operating system.
        /// </summary>
        public long? ReadBytes { get; }

        /// <summary>
        /// Gets the bytes that were written by the process, or <c>null</c> if it is not available on the operating system.
        /// </summary>
        public long? WriteBytes { get; }

        #endregion
    }
}
