#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Attributes.Parameter.CockroachDb;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.CockroachDB.UnitTests.Attributes
{
    [TestClass]
    public class CockroachDbTypeMapAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<CockroachDbConnection>(new CockroachDbDbSetting(), true);
        }

        #region Classes

        private class CockroachDbTypeMapAttributeTestClass
        {
            [CockroachDbType(CockroachDbType.Jsonb)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestCockroachDbTypeMapAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new CockroachDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new CockroachDbTypeMapAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = (CockroachDbParameter)command.Parameters["@ColumnName"];
                    Assert.AreEqual(CockroachDbType.Jsonb, parameter.CockroachDbType);
                }
            }
        }

        [TestMethod]
        public void TestCockroachDbTypeMapAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new CockroachDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(CockroachDbTypeMapAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = (CockroachDbParameter)command.Parameters["@ColumnName"];
                    Assert.AreEqual(CockroachDbType.Jsonb, parameter.CockroachDbType);
                }
            }
        }
    }
}
