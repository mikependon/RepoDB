#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.EnterpriseDb;
using System.Data;

namespace RepoDb.Attributes.Parameter.EnterpriseDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="EDBParameter.DbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class DbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbTypeAttribute"/> class.
        /// </summary>
        /// <param name="dbType">The equivalent <see cref="System.Data.DbType"/> value of the parameter.</param>
        public DbTypeAttribute(DbType dbType)
            : base(typeof(EDBParameter), nameof(EDBParameter.DbType), dbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="System.Data.DbType"/> value of the parameter.
        /// </summary>
        public DbType DbType => (DbType)Value;
    }
}
