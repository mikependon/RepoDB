// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

using System.Data.Common;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration that defines the persistency of the <see cref="DbConnection"/> object used by the repository.
    /// </summary>
    public enum ConnectionPersistency
    {
        /// <summary>
        /// A new connection is being created on every call of the repository operation.
        /// </summary>
        PerCall,
        /// <summary>
        /// A single connection is being used until the lifetime of the repository.
        /// </summary>
        Instance,
        ///// <summary>
        ///// A single connection is being used by all repositories.
        ///// </summary>
        //Singleton
    }
}
