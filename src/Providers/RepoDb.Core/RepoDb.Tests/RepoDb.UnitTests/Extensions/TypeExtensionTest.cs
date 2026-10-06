#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.UnitTests.Extensions
{
    [TestClass]
    public class TypeExtensionTest
    {
        #region SubClasses

        private enum Kind
        {
            None,
            Some
        }

        private class StringHandler : IPropertyHandler<string, string>
        {
            public string Get(string input, PropertyHandlerGetOptions options) => input;

            public string Set(string input, PropertyHandlerSetOptions options) => input;
        }

        private class DerivedStringHandler : StringHandler
        {
        }

        private class PersonHandler : IClassHandler<Person>
        {
            public Person Get(Person entity, ClassHandlerGetOptions options) => entity;

            public Person Set(Person entity, ClassHandlerSetOptions options) => entity;
        }

        private class Person
        {
            public int Id { get; set; }
        }

        private class Employee : Person
        {
        }

        #endregion

        #region IsInterfacedTo

        [TestMethod]
        public void TestTypeIsInterfacedToGenericTypeDefinition()
        {
            Assert.IsTrue(typeof(StringHandler).IsInterfacedTo(typeof(IPropertyHandler<,>)));
            Assert.IsTrue(typeof(DerivedStringHandler).IsInterfacedTo(typeof(IPropertyHandler<,>)));
            Assert.IsTrue(typeof(PersonHandler).IsInterfacedTo(typeof(IClassHandler<>)));
        }

        [TestMethod]
        public void TestTypeIsInterfacedToGenericTypeDefinitionForTheInterfaceItself()
        {
            Assert.IsTrue(typeof(IPropertyHandler<string, int>).IsInterfacedTo(typeof(IPropertyHandler<,>)));
        }

        [TestMethod]
        public void TestTypeIsNotInterfacedToGenericTypeDefinition()
        {
            Assert.IsFalse(typeof(StringHandler).IsInterfacedTo(typeof(IClassHandler<>)));
            Assert.IsFalse(typeof(Person).IsInterfacedTo(typeof(IPropertyHandler<,>)));
            Assert.IsFalse(typeof(IList<string>).IsInterfacedTo(typeof(IPropertyHandler<,>)));
        }

        [TestMethod]
        public void TestTypeIsInterfacedToNonGenericInterface()
        {
            Assert.IsTrue(typeof(List<string>).IsInterfacedTo(typeof(System.Collections.IEnumerable)));
            Assert.IsFalse(typeof(Person).IsInterfacedTo(typeof(System.Collections.IEnumerable)));
        }

        [TestMethod]
        public void TestTypeIsInterfacedToWithNullArguments()
        {
            Assert.IsFalse(((Type)null).IsInterfacedTo(typeof(IPropertyHandler<,>)));
            Assert.IsFalse(typeof(StringHandler).IsInterfacedTo(null));
        }

        #endregion

        #region MakeNullableType / GetDefaultValue

        [TestMethod]
        public void TestTypeMakeNullableType()
        {
            Assert.AreEqual(typeof(int?), typeof(int).MakeNullableType());
            Assert.AreEqual(typeof(Kind?), typeof(Kind).MakeNullableType());
            Assert.AreEqual(typeof(DateTime?), typeof(DateTime).MakeNullableType());
        }

        [TestMethod]
        public void TestTypeGetDefaultValue()
        {
            Assert.AreEqual(0, typeof(int).GetDefaultValue());
            Assert.AreEqual(Kind.None, typeof(Kind).GetDefaultValue());
            Assert.AreEqual(Guid.Empty, typeof(Guid).GetDefaultValue());
            Assert.IsNull(typeof(string).GetDefaultValue());
            Assert.IsNull(typeof(int?).GetDefaultValue());
            Assert.IsNull(((Type)null).GetDefaultValue());
        }

        #endregion

        #region GetRuntimeType

        [TestMethod]
        public void TestTypeGetRuntimeTypeOfTheGenericTypeArgument()
        {
            Assert.AreEqual(typeof(Person), TypeExtension.GetRuntimeType<Person>(new Person()));
            Assert.AreEqual(typeof(Person), TypeExtension.GetRuntimeType<Person>(null));
        }

        [TestMethod]
        public void TestTypeGetRuntimeTypeOfDerivedOrWidenedInstances()
        {
            Assert.AreEqual(typeof(Employee), TypeExtension.GetRuntimeType<Person>(new Employee()));
            Assert.AreEqual(typeof(Employee), TypeExtension.GetRuntimeType<object>(new Employee()));
            Assert.AreEqual(typeof(Employee), new Employee().GetRuntimeType());
        }

        #endregion

        #region Converter.ToType

        [TestMethod]
        public void TestConverterToTypeReturnsTheValueOfTheSameType()
        {
            Assert.AreEqual(10, Converter.ToType(10, typeof(int)));
            Assert.AreEqual("A", Converter.ToType("A", typeof(string)));
        }

        [TestMethod]
        public void TestConverterToTypeConvertsTheValue()
        {
            Assert.AreEqual(10L, Converter.ToType(10, typeof(long)));
            Assert.AreEqual(10.5m, Converter.ToType("10.5", typeof(decimal)));
            var guid = Guid.NewGuid();
            Assert.AreEqual(guid, Converter.ToType(guid.ToString(), typeof(Guid)));
        }

        [TestMethod]
        public void TestConverterToTypeReturnsNullForTheDefaultValueOfNullableTypes()
        {
            Assert.IsNull(Converter.ToType(null, typeof(int?)));
            Assert.IsNull(Converter.ToType(DBNull.Value, typeof(int?)));
            Assert.IsNull(Converter.ToType(null, typeof(string)));
        }

        [TestMethod]
        public void ThrowExceptionOnConverterToTypeIfTheValueIsNullForValueTypes()
        {
            Assert.ThrowsExactly<InvalidCastException>(() => Converter.ToType(DBNull.Value, typeof(int)));
        }

        [TestMethod]
        public void ThrowExceptionOnConverterToTypeIfTheGuidIsInvalid()
        {
            Assert.ThrowsExactly<InvalidCastException>(() => Converter.ToType("not-a-guid", typeof(Guid)));
        }

        #endregion
    }
}
