using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace RepoDb.UnitTests.Enumerations
{
    [TestClass]
    public class ConjunctionTextTest
    {
        [TestMethod]
        public void TestConjunctionAndText()
        {
            // Prepare
            var operation = Conjunction.And;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("AND", text);
        }

        [TestMethod]
        public void TestConjunctionOrText()
        {
            // Prepare
            var operation = Conjunction.Or;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("OR", text);
        }

    }
}
