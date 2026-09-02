#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Attributes.Parameter.SapHana;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.SapHana.UnitTests.Attributes.Parameter.SapHana
{
    [TestClass]
    public class SourceColumnNullMappingAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<HanaConnection>(new SapHanaDbSetting(), true);
        }

        #region Classes

        private class SourceColumnNullMappingAttributeTestClass
        {
            [SourceColumnNullMapping(true)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestSourceColumnNullMappingAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new HanaConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new SourceColumnNullMappingAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.SourceColumnNullMapping);
        }

        [TestMethod]
        public void TestSourceColumnNullMappingAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new HanaConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(SourceColumnNullMappingAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.SourceColumnNullMapping);
        }
    }
}
