// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a primary property for the data entity object.
    /// </summary>
    public class PrimaryAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="PrimaryAttribute"/> class.
        /// </summary>
        public PrimaryAttribute() { }
    }
}
