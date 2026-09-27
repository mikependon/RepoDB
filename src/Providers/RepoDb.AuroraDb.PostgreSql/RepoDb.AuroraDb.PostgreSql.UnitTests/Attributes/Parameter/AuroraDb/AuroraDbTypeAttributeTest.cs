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

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Attributes.Parameter.AuroraDb
{
    [TestClass]
    public class AuroraDbTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<AuroraDbConnection>(new AuroraDbDbSetting(), true);
        }

        #region Classes

        private class AuroraDbTypeAttributeTestClass
        {
            [AuroraDbType(AuroraDbType.Jsonb)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestAuroraDbTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new AuroraDbConnection(Helper.ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new AuroraDbTypeAttributeTestClass
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
        public void TestAuroraDbTypeAttributeViaAnonymousViaCreateParameters()
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
                        typeof(AuroraDbTypeAttributeTestClass));

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
