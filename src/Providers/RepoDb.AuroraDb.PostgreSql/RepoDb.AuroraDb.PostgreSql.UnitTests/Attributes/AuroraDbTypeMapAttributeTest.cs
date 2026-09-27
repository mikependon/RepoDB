#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Attributes.Parameter.AuroraDb;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Attributes
{
    [TestClass]
    public class AuroraDbTypeMapAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<AuroraDbConnection>(new AuroraDbDbSetting(), true);
        }

        #region Classes

        private class AuroraDbTypeMapAttributeTestClass
        {
            [AuroraDbType(AuroraDbType.Jsonb)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestAuroraDbTypeMapAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new AuroraDbConnection(Helper.ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new AuroraDbTypeMapAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = (AuroraDbParameter)command.Parameters["@ColumnName"];
                    Assert.AreEqual(AuroraDbType.Jsonb, parameter.AuroraDbType);
                }
            }
        }

        [TestMethod]
        public void TestAuroraDbTypeMapAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new AuroraDbConnection(Helper.ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(AuroraDbTypeMapAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = (AuroraDbParameter)command.Parameters["@ColumnName"];
                    Assert.AreEqual(AuroraDbType.Jsonb, parameter.AuroraDbType);
                }
            }
        }
    }
}
