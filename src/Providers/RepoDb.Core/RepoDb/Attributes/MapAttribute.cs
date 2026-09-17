#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of the class/property into its equivalent object/field name in the database.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="MapAttribute"/> class.
    /// </remarks>
    /// <param name="name">The name of the mapping that is equivalent to the database object/field.</param>
    [AttributeUsage(AttributeTargets.All)]
    public class MapAttribute(string name) : Attribute
    {

        /// <summary>
        /// Gets the name of the mapping that is equivalent to the database object/field.
        /// </summary>
        public string Name { get; } = name;
    }
}
