#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace RepoDb
{
    /// <summary>
    /// Reads the bytes that were read and written by the current process: the IO counters on Windows, /proc/self/io on Linux and proc_pid_rusage on macOS.
    /// </summary>
    internal static class ProcessIo
    {
        #region Private Variables

        /// <summary>
        /// The version of the rusage_info structure that has the disk I/O fields.
        /// </summary>
        private const int RusageInfoV2 = 2;

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the handle of the current process.
        /// </summary>
        /// <returns>The pseudo handle of the current process.</returns>
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        /// <summary>
        /// Gets the IO counters of a process.
        /// </summary>
        /// <param name="process">The handle of the process.</param>
        /// <param name="counters">The IO counters.</param>
        /// <returns><c>true</c> if the counters are read.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetProcessIoCounters(IntPtr process,
            out IoCounters counters);

        /// <summary>
        /// Gets the resource usage of a process on macOS.
        /// </summary>
        /// <param name="pid">The identifier of the process.</param>
        /// <param name="flavor">The version of the structure (2 is the rusage_info_v2).</param>
        /// <param name="buffer">The resource usage.</param>
        /// <returns>0 if it succeeded.</returns>
        [DllImport("libproc", EntryPoint = "proc_pid_rusage")]
        private static extern int ProcPidRusage(int pid,
            int flavor,
            out RusageInfo buffer);

        /// <summary>
        /// Gets the identifier of the current process.
        /// </summary>
        /// <returns>The identifier of the process.</returns>
        private static int GetProcessId()
        {
#if NET
            return Environment.ProcessId;
#else
            return System.Diagnostics.Process.GetCurrentProcess().Id;
#endif
        }

        /// <summary>
        /// Reads the counters from the macOS operating system. They are the bytes that reached the disk, not the bytes requested by the process.
        /// </summary>
        /// <param name="read">The bytes that were read.</param>
        /// <param name="write">The bytes that were written.</param>
        /// <returns><c>true</c> if the counters are read.</returns>
        private static bool TryReadMac(out long read,
            out long write)
        {
            read = 0;
            write = 0;
            if (ProcPidRusage(GetProcessId(), RusageInfoV2, out var info) != 0)
            {
                return false;
            }

            read = (long)info.DiskIoBytesRead;
            write = (long)info.DiskIoBytesWritten;
            return true;
        }

        /// <summary>
        /// Checks whether the application runs on an operating system.
        /// </summary>
        /// <param name="platform">The operating system.</param>
        /// <returns><c>true</c> if the application runs on it.</returns>
        private static bool IsPlatform(OSPlatform platform)
        {
#if NET
            if (platform == OSPlatform.OSX)
            {
                return OperatingSystem.IsMacOS();
            }

            return platform == OSPlatform.Windows ? OperatingSystem.IsWindows() : OperatingSystem.IsLinux();
#else
            return RuntimeInformation.IsOSPlatform(platform);
#endif
        }

        /// <summary>
        /// Reads the counters from the Windows operating system.
        /// </summary>
        /// <param name="read">The bytes that were read.</param>
        /// <param name="write">The bytes that were written.</param>
        /// <returns><c>true</c> if the counters are read.</returns>
        private static bool TryReadWindows(out long read,
            out long write)
        {
            read = 0;
            write = 0;
            if (!GetProcessIoCounters(GetCurrentProcess(), out var counters))
            {
                return false;
            }

            read = (long)counters.ReadTransferCount;
            write = (long)counters.WriteTransferCount;
            return true;
        }

        /// <summary>
        /// Reads the counters from the Linux operating system.
        /// </summary>
        /// <param name="read">The bytes that were read.</param>
        /// <param name="write">The bytes that were written.</param>
        /// <returns><c>true</c> if the counters are read.</returns>
        private static bool TryReadLinux(out long read,
            out long write)
        {
            read = 0;
            write = 0;
            return File.Exists("/proc/self/io") &&
                TryParse(File.ReadAllText("/proc/self/io"), out read, out write);
        }

        /// <summary>
        /// Gets the value of a line of the form "name: value".
        /// </summary>
        /// <param name="text">The content of the file.</param>
        /// <param name="name">The name of the line.</param>
        /// <param name="value">The value of the line.</param>
        /// <returns><c>true</c> if the line is found.</returns>
        private static bool TryGetValue(string text,
            string name,
            out long value)
        {
            value = 0;
            foreach (var line in text.Split('\n'))
            {
                if (line.StartsWith(name + ":", StringComparison.Ordinal))
                {
                    return long.TryParse(line.Substring(name.Length + 1).Trim(),
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out value);
                }
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads the bytes that were read and written by the current process. Nothing is read, and no error is raised, on an operating system that is not supported.
        /// </summary>
        /// <param name="read">The bytes that were read.</param>
        /// <param name="write">The bytes that were written.</param>
        /// <returns><c>true</c> if the counters are read.</returns>
        public static bool TryRead(out long read,
            out long write)
        {
            read = 0;
            write = 0;
            try
            {
                if (IsPlatform(OSPlatform.Windows))
                {
                    return TryReadWindows(out read, out write);
                }

                if (IsPlatform(OSPlatform.OSX))
                {
                    return TryReadMac(out read, out write);
                }

                return IsPlatform(OSPlatform.Linux) &&
                    TryReadLinux(out read, out write);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the bytes that were read and written from the content of /proc/self/io (the rchar and wchar lines).
        /// </summary>
        /// <param name="text">The content of the file.</param>
        /// <param name="read">The bytes that were read.</param>
        /// <param name="write">The bytes that were written.</param>
        /// <returns><c>true</c> if both of the lines are found.</returns>
        public static bool TryParse(string text,
            out long read,
            out long write)
        {
            read = 0;
            write = 0;
            return text != null &&
                TryGetValue(text, "rchar", out read) &&
                TryGetValue(text, "wchar", out write);
        }

        #endregion
    }
}
