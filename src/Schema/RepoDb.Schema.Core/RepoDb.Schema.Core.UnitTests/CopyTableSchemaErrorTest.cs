#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class CopyTableSchemaErrorTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyTableSchemaErrorExceptionPropertyDefaultValue()
        {
            // Act
            var error = new CopyTableSchemaError();

            // Assert
            Assert.IsNull(error.Exception);
        }

        [TestMethod]
        public void TestCopyTableSchemaErrorExceptionProperty()
        {
            // Setup
            var exception = new InvalidOperationException();

            // Act
            var error = new CopyTableSchemaError
            {
                Exception = exception
            };

            // Assert
            Assert.AreSame(exception, error.Exception);
        }

        [TestMethod]
        public void TestCopyTableSchemaErrorStringPropertiesDefaultValues()
        {
            // Act
            var error = new CopyTableSchemaError();

            // Assert
            Assert.IsNull(error.Statement);
            Assert.IsNull(error.TableName);
            Assert.IsNull(error.SchemaName);
        }

        [TestMethod]
        public void TestCopyTableSchemaErrorStringProperties()
        {
            // Act
            var error = new CopyTableSchemaError
            {
                Statement = "CREATE TABLE [Person] ([Id] int);",
                TableName = "Person",
                SchemaName = "dbo"
            };

            // Assert
            Assert.AreEqual("CREATE TABLE [Person] ([Id] int);", error.Statement, StringComparer.Ordinal);
            Assert.AreEqual("Person", error.TableName, StringComparer.Ordinal);
            Assert.AreEqual("dbo", error.SchemaName, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaErrorStatementIndexPropertyDefaultValue()
        {
            // Act
            var error = new CopyTableSchemaError();

            // Assert
            Assert.AreEqual(0, error.StatementIndex);
        }

        [TestMethod]
        public void TestCopyTableSchemaErrorStatementIndexProperty()
        {
            // Act
            var error = new CopyTableSchemaError
            {
                StatementIndex = 7
            };

            // Assert
            Assert.AreEqual(7, error.StatementIndex);
        }

        #endregion
    }
}
