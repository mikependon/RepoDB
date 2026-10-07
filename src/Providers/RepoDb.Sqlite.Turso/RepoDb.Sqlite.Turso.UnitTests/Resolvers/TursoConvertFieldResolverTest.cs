#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Turso.UnitTests.Resolvers
{
    [TestClass]
    public class TursoConvertFieldResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseTurso();
        }

        #region MDS

        [TestMethod]
        public void TestMdsTursoConvertFieldResolverForInt32()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new TursoConvertFieldResolver();
            var field = new Field("Field", typeof(int));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [INT])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoConvertFieldResolverForInt64()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new TursoConvertFieldResolver();
            var field = new Field("Field", typeof(long));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [BIGINT])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoConvertFieldResolverForInt16()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new TursoConvertFieldResolver();
            var field = new Field("Field", typeof(short));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [INT])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoConvertFieldResolverForDateTime()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new TursoConvertFieldResolver();
            var field = new Field("Field", typeof(DateTime));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [DATETIME])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoConvertFieldResolverForString()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new TursoConvertFieldResolver();
            var field = new Field("Field", typeof(string));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [TEXT])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoConvertFieldResolverForByte()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new TursoConvertFieldResolver();
            var field = new Field("Field", typeof(byte));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [BLOB])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoConvertFieldResolverForDecimal()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new TursoConvertFieldResolver();
            var field = new Field("Field", typeof(decimal));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [DECIMAL])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoConvertFieldResolverForFloat()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new TursoConvertFieldResolver();
            var field = new Field("Field", typeof(float));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [REAL])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoConvertFieldResolverForTimeSpan()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new TursoConvertFieldResolver();
            var field = new Field("Field", typeof(TimeSpan));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [TIME])", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
