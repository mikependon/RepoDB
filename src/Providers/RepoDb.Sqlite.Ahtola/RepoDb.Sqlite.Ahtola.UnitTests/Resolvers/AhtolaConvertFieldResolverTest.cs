#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Ahtola.UnitTests.Resolvers
{
    [TestClass]
    public class AhtolaConvertFieldResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAhtola();
        }

        #region MDS

        [TestMethod]
        public void TestMdsAhtolaConvertFieldResolverForInt32()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new AhtolaConvertFieldResolver();
            var field = new Field("Field", typeof(int));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [INT])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaConvertFieldResolverForInt64()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new AhtolaConvertFieldResolver();
            var field = new Field("Field", typeof(long));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [BIGINT])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaConvertFieldResolverForInt16()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new AhtolaConvertFieldResolver();
            var field = new Field("Field", typeof(short));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [INT])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaConvertFieldResolverForDateTime()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new AhtolaConvertFieldResolver();
            var field = new Field("Field", typeof(DateTime));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [DATETIME])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaConvertFieldResolverForString()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new AhtolaConvertFieldResolver();
            var field = new Field("Field", typeof(string));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [TEXT])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaConvertFieldResolverForByte()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new AhtolaConvertFieldResolver();
            var field = new Field("Field", typeof(byte));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [BLOB])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaConvertFieldResolverForDecimal()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new AhtolaConvertFieldResolver();
            var field = new Field("Field", typeof(decimal));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [DECIMAL])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaConvertFieldResolverForFloat()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new AhtolaConvertFieldResolver();
            var field = new Field("Field", typeof(float));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [REAL])", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaConvertFieldResolverForTimeSpan()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();
            var resolver = new AhtolaConvertFieldResolver();
            var field = new Field("Field", typeof(TimeSpan));

            // Act
            var result = resolver.Resolve(field, setting);

            // Assert
            Assert.AreEqual("CAST([Field] AS [TIME])", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
