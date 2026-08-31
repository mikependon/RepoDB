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
    public class SapHanaDbTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<HanaConnection>(new SapHanaDbSetting(), true);
        }

        #region Classes

        private class SapHanaDbTypeAttributeTestClass
        {
            [SapHanaDbType(HanaDbType.NVarChar)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestSapHanaDbTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new HanaConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new SapHanaDbTypeAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters[":ColumnName"];
                    Assert.AreEqual(HanaDbType.NVarChar, parameter.HanaDbType);
                }
            }
        }

        [TestMethod]
        public void TestSapHanaDbTypeAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new HanaConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(SapHanaDbTypeAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters[":ColumnName"];
                    Assert.AreEqual(HanaDbType.NVarChar, parameter.HanaDbType);
                }
            }
        }
    }
}
