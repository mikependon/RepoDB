#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define the level (the kind) of a log of the copy operation.
    /// </summary>
    public enum LogLevel : short
    {
        /// <summary>
        /// An information about the copy operation.
        /// </summary>
        Info,

        /// <summary>
        /// A warning: the copy operation continued, but the result may not be the expected one (i.e.: the rows were skipped by the provider).
        /// </summary>
        Warning,

        /// <summary>
        /// An error: a part of the copy operation has failed.
        /// </summary>
        Error,

        /// <summary>
        /// A critical information: the copy operation can not continue or the data is not reliable anymore.
        /// </summary>
        Critical
    }
}
