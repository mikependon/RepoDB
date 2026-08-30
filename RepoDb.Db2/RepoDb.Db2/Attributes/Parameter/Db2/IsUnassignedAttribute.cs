#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;

namespace RepoDb.Attributes.Parameter.Db2
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DB2Parameter.IsUnassigned"/>
    /// property via an entity property before the actual execution. Indicates whether the
    /// parameter is treated as an unassigned value by the data server.
    /// </summary>
    public class IsUnassignedAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="IsUnassignedAttribute"/> class.
        /// </summary>
        /// <param name="isUnassigned">The value that indicates whether the parameter is treated as an unassigned value by the data server.</param>
        public IsUnassignedAttribute(bool isUnassigned)
            : base(typeof(DB2Parameter), nameof(DB2Parameter.IsUnassigned), isUnassigned)
        { }

        /// <summary>
        /// Gets the mapped value that indicates whether the parameter is treated as an unassigned value by the data server.
        /// </summary>
        public bool IsUnassigned => (bool)Value;
    }
}
