#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class SchemaCollationTest
    {
        #region Helpers

        private static ColumnInfo GetColumn(string collation,
            string provider) =>
            new ColumnInfo
            {
                Collation = collation,
                Field = new DbField("Name", false, false, true, typeof(string), 50, null, null, "varchar", false, provider)
            };

        #endregion

        #region Resolve

        [TestMethod]
        public void TestSchemaCollationResolveReturnsTheCollationIfTheProviderIsCompatible()
        {
            // Act
            var actual = SchemaCollation.Resolve(GetColumn("utf8mb4_bin", "MySql"), "MySql", "MySqlConnector");

            // Assert
            Assert.AreEqual("utf8mb4_bin", actual);
        }

        [TestMethod]
        public void TestSchemaCollationResolveMatchesTheProviderWithoutCase()
        {
            // Act
            var actual = SchemaCollation.Resolve(GetColumn("NOCASE", "sqlite"), "SQLite");

            // Assert
            Assert.AreEqual("NOCASE", actual);
        }

        [TestMethod]
        public void TestSchemaCollationResolveReturnsNullIfTheProviderIsNotCompatible()
        {
            // Act
            var actual = SchemaCollation.Resolve(GetColumn("SQL_Latin1_General_CP1_CI_AS", "SqlServer"), "MySql", "MySqlConnector");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestSchemaCollationResolveReturnsNullIfThereAreNoCompatibleProviders()
        {
            // Act/Assert
            Assert.IsNull(SchemaCollation.Resolve(GetColumn("utf8mb4_bin", "MySql")));
            Assert.IsNull(SchemaCollation.Resolve(GetColumn("utf8mb4_bin", "MySql"), (string[])null));
        }

        [TestMethod]
        public void TestSchemaCollationResolveReturnsTheCollationIfTheProviderIsUnknown()
        {
            // Act/Assert
            Assert.AreEqual("Latin1", SchemaCollation.Resolve(GetColumn("Latin1", null), "MySql"));
            Assert.AreEqual("Latin1", SchemaCollation.Resolve(GetColumn("Latin1", " "), "MySql"));
            Assert.AreEqual("Latin1", SchemaCollation.Resolve(new ColumnInfo { Collation = "Latin1" }, "MySql"));
        }

        [TestMethod]
        public void TestSchemaCollationResolveReturnsNullIfTheColumnHasNoCollation()
        {
            // Act/Assert
            Assert.IsNull(SchemaCollation.Resolve(GetColumn(null, "MySql"), "MySql"));
            Assert.IsNull(SchemaCollation.Resolve(GetColumn(" ", "MySql"), "MySql"));
            Assert.IsNull(SchemaCollation.Resolve(null, "MySql"));
        }

        #endregion
    }
}
