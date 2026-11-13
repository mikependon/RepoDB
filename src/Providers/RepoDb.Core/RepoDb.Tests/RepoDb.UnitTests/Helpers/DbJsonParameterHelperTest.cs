#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Helpers;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Data;

namespace RepoDb.UnitTests.Helpers
{
    [TestClass]
    public class DbJsonParameterHelperTest
    {
        #region Configure

        [TestMethod]
        public void TestDbJsonParameterHelperConfigureWithDbSetting()
        {
            // Setup
            var parameter = new CustomDbParameter { DbType = DbType.Int32 };

            // Act
            DbJsonParameterHelper.Configure(parameter, new CustomDbSetting());

            // Assert
            Assert.AreEqual(DbType.String, parameter.DbType);
        }

        [TestMethod]
        public void TestDbJsonParameterHelperConfigureWithoutDbSetting()
        {
            // Setup
            var parameter = new CustomDbParameter { DbType = DbType.Int32 };

            // Act
            DbJsonParameterHelper.Configure(parameter, null);

            // Assert
            Assert.AreEqual(DbType.String, parameter.DbType);
        }

        #endregion

        #region SetValue

        [TestMethod]
        public void TestDbJsonParameterHelperSetValueWithJsonValue()
        {
            // Setup
            var parameter = new CustomDbParameter();
            var value = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            DbJsonParameterHelper.SetValue(parameter, value, new CustomDbSetting());

            // Assert
            Assert.AreEqual("{\"Id\":5}", parameter.Value);
        }

        [TestMethod]
        public void TestDbJsonParameterHelperSetValueWithDbNull()
        {
            // Setup
            var parameter = new CustomDbParameter();

            // Act
            DbJsonParameterHelper.SetValue(parameter, DBNull.Value, new CustomDbSetting());

            // Assert
            Assert.AreEqual(DBNull.Value, parameter.Value);
        }

        [TestMethod]
        public void TestDbJsonParameterHelperSetValueConfiguresTheParameter()
        {
            // Setup
            var parameter = new CustomDbParameter { DbType = DbType.Int32 };

            // Act
            DbJsonParameterHelper.SetValue(parameter, default(DbJsonValue<JsonSampleDocument>), new CustomDbSetting());

            // Assert
            Assert.AreEqual(DbType.String, parameter.DbType);
        }

        #endregion

        #region TryConvert

        [TestMethod]
        public void TestDbJsonParameterHelperTryConvertWithJsonValue()
        {
            // Setup
            var value = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            var isJson = DbJsonParameterHelper.TryConvert(value, out var converted);

            // Assert
            Assert.IsTrue(isJson);
            Assert.AreEqual("{\"Id\":5}", converted);
        }

        [TestMethod]
        public void TestDbJsonParameterHelperTryConvertWithEmptyJsonValue()
        {
            // Act
            var isJson = DbJsonParameterHelper.TryConvert(default(DbJsonValue<JsonSampleDocument>), out var converted);

            // Assert
            Assert.IsTrue(isJson);
            Assert.IsNull(converted);
        }

        [TestMethod]
        public void TestDbJsonParameterHelperTryConvertWithOtherValue()
        {
            // Act
            var isJson = DbJsonParameterHelper.TryConvert("text", out var converted);

            // Assert
            Assert.IsFalse(isJson);
            Assert.AreEqual("text", converted);
        }

        [TestMethod]
        public void TestDbJsonParameterHelperTryConvertWithNull()
        {
            // Act
            var isJson = DbJsonParameterHelper.TryConvert(null, out var converted);

            // Assert
            Assert.IsFalse(isJson);
            Assert.IsNull(converted);
        }

        #endregion
    }
}
