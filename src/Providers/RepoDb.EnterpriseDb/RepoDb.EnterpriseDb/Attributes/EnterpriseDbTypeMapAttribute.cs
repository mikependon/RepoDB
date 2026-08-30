#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using EDBTypes;
using RepoDb.Attributes.Parameter.EnterpriseDb;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="EDBDbType"/> value.
    /// </summary>
    [Obsolete("Please use the RepoDb.Attributes.Parameter.EnterpriseDb.EnterpriseDbTypeAttribute instead.")]
    public class EnterpriseDbTypeMapAttribute : EnterpriseDbTypeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="EnterpriseDbTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="EDBDbType"/> value.</param>
        public EnterpriseDbTypeMapAttribute(EDBDbType dbType)
            : base(dbType)
        { }
    }
}