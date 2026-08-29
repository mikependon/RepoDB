#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter.Db2
{
    [TestClass]
    public class IsDefaultAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<DB2Connection>(new Db2DbSetting(), true);
        }

        #region Classes

        private class IsDefaultAttributeTestClass
        {
            [IsDefault(true)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestIsDefaultAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new DB2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new IsDefaultAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (DB2Parameter)command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.IsDefault);
        }

        [TestMethod]
        public void TestIsDefaultAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new DB2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(IsDefaultAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (DB2Parameter)command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.IsDefault);
        }
    }
}
