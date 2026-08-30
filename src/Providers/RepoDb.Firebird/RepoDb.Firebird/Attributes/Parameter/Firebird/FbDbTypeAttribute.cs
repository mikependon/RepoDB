#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.Attributes.Parameter.Firebird
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="FbParameter.FbDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class FbDbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="FbDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="fbDbType">A target <see cref="global::FirebirdSql.Data.FirebirdClient.FbDbType"/> value.</param>
        public FbDbTypeAttribute(FbDbType fbDbType)
            : base(typeof(FbParameter), nameof(FbParameter.FbDbType), fbDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="global::FirebirdSql.Data.FirebirdClient.FbDbType"/> value of the parameter.
        /// </summary>
        public FbDbType FbDbType => (FbDbType)Value;
    }
}
