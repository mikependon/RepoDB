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
