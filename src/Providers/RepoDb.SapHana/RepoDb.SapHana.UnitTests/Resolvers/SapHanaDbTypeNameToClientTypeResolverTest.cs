#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.SapHana.UnitTests.Resolvers
{
    [TestClass]
    public class SapHanaDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSapHana();
        }

        private static void AssertResolves(string dbTypeName, Type expected)
        {
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();
            var result = resolver.Resolve(dbTypeName);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForTinyInt() =>
            AssertResolves("tinyint", typeof(byte));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForSmallInt() =>
            AssertResolves("smallint", typeof(short));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForInteger() =>
            AssertResolves("integer", typeof(int));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForInt() =>
            AssertResolves("int", typeof(int));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForBigInt() =>
            AssertResolves("bigint", typeof(long));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForDecimal() =>
            AssertResolves("decimal", typeof(decimal));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForSmallDecimal() =>
            AssertResolves("smalldecimal", typeof(decimal));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForNumeric() =>
            AssertResolves("numeric", typeof(decimal));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForReal() =>
            AssertResolves("real", typeof(float));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForFloat() =>
            AssertResolves("float", typeof(float));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForDouble() =>
            AssertResolves("double", typeof(double));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForBoolean() =>
            AssertResolves("boolean", typeof(bool));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForVarChar() =>
            AssertResolves("varchar", typeof(string));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForNVarChar() =>
            AssertResolves("nvarchar", typeof(string));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForAlphanum() =>
            AssertResolves("alphanum", typeof(string));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForShortText() =>
            AssertResolves("shorttext", typeof(string));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForChar() =>
            AssertResolves("char", typeof(string));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForNChar() =>
            AssertResolves("nchar", typeof(string));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForText() =>
            AssertResolves("text", typeof(string));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForClob() =>
            AssertResolves("clob", typeof(string));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForNClob() =>
            AssertResolves("nclob", typeof(string));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForString() =>
            AssertResolves("string", typeof(string));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForDate() =>
            AssertResolves("date", typeof(DateTime));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForTime() =>
            AssertResolves("time", typeof(TimeSpan));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForTimestamp() =>
            AssertResolves("timestamp", typeof(DateTime));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForSecondDate() =>
            AssertResolves("seconddate", typeof(DateTime));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForBlob() =>
            AssertResolves("blob", typeof(byte[]));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForVarBinary() =>
            AssertResolves("varbinary", typeof(byte[]));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForBinary() =>
            AssertResolves("binary", typeof(byte[]));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForBinText() =>
            AssertResolves("bintext", typeof(byte[]));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForNone() =>
            AssertResolves("none", typeof(object));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForUnknown() =>
            AssertResolves("some_unrecognized_type_name", typeof(object));

        [TestMethod]
        public void TestSapHanaDbTypeNameToClientTypeResolverForNullThrowsException()
        {
            // Setup
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();

            // Act
            Assert.Throws<NullReferenceException>(() => resolver.Resolve(null));
        }
    }
}
