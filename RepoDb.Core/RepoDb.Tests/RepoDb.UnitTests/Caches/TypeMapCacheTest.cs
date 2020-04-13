using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System.Data;
using System.Linq;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class TypeMapCacheTest
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
            TypeMapCache.Flush();
        }

        #region SubClasses

        private class TypeMapCacheTestClass
        {
            public string ColumnString { get; set; }
            [TypeMap(DbType.String)]
            public string PropertyString { get; set; }
        }

        #endregion

        #region Methods

        #region Type Level

        [TestMethod]
        public void TestWithoutMapping()
        {
            // Act
            var result = TypeMapCache.Get<TypeMapCacheTest>();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestWithMapping()
        {
            // Setup
            TypeMapper.Add<TypeMapCacheTestClass>(DbType.Object);

            // Act
            var result = TypeMapCache.Get<TypeMapCacheTestClass>();
            var expected = DbType.Object;

            // Assert
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Property Level

        /*
         * No MapAttribute
         */

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaPropertyName()
        {
            // Setup
            var propertyName = "ColumnString";
            TypeMapper.Add<TypeMapCacheTestClass>(propertyName, DbType.StringFixedLength);

            // Act
            var actual = TypeMapCache.Get<TypeMapCacheTestClass>(propertyName);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaField()
        {
            // Setup
            var field = new Field("ColumnString");
            TypeMapper.Add<TypeMapCacheTestClass>(field, DbType.StringFixedLength);

            // Act
            var actual = TypeMapCache.Get<TypeMapCacheTestClass>(field);
            var expected = DbType.StringFixedLength;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaExpression()
        {
            // Setup
            TypeMapper.Add<TypeMapCacheTestClass>(e => e.ColumnString, DbType.StringFixedLength);

            // Act
            var actual = TypeMapCache.Get<TypeMapCacheTestClass>(e => e.ColumnString);
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
            TypeMapper.Add<TypeMapCacheTestClass>(propertyName, DbType.StringFixedLength);

            // Act
            var actual = TypeMapCache.Get<TypeMapCacheTestClass>(propertyName);
            var expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaFieldWithMapAttribute()
        {
            // Setup
            var field = new Field("PropertyString");
            TypeMapper.Add<TypeMapCacheTestClass>(field, DbType.StringFixedLength);

            // Act
            var actual = TypeMapCache.Get<TypeMapCacheTestClass>(field);
            var expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTypeMapperPropertyMappingViaExpressionWithMapAttribute()
        {
            // Setup
            TypeMapper.Add<TypeMapCacheTestClass>(e => e.PropertyString, DbType.StringFixedLength);

            // Act
            var actual = TypeMapCache.Get<TypeMapCacheTestClass>(e => e.PropertyString);
            var expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #endregion
    }
}
