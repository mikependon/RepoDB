using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class ConjunctionTextTest
    {
        private TextAttribute GetConjunctionTextAttribute(Conjunction conjunction)
        {
            return typeof(Conjunction)
                .GetMembers()
                .First(member => member.Name.ToLower() == conjunction.ToString().ToLower())
                .GetCustomAttribute<TextAttribute>();
        }

        [TestMethod]
        public void TestAnd()
        {
            // Prepare
            var operation = Conjunction.And;

            // Act
            var parsed = GetConjunctionTextAttribute(operation);

            // Assert
            Assert.AreEqual("AND", parsed.Text);
        }

        [TestMethod]
        public void TestOr()
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
