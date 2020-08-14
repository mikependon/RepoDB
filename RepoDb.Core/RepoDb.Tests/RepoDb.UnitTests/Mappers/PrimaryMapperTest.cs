using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System;

namespace RepoDb.UnitTests.Mappers
{
    [TestClass]
    public partial class PrimaryMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            PrimaryCache.Flush();
            PrimaryMapper.Clear();
        }

        #region SubClasses

        private class PrimaryMapperTestClass
        {
            public string ColumnString { get; set; }
            public int ColumnInt { get; set; }
        }

        private class PrimaryMapperTestWithAttributeClass
        {
            [Primary]
            public string ColumnString { get; set; }
            public int ColumnInt { get; set; }
        }

        private class PrimaryMapperTestBaseClass
        {
            public int ColumnId { get; set; }
        }

        private class PrimaryMapperTestDerivedClass1 : PrimaryMapperTestBaseClass
        { }

        private class PrimaryMapperTestDerivedClass2 : PrimaryMapperTestBaseClass
        { }

        #endregion

        #region Methods

        /*
         * No PrimaryAttribute
         */

        [TestMethod]
        public void TestPrimaryMapperMappingViaPropertyName()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>("ColumnInt");

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestPrimaryMapperMappingViaField()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(new Field("ColumnInt"));

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestPrimaryMapperMappingViaExpression()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(e => e.ColumnInt);

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        /*
         * With PrimaryAttribute
         */

        [TestMethod]
        public void TestPrimaryMapperMappingViaPropertyNameWithMapAttribute()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestWithAttributeClass>("ColumnInt");

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestWithAttributeClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Act
            actual = PrimaryCache.Get<PrimaryMapperTestWithAttributeClass>();
            expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestPrimaryMapperMappingViaFieldWithMapAttribute()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestWithAttributeClass>(new Field("ColumnInt"));

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestWithAttributeClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Act
            actual = PrimaryCache.Get<PrimaryMapperTestWithAttributeClass>();
            expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestPrimaryMapperMappingViaExpressionWithMapAttribute()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestWithAttributeClass>(e => e.ColumnInt);

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestWithAttributeClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Act
            actual = PrimaryCache.Get<PrimaryMapperTestWithAttributeClass>();
            expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        /*
         * Override
         */

        [TestMethod]
        public void TestPrimaryMapperMappingViaPropertyNameOverride()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>("ColumnInt");
            PrimaryMapper.Add<PrimaryMapperTestClass>("ColumnString", true);

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestClass>();
            var expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestPrimaryMapperMappingViaFieldOverride()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(new Field("ColumnInt"));
            PrimaryMapper.Add<PrimaryMapperTestClass>(new Field("ColumnString"), true);

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestClass>();
            var expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestPrimaryMapperMappingViaExpressionOverride()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(e => e.ColumnInt);
            PrimaryMapper.Add<PrimaryMapperTestClass>(e => e.ColumnString, true);

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestClass>();
            var expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPrimaryMapperViaPropertyNameThatIsAlreadyExisting()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>("ColumnInt");
            PrimaryMapper.Add<PrimaryMapperTestClass>("ColumnString");
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPrimaryMapperViaFieldThatIsAlreadyExisting()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(new Field("ColumnInt"));
            PrimaryMapper.Add<PrimaryMapperTestClass>(new Field("ColumnString"));
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPrimaryMapperViaExpressionThatIsAlreadyExisting()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(e => e.ColumnInt);
            PrimaryMapper.Add<PrimaryMapperTestClass>(e => e.ColumnString);
        }

        /*
         * Base Property
         */

        [TestMethod]
        public void TestPrimaryMapperMappingViaPropertyNameForBaseProperty()
        {
            // Derived 1

            // Setup
            PrimaryMapper.Add<PrimaryMapperTestDerivedClass1>("ColumnId");

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestDerivedClass1>();
            var expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Derived 2

            // Setup
            PrimaryMapper.Add<PrimaryMapperTestDerivedClass2>("ColumnId");

            // Act
            actual = PrimaryMapper.Get<PrimaryMapperTestDerivedClass2>();
            expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestPrimaryMapperMappingViaFieldForBaseProperty()
        {
            // Derived 1

            // Setup
            PrimaryMapper.Add<PrimaryMapperTestDerivedClass1>(new Field("ColumnId"));

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestDerivedClass1>();
            var expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Derived 2

            // Setup
            PrimaryMapper.Add<PrimaryMapperTestDerivedClass2>(new Field("ColumnId"));

            // Act
            actual = PrimaryMapper.Get<PrimaryMapperTestDerivedClass2>();
            expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestPrimaryMapperMappingViaExpressionForBaseProperty()
        {
            // Derived 1

            // Setup
            PrimaryMapper.Add<PrimaryMapperTestDerivedClass1>(e => e.ColumnId);

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestDerivedClass1>();
            var expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

            // Derived 2

            // Setup
            PrimaryMapper.Add<PrimaryMapperTestDerivedClass2>(e => e.ColumnId);

            // Act
            actual = PrimaryMapper.Get<PrimaryMapperTestDerivedClass2>();
            expected = "ColumnId";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());

        }

        /*
         * Null Properties
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPrimaryMapperViaPropertyNameThatIsNull()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(propertyName: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPrimaryMapperViaFieldThatIsNull()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(field: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPrimaryMapperViaExpressionThatIsNull()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(expression: null);
        }

        /*
         * Empty Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPrimaryMapperViaPropertyNameThatIsEmpty()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(propertyName: "");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPrimaryMapperViaFieldThatIsEmpty()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(field: new Field(""));
        }

        /*
         * Empty Spaces Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPrimaryMapperViaPropertyNameThatIsEmptySpaces()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(propertyName: "  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPrimaryMapperViaFieldThatIsEmptySpaces()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(field: new Field("  "));
        }

        /*
         * Missing Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPrimaryMapperViaPropertyNameThatIsMissing()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>("Whatever");
        }

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPrimaryMapperViaFieldThatIsIsMissing()
        {
            // Setup
            PrimaryMapper.Add<PrimaryMapperTestClass>(new Field("Whatever"));
        }

        #endregion
    }
}
