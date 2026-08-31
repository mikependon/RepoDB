#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;

namespace RepoDb.Attributes.Parameter.SapHana
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="HanaParameter.HanaDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SapHanaDbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SapHanaDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="hanaDbType">A target <see cref="global::Sap.Data.Hana.HanaDbType"/> value.</param>
        public SapHanaDbTypeAttribute(HanaDbType hanaDbType)
            : base(typeof(HanaParameter), nameof(HanaParameter.HanaDbType), hanaDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="global::Sap.Data.Hana.HanaDbType"/> value of the parameter.
        /// </summary>
        public HanaDbType HanaDbType => (HanaDbType)Value;
    }
}
