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
    public class CharsetAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<FbConnection>(new FirebirdDbSetting(), true);
        }

        #region Classes

        private class CharsetAttributeTestClass
        {
            [Charset(FbCharset.Utf8)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestCharsetAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new FbConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new CharsetAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (FbParameter)command.Parameters["@ColumnName"];
            Assert.AreEqual(FbCharset.Utf8, parameter.Charset);
        }

        [TestMethod]
        public void TestCharsetAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new FbConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(CharsetAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (FbParameter)command.Parameters["@ColumnName"];
            Assert.AreEqual(FbCharset.Utf8, parameter.Charset);
        }
    }
}
