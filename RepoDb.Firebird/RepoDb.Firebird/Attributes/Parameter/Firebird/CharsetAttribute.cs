#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.Attributes.Parameter.Firebird
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="FbParameter.Charset"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class CharsetAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="CharsetAttribute"/> class.
        /// </summary>
        /// <param name="charset">A target <see cref="global::FirebirdSql.Data.FirebirdClient.FbCharset"/> value.</param>
        public CharsetAttribute(FbCharset charset)
            : base(typeof(FbParameter), nameof(FbParameter.Charset), charset)
        { }

        /// <summary>
        /// Gets the mapped <see cref="global::FirebirdSql.Data.FirebirdClient.FbCharset"/> value of the parameter.
        /// </summary>
        public FbCharset Charset => (FbCharset)Value;
    }
}
