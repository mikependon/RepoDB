#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface that is used to mark a class to be expirable.
    /// </summary>
    public interface IExpirable
    {
        /// <summary>
        /// Gets the created timestamp of this class.
        /// </summary>
        DateTime CreatedDate { get; }

        /// <summary>
        /// Gets or sets the expiration date of this class.
        /// </summary>
        DateTime Expiration { get; set; }

        /// <summary>
        /// Identifies whether this class is expired.
        /// </summary>
        /// <returns>A boolean value that indicate whether this class is expired.</returns>
        bool IsExpired();
    }
}
