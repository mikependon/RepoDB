using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System;
using System.Data;
using System.Linq;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class TypeMapAttributeTest
    {
        private class TypeMapAttributeTestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [TypeMap(DbType.DateTime2)]
            public DateTime ColumnDateTime { get; set; }
        }

        [TestMethod]
        public void TestTypeMapAttribute()
        {
            // Act
            var actual = PropertyCache.Get<TypeMapAttributeTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnDateTime");
            var result = actual.GetDbType();
            var expected = DbType.DateTime2;

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
