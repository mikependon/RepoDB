using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class KeyAttributeTest
    {
        #region SubClasses

        private class KeyAttributeTestClass
        {
            [Key]
            public int WhateverId { get; set; }
            public string Name { get; set; }
        }

        private class KeyAttributeCollisionTestClass
        {
            [Key]
            public int KeyId { get; set; }
            [Primary]
            public int PrimaryId { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestKeyAttribute()
        {
            // Act
            var actual = PrimaryCache.Get<KeyAttributeTestClass>();
            var expected = "WhateverId";

            // Assert
            Assert.AreEqual(expected, actual.PropertyInfo.Name);
        }

        [TestMethod]
        public void TestKeyAndPrimaryAttributeCollision()
        {
            // Act
            var actual = PrimaryCache.Get<KeyAttributeCollisionTestClass>();
            var expected = "KeyId";

            // Assert
            Assert.AreEqual(expected, actual.PropertyInfo.Name);
        }
    }
}
