#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter.Db2
{
    [TestClass]
    public class SourceVersionAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<DB2Connection>(new Db2DbSetting(), true);
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
            using var connection = new DB2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new SourceVersionAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (DB2Parameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(DataRowVersion.Original, parameter.SourceVersion);
        }

        [TestMethod]
        public void TestSourceVersionAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new DB2Connection();
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
            var parameter = (DB2Parameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(DataRowVersion.Original, parameter.SourceVersion);
        }
    }
}
