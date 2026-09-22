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
using System.Xml;

namespace RepoDb.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// The integration tests of <see cref="XmlToXmlDocumentPropertyHandler"/>. The <c>XML</c> column (returned by the driver as a <see cref="string"/>)
    /// and the <c>NVARCHAR(MAX)</c> column of the <c>[dbo].[CompleteTable]</c> are used to store the <see cref="XmlDocument"/>.
    /// </summary>
    [TestClass]
    public class XmlToXmlDocumentPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(XmlDocument));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class XmlDocumentXmlColumnAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnXml"), PropertyHandler(typeof(XmlToXmlDocumentPropertyHandler))]
            public XmlDocument Document { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class XmlDocumentTextColumnAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(XmlToXmlDocumentPropertyHandler))]
            public XmlDocument Document { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class XmlDocumentFluentModel
        {
            public Guid SessionId { get; set; }

            public XmlDocument ColumnXml { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="XmlDocument"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class XmlDocumentTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public XmlDocument ColumnXml { get; set; }
        }

        #endregion

        #region Helpers

        private static XmlDocument Parse(string xml)
        {
            var document = new XmlDocument();
            document.LoadXml(xml);
            return document;
        }

        private static XmlDocumentXmlColumnAttributeModel CreateModel(string xml) =>
            new XmlDocumentXmlColumnAttributeModel { SessionId = Guid.NewGuid(), Document = xml == null ? null : Parse(xml) };

        private static XmlDocumentTextColumnAttributeModel CreateTextModel(string xml) =>
            new XmlDocumentTextColumnAttributeModel { SessionId = Guid.NewGuid(), Document = xml == null ? null : Parse(xml) };

        private static string GetRawValue(SqlConnection connection,
            string column,
            Guid sessionId) =>
            connection.ExecuteScalar(
                $"SELECT CAST([{column}] AS NVARCHAR(MAX)) FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                new { SessionId = sessionId }) as string;

        private static bool IsRawValueNull(SqlConnection connection,
            string column,
            Guid sessionId) =>
            connection.ExecuteScalar<int>(
                $"SELECT COUNT(1) FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId AND [{column}] IS NULL;",
                new { SessionId = sessionId }) == 1;

        private static Guid InsertRawValue(SqlConnection connection,
            string column,
            string value)
        {
            var sessionId = Guid.NewGuid();
            connection.ExecuteNonQuery(
                $"INSERT INTO [dbo].[CompleteTable] ([SessionId], [{column}]) VALUES (@SessionId, @Value);",
                new { SessionId = sessionId, Value = value });
            return sessionId;
        }

        #endregion

        #region Positive

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root a=\"1\"><child>text</child></root>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(result.Document);
                Assert.AreEqual("root", result.Document.DocumentElement.Name, StringComparer.Ordinal);
                Assert.AreEqual("1", result.Document.DocumentElement.GetAttribute("a"), StringComparer.Ordinal);
                Assert.AreEqual("text", result.Document.SelectSingleNode("/root/child").InnerText, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestXmlToXmlDocumentPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root a=\"1\"><child>text</child></root>");

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.AreEqual("text", result.Document.SelectSingleNode("/root/child").InnerText, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWritesTheOuterXml()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateTextModel("<root>\n  <child a=\"1\">text</child>\n  <empty/>\n</root>");

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, "ColumnNVarChar", model.SessionId);

                // Assert
                // The default loading drops the insignificant whitespaces
                Assert.AreEqual("<root><child a=\"1\">text</child><empty /></root>", raw, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerReadsXmlText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", "<items><item id=\"1\"/><item id=\"2\"/></items>");

                // Act
                var result = connection.Query<XmlDocumentTextColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual("items", result.Document.DocumentElement.Name, StringComparer.Ordinal);
                Assert.AreEqual(2, result.Document.SelectNodes("/items/item").Count);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerReadsXmlColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnXml", "<items><item id=\"1\"/><item id=\"2\"/></items>");

                // Act
                var result = connection.Query<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual("items", result.Document.DocumentElement.Name, StringComparer.Ordinal);
                Assert.AreEqual(2, result.Document.SelectNodes("/items/item").Count);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWritesXmlColumnAsQueryableXml()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root><child>value-1</child></root>");

                // Act
                connection.Insert(model);
                var value = connection.ExecuteScalar<string>(
                    "SELECT [ColumnXml].value('(/root/child)[1]', 'NVARCHAR(50)') FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId });

                // Assert
                Assert.AreEqual("value-1", value, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWithNamespacesAndPrefixes()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<a:root xmlns:a=\"urn:a\" xmlns=\"urn:default\"><child a:attr=\"1\">x</child><a:other/></a:root>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                var namespaces = new XmlNamespaceManager(result.Document.NameTable);
                namespaces.AddNamespace("a", "urn:a");
                namespaces.AddNamespace("d", "urn:default");
                Assert.AreEqual("urn:a", result.Document.DocumentElement.NamespaceURI, StringComparer.Ordinal);
                Assert.AreEqual("x", result.Document.SelectSingleNode("/a:root/d:child", namespaces).InnerText, StringComparer.Ordinal);
                Assert.AreEqual("1", result.Document.SelectSingleNode("/a:root/d:child/@a:attr", namespaces).Value, StringComparer.Ordinal);
                Assert.IsNotNull(result.Document.SelectSingleNode("/a:root/a:other", namespaces));
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWithEscapedAndUnicodeCharacters()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<t a=\"&quot;q&quot;\">a &amp; b &lt; c 日本語 Ñandú 😀</t>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("a & b < c 日本語 Ñandú 😀", result.Document.DocumentElement.InnerText, StringComparer.Ordinal);
                Assert.AreEqual("\"q\"", result.Document.DocumentElement.GetAttribute("a"), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWithCData()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<t><![CDATA[<not>&xml]]></t>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("<not>&xml", result.Document.DocumentElement.InnerText, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWithXmlDeclarationOnTextColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // Unlike the 'XDocument', the outer xml of the 'XmlDocument' contains the declaration
                var model = CreateTextModel("<?xml version=\"1.0\" encoding=\"utf-8\"?><root/>");

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, "ColumnNVarChar", model.SessionId);
                var result = connection.Query<XmlDocumentTextColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><root />", raw, StringComparer.Ordinal);
                var declaration = result.Document.FirstChild as XmlDeclaration;
                Assert.IsNotNull(declaration);
                Assert.AreEqual("1.0", declaration.Version, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWithLargeDocument()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var xml = "<root>" + string.Concat(Enumerable.Range(1, 2000).Select(i => $"<item id=\"{i}\">value-{i}</item>")) + "</root>";
                var model = CreateModel(xml);

                // Act
                connection.Insert(model);
                var result = connection.Query<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(2000, result.Document.SelectNodes("/root/item").Count);
                Assert.AreEqual("value-2000", result.Document.SelectSingleNode("/root/item[@id='2000']").InnerText, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10).Select(i => CreateModel($"<root id=\"{i}\"/>")).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<XmlDocumentXmlColumnAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                for (var i = 0; i < models.Count; i++)
                {
                    var item = result.First(e => e.SessionId == models[i].SessionId);
                    Assert.AreEqual((i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture), item.Document.DocumentElement.GetAttribute("id"), StringComparer.Ordinal);
                }
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root v=\"1\"/>");
                connection.Insert(model);

                // Act
                model.Document = Parse("<root v=\"2\"/>");
                var affectedRows = connection.Update(model);
                var result = connection.Query<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual("2", result.Document.DocumentElement.GetAttribute("v"), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root v=\"1\"/>");
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<XmlDocumentXmlColumnAttributeModel>(
                    "SELECT [SessionId], [ColumnXml] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                Assert.AreEqual("1", result.Document.DocumentElement.GetAttribute("v"), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<XmlDocumentFluentModel>()
                    .PropertyHandler(e => e.ColumnXml, new XmlToXmlDocumentPropertyHandler());
                var model = new XmlDocumentFluentModel { SessionId = Guid.NewGuid(), ColumnXml = Parse("<root v=\"1\"/>") };

                // Act
                connection.Insert(model);
                var result = connection.Query<XmlDocumentFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("1", result.ColumnXml.DocumentElement.GetAttribute("v"), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<XmlDocument, XmlToXmlDocumentPropertyHandler>(true);
                var model = new XmlDocumentTypeLevelModel { SessionId = Guid.NewGuid(), ColumnXml = Parse("<root v=\"1\"/>") };

                // Act
                connection.Insert(model);
                var result = connection.Query<XmlDocumentTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("1", result.ColumnXml.DocumentElement.GetAttribute("v"), StringComparer.Ordinal);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWithNullDocument()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, "ColumnXml", model.SessionId);
                var result = connection.Query<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnXml]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", string.Empty);

                // Act
                var result = connection.Query<XmlDocumentTextColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerUpdateToNullDocument()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root/>");
                connection.Insert(model);

                // Act
                model.Document = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, "ColumnXml", model.SessionId);
                var result = connection.Query<XmlDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWithEmptyDocument()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // An 'XmlDocument' with no content has an empty outer xml, which is read back as null
                var model = new XmlDocumentTextColumnAttributeModel { SessionId = Guid.NewGuid(), Document = new XmlDocument() };

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, "ColumnNVarChar", model.SessionId);
                var result = connection.Query<XmlDocumentTextColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(string.Empty, raw, StringComparer.Ordinal);
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        [DataRow("<root><child></root>")]
        [DataRow("<root>")]
        [DataRow("not an xml")]
        [DataRow("<root/><second/>")]
        public void TestXmlToXmlDocumentPropertyHandlerWithMalformedXml(string text)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", text);

                // Act / Assert
                Assert.Throws<XmlException>(() =>
                    connection.Query<XmlDocumentTextColumnAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestXmlToXmlDocumentPropertyHandlerWithUtf8DeclarationOnXmlColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // The outer xml has an 'utf-8' declaration, which SQL Server cannot accept for a text (UTF-16) to 'XML' conversion
                var model = CreateModel("<?xml version=\"1.0\" encoding=\"utf-8\"?><root/>");

                // Act / Assert
                Assert.Throws<SqlException>(() => connection.Insert(model));
            }
        }

        #endregion
    }
}
