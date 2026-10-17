#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class DbSettingAreIndexNamesUniquePerSchemaTest
    {
        private sealed class UniquePerSchemaDbSetting : BaseDbSetting
        {
            public UniquePerSchemaDbSetting()
            {
                AreIndexNamesUniquePerSchema = true;
            }
        }

        [TestMethod]
        public void TestBaseDbSettingAreIndexNamesUniquePerSchemaIsFalseByDefault()
        {
            // Act
            var setting = new CustomDbSetting();

            // Assert
            Assert.IsFalse(setting.AreIndexNamesUniquePerSchema);
        }

        [TestMethod]
        public void TestBaseDbSettingAreIndexNamesUniquePerSchemaCanBeSetByTheDerivedSetting()
        {
            // Act
            var setting = new UniquePerSchemaDbSetting();

            // Assert
            Assert.IsTrue(setting.AreIndexNamesUniquePerSchema);
        }

        [TestMethod]
        public void TestDbSettingAreIndexNamesUniquePerSchemaIsExposedByTheInterface()
        {
            // Act
            IDbSetting notUnique = new CustomDbSetting();
            IDbSetting unique = new UniquePerSchemaDbSetting();

            // Assert
            Assert.IsFalse(notUnique.AreIndexNamesUniquePerSchema);
            Assert.IsTrue(unique.AreIndexNamesUniquePerSchema);
        }

        [TestMethod]
        public void TestBaseDbSettingEqualityTakesAreIndexNamesUniquePerSchemaIntoAccount()
        {
            // Act
            var unique = new UniquePerSchemaDbSetting();
            var sameAsUnique = new UniquePerSchemaDbSetting();
            var notUnique = new NotUniquePerSchemaDbSetting();

            // Assert
            Assert.IsTrue(unique.Equals(sameAsUnique));
            Assert.AreEqual(unique.GetHashCode(), sameAsUnique.GetHashCode());
            Assert.IsFalse(unique.Equals(notUnique));
            Assert.AreNotEqual(unique.GetHashCode(), notUnique.GetHashCode());
        }

        private sealed class NotUniquePerSchemaDbSetting : BaseDbSetting
        {
        }
    }
}
