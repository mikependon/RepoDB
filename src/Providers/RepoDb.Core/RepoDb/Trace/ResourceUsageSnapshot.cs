#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RepoDb
{
    /// <summary>
    /// A reading of the memory and the process I/O at a point in time.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct ResourceUsageSnapshot
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ResourceUsageSnapshot"/> struct.
        /// </summary>
        /// <param name="memoryBytes">The working set of the process, or <c>null</c> if it is not available.</param>
        /// <param name="readBytes">The bytes that were read so far, or <c>null</c> if it is not available.</param>
        /// <param name="writeBytes">The bytes that were written so far, or <c>null</c> if it is not available.</param>
        public ResourceUsageSnapshot(long? memoryBytes,
            long? readBytes,
            long? writeBytes)
        {
            MemoryBytes = memoryBytes;
            ReadBytes = readBytes;
            WriteBytes = writeBytes;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the difference of two values.
        /// </summary>
        /// <param name="start">The first value.</param>
        /// <param name="end">The last value.</param>
        /// <returns>The difference, or <c>null</c> if one of the values is not available.</returns>
        private static long? Difference(long? start,
            long? end) =>
            start.HasValue && end.HasValue ? end.Value - start.Value : null;

        /// <summary>
        /// Gets the working set of the process, which is the memory that the process holds in the physical memory.
        /// </summary>
        /// <returns>The bytes, or <c>null</c> if it is not available.</returns>
        private static long? GetMemoryBytes()
        {
            try
            {
                using var process = Process.GetCurrentProcess();
                return process.WorkingSet64;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Takes a reading now.
        /// </summary>
        /// <returns>The reading.</returns>
        public static ResourceUsageSnapshot Capture()
        {
            return ProcessIo.TryRead(out var read, out var write)
                ? new ResourceUsageSnapshot(GetMemoryBytes(), read, write)
                : new ResourceUsageSnapshot(GetMemoryBytes(), null, null);
        }

        /// <summary>
        /// Gets the difference of this reading from an earlier one. The memory can be negative (the working set shrinks), the I/O never is.
        /// </summary>
        /// <param name="start">The earlier reading.</param>
        /// <returns>The usage between the two readings.</returns>
        public ResourceUsage Subtract(ResourceUsageSnapshot start)
        {
            return new ResourceUsage(Difference(start.MemoryBytes, MemoryBytes),
                Difference(start.ReadBytes, ReadBytes) is { } read ? Math.Max(0, read) : null,
                Difference(start.WriteBytes, WriteBytes) is { } write ? Math.Max(0, write) : null);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the working set of the process.
        /// </summary>
        public long? MemoryBytes { get; }

        /// <summary>
        /// Gets the bytes that were read so far.
        /// </summary>
        public long? ReadBytes { get; }

        /// <summary>
        /// Gets the bytes that were written so far.
        /// </summary>
        public long? WriteBytes { get; }

        #endregion
    }
}
