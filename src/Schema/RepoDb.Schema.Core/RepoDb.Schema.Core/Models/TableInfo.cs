#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the identity of a table: its name and the name of the schema that owns it.
    /// </summary>
    public class TableInfo
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="schema">The name of the schema that owns the table.</param>
        public TableInfo(string name,
            string schema)
        {
            Name = name;
            Schema = schema;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the name of the schema that owns the table.
        /// </summary>
        public string Schema { get; internal set; }

        #endregion
    }
}
