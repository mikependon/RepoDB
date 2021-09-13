using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class ParameterPropertyValueSetterAttributeTest
    {
        #region Classes

        private class ParameterTagAttributeTestClass : ParameterPropertyValueSetterAttribute
        {
            public ParameterTagAttributeTestClass(string tag)
                : base(typeof(CustomDbParameter), "Tag", tag)
            { }

            public string Tag => (string)Value;
        }

        private class ParameterPropertyValueSetterAttributeTestClass
        {
            public int Id { get; set; }

            [ParameterPropertyValueSetter(typeof(CustomDbParameter),
                "Tag",
                "ValueOfTag")]
            public string ColumnTag { get; set; }
        }

        private class ParameterPropertyValueSetterAttributeViaDerivedTestClass
        {
            public int Id { get; set; }

            [ParameterTagAttributeTestClass("ValueOfTag")]
            public string ColumnTag { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestParameterPropertyValueSetterAttributeViaCreateParameters()
        {
            // Act
            using (var connection = new CustomDbConnection())
            {
                var command = connection.CreateCommand();
                DbCommandExtension
                    .CreateParameters(command, new
                    {
                        ColumnTag = "Test"
                    },
                    typeof(ParameterPropertyValueSetterAttributeTestClass));

                // Assert
                Assert.AreEqual(1, command.Parameters.Count);

                // Assert
                var parameter = command.Parameters["ColumnTag"];
                Assert.AreEqual("ValueOfTag", ((CustomDbParameter)parameter).Tag);
                Assert.AreEqual("Test", parameter.Value);
            }
        }

        [TestMethod]
        public void TestParameterPropertyValueSetterAttributeViaDerivedClassViaCreateParameters()
        {
            // Act
            using (var connection = new CustomDbConnection())
            {
                var command = connection.CreateCommand();
                DbCommandExtension
                    .CreateParameters(command, new
                    {
                        ColumnTag = "Test"
                    },
                    typeof(ParameterPropertyValueSetterAttributeViaDerivedTestClass));

                // Assert
                Assert.AreEqual(1, command.Parameters.Count);

                // Assert
                var parameter = command.Parameters["ColumnTag"];
                Assert.AreEqual("ValueOfTag", ((CustomDbParameter)parameter).Tag);
                Assert.AreEqual("Test", parameter.Value);
            }
        }
    }
}
