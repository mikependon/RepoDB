#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.LocaleId"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class LocaleIdAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="LocaleIdAttribute"/> class.
        /// </summary>
        /// <param name="localeId">The value of the locale identifier.</param>
        public LocaleIdAttribute(int localeId)
            : base(typeof(SqlParameter), nameof(SqlParameter.LocaleId), localeId)
        { }

        /// <summary>
        /// Gets the mapped value of the local identifier of the parameter.
        /// </summary>
        public int LocaleId => (int)Value;
    }
}