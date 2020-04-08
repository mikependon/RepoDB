using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class PrimaryAttributeTest
    {
        private class PrimaryAttributeTestClass
        {
            [Primary]
            public int WhateverId { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void TestPrimaryAttribute()
        {
            // Act
            var actual = PrimaryCache.Get<PrimaryAttributeTestClass>();
            var expected = "WhateverId";

            // Assert
            Assert.AreEqual(expected, actual.PropertyInfo.Name);
        }
    }
}
