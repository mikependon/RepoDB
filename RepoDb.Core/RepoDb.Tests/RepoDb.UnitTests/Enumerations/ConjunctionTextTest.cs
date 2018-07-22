using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests
{
    [TestFixture]
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

        [Test]
        public void TestAnd()
        {
            // Prepare
            var operation = Conjunction.And;

            // Act
            var parsed = GetConjunctionTextAttribute(operation);

            // Assert
            Assert.AreEqual("AND", parsed.Text);
        }

        [Test]
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
