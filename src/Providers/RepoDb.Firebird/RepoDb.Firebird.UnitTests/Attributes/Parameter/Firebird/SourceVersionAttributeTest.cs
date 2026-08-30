#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Attributes.Parameter.Firebird;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Firebird.UnitTests.Attributes.Parameter.Firebird
{
    [TestClass]
    public class SourceVersionAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<FbConnection>(new FirebirdDbSetting(), true);
        }

        #region Classes

        private class SourceVersionAttributeTestClass
        {
            [SourceVersion(DataRowVersion.Original)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestSourceVersionAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new FbConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new SourceVersionAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (FbParameter)command.Parameters["@ColumnName"];
            Assert.AreEqual(DataRowVersion.Original, parameter.SourceVersion);
        }

        [TestMethod]
        public void TestSourceVersionAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new FbConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(SourceVersionAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (FbParameter)command.Parameters["@ColumnName"];
            Assert.AreEqual(DataRowVersion.Original, parameter.SourceVersion);
        }
    }
}
