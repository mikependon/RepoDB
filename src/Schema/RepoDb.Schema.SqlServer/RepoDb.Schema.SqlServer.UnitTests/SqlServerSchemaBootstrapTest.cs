#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.SqlServer.UnitTests
{
    [TestClass]
    public class SqlServerSchemaBootstrapTest
    {
        #region Methods

        [TestMethod]
        public void TestSqlServerSchemaBootstrapIsInitializedAfterInitialize()
        {
            // Act
            SqlServerSchemaBootstrap.Initialize();

            // Assert
            Assert.IsTrue(SqlServerSchemaBootstrap.IsInitialized);
        }

        [TestMethod]
        public void TestSqlServerSchemaBootstrapRegistersTheSchemaComposer()
        {
            // Act
            SqlServerSchemaBootstrap.Initialize();
            var actual = SchemaComposerMapper.Get<SqlConnection>();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<SqlServerSchemaComposer>(actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaBootstrapCanBeInitializedMoreThanOnce()
        {
            // Act
            SqlServerSchemaBootstrap.Initialize();
            var first = SchemaComposerMapper.Get<SqlConnection>();
            SqlServerSchemaBootstrap.Initialize();
            var second = SchemaComposerMapper.Get<SqlConnection>();

            // Assert
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void TestSqlServerSchemaBootstrapDoesNotRegisterASchemaReader()
        {
            // Act
            SqlServerSchemaBootstrap.Initialize();
            var actual = SchemaReaderMapper.Get<SqlConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
