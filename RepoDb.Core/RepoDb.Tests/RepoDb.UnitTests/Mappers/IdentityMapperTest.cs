using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System;

namespace RepoDb.UnitTests.Mappers
{
    [TestClass]
    public partial class IdentityMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            IdentityCache.Flush();
            IdentityMapper.Clear();
        }

        #region SubClasses

        private class IdentityMapperTestClass
        {
            public string ColumnString { get; set; }
            public int ColumnInt { get; set; }
        }

        private class IdentityMapperTestWithAttributeClass
        {
            [Identity]
            public string ColumnString { get; set; }
            public int ColumnInt { get; set; }
        }

        private class IdentityMapperTestBaseClass
        {
            public int ColumnId { get; set; }
        }

        private class IdentityMapperTestDerivedClass1 : IdentityMapperTestBaseClass
        { }

        private class IdentityMapperTestDerivedClass2 : IdentityMapperTestBaseClass
        { }

        #endregion

        #region Methods

        /*
         * No IdentityAttribute
         */

        [TestMethod]
        public void TestIdentityMapperMappingViaPropertyName()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>("ColumnInt");

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestIdentityMapperMappingViaField()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(new Field("ColumnInt"));

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestIdentityMapperMappingViaExpression()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(e => e.ColumnInt);

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        /*
         * With IdentityAttribute
         */

        [TestMethod]
        public void TestIdentityMapperMappingViaPropertyNameWithMapAttribute()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestWithAttributeClass>("ColumnInt");

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestWithAttributeClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Act
            actual = IdentityCache.Get<IdentityMapperTestWithAttributeClass>();
            expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestIdentityMapperMappingViaFieldWithMapAttribute()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestWithAttributeClass>(new Field("ColumnInt"));

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestWithAttributeClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Act
            actual = IdentityCache.Get<IdentityMapperTestWithAttributeClass>();
            expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestIdentityMapperMappingViaExpressionWithMapAttribute()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestWithAttributeClass>(e => e.ColumnInt);

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestWithAttributeClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Act
            actual = IdentityCache.Get<IdentityMapperTestWithAttributeClass>();
            expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        /*
         * Override
         */

        [TestMethod]
        public void TestIdentityMapperMappingViaPropertyNameOverride()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>("ColumnInt");
            IdentityMapper.Add<IdentityMapperTestClass>("ColumnString", true);

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestClass>();
            var expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestIdentityMapperMappingViaFieldOverride()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(new Field("ColumnInt"));
            IdentityMapper.Add<IdentityMapperTestClass>(new Field("ColumnString"), true);

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestClass>();
            var expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestIdentityMapperMappingViaExpressionOverride()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(e => e.ColumnInt);
            IdentityMapper.Add<IdentityMapperTestClass>(e => e.ColumnString, true);

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestClass>();
            var expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnIdentityMapperViaPropertyNameThatIsAlreadyExisting()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>("ColumnInt");
            IdentityMapper.Add<IdentityMapperTestClass>("ColumnString");
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnIdentityMapperViaFieldThatIsAlreadyExisting()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(new Field("ColumnInt"));
            IdentityMapper.Add<IdentityMapperTestClass>(new Field("ColumnString"));
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnIdentityMapperViaExpressionThatIsAlreadyExisting()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(e => e.ColumnInt);
            IdentityMapper.Add<IdentityMapperTestClass>(e => e.ColumnString);
        }

        /*
         * Base Property
         */

        [TestMethod]
        public void TestIdentityMapperMappingViaPropertyNameForBaseProperty()
        {
            // Derived 1

            // Setup
            IdentityMapper.Add<IdentityMapperTestDerivedClass1>("ColumnId");

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestDerivedClass1>();
            var expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Derived 2

            // Setup
            IdentityMapper.Add<IdentityMapperTestDerivedClass2>("ColumnId");

            // Act
            actual = IdentityMapper.Get<IdentityMapperTestDerivedClass2>();
            expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestIdentityMapperMappingViaFieldForBaseProperty()
        {
            // Derived 1

            // Setup
            IdentityMapper.Add<IdentityMapperTestDerivedClass1>(new Field("ColumnId"));

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestDerivedClass1>();
            var expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Derived 2

            // Setup
            IdentityMapper.Add<IdentityMapperTestDerivedClass2>(new Field("ColumnId"));

            // Act
            actual = IdentityMapper.Get<IdentityMapperTestDerivedClass2>();
            expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestIdentityMapperMappingViaExpressionForBaseProperty()
        {
            // Derived 1

            // Setup
            IdentityMapper.Add<IdentityMapperTestDerivedClass1>(e => e.ColumnId);

            // Act
            var actual = IdentityMapper.Get<IdentityMapperTestDerivedClass1>();
            var expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Derived 2

            // Setup
            IdentityMapper.Add<IdentityMapperTestDerivedClass2>(e => e.ColumnId);

            // Act
            actual = IdentityMapper.Get<IdentityMapperTestDerivedClass2>();
            expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsIdentity() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

        }

        /*
         * Null Properties
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnIdentityMapperViaPropertyNameThatIsNull()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(propertyName: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnIdentityMapperViaFieldThatIsNull()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(field: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnIdentityMapperViaExpressionThatIsNull()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(expression: null);
        }

        /*
         * Empty Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnIdentityMapperViaPropertyNameThatIsEmpty()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(propertyName: "");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnIdentityMapperViaFieldThatIsEmpty()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(field: new Field(""));
        }

        /*
         * Empty Spaces Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnIdentityMapperViaPropertyNameThatIsEmptySpaces()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(propertyName: "  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnIdentityMapperViaFieldThatIsEmptySpaces()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(field: new Field("  "));
        }

        /*
         * Missing Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnIdentityMapperViaPropertyNameThatIsMissing()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>("Whatever");
        }

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnIdentityMapperViaFieldThatIsIsMissing()
        {
            // Setup
            IdentityMapper.Add<IdentityMapperTestClass>(new Field("Whatever"));
        }

        #endregion
    }
}
