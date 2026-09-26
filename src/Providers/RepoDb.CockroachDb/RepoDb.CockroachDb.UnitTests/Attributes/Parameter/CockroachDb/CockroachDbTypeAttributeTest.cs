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

namespace RepoDb.CockroachDB.UnitTests.Attributes.Parameter.CockroachDb
{
    [TestClass]
    public class CockroachDbTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<CockroachDbConnection>(new CockroachDbDbSetting(), true);
        }

        #region Classes

        private class CockroachDbTypeAttributeTestClass
        {
            [CockroachDbType(CockroachDbType.Jsonb)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestCockroachDbTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new CockroachDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new CockroachDbTypeAttributeTestClass
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
        public void TestCockroachDbTypeAttributeViaAnonymousViaCreateParameters()
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
                        typeof(CockroachDbTypeAttributeTestClass));

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
