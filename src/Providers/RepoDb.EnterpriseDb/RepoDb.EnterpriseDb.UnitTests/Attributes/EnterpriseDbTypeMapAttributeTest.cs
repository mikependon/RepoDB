#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Attributes.Parameter.EnterpriseDb;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.EnterpriseDb.UnitTests.Attributes
{
    [TestClass]
    public class EnterpriseDbTypeMapAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<EDBConnection>(new EnterpriseDbDbSetting(), true);
        }

        #region Classes

        private class EnterpriseDbTypeMapAttributeTestClass
        {
            [EnterpriseDbType(EDBType.Box)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestEnterpriseDbTypeMapAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new EDBConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new EnterpriseDbTypeMapAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = (EDBParameter)command.Parameters["@ColumnName"];
                    Assert.AreEqual(EDBType.Box, parameter.EDBType);
                }
            }
        }

        [TestMethod]
        public void TestEnterpriseDbTypeMapAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new EDBConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(EnterpriseDbTypeMapAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = (EDBParameter)command.Parameters["@ColumnName"];
                    Assert.AreEqual(EDBType.Box, parameter.EDBType);
                }
            }
        }
    }
}
