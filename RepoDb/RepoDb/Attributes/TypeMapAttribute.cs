// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

using System;
using System.Data;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of data entity property type into its equivalent database type.
    /// </summary>
    public class TypeMapAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="TypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target database type.</param>
        public TypeMapAttribute(DbType dbType)
        {
            DbType = dbType;
        }

        /// <summary>
        /// Gets a database type that is currently mapped.
        /// </summary>
        public DbType DbType { get; }
    }
}