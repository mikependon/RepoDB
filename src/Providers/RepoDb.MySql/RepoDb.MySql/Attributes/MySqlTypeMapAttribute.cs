#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySql.Data.MySqlClient;
using RepoDb.Attributes.Parameter.MySql;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="MySqlDbType"/> value.
    /// </summary>
    [Obsolete("Use the RepoDb.Attributes.Parameter.MySqlDbTypeAttribute instead.")]
    public class MySqlTypeMapAttribute : MySqlDbTypeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="mySqlDbType">A target <see cref="MySqlDbType"/> value.</param>
        public MySqlTypeMapAttribute(MySqlDbType mySqlDbType)
            : base(mySqlDbType)
        { }
    }
}