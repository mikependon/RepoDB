#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using EnterpriseDB.EDBClient;
using EDBTypes;

namespace RepoDb.Attributes.Parameter.EnterpriseDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="EDBParameter.EDBDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class EnterpriseDbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="EnterpriseDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="edbDbType">The target <see cref="EDBTypes.EDBDbType"/> value.</param>
        public EnterpriseDbTypeAttribute(EDBDbType edbDbType)
            : base(typeof(EDBParameter), nameof(EDBParameter.EDBDbType), edbDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="EDBTypes.EDBDbType"/> value of the parameter.
        /// </summary>
        public EDBDbType EDBDbType => (EDBDbType)Value;
    }
}