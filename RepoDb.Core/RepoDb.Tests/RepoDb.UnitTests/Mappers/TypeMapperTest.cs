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
            TypeMapper.Flush();
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

        // TODO: Add the type level here

        #endregion

        #region PropertyLevel

        /*
         * No MapAttribute
         */

        [TestMethod]
        public void TestTypeMapperMappingViaPropertyName()
        {
            // Setup
            var propertyInfo = typeof(TypeMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            TypeMapper.Add<TypeMapperTestClass>("ColumnString", DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get(propertyInfo);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperMappingViaField()
        {
            // Setup
            var propertyInfo = typeof(TypeMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            TypeMapper.Add<TypeMapperTestClass>(new Field("ColumnString"), DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get(propertyInfo);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperMappingViaExpression()
        {
            // Setup
            var propertyInfo = typeof(TypeMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            TypeMapper.Add<TypeMapperTestClass>(e => e.ColumnString, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get(propertyInfo);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperMappingViaClassProperty()
        {
            // Setup
            var classProperty = PropertyCache.Get<TypeMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            TypeMapper.Add(classProperty, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get(classProperty.PropertyInfo);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperMappingViaPropertyInfo()
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
        public void TestTypeMapperMappingViaPropertyNameWithMapAttribute()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>("ColumnString", DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>("ColumnString");
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);

            // TODO: Add the TypeMapCache assertion here
        }

        [TestMethod]
        public void TestTypeMapperMappingViaFieldWithMapAttribute()
        {
            // Setup
            var field = new Field("ColumnString");
            TypeMapper.Add<TypeMapperTestClass>(field, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(field);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);

            // TODO: Add the TypeMapCache assertion here
        }

        [TestMethod]
        public void TestTypeMapperMappingViaExpressionWithMapAttribute()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>(e => e.PropertyString, DbType.StringFixedLength);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>(e => e.PropertyString);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);

            // TODO: Add the TypeMapCache assertion here
        }

        [TestMethod]
        public void TestTypeMapperMappingViaClassPropertyWithMapAttribute()
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

            // TODO: Add the TypeMapCache assertion here
        }

        [TestMethod]
        public void TestTypeMapperMappingViaPropertyInfoWithMapAttribute()
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

            // TODO: Add the TypeMapCache assertion here
        }

        /*
         * Override
         */

        [TestMethod]
        public void TestTypeMapperMappingViaPropertyNameOverride()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>("ColumnString", DbType.StringFixedLength);
            TypeMapper.Add<TypeMapperTestClass>("ColumnString", DbType.AnsiStringFixedLength, true);

            // Act
            var actual = TypeMapper.Get<TypeMapperTestClass>("ColumnString");
            var expected = DbType.AnsiStringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperMappingViaFieldOverride()
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
        public void TestTypeMapperMappingViaExpressionOverride()
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
        public void TestTypeMapperMappingViaClassPropertyOverride()
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
        public void TestTypeMapperMappingViaPropertyInfoOverride()
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
            TypeMapper.Add<TypeMapperTestClass>("ColumnString", DbType.StringFixedLength);
            TypeMapper.Add<TypeMapperTestClass>("ColumnString", DbType.AnsiStringFixedLength);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnTypeMapperViaFieldThatIsAlreadyExisting()
        {
            // Setup
            TypeMapper.Add<TypeMapperTestClass>(new Field("ColumnString"), DbType.StringFixedLength);
            TypeMapper.Add<TypeMapperTestClass>(new Field("ColumnString"), DbType.AnsiStringFixedLength);
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
