using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class PropertyHandlerTypeLevelResolverTest
    {
        #region PropertyHandlers

        private class IntPropertyHandler : IPropertyHandler<int, int>
        {
            public int Get(int input, ClassProperty property)
            {
                return input;
            }

            public int Set(int input, ClassProperty property)
            {
                return input;
            }
        }

        #endregion

        [TestMethod]
        public void TestPropertyHandlerTypeLevelResolverWithAttributes()
        {
            // Setup
            var resolver = new PropertyHandlerTypeLevelResolver();
            FluentMapper
                .Type<int>()
                .PropertyHandler<IntPropertyHandler>();

            // Act
            var result = resolver.Resolve(typeof(int))?.GetType();
            var expected = typeof(IntPropertyHandler);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
