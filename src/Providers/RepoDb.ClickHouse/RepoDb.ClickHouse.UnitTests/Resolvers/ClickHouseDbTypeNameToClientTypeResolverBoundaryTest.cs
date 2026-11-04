using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.ClickHouse.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClickHouseDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt8()
        {
            // Setup
            string name = "int8";
            Type expected = typeof(sbyte);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt8InUppercase()
        {
            // Setup
            string name = "INT8";
            Type expected = typeof(sbyte);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt16()
        {
            // Setup
            string name = "int16";
            Type expected = typeof(short);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt16InUppercase()
        {
            // Setup
            string name = "INT16";
            Type expected = typeof(short);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt32()
        {
            // Setup
            string name = "int32";
            Type expected = typeof(int);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt32InUppercase()
        {
            // Setup
            string name = "INT32";
            Type expected = typeof(int);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt64()
        {
            // Setup
            string name = "int64";
            Type expected = typeof(long);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt64InUppercase()
        {
            // Setup
            string name = "INT64";
            Type expected = typeof(long);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt128()
        {
            // Setup
            string name = "int128";
            Type expected = typeof(long);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt128InUppercase()
        {
            // Setup
            string name = "INT128";
            Type expected = typeof(long);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt256()
        {
            // Setup
            string name = "int256";
            Type expected = typeof(long);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt256InUppercase()
        {
            // Setup
            string name = "INT256";
            Type expected = typeof(long);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint8()
        {
            // Setup
            string name = "uint8";
            Type expected = typeof(byte);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint8InUppercase()
        {
            // Setup
            string name = "UINT8";
            Type expected = typeof(byte);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint16()
        {
            // Setup
            string name = "uint16";
            Type expected = typeof(ushort);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint16InUppercase()
        {
            // Setup
            string name = "UINT16";
            Type expected = typeof(ushort);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint32()
        {
            // Setup
            string name = "uint32";
            Type expected = typeof(uint);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint32InUppercase()
        {
            // Setup
            string name = "UINT32";
            Type expected = typeof(uint);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint64()
        {
            // Setup
            string name = "uint64";
            Type expected = typeof(ulong);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint64InUppercase()
        {
            // Setup
            string name = "UINT64";
            Type expected = typeof(ulong);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint128()
        {
            // Setup
            string name = "uint128";
            Type expected = typeof(ulong);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint128InUppercase()
        {
            // Setup
            string name = "UINT128";
            Type expected = typeof(ulong);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint256()
        {
            // Setup
            string name = "uint256";
            Type expected = typeof(ulong);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUint256InUppercase()
        {
            // Setup
            string name = "UINT256";
            Type expected = typeof(ulong);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat32()
        {
            // Setup
            string name = "float32";
            Type expected = typeof(float);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat32InUppercase()
        {
            // Setup
            string name = "FLOAT32";
            Type expected = typeof(float);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat64()
        {
            // Setup
            string name = "float64";
            Type expected = typeof(double);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat64InUppercase()
        {
            // Setup
            string name = "FLOAT64";
            Type expected = typeof(double);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBfloat16()
        {
            // Setup
            string name = "bfloat16";
            Type expected = typeof(float);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBfloat16InUppercase()
        {
            // Setup
            string name = "BFLOAT16";
            Type expected = typeof(float);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBool()
        {
            // Setup
            string name = "bool";
            Type expected = typeof(bool);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBoolInUppercase()
        {
            // Setup
            string name = "BOOL";
            Type expected = typeof(bool);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBoolean()
        {
            // Setup
            string name = "boolean";
            Type expected = typeof(bool);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBooleanInUppercase()
        {
            // Setup
            string name = "BOOLEAN";
            Type expected = typeof(bool);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForString()
        {
            // Setup
            string name = "string";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForStringInUppercase()
        {
            // Setup
            string name = "STRING";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFixedstring()
        {
            // Setup
            string name = "fixedstring";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFixedstringInUppercase()
        {
            // Setup
            string name = "FIXEDSTRING";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForJson()
        {
            // Setup
            string name = "json";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForJsonInUppercase()
        {
            // Setup
            string name = "JSON";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForObject()
        {
            // Setup
            string name = "object";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForObjectInUppercase()
        {
            // Setup
            string name = "OBJECT";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForEnum8()
        {
            // Setup
            string name = "enum8";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForEnum8InUppercase()
        {
            // Setup
            string name = "ENUM8";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForEnum16()
        {
            // Setup
            string name = "enum16";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForEnum16InUppercase()
        {
            // Setup
            string name = "ENUM16";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDate()
        {
            // Setup
            string name = "date";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDateInUppercase()
        {
            // Setup
            string name = "DATE";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDate32()
        {
            // Setup
            string name = "date32";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDate32InUppercase()
        {
            // Setup
            string name = "DATE32";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDatetime()
        {
            // Setup
            string name = "datetime";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDatetimeInUppercase()
        {
            // Setup
            string name = "DATETIME";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDatetime32()
        {
            // Setup
            string name = "datetime32";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDatetime32InUppercase()
        {
            // Setup
            string name = "DATETIME32";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDatetime64()
        {
            // Setup
            string name = "datetime64";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDatetime64InUppercase()
        {
            // Setup
            string name = "DATETIME64";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTime()
        {
            // Setup
            string name = "time";
            Type expected = typeof(TimeSpan);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimeInUppercase()
        {
            // Setup
            string name = "TIME";
            Type expected = typeof(TimeSpan);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTime64()
        {
            // Setup
            string name = "time64";
            Type expected = typeof(TimeSpan);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTime64InUppercase()
        {
            // Setup
            string name = "TIME64";
            Type expected = typeof(TimeSpan);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal()
        {
            // Setup
            string name = "decimal";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimalInUppercase()
        {
            // Setup
            string name = "DECIMAL";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal32()
        {
            // Setup
            string name = "decimal32";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal32InUppercase()
        {
            // Setup
            string name = "DECIMAL32";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal64()
        {
            // Setup
            string name = "decimal64";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal64InUppercase()
        {
            // Setup
            string name = "DECIMAL64";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal128()
        {
            // Setup
            string name = "decimal128";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal128InUppercase()
        {
            // Setup
            string name = "DECIMAL128";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal256()
        {
            // Setup
            string name = "decimal256";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal256InUppercase()
        {
            // Setup
            string name = "DECIMAL256";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUuid()
        {
            // Setup
            string name = "uuid";
            Type expected = typeof(Guid);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUuidInUppercase()
        {
            // Setup
            string name = "UUID";
            Type expected = typeof(Guid);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForIpv4()
        {
            // Setup
            string name = "ipv4";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForIpv4InUppercase()
        {
            // Setup
            string name = "IPV4";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForIpv6()
        {
            // Setup
            string name = "ipv6";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForIpv6InUppercase()
        {
            // Setup
            string name = "IPV6";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNone()
        {
            // Setup
            string name = "none";
            Type expected = typeof(object);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNoneInUppercase()
        {
            // Setup
            string name = "NONE";
            Type expected = typeof(object);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNothing()
        {
            // Setup
            string name = "nothing";
            Type expected = typeof(object);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNothingInUppercase()
        {
            // Setup
            string name = "NOTHING";
            Type expected = typeof(object);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Falls back for empty and unknown names.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNameReturnsObjectForEmptyString()
        {
            // Setup
            string name = "";
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(typeof(object),
                actual);
        }

        /// <summary>
        /// Falls back for empty and unknown names.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNameReturnsObjectForUnsupportedType()
        {
            // Setup
            string name = "__unsupported_type__";
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(typeof(object),
                actual);
        }

        /// <summary>
        /// Falls back for empty and unknown names.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNameReturnsObjectForWhitespace()
        {
            // Setup
            string name = "   ";
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(typeof(object),
                actual);
        }

        /// <summary>
        /// Identifies the missing database type name.
        /// </summary>
        [TestMethod]
        public void ResolveNullReportsArgumentName()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(null));

            // Assert
            Assert.AreEqual("dbTypeName",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        #endregion
    }
}