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
    public class DbSettingIsSchemaSupportedTest
    {
        private sealed class NoSchemaDbSetting : BaseDbSetting
        {
            public NoSchemaDbSetting()
            {
                IsSchemaSupported = false;
            }
        }

        [TestMethod]
        public void TestBaseDbSettingIsSchemaSupportedByDefault()
        {
            // Act
            var setting = new CustomDbSetting();

            // Assert
            Assert.IsTrue(setting.IsSchemaSupported);
        }

        [TestMethod]
        public void TestBaseDbSettingIsSchemaSupportedCanBeSetByTheDerivedSetting()
        {
            // Act
            var setting = new NoSchemaDbSetting();

            // Assert
            Assert.IsFalse(setting.IsSchemaSupported);
        }

        [TestMethod]
        public void TestDbSettingIsSchemaSupportedIsExposedByTheInterface()
        {
            // Act
            IDbSetting supported = new CustomDbSetting();
            IDbSetting unsupported = new NoSchemaDbSetting();

            // Assert
            Assert.IsTrue(supported.IsSchemaSupported);
            Assert.IsFalse(unsupported.IsSchemaSupported);
        }

        [TestMethod]
        public void TestBaseDbSettingEqualityTakesIsSchemaSupportedIntoAccount()
        {
            // Act
            var unsupported = new NoSchemaDbSetting();
            var sameAsUnsupported = new NoSchemaDbSetting();
            var supported = new SchemaDbSetting();

            // Assert
            Assert.IsTrue(unsupported.Equals(sameAsUnsupported));
            Assert.AreEqual(unsupported.GetHashCode(), sameAsUnsupported.GetHashCode());
            Assert.IsFalse(unsupported.Equals(supported));
            Assert.AreNotEqual(unsupported.GetHashCode(), supported.GetHashCode());
        }

        private sealed class SchemaDbSetting : BaseDbSetting
        {
        }
    }
}
