using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System.Linq;

namespace RepoDb.UnitTests.Extensions
{
    [TestClass]
    public class TypeExtensionsTest
    {
        [TestMethod]
        public void TestIsPlainTypeForAnoynmousType()
        {
            var type = (new { Property = "ABC" }).GetType();
            Assert.IsTrue(type.IsClassType() || type.IsAnonymousType());
            Assert.IsFalse(type.IsQueryObjectType());
            Assert.IsFalse(type.IsDictionaryStringObject());
            Assert.IsFalse(type.GetEnumerableClassProperties().Any());
        }
    }
}
