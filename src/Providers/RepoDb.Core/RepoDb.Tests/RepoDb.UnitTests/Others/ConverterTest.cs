#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class ConverterTest
    {
        #region SubClasses

        private class ConverterTestClass
        {
            public int Id { get; set; }
        }

        private enum ConverterTestEnum
        {
            None = 0,
            Some = 1
        }

        #endregion

        #region Private Variables

        private ConversionType _previousConversionType;

        #endregion

        #region Setup

        [TestInitialize]
        public void Initialize()
        {
            _previousConversionType = GlobalConfiguration.Options.ConversionType;
            GlobalConfiguration.Options.ConversionType = ConversionType.Default;
        }

        [TestCleanup]
        public void Cleanup()
        {
            GlobalConfiguration.Options.ConversionType = _previousConversionType;
        }

        #endregion

        #region ToType<T> (DBNull)

        [TestMethod]
        public void TestToTypeOfObjectWithDbNullReturnsNull()
        {
            // Act
            var result = Converter.ToType<object>(DBNull.Value);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfObjectWithNullReturnsNull()
        {
            // Act
            var result = Converter.ToType<object>(null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfStringWithDbNullReturnsNull()
        {
            // Act
            var result = Converter.ToType<string>(DBNull.Value);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfByteArrayWithDbNullReturnsNull()
        {
            // Act
            var result = Converter.ToType<byte[]>(DBNull.Value);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfClassWithDbNullReturnsNull()
        {
            // Act
            var result = Converter.ToType<ConverterTestClass>(DBNull.Value);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfNullableValueTypeWithDbNullReturnsNull()
        {
            // Act
            var result = Converter.ToType<int?>(DBNull.Value);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region ToType<T> (Value Types)

        [TestMethod]
        public void ThrowExceptionOnToTypeOfValueTypeWithDbNull()
        {
            // Act/Assert
            Assert.ThrowsExactly<InvalidCastException>(() => Converter.ToType<int>(DBNull.Value));
        }

        [TestMethod]
        public void ThrowExceptionOnToTypeOfValueTypeWithNull()
        {
            // Act/Assert
            Assert.ThrowsExactly<InvalidCastException>(() => Converter.ToType<int>(null));
        }

        [TestMethod]
        public void ThrowExceptionOnToTypeOfEnumWithDbNull()
        {
            // Act/Assert
            Assert.ThrowsExactly<InvalidCastException>(() => Converter.ToType<ConverterTestEnum>(DBNull.Value));
        }

        [TestMethod]
        public void TestToTypeOfValueTypeWithDbNullReturnsDefaultIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<int>(DBNull.Value);

            // Assert
            Assert.AreEqual(0, result);
        }

        #endregion

        #region ToType<T> (Values)

        [TestMethod]
        public void TestToTypeOfObjectWithValueReturnsTheValue()
        {
            // Act
            var result = Converter.ToType<object>("value");

            // Assert
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void TestToTypeOfStringWithValueReturnsTheValue()
        {
            // Act
            var result = Converter.ToType<string>("value");

            // Assert
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void TestToTypeOfIntWithLongValueReturnsTheConvertedValue()
        {
            // Act
            var result = Converter.ToType<int>(5L);

            // Assert
            Assert.AreEqual(5, result);
        }

        #endregion

        #region ToType (Non-Generic)

        [TestMethod]
        public void TestNonGenericToTypeOfObjectWithDbNullReturnsNull()
        {
            // Act
            var result = Converter.ToType(DBNull.Value,
                typeof(object));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNonGenericToTypeOfObjectWithNullReturnsNull()
        {
            // Act
            var result = Converter.ToType(null,
                typeof(object));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNonGenericToTypeOfStringWithDbNullReturnsNull()
        {
            // Act
            var result = Converter.ToType(DBNull.Value,
                typeof(string));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNonGenericToTypeOfByteArrayWithDbNullReturnsNull()
        {
            // Act
            var result = Converter.ToType(DBNull.Value,
                typeof(byte[]));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNonGenericToTypeOfNullableValueTypeWithDbNullReturnsNull()
        {
            // Act
            var result = Converter.ToType(DBNull.Value,
                typeof(int?));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ThrowExceptionOnNonGenericToTypeOfValueTypeWithDbNull()
        {
            // Act/Assert
            Assert.ThrowsExactly<InvalidCastException>(() => Converter.ToType(DBNull.Value,
                typeof(int)));
        }

        [TestMethod]
        public void TestNonGenericToTypeOfValueTypeWithDbNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType(DBNull.Value,
                typeof(int));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNonGenericToTypeOfObjectWithValueReturnsTheValue()
        {
            // Act
            var result = Converter.ToType("value",
                typeof(object));

            // Assert
            Assert.AreEqual("value", result);
        }

        #endregion

        #region ToType<T> (Automatic)

        [TestMethod]
        public void TestToTypeOfObjectWithDbNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<object>(DBNull.Value);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfObjectWithNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<object>(null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfStringWithDbNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<string>(DBNull.Value);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfByteArrayWithDbNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<byte[]>(DBNull.Value);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfClassWithDbNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<ConverterTestClass>(DBNull.Value);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfNullableValueTypeWithDbNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<int?>(DBNull.Value);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestToTypeOfValueTypeWithNullReturnsDefaultIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<int>(null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestToTypeOfEnumWithDbNullReturnsDefaultIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<ConverterTestEnum>(DBNull.Value);

            // Assert
            Assert.AreEqual(ConverterTestEnum.None, result);
        }

        [TestMethod]
        public void TestToTypeOfObjectWithValueReturnsTheValueIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<object>("value");

            // Assert
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void TestToTypeOfIntWithLongValueReturnsTheConvertedValueIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType<int>(5L);

            // Assert
            Assert.AreEqual(5, result);
        }

        #endregion

        #region ToType<T> (Force Automatic)

        [TestMethod]
        public void TestToTypeOfValueTypeWithDbNullReturnsDefaultIfForcedAutomatic()
        {
            // Act
            var result = Converter.ToType<int>(DBNull.Value,
                forceAutomatic: true);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestToTypeOfObjectWithDbNullReturnsNullIfForcedAutomatic()
        {
            // Act
            var result = Converter.ToType<object>(DBNull.Value,
                forceAutomatic: true);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ThrowExceptionOnToTypeOfValueTypeWithDbNullIfNotForcedAutomatic()
        {
            // Act/Assert
            Assert.ThrowsExactly<InvalidCastException>(() => Converter.ToType<int>(DBNull.Value,
                forceAutomatic: false));
        }

        #endregion

        #region ToType (Non-Generic, Automatic)

        [TestMethod]
        public void TestNonGenericToTypeOfObjectWithDbNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType(DBNull.Value,
                typeof(object));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNonGenericToTypeOfStringWithDbNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType(DBNull.Value,
                typeof(string));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNonGenericToTypeOfByteArrayWithDbNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType(DBNull.Value,
                typeof(byte[]));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNonGenericToTypeOfNullableValueTypeWithDbNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType(DBNull.Value,
                typeof(int?));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNonGenericToTypeOfValueTypeWithNullReturnsNullIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType(null,
                typeof(int));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestNonGenericToTypeOfObjectWithValueReturnsTheValueIfAutomatic()
        {
            // Setup
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;

            // Act
            var result = Converter.ToType("value",
                typeof(object));

            // Assert
            Assert.AreEqual("value", result);
        }

        #endregion
    }
}
