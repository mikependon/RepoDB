using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Attributes.Parameter
{
    [TestClass]
    public class PropertyValueAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<CustomDbConnection>(new CustomDbSetting(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            DbSettingMapper.Clear();
        }

        #region Classes

        private class TagAttributeTestClass : PropertyValueAttribute
        {
            public TagAttributeTestClass(string tag)
                : base(typeof(CustomDbParameter), "Tag", tag)
            { }

            public string Tag => (string)Value;
        }

        private class PropertyValueAttributeTestClass
        {
            [PropertyValue(typeof(CustomDbParameter),
                "Tag",
                "ValueOfTag")]
            public string ColumnName { get; set; }
        }

        private class PropertyValueAttributeViaDerivedTestClass
        {
            [TagAttributeTestClass("ValueOfTag")]
            public string ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestPropertyValueAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new CustomDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new PropertyValueAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("ValueOfTag", ((CustomDbParameter)parameter).Tag);
                }
            }
        }

        [TestMethod]
        public void TestPropertyValueAttributeViaDerivedClassViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new CustomDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                       .CreateParameters(command, new PropertyValueAttributeViaDerivedTestClass
                       {
                           ColumnName = "Test"
                       });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("ValueOfTag", ((CustomDbParameter)parameter).Tag);
                }
            }
        }

        [TestMethod]
        public void TestPropertyValueAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new CustomDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(PropertyValueAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("ValueOfTag", ((CustomDbParameter)parameter).Tag);
                }
            }
        }

        [TestMethod]
        public void TestPropertyValueAttributeViaDerivedClassViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new CustomDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(PropertyValueAttributeViaDerivedTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("ValueOfTag", ((CustomDbParameter)parameter).Tag);
                }
            }
        }
    }
}
