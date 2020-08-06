using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests.Enumerations
{
    [TestClass]
    public class ConjunctionTextTest
    {
        private TextAttribute GetConjunctionTextAttribute(Conjunction conjunction)
        {
            return typeof(Conjunction)
                .GetMembers()
                .First(member => member.Name.ToLowerInvariant() == conjunction.ToString().ToLowerInvariant())
                .GetCustomAttribute<TextAttribute>();
        }

        [TestMethod]
        public void TestConjunctionAndText()
        {
            // Prepare
            var operation = Conjunction.And;

            // Act
            var parsed = GetConjunctionTextAttribute(operation);

            // Assert
            Assert.AreEqual("AND", parsed.Text);
        }

        [TestMethod]
        public void TestConjunctionOrText()
        {
            // Prepare
            var operation = Conjunction.Or;

            // Act
            var parsed = GetConjunctionTextAttribute(operation);

            // Assert
            Assert.AreEqual("OR", parsed.Text);
        }

    }
}
