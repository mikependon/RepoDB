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
    public class CopySchemaErrorTest
    {
        #region Methods

        [TestMethod]
        public void TestCopySchemaErrorExceptionPropertyDefaultValue()
        {
            // Act
            var error = new CopySchemaError();

            // Assert
            Assert.IsNull(error.Exception);
        }

        [TestMethod]
        public void TestCopySchemaErrorExceptionProperty()
        {
            // Setup
            var exception = new InvalidOperationException();

            // Act
            var error = new CopySchemaError
            {
                Exception = exception
            };

            // Assert
            Assert.AreSame(exception, error.Exception);
        }

        [TestMethod]
        public void TestCopySchemaErrorStringPropertiesDefaultValues()
        {
            // Act
            var error = new CopySchemaError();

            // Assert
            Assert.IsNull(error.Statement);
            Assert.IsNull(error.TableName);
            Assert.IsNull(error.SchemaName);
        }

        [TestMethod]
        public void TestCopySchemaErrorStringProperties()
        {
            // Act
            var error = new CopySchemaError
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
        public void TestCopySchemaErrorStatementIndexPropertyDefaultValue()
        {
            // Act
            var error = new CopySchemaError();

            // Assert
            Assert.AreEqual(0, error.StatementIndex);
        }

        [TestMethod]
        public void TestCopySchemaErrorStatementIndexProperty()
        {
            // Act
            var error = new CopySchemaError
            {
                StatementIndex = 7
            };

            // Assert
            Assert.AreEqual(7, error.StatementIndex);
        }

        #endregion
    }
}
