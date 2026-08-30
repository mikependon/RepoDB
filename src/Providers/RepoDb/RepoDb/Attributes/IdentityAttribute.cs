#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define an identity property for the data entity object.
    /// </summary>
    public class IdentityAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="IdentityAttribute"/> class.
        /// </summary>
        public IdentityAttribute() { }
    }
}
