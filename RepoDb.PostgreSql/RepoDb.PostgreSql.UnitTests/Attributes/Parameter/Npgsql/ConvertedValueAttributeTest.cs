#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Attributes.Parameter.Npgsql;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.PostgreSql.UnitTests.Attributes.Parameter.Npgsql
{
    [TestClass]
    public class ConvertedValueAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<NpgsqlConnection>(new PostgreSqlDbSetting(), true);
        }

        #region Classes

        private class ConvertedValueAttributeTestClass
        {
            [ConvertedValue("NameColumn")]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestConvertedValueAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new NpgsqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new ConvertedValueAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("NameColumn", parameter.Value);
                }
            }
        }

        [TestMethod]
        public void TestConvertedValueAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new NpgsqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(ConvertedValueAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("NameColumn", parameter.Value);
                }
            }
        }
    }
}
