#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// Registers the dialects that the tests use for the providers that they name, because the core knows no provider.
    /// </summary>
    [TestClass]
    public static class CustomSchemaDialects
    {
        #region Public Methods

        /// <summary>
        /// Registers the dialects of the providers that the tests name.
        /// </summary>
        /// <param name="context">The context of the test run.</param>
        [AssemblyInitialize]
        public static void Register(TestContext context)
        {
            SchemaDialectMapper.Add("MSSQL",
                CustomSchemaDialect.Instance);
            SchemaDialectMapper.Add("TestProvider",
                CustomSchemaDialect.Instance);
            SchemaDialectMapper.Add("PGSQL",
                OtherSchemaDialect.Instance);
        }

        #endregion
    }
}
