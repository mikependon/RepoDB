using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class TypeMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            TypeMapper.Clear();
        }

        #region SubClasses

        private class TypeMapperTestClass
        {
            public string ColumnString { get; set; }
            [TypeMap(DbType.String)]
            public string PropertyString { get; set; }
        }

        #endregion

        #region Methods

        #region TypeLevel

        [TestMethod]
        public void TestTypeMapperTypeMapping()
        {
            // Setup
            TypeMapper.Add<DateTime>(DbType.DateTime2);

            // Act
            var actual = TypeMapper.Get<DateTime>();
            var expected = DbType.DateTime2;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperTypeMappingWithoutMapping()
        {
            // Act
            var actual = TypeMapper.Get<DateTime>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion

        #region PropertyLevel

        /*
         * No MapAttribute
         */

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaPropertyName()
        {
            // Setup
            var propertyName = "ColumnString";
            TypeMapper.Add<TypeMapperTestClass>(propertyName, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(propertyName);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaField()
        {
            // Setup
            var field = new Field("ColumnString");
            TypeMapper.Add<TypeMapperTestClass>(field, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(field);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaExpression()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>(e => e.ColumnString, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(e => e.ColumnString);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaClassProperty()
        {
            // Setup
            var classProperty = PropertyCache.Get<TypeMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            TypeMapper.Add(classProperty, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get(classProperty);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaPropertyInfo()
        {
            // Setup
            var propertyInfo = typeof(TypeMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            TypeMapper.Add(propertyInfo, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get(propertyInfo);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaPropertyNameWithMapAttribute()
        {
            // Setup
            var propertyName = "PropertyString";
            TypeMapper.Add<TypeMapperTestClass>(propertyName, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(propertyName);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);

            // Act
            actual = TypeMapCache.Get<TypeMapperTestClass>(propertyName);
            expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaFieldWithMapAttribute()
        {
            // Setup
            var field = new Field("PropertyString");
            TypeMapper.Add<TypeMapperTestClass>(field, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(field);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);

            // Act
            actual = TypeMapCache.Get<TypeMapperTestClass>(field);
            expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaExpressionWithMapAttribute()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>(e => e.PropertyString, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(e => e.PropertyString);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);

            // Act
            actual = TypeMapCache.Get<TypeMapperTestClass>(e => e.PropertyString);
            expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaClassPropertyWithMapAttribute()
        {
            // Setup
            var classProperty = PropertyCache.Get<TypeMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "PropertyString");
            TypeMapper.Add(classProperty, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get(classProperty);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);

            // Act
            actual = TypeMapCache.Get(classProperty);
            expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaPropertyInfoWithMapAttribute()
        {
            // Setup
            var propertyInfo = typeof(TypeMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "PropertyString");
            TypeMapper.Add(propertyInfo, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get(propertyInfo);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);

            // Act
            actual = TypeMapCache.Get(propertyInfo);
            expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override
         */

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaPropertyNameOverride()
        {
            // Setup
            var propertyName = "ColumnString";
            TypeMapper.Add<TypeMapperTestClass>(propertyName, DbType.StringFixedLength);
            TypeMapper.Add<TypeMapperTestClass>(propertyName, DbType.AnsiStringFixedLength, true);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(propertyName);
            var expected = DbType.AnsiStringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaFieldOverride()
        {
            // Setup
            var field = new Field("ColumnString");
            TypeMapper.Add<TypeMapperTestClass>(field, DbType.StringFixedLength);
            TypeMapper.Add<TypeMapperTestClass>(field, DbType.AnsiStringFixedLength, true);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(field);
            var expected = DbType.AnsiStringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaExpressionOverride()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>(e => e.ColumnString, DbType.StringFixedLength);
            TypeMapper.Add<TypeMapperTestClass>(e => e.ColumnString, DbType.AnsiStringFixedLength, true);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(e => e.ColumnString);
            var expected = DbType.AnsiStringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaClassPropertyOverride()
        {
            // Setup
            var classProperty = PropertyCache.Get<TypeMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            TypeMapper.Add(classProperty, DbType.StringFixedLength);
            TypeMapper.Add(classProperty, DbType.AnsiStringFixedLength, true);

            // Act
            var actual = TypeMapper.Get(classProperty);
            var expected = DbType.AnsiStringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaPropertyInfoOverride()
        {
            // Setup
            var propertyInfo = typeof(TypeMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            TypeMapper.Add(propertyInfo, DbType.StringFixedLength);
            TypeMapper.Add(propertyInfo, DbType.AnsiStringFixedLength, true);

            // Act
            var actual = TypeMapper.Get(propertyInfo);
            var expected = DbType.AnsiStringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnTypeMapperViaPropertyNameThatIsAlreadyExisting()
        {
            // Setup
            var propertyName = "ColumnString";
            TypeMapper.Add<TypeMapperTestClass>(propertyName, DbType.StringFixedLength);
            TypeMapper.Add<TypeMapperTestClass>(propertyName, DbType.AnsiStringFixedLength);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnTypeMapperViaFieldThatIsAlreadyExisting()
        {
            // Setup
            var field = new Field("ColumnString");
            TypeMapper.Add<TypeMapperTestClass>(field, DbType.StringFixedLength);
            TypeMapper.Add<TypeMapperTestClass>(field, DbType.AnsiStringFixedLength);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnTypeMapperViaExpressionThatIsAlreadyExisting()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>(e => e.ColumnString, DbType.StringFixedLength);
            TypeMapper.Add<TypeMapperTestClass>(e => e.ColumnString, DbType.AnsiStringFixedLength);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnTypeMapperViaClassPropertyThatIsAlreadyExisting()
        {
            // Setup
            var classProperty = PropertyCache.Get<TypeMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            TypeMapper.Add(classProperty, DbType.StringFixedLength);
            TypeMapper.Add(classProperty, DbType.AnsiStringFixedLength);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnTypeMapperViaPropertyInfoThatIsAlreadyExisting()
        {
            // Setup
            var propertyInfo = typeof(TypeMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            TypeMapper.Add(propertyInfo, DbType.StringFixedLength);
            TypeMapper.Add(propertyInfo, DbType.AnsiStringFixedLength);
        }

        /*
         * Null Properties
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnTypeMapperViaPropertyNameThatIsNull()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>((string)null, DbType.AnsiStringFixedLength);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnTypeMapperViaFieldThatIsNull()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>((Field)null, DbType.AnsiStringFixedLength);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnTypeMapperViaExpressionThatIsNull()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>(expression: null, dbType: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnTypeMapperViaClassPropertyThatIsNull()
        {
            // Setup
            TypeMapper.Add((ClassProperty)null, DbType.StringFixedLength);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnTypeMapperViaPropertyInfoThatIsNull()
        {
            // Setup
            TypeMapper.Add((PropertyInfo)null, DbType.StringFixedLength);
        }

        /*
         * Missing Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnTypeMapperViaPropertyNameThatIsMissing()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>("Whatever", DbType.AnsiStringFixedLength);
        }

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnTypeMapperViaFieldThatIsIsMissing()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>(new Field("Whatever"), DbType.AnsiStringFixedLength);
        }

        /*
         * Null ColumnName
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnTypeMapperViaPropertyNameWithNullTargetColumnName()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>("ColumnString", null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnTypeMapperViaFieldWithNullTargetColumnName()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>(new Field("ColumnString"), null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnTypeMapperViaExpressionWithNullTargetColumnName()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>(e => e.ColumnString, null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnTypeMapperViaClassPropertyWithNullTargetColumnName()
        {
            // Setup
            var classProperty = PropertyCache.Get<TypeMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            TypeMapper.Add(classProperty, null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnTypeMapperViaPropertyInfoWithNullTargetColumnName()
        {
            // Setup
            var propertyInfo = typeof(TypeMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            TypeMapper.Add(propertyInfo, null);
        }

        #endregion

        #endregion
    }
}
