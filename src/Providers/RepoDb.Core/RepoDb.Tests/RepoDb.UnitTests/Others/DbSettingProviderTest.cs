#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class DbSettingProviderTest
    {
        private sealed class SqlServerLikeDbSetting : BaseDbSetting
        {
            public SqlServerLikeDbSetting()
            {
                Provider = "SqlServer";
            }
        }

        private sealed class AnotherSqlServerLikeDbSetting : BaseDbSetting
        {
            public AnotherSqlServerLikeDbSetting()
            {
                Provider = "SqlServer";
            }
        }

        private sealed class MySqlLikeDbSetting : BaseDbSetting
        {
            public MySqlLikeDbSetting()
            {
                Provider = "MySql";
            }
        }

        private sealed class NoProviderDbSetting : BaseDbSetting
        {
        }

        [TestMethod]
        public void TestBaseDbSettingProviderIsNullByDefault()
        {
            // Act
            var setting = new NoProviderDbSetting();

            // Assert
            Assert.IsNull(setting.Provider);
        }

        [TestMethod]
        public void TestBaseDbSettingProviderCanBeSetByTheDerivedSetting()
        {
            // Act
            var setting = new SqlServerLikeDbSetting();

            // Assert
            Assert.AreEqual("SqlServer", setting.Provider, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbSettingProviderIsExposedByTheInterface()
        {
            // Act
            IDbSetting sqlServer = new SqlServerLikeDbSetting();
            IDbSetting custom = new CustomDbSetting();

            // Assert
            Assert.AreEqual("SqlServer", sqlServer.Provider, StringComparer.Ordinal);
            Assert.IsNull(custom.Provider);
        }

        [TestMethod]
        public void TestBaseDbSettingEqualityTakesProviderIntoAccount()
        {
            // Act
            var sqlServer = new SqlServerLikeDbSetting();
            var sameAsSqlServer = new AnotherSqlServerLikeDbSetting();
            var mySql = new MySqlLikeDbSetting();
            var noProvider = new NoProviderDbSetting();

            // Assert
            Assert.IsTrue(sqlServer.Equals(sameAsSqlServer));
            Assert.AreEqual(sqlServer.GetHashCode(), sameAsSqlServer.GetHashCode());
            Assert.IsFalse(sqlServer.Equals(mySql));
            Assert.AreNotEqual(sqlServer.GetHashCode(), mySql.GetHashCode());
            Assert.IsFalse(sqlServer.Equals(noProvider));
            Assert.AreNotEqual(sqlServer.GetHashCode(), noProvider.GetHashCode());
        }
    }
}
