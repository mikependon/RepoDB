#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Reflection;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class CompiledParameterSetterTest
    {
        [TestMethod]
        public void TestEntityParameterSetterValue()
        {
            // Setup
            using var connection = new CustomDbConnection();
            using var command = connection.CreateCommand();
            var setting = new CustomDbSetting();
            var field = new DbField("Name", false, false, true, typeof(string), null, null, null, null);
            var setter = FunctionFactory.CompileDataEntityDbParameterSetter(typeof(ParameterEntity), new[] { field }, null, setting, new CustomDbHelper());
            var entity = new ParameterEntity { Name = "Value" };

            // Act
            setter(command, entity);

            // Assert
            Assert.AreEqual("Value", command.Parameters[0].Value);
            Assert.AreEqual("@Name", command.Parameters[0].ParameterName, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEntityParameterSetterMetadata()
        {
            // Setup
            using var connection = new CustomDbConnection();
            using var command = connection.CreateCommand();
            var setting = new DirectionSupportingDbSetting();
            var field = new DbField("Name", false, false, true, typeof(string), 32, (byte)10, (byte)2, null);
            var setter = FunctionFactory.CompileDataEntityDbParameterSetter(typeof(ParameterEntity), new[] { field }, null, setting, new CustomDbHelper());
            var entity = new ParameterEntity { Name = "Value" };

            // Act
            setter(command, entity);

            // Assert
            Assert.AreEqual(32, command.Parameters[0].Size);
            Assert.AreEqual((byte)10, command.Parameters[0].Precision);
            Assert.AreEqual((byte)2, command.Parameters[0].Scale);
        }

        [TestMethod]
        public void TestEntityListParameterSetter()
        {
            // Setup
            using var connection = new CustomDbConnection();
            using var command = connection.CreateCommand();
            var fields = new[] { new DbField("Name", false, false, true, typeof(string), null, null, null, null) };
            var setter = FunctionFactory.CompileDataEntityListDbParameterSetter(typeof(ParameterEntity), fields, null, 2, new CustomDbSetting(), new CustomDbHelper());
            var entities = new List<object> { new ParameterEntity { Name = "Value" }, new ParameterEntity { Name = "Second" } };

            // Act
            setter(command, entities);

            // Assert
            Assert.AreEqual(2, command.Parameters.Count);
            Assert.AreEqual("Second", command.Parameters[1].Value);
        }

        [TestMethod]
        public void TestDictionaryParameterSetterValue()
        {
            // Setup
            using var connection = new CustomDbConnection();
            using var command = connection.CreateCommand();
            var setting = new CustomDbSetting();
            var field = new DbField("Name", false, false, true, typeof(string), null, null, null, null);
            var setter = FunctionFactory.CompileDictionaryStringObjectDbParameterSetter(typeof(Dictionary<string, object>), new[] { field }, setting, new CustomDbHelper());
            var entity = new Dictionary<string, object>(StringComparer.Ordinal) { ["Name"] = "Value" };

            // Act
            setter(command, entity);

            // Assert
            Assert.AreEqual("Value", command.Parameters[0].Value);
            Assert.AreEqual("@Name", command.Parameters[0].ParameterName, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDictionaryParameterSetterMetadata()
        {
            // Setup
            using var connection = new CustomDbConnection();
            using var command = connection.CreateCommand();
            var setting = new DirectionSupportingDbSetting();
            var field = new DbField("Name", false, false, true, typeof(string), 32, (byte)10, (byte)2, null);
            var setter = FunctionFactory.CompileDictionaryStringObjectDbParameterSetter(typeof(Dictionary<string, object>), new[] { field }, setting, new CustomDbHelper());
            var entity = new Dictionary<string, object>(StringComparer.Ordinal) { ["Name"] = "Value" };

            // Act
            setter(command, entity);

            // Assert
            Assert.AreEqual(32, command.Parameters[0].Size);
            Assert.AreEqual((byte)10, command.Parameters[0].Precision);
            Assert.AreEqual((byte)2, command.Parameters[0].Scale);
        }

        [TestMethod]
        public void TestDictionaryListParameterSetter()
        {
            // Setup
            using var connection = new CustomDbConnection();
            using var command = connection.CreateCommand();
            var fields = new[] { new DbField("Name", false, false, true, typeof(string), null, null, null, null) };
            var setter = FunctionFactory.CompileDictionaryStringObjectListDbParameterSetter(typeof(Dictionary<string, object>), fields, 2, new CustomDbSetting(), new CustomDbHelper());
            var entities = new List<object> { new Dictionary<string, object>(StringComparer.Ordinal) { ["Name"] = "Value" }, new Dictionary<string, object>(StringComparer.Ordinal) { ["Name"] = "Second" } };

            // Act
            setter(command, entities);

            // Assert
            Assert.AreEqual(2, command.Parameters.Count);
            Assert.AreEqual("Second", command.Parameters[1].Value);
        }

        [TestMethod]
        public void TestDictionaryItemSetter()
        {
            // Setup
            var input = new Dictionary<string, object>(StringComparer.Ordinal);
            var setter = FunctionFactory.CompileDictionaryStringObjectItemSetter(typeof(Dictionary<string, object>), new Field("Name", typeof(string)));

            // Act
            setter(input, "Value");

            // Assert
            Assert.AreEqual("Value", input["Name"]);
        }

        [TestMethod]
        public void TestCommandOutputPropertySetter()
        {
            // Setup
            using var connection = new CustomDbConnection();
            using var command = connection.CreateCommand();
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@Name";
            parameter.Value = "Value";
            command.Parameters.Add(parameter);
            var setter = FunctionFactory.CompileDbCommandToProperty<ParameterEntity>(new Field("Name", typeof(string)), "@Name", 0, new CustomDbSetting());
            var entity = new ParameterEntity();

            // Act
            setter(entity, command);

            // Assert
            Assert.AreEqual("Value", entity.Name, StringComparer.Ordinal);
        }

        private sealed class DirectionSupportingDbSetting : CustomDbSetting
        {
            public DirectionSupportingDbSetting()
            {
                IsDirectionSupported = true;
            }
        }

        public class ParameterEntity
        {
            public string Name { get; set; }
        }
    }
}
