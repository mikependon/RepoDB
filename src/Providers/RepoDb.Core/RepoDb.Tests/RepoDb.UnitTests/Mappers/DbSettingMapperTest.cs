#region Copyright Attributions

// Copyright (c) 2021 Tommaso Bertoni and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Mappers
{
    [TestClass]
    public partial class DbSettingMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            DbSettingMapper.Clear();
        }

        #region Methods

        [TestMethod]
        public void TestDbSettingMapperMappingViaGeneric()
        {
            // Setup
            var dbSetting = new CustomDbSetting();
            DbSettingMapper.Add<CustomDbConnection>(dbSetting, true);

            // Act
            var actual = DbSettingMapper.Get<CustomDbConnection>();

            // Assert
            Assert.AreSame(dbSetting, actual);
        }

        [TestMethod]
        public void TestDbSettingMapperMappingCanBeRemovedViaGeneric()
        {
            // Setup
            var dbSetting = new CustomDbSetting();
            DbSettingMapper.Add<CustomDbConnection>(dbSetting, true);

            // Act
            DbSettingMapper.Remove<CustomDbConnection>();

            // Assert
            var actual = DbSettingMapper.Get<CustomDbConnection>();
            Assert.IsNull(actual);
        }

        #endregion
    }
}
