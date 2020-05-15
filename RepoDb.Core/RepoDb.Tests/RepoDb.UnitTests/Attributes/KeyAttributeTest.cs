using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class KeyAttributeTest
    {
        private class KeyAttributeTestClass
        {
            [Key]
            public int WhateverId { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void TestPrimaryAttribute()
        {
            // Act
            var actual = PrimaryCache.Get<KeyAttributeTestClass>();
            var expected = "WhateverId";

            // Assert
            Assert.AreEqual(expected, actual.PropertyInfo.Name);
        }
    }
}
