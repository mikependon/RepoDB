#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PropertyHandlers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// The integration tests of <see cref="StringToEnumPropertyHandler{TEnum}"/>. The <c>NVARCHAR(MAX)</c> column
    /// of the <c>[dbo].[CompleteTable]</c> is used to store the enumeration by its name.
    /// </summary>
    [TestClass]
    public class StringToEnumPropertyHandlerTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyHandlerMapper.Remove(typeof(Status));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Enumerations

        public enum Status
        {
            Unknown = 0,
            Active = 1,
            Inactive = 2
        }

        [Flags]
        public enum Permissions
        {
            None = 0,
            Read = 1,
            Write = 2,
            Execute = 4
        }

        #endregion

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class StatusAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(StringToEnumPropertyHandler<Status>))]
            public Status Status { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class PermissionsAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(StringToEnumPropertyHandler<Permissions>))]
            public Permissions Permissions { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class StatusFluentModel
        {
            public Guid SessionId { get; set; }

            public Status ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="Status"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class StatusTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public Status ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Negative: the handler expects a <see cref="string"/>, but the column is returned as an <see cref="int"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class StatusOnIntColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnInt"), PropertyHandler(typeof(StringToEnumPropertyHandler<Status>))]
            public Status Status { get; set; }
        }

        #endregion

        #region Helpers

        private static StatusAttributeModel CreateModel(Status status) =>
            new StatusAttributeModel { SessionId = Guid.NewGuid(), Status = status };

        private static string GetRawValue(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar(
                "SELECT [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                new { SessionId = sessionId }) as string;

        private static Guid InsertRawValue(SqlConnection connection,
            string value)
        {
            var sessionId = Guid.NewGuid();
            connection.ExecuteNonQuery(
                "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, @Value);",
                new { SessionId = sessionId, Value = value });
            return sessionId;
        }

        #endregion

        #region Positive

        [TestMethod]
        public void TestStringToEnumPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Status.Active);

                // Act
                connection.Insert(model);
                var result = connection.Query<StatusAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(Status.Active, result.Status);
            }
        }

        [TestMethod]
        public async Task TestStringToEnumPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Status.Inactive);

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<StatusAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.AreEqual(Status.Inactive, result.Status);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerWritesTheNameOfTheEnumeration()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Status.Inactive);

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("Inactive", raw, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerReadsTheNameOfTheEnumeration()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "Active");

                // Act
                var result = connection.Query<StatusAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(Status.Active, result.Status);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerWithAllTheMembers()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                foreach (var status in Enum.GetValues<Status>())
                {
                    // Setup
                    var model = CreateModel(status);

                    // Act
                    connection.Insert(model);
                    var raw = GetRawValue(connection, model.SessionId);
                    var result = connection.Query<StatusAttributeModel>(e => e.SessionId == model.SessionId).First();

                    // Assert
                    Assert.AreEqual(status.ToString(), raw, StringComparer.Ordinal);
                    Assert.AreEqual(status, result.Status);
                }
            }
        }

        [TestMethod]
        [DataRow("active")]
        [DataRow("ACTIVE")]
        [DataRow("aCtIvE")]
        public void TestStringToEnumPropertyHandlerReadsTheNameCaseInsensitively(string name)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, name);

                // Act
                var result = connection.Query<StatusAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(Status.Active, result.Status);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerReadsTheNameSurroundedBySpaces()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "  Inactive  ");

                // Act
                var result = connection.Query<StatusAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(Status.Inactive, result.Status);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerReadsTheNumericValueAsString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "2");

                // Act
                var result = connection.Query<StatusAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(Status.Inactive, result.Status);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerWithFlagsEnumeration()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = new PermissionsAttributeModel { SessionId = Guid.NewGuid(), Permissions = Permissions.Read | Permissions.Write };

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<PermissionsAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("Read,Write", raw, StringComparer.Ordinal);
                Assert.AreEqual(Permissions.Read | Permissions.Write, result.Permissions);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerWithFlagsEnumerationWrittenWithoutSpaces()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = new PermissionsAttributeModel { SessionId = Guid.NewGuid(), Permissions = Permissions.Read | Permissions.Write | Permissions.Execute };

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("Read,Write,Execute", raw, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        [DataRow("Read,Execute", Permissions.Read | Permissions.Execute)]
        [DataRow("Read, Execute", Permissions.Read | Permissions.Execute)]
        [DataRow("write,read", Permissions.Read | Permissions.Write)]
        [DataRow("None", Permissions.None)]
        [DataRow("Execute", Permissions.Execute)]
        public void TestStringToEnumPropertyHandlerReadsTheFlagsEnumeration(string text,
            Permissions expected)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, text);

                // Act
                var result = connection.Query<PermissionsAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(expected, result.Permissions);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerWithNoneFlag()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = new PermissionsAttributeModel { SessionId = Guid.NewGuid(), Permissions = Permissions.None };

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<PermissionsAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("None", raw, StringComparer.Ordinal);
                Assert.AreEqual(Permissions.None, result.Permissions);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = new[] { Status.Unknown, Status.Active, Status.Inactive, Status.Active }.Select(CreateModel).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<StatusAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    Assert.AreEqual(model.Status, result.First(e => e.SessionId == model.SessionId).Status);
                }
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Status.Active);
                connection.Insert(model);

                // Act
                model.Status = Status.Inactive;
                var affectedRows = connection.Update(model);
                var result = connection.Query<StatusAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual(Status.Inactive, result.Status);
                Assert.AreEqual("Inactive", GetRawValue(connection, model.SessionId), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Status.Inactive);
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<StatusAttributeModel>(
                    "SELECT [SessionId], [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                Assert.AreEqual(Status.Inactive, result.Status);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerOnWhereCondition()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = new[] { Status.Active, Status.Inactive, Status.Active }.Select(CreateModel).ToList();
                connection.InsertAll(models);

                // Act
                var result = connection.Query<StatusAttributeModel>(e => e.Status == Status.Active).ToList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.IsTrue(result.All(e => e.Status == Status.Active));
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<StatusFluentModel>()
                    .PropertyHandler(e => e.ColumnNVarChar, new StringToEnumPropertyHandler<Status>());
                var model = new StatusFluentModel { SessionId = Guid.NewGuid(), ColumnNVarChar = Status.Inactive };

                // Act
                connection.Insert(model);
                var result = connection.Query<StatusFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(Status.Inactive, result.ColumnNVarChar);
                Assert.AreEqual("Inactive", GetRawValue(connection, model.SessionId), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<Status, StringToEnumPropertyHandler<Status>>(true);
                var model = new StatusTypeLevelModel { SessionId = Guid.NewGuid(), ColumnNVarChar = Status.Inactive };

                // Act
                connection.Insert(model);
                var result = connection.Query<StatusTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(Status.Inactive, result.ColumnNVarChar);
                Assert.AreEqual("Inactive", GetRawValue(connection, model.SessionId), StringComparer.Ordinal);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestStringToEnumPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<StatusAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(default(Status), result.Status);
            }
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void TestStringToEnumPropertyHandlerWithBlankColumn(string text)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, text);

                // Act
                var result = connection.Query<StatusAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(default(Status), result.Status);
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerWithUnknownName()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "Purple");

                // Act / Assert
                Assert.Throws<ArgumentException>(() =>
                    connection.Query<StatusAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerWithUnknownFlagName()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "Read,Purple");

                // Act / Assert
                Assert.Throws<ArgumentException>(() =>
                    connection.Query<PermissionsAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerWithUndefinedNumericValue()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // A number that is not a member is not validated, it is written as its number and read as the same undefined value
                var model = CreateModel((Status)99);

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<StatusAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("99", raw, StringComparer.Ordinal);
                Assert.AreEqual((Status)99, result.Status);
                Assert.IsFalse(Enum.IsDefined(result.Status));
            }
        }

        [TestMethod]
        public void TestStringToEnumPropertyHandlerWithNonStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnInt]) VALUES (@SessionId, 1);",
                    new { SessionId = sessionId });

                // Act / Assert
                // The driver returns 'int', which cannot be handled by the handler (string)
                Assert.Throws<InvalidOperationException>(() =>
                    connection.Query<StatusOnIntColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        #endregion
    }
}
