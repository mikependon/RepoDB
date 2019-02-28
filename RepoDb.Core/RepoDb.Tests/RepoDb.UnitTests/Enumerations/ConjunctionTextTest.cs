using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Extensions;
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
                .GetTypeInfo()
                .GetMembers()
                .First(member => member.Name.ToLower() == conjunction.ToString().ToLower())
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
