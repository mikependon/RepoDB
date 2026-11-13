#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;
using System.Data;

namespace RepoDb.Firebird.UnitTests.Dialects
{
    [TestClass]
    public class FirebirdDbJsonDialectTest
    {
        #region ConfigureParameter

        [TestMethod]
        public void TestFirebirdDbJsonDialectConfigureParameter()
        {
            // Setup
            var parameter = new FbParameter { DbType = DbType.Int32 };

            // Act
            new FirebirdDbJsonDialect().ConfigureParameter(parameter);

            // Assert
            Assert.AreEqual(DbType.String, parameter.DbType);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestFirebirdDbJsonDialectFormatPath()
        {
            // Act and Assert
            Assert.Throws<NotSupportedException>(() => new FirebirdDbJsonDialect().FormatPath(new[] { "Id" }));
        }

        #endregion

        #region GetExtractFormat

        [TestMethod]
        public void TestFirebirdDbJsonDialectGetExtractFormat()
        {
            // Act and Assert
            Assert.Throws<NotSupportedException>(() => new FirebirdDbJsonDialect().GetExtractFormat(typeof(int)));
        }

        #endregion
    }
}
