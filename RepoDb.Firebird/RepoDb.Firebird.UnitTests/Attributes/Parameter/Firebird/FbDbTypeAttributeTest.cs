#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Attributes.Parameter.Firebird;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Firebird.UnitTests.Attributes.Parameter.Firebird
{
    [TestClass]
    public class FbDbTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<FbConnection>(new FirebirdDbSetting(), true);
        }

        #region Classes

        private class FbDbTypeAttributeTestClass
        {
            [FbDbType(FbDbType.Text)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestFbDbTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new FbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new FbDbTypeAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(FbDbType.Text, parameter.FbDbType);
                }
            }
        }

        [TestMethod]
        public void TestFbDbTypeAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new FbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(FbDbTypeAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(FbDbType.Text, parameter.FbDbType);
                }
            }
        }
    }
}
