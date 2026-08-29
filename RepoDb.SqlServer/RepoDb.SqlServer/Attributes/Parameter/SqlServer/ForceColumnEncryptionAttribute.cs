#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.ForceColumnEncryption"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class ForceColumnEncryptionAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ForceColumnEncryptionAttribute"/> class.
        /// </summary>
        /// <param name="forceColumnEncryption">The value that determines whether the parameter is force encrypted when using Always Encrypted.</param>
        public ForceColumnEncryptionAttribute(bool forceColumnEncryption)
            : base(typeof(SqlParameter), nameof(SqlParameter.ForceColumnEncryption), forceColumnEncryption)
        { }

        /// <summary>
        /// Gets the mapped value that determines whether the parameter is forced encrypted.
        /// </summary>
        public bool ForceColumnEncryption => (bool)Value;
    }
}