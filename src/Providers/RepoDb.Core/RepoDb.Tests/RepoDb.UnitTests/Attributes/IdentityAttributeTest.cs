#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class IdentityAttributeTest
    {
        private class IdentityAttributeTestClass
        {
            [Identity]
            public int WhateverId { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void TestPrimaryAttribute()
        {
            // Act
            var actual = IdentityCache.Get<IdentityAttributeTestClass>();
            var expected = "WhateverId";

            // Assert
            Assert.AreEqual(expected, actual.PropertyInfo.Name);
        }
    }
}
