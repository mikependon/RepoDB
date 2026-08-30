#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySqlConnector;

namespace RepoDb.Attributes.Parameter.MySqlConnector
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="MySqlParameter.MySqlDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class MySqlDbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="mySqlDbType">A target <see cref="global::MySqlConnector.MySqlDbType"/> value.</param>
        public MySqlDbTypeAttribute(MySqlDbType mySqlDbType)
            : base(typeof(MySqlParameter), nameof(MySqlParameter.MySqlDbType), mySqlDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="global::MySqlConnector.MySqlDbType"/> value of the parameter.
        /// </summary>
        public MySqlDbType MySqlDbType => (MySqlDbType)Value;
    }
}