#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion


namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the definition of a check constraint of a table.
    /// </summary>
    public class CheckConstraintInfo
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CheckConstraintInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the constraint or index.</param>
        public CheckConstraintInfo(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the check constraint.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the expression of the check constraint.
        /// </summary>
        public string Expression { get; internal set; }

        #endregion
    }
}
