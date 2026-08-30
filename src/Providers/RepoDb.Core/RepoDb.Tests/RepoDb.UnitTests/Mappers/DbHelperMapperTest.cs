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
    public partial class DbHelperMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            DbHelperMapper.Clear();
        }

        #region Methods

        [TestMethod]
        public void TestDbHelperMapperMappingViaGeneric()
        {
            // Setup
            var dbHelper = new CustomDbHelper();
            DbHelperMapper.Add<CustomDbConnection>(dbHelper, true);

            // Act
            var actual = DbHelperMapper.Get<CustomDbConnection>();

            // Assert
            Assert.AreSame(dbHelper, actual);
        }

        [TestMethod]
        public void TestDbHelperMapperMappingCanBeRemovedViaGeneric()
        {
            // Setup
            var dbHelper = new CustomDbHelper();
            DbHelperMapper.Add<CustomDbConnection>(dbHelper, true);

            // Act
            DbHelperMapper.Remove<CustomDbConnection>();

            // Assert
            var actual = DbHelperMapper.Get<CustomDbConnection>();
            Assert.IsNull(actual);
        }

        #endregion
    }
}
