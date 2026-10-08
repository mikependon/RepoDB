#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb
{
    /// <summary>
    /// A class that holds the constant values for the library.
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// The default value of the batch operation size.
        /// </summary>
        public const int DefaultBatchOperationSize = 10;

        /// <summary>
        /// The default value of the cache expiration in minutes.
        /// </summary>
        public const int DefaultCacheItemExpirationInMinutes = 180;
    }
}
