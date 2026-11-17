#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    [TestClass]
    public class DbConnectionGetRemovedSchemaTest
    {
        #region Private Methods

        private static void MapSetting(bool isSchemaSupported,
            string defaultSchema)
        {
            var setting = new Mock<IDbSetting>();
            setting.SetupGet(item => item.IsSchemaSupported).Returns(isSchemaSupported);
            setting.SetupGet(item => item.DefaultSchema).Returns(defaultSchema);
            DbSettingMapper.Add<CustomDbConnection>(setting.Object, true);
        }

        #endregion

        #region GetRemovedSchema

        [TestMethod]
        public void TestGetRemovedSchemaWithoutSchemaSupportAndTargetSchema()
        {
            // Setup
            MapSetting(false, "dbo");

            // Act
            var result = DbConnectionExtension.GetRemovedSchema(null, new CustomDbConnection(), new CustomDbConnection());

            // Assert
            Assert.AreEqual("dbo", result);
        }

        [TestMethod]
        public void TestGetRemovedSchemaWithoutSchemaSupportAndBlankTargetSchema()
        {
            // Setup
            MapSetting(false, "dbo");

            // Act
            var result = DbConnectionExtension.GetRemovedSchema(" ", new CustomDbConnection(), new CustomDbConnection());

            // Assert
            Assert.AreEqual("dbo", result);
        }

        [TestMethod]
        public void TestGetRemovedSchemaWithoutSchemaSupportButWithTargetSchema()
        {
            // Setup
            MapSetting(false, "dbo");

            // Act
            var result = DbConnectionExtension.GetRemovedSchema("test", new CustomDbConnection(), new CustomDbConnection());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetRemovedSchemaWithSchemaSupport()
        {
            // Setup
            MapSetting(true, "dbo");

            // Act
            var result = DbConnectionExtension.GetRemovedSchema(null, new CustomDbConnection(), new CustomDbConnection());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetRemovedSchemaWithoutDefaultSchemaInTheSource()
        {
            // Setup
            MapSetting(false, null);

            // Act
            var result = DbConnectionExtension.GetRemovedSchema(null, new CustomDbConnection(), new CustomDbConnection());

            // Assert
            Assert.IsNull(result);
        }

        #endregion
    }
}
