#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests.Cachers
{
    [TestClass]
    public partial class PropertyMappedNameCacheTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyMappedNameCache.Flush();
        }

        #region SubClasses

        private class PropertyMappedNameCacheTestClass
        {
            public string ColumnString { get; set; }
            [Map("PropertyName")]
            public string PropertyString { get; set; }
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestWithoutMapAttribute()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMappedNameCacheTestClass>(e => e.ColumnString);
            var expected = "ColumnString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestWithMapAttribute()
        {
            // Act
            var property = PropertyCache.Get<PropertyMappedNameCacheTestClass>()
                .First(p => string.Equals(p.PropertyInfo.Name, "PropertyString", StringComparison.Ordinal));
            var expected = "PropertyName";

            // Assert
            Assert.AreEqual(expected, property.GetMappedName());
        }

        [TestMethod]
        public void ThrowExcpetionOnPropertyMappingCacheIfThePropertyIsNull()
        {
            // Setup
            Assert.Throws<ArgumentNullException>(() => PropertyMappedNameCache.Get<PropertyMappedNameCacheTestClass>((Field)null));
        }

        #endregion
    }
}
