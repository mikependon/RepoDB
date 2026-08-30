#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Attributes.Parameter.Vertica;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Vertica.UnitTests.Attributes.Parameter.Vertica
{
    [TestClass]
    public class VerticaTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<VerticaConnection>(new VerticaDbSetting(), true);
        }

        #region Classes

        private class VerticaTypeAttributeTestClass
        {
            [VerticaType(VerticaType.VarChar)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestVerticaTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new VerticaConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new VerticaTypeAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(VerticaType.VarChar, parameter.Type);
                }
            }
        }

        [TestMethod]
        public void TestVerticaTypeAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new VerticaConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(VerticaTypeAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(VerticaType.VarChar, parameter.Type);
                }
            }
        }
    }
}
