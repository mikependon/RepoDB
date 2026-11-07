#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// A dialect of another made-up database, so the tests can convert from one dialect to another. It is of the family <c>Other</c> and it has the standard behavior.
    /// </summary>
    public class OtherSchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="OtherSchemaDialect"/> class.
        /// </summary>
        public OtherSchemaDialect() : base("Other")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the single instance of the dialect.
        /// </summary>
        public static OtherSchemaDialect Instance { get; } = new OtherSchemaDialect();

        #endregion
    }
}
