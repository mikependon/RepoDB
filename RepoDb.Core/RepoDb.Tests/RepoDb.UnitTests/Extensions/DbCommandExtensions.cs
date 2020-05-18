using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.UnitTests.Extensions
{
    [TestClass]
    public class DbCommandExtensionsTest
    {
        class TestClass
        {
            public Guid Id { get; set; } = Guid.NewGuid();
        }

        class StringToGuidPropertyHandler : IPropertyHandler<string, Guid>
        {
            public Guid Get(string input, ClassProperty property)
            {
                Guid.TryParse(input, out Guid output);
                return output;
            }

            public string Set(Guid input, ClassProperty property)
            {
                return input.ToString();
            }
        }

        [TestMethod]
        public void CreateParametersPropertyHandlerWillBeUsedOnParameterResolution()
        {
            // Arrange
            FluentMapper.Entity<TestClass>().PropertyHandler<StringToGuidPropertyHandler>(e => e.Id).;
            var cmd = new SqlConnection().CreateCommand();

            // Act
            cmd.CreateParameters(new TestClass { Id = Guid.Parse("9963c864-ab4f-43f8-9dc9-43038565b971") });

            // Assert
            Assert.AreEqual(1, cmd.Parameters.Count);
            Assert.AreEqual("9963c864-ab4f-43f8-9dc9-43038565b971", cmd.Parameters[0].Value);
        }

        [TestMethod]
        public void CreateParametersQueryFieldPropertyHandlerWillBeUsedOnParameterResolution()
        {
            // Arrange
            PropertyHandlerMapper.Add(typeof(Guid), new StringToGuidPropertyHandler(), true);
            var cmd = new SqlConnection().CreateCommand();

            // Act
            cmd.CreateParameters(new QueryField("Id", Guid.Parse("9963c864-ab4f-43f8-9dc9-43038565b971")));

            // Assert
            Assert.AreEqual(1, cmd.Parameters.Count);
            Assert.AreEqual("9963c864-ab4f-43f8-9dc9-43038565b971", cmd.Parameters[0].Value);
        }
    }
}
