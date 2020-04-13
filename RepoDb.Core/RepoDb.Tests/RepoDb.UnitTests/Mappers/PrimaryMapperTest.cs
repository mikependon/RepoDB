using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests.Others
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

        [TestMethod]
        public void TestPrimaryMapperMappingViaPropertyInfo()
        {
            // Setup
            var propertyInfo = typeof(PrimaryMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnInt");
            PrimaryMapper.Add(propertyInfo);

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestClass>();
            var expected = "ColumnInt";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestPrimaryMapperMappingViaClassProperty()
        {
            // Setup
            var classProperty = PropertyCache.Get<PrimaryMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnInt");
            PrimaryMapper.Add(classProperty);

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

        [TestMethod]
        public void TestPrimaryMapperMappingViaPropertyInfoWithMapAttribute()
        {
            // Setup
            var propertyInfo = typeof(PrimaryMapperTestWithAttributeClass)
                .GetProperties()
                .First(p => p.Name == "ColumnInt");
            PrimaryMapper.Add(propertyInfo);

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
        public void TestPrimaryMapperMappingViaClassPropertyWithMapAttribute()
        {
            // Setup
            var classProperty = PropertyCache.Get<PrimaryMapperTestWithAttributeClass>()
                .First(p => p.PropertyInfo.Name == "ColumnInt");
            PrimaryMapper.Add(classProperty);

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

        [TestMethod]
        public void TestPrimaryMapperMappingViaPropertyInfoOverride()
        {
            // Setup
            var columnNamePropertyInfo = typeof(PrimaryMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnInt");
            var primaryColumnPropertyInfo = typeof(PrimaryMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PrimaryMapper.Add(columnNamePropertyInfo);
            PrimaryMapper.Add(primaryColumnPropertyInfo, true);

            // Act
            var actual = PrimaryMapper.Get<PrimaryMapperTestClass>();
            var expected = "ColumnString";

            // Assert
            Assert.IsTrue(actual?.IsPrimary() == true);
            Assert.AreEqual(expected, actual?.GetMappedName());
        }

        [TestMethod]
        public void TestPrimaryMapperMappingViaClassPropertyOverride()
        {
            // Setup
            var columnNameClassProperty = PropertyCache.Get<PrimaryMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnInt");
            var primaryColumnClassProperty = PropertyCache.Get<PrimaryMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            PrimaryMapper.Add(columnNameClassProperty);
            PrimaryMapper.Add(primaryColumnClassProperty, true);

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

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPrimaryMapperViaPropertyInfoThatIsAlreadyExisting()
        {
            // Setup
            var columnNamePropertyInfo = typeof(PrimaryMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnInt");
            var primaryColumnPropertyInfo = typeof(PrimaryMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PrimaryMapper.Add(columnNamePropertyInfo);
            PrimaryMapper.Add(primaryColumnPropertyInfo);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPrimaryMapperViaClassPropertyThatIsAlreadyExisting()
        {
            // Setup
            var columnNameClassProperty = PropertyCache.Get<PrimaryMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnInt");
            var primaryColumnClassProperty = PropertyCache.Get<PrimaryMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            PrimaryMapper.Add(columnNameClassProperty);
            PrimaryMapper.Add(primaryColumnClassProperty);
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

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPrimaryMapperViaPropertyInfoThatIsNull()
        {
            // Setup
            PrimaryMapper.Add((PropertyInfo)null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPrimaryMapperViaClassPropertyThatIsNull()
        {
            // Setup
            PrimaryMapper.Add((ClassProperty)null);
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
