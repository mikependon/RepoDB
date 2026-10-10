#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Core.UnitTests.Fake.BulkOperations;

namespace RepoDb.Data.Core.UnitTests
{
    [TestClass]
    public class ConnectionManagerTest
    {
        #region Helpers

        private static string GetName() =>
            "Connection-" + Guid.NewGuid().ToString("N");

        #endregion

        #region Register

        [TestMethod]
        public void ThrowExceptionOnRegisterIfTheNameIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ConnectionManager.Register(null, new FakeDbConnection()));
            Assert.Throws<ArgumentNullException>(() => ConnectionManager.Register(string.Empty, new FakeDbConnection()));
            Assert.Throws<ArgumentNullException>(() => ConnectionManager.Register("  ", new FakeDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnRegisterIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ConnectionManager.Register(GetName(), null));
        }

        [TestMethod]
        public void TestRegisterRegistersTheConnectionByItsName()
        {
            // Setup
            var name = GetName();
            var connection = new FakeDbConnection();

            // Act
            ConnectionManager.Register(name, connection);

            // Assert
            Assert.AreSame(connection, ConnectionManager.Get(name));
        }

        [TestMethod]
        public void ThrowExceptionOnRegisterIfTheNameIsAlreadyRegistered()
        {
            // Setup
            var name = GetName();
            var connection = new FakeDbConnection();
            ConnectionManager.Register(name, connection);

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => ConnectionManager.Register(name, new FakeDbConnection()));

            // Assert
            StringAssert.Contains(exception.Message, name);
            Assert.AreSame(connection, ConnectionManager.Get(name));
        }

        [TestMethod]
        public void TestRegisterWithForceReplacesTheRegisteredConnection()
        {
            // Setup
            var name = GetName();
            var replacement = new FakeDbConnection();
            ConnectionManager.Register(name, new FakeDbConnection());

            // Act
            ConnectionManager.Register(name, replacement, true);

            // Assert
            Assert.AreSame(replacement, ConnectionManager.Get(name));
        }

        [TestMethod]
        public void TestRegisterWithForceRegistersTheConnectionIfTheNameIsNotRegistered()
        {
            // Setup
            var name = GetName();
            var connection = new FakeDbConnection();

            // Act
            ConnectionManager.Register(name, connection, true);

            // Assert
            Assert.AreSame(connection, ConnectionManager.Get(name));
        }

        [TestMethod]
        public void TestRegisterIsCaseSensitive()
        {
            // Setup
            var name = GetName();
            var lower = new FakeDbConnection();
            var upper = new FakeDbConnection();

            // Act
            ConnectionManager.Register(name.ToLowerInvariant(), lower);
            ConnectionManager.Register(name.ToUpperInvariant(), upper);

            // Assert
            Assert.AreSame(lower, ConnectionManager.Get(name.ToLowerInvariant()));
            Assert.AreSame(upper, ConnectionManager.Get(name.ToUpperInvariant()));
        }

        #endregion

        #region Get

        [TestMethod]
        public void ThrowExceptionOnGetIfTheNameIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ConnectionManager.Get(null));
            Assert.Throws<ArgumentNullException>(() => ConnectionManager.Get(" "));
        }

        [TestMethod]
        public void ThrowExceptionOnGetIfTheNameIsNotRegistered()
        {
            // Setup
            var name = GetName();

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => ConnectionManager.Get(name));

            // Assert
            StringAssert.Contains(exception.Message, name);
        }

        [TestMethod]
        public void TestGetReturnsTheSameInstanceEveryTime()
        {
            // Setup
            var name = GetName();
            ConnectionManager.Register(name, new FakeDbConnection());

            // Act/Assert
            Assert.AreSame(ConnectionManager.Get(name), ConnectionManager.Get(name));
        }

        #endregion

        #region Get<T>

        [TestMethod]
        public void TestGetOfTReturnsTheConnectionAsTheType()
        {
            // Setup
            var name = GetName();
            var connection = new FakeDbConnection();
            ConnectionManager.Register(name, connection);

            // Act/Assert
            Assert.AreSame(connection, ConnectionManager.Get<FakeDbConnection>(name));
            Assert.AreSame(connection, ConnectionManager.Get<IDbConnection>(name));
        }

        [TestMethod]
        public void ThrowExceptionOnGetOfTIfTheConnectionIsNotOfTheType()
        {
            // Setup
            var name = GetName();
            ConnectionManager.Register(name, new FakeDbConnection());

            // Act
            var exception = Assert.Throws<InvalidCastException>(() => ConnectionManager.Get<FakeBulkDbConnection>(name));

            // Assert
            StringAssert.Contains(exception.Message, name);
            StringAssert.Contains(exception.Message, typeof(FakeDbConnection).FullName);
            StringAssert.Contains(exception.Message, typeof(FakeBulkDbConnection).FullName);
        }

        [TestMethod]
        public void ThrowExceptionOnGetOfTIfTheNameIsNotRegistered()
        {
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => ConnectionManager.Get<FakeDbConnection>(GetName()));
        }

        [TestMethod]
        public void ThrowExceptionOnGetOfTIfTheNameIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ConnectionManager.Get<FakeDbConnection>(null));
        }

        #endregion
    }
}
