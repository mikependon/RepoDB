#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;
using System.Data;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class TypeMapTypeLevelResolverTest
    {
        [TestMethod]
        public void TestTypeMapTypeLevelResolverWithAttributes()
        {
            // Setup
            var resolver = new TypeMapTypeLevelResolver();
            FluentMapper
                .Type<Guid>()
                .DbType(DbType.AnsiStringFixedLength);

            // Act
            var result = resolver.Resolve(typeof(Guid));
            var expected = DbType.AnsiStringFixedLength;

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
