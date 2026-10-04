#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the definition of a unique constraint of a table.
    /// </summary>
    public class UniqueConstraintInfo
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="UniqueConstraintInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the constraint or index.</param>
        public UniqueConstraintInfo(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the unique constraint.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the ordered names of the columns of the constraint.
        /// </summary>
        public IList<string> Columns { get; internal set; } = new List<string>();

        #endregion
    }
}
