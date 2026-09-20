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
using System.Xml.Linq;

namespace RepoDb.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// The integration tests of <see cref="XmlToXDocumentPropertyHandler"/>. The <c>XML</c> column (returned by the driver as a <see cref="string"/>)
    /// and the <c>NVARCHAR(MAX)</c> column of the <c>[dbo].[CompleteTable]</c> are used to store the <see cref="XDocument"/>.
    /// </summary>
    [TestClass]
    public class XmlToXDocumentPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(XDocument));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class XDocumentXmlColumnAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnXml"), PropertyHandler(typeof(XmlToXDocumentPropertyHandler))]
            public XDocument Document { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class XDocumentTextColumnAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(XmlToXDocumentPropertyHandler))]
            public XDocument Document { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class XDocumentFluentModel
        {
            public Guid SessionId { get; set; }

            public XDocument ColumnXml { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="XDocument"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class XDocumentTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public XDocument ColumnXml { get; set; }
        }

        #endregion

        #region Helpers

        private static XDocumentXmlColumnAttributeModel CreateModel(string xml) =>
            new XDocumentXmlColumnAttributeModel { SessionId = Guid.NewGuid(), Document = xml == null ? null : XDocument.Parse(xml) };

        private static XDocumentTextColumnAttributeModel CreateTextModel(string xml) =>
            new XDocumentTextColumnAttributeModel { SessionId = Guid.NewGuid(), Document = xml == null ? null : XDocument.Parse(xml) };

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
        public void TestXmlToXDocumentPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root a=\"1\"><child>text</child></root>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(result.Document);
                Assert.IsTrue(XNode.DeepEquals(model.Document, result.Document));
                Assert.AreEqual("1", result.Document.Root.Attribute("a").Value);
                Assert.AreEqual("text", result.Document.Root.Element("child").Value);
            }
        }

        [TestMethod]
        public async Task TestXmlToXDocumentPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root a=\"1\"><child>text</child></root>");

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<XDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.IsTrue(XNode.DeepEquals(model.Document, result.Document));
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerWritesCompactXmlText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateTextModel("<root>\n  <child a=\"1\">text</child>\n</root>");

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, "ColumnNVarChar", model.SessionId);

                // Assert
                // The default parsing drops the insignificant whitespaces, and the writing is not indented
                Assert.AreEqual("<root><child a=\"1\">text</child></root>", raw);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerReadsXmlText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", "<items><item id=\"1\"/><item id=\"2\"/></items>");

                // Act
                var result = connection.Query<XDocumentTextColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual("items", result.Document.Root.Name.LocalName);
                Assert.AreEqual(2, result.Document.Root.Elements("item").Count());
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerReadsXmlColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnXml", "<items><item id=\"1\"/><item id=\"2\"/></items>");

                // Act
                var result = connection.Query<XDocumentXmlColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual("items", result.Document.Root.Name.LocalName);
                Assert.AreEqual(2, result.Document.Root.Elements("item").Count());
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerWritesXmlColumnAsQueryableXml()
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
                Assert.AreEqual("value-1", value);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerWithNamespacesAndPrefixes()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<a:root xmlns:a=\"urn:a\" xmlns=\"urn:default\"><child a:attr=\"1\">x</child><a:other/></a:root>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                XNamespace a = "urn:a";
                XNamespace defaultNamespace = "urn:default";
                Assert.AreEqual(a + "root", result.Document.Root.Name);
                Assert.AreEqual("x", result.Document.Root.Element(defaultNamespace + "child").Value);
                Assert.AreEqual("1", result.Document.Root.Element(defaultNamespace + "child").Attribute(a + "attr").Value);
                Assert.IsNotNull(result.Document.Root.Element(a + "other"));
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerWithEscapedAndUnicodeCharacters()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<t a=\"&quot;q&quot;\">a &amp; b &lt; c 日本語 Ñandú 😀</t>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("a & b < c 日本語 Ñandú 😀", result.Document.Root.Value);
                Assert.AreEqual("\"q\"", result.Document.Root.Attribute("a").Value);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerWithCData()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<t><![CDATA[<not>&xml]]></t>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("<not>&xml", result.Document.Root.Value);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerWithXmlDeclarationDoesNotWriteTheDeclaration()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateTextModel("<?xml version=\"1.0\" encoding=\"utf-8\"?><root/>");

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, "ColumnNVarChar", model.SessionId);
                var result = connection.Query<XDocumentTextColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("<root />", raw);
                Assert.IsNull(result.Document.Declaration);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerReadsXmlDeclaration()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", "<?xml version=\"1.0\" encoding=\"utf-8\"?><root/>");

                // Act
                var result = connection.Query<XDocumentTextColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNotNull(result.Document.Declaration);
                Assert.AreEqual("1.0", result.Document.Declaration.Version);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerWithLargeDocument()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var root = new XElement("root", Enumerable.Range(1, 2000).Select(i => new XElement("item", new XAttribute("id", i), $"value-{i}")));
                var model = new XDocumentXmlColumnAttributeModel { SessionId = Guid.NewGuid(), Document = new XDocument(root) };

                // Act
                connection.Insert(model);
                var result = connection.Query<XDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(2000, result.Document.Root.Elements("item").Count());
                Assert.IsTrue(XNode.DeepEquals(model.Document, result.Document));
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10).Select(i => CreateModel($"<root id=\"{i}\"/>")).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<XDocumentXmlColumnAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    Assert.IsTrue(XNode.DeepEquals(model.Document, result.First(e => e.SessionId == model.SessionId).Document));
                }
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root v=\"1\"/>");
                connection.Insert(model);

                // Act
                model.Document = XDocument.Parse("<root v=\"2\"/>");
                var affectedRows = connection.Update(model);
                var result = connection.Query<XDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual("2", result.Document.Root.Attribute("v").Value);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root v=\"1\"/>");
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<XDocumentXmlColumnAttributeModel>(
                    "SELECT [SessionId], [ColumnXml] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                Assert.AreEqual("1", result.Document.Root.Attribute("v").Value);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<XDocumentFluentModel>()
                    .PropertyHandler(e => e.ColumnXml, new XmlToXDocumentPropertyHandler());
                var model = new XDocumentFluentModel { SessionId = Guid.NewGuid(), ColumnXml = XDocument.Parse("<root v=\"1\"/>") };

                // Act
                connection.Insert(model);
                var result = connection.Query<XDocumentFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("1", result.ColumnXml.Root.Attribute("v").Value);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<XDocument, XmlToXDocumentPropertyHandler>(true);
                var model = new XDocumentTypeLevelModel { SessionId = Guid.NewGuid(), ColumnXml = XDocument.Parse("<root v=\"1\"/>") };

                // Act
                connection.Insert(model);
                var result = connection.Query<XDocumentTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("1", result.ColumnXml.Root.Attribute("v").Value);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerWithNullDocument()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, "ColumnXml", model.SessionId);
                var result = connection.Query<XDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnXml]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<XDocumentXmlColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", string.Empty);

                // Act
                var result = connection.Query<XDocumentTextColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestXmlToXDocumentPropertyHandlerUpdateToNullDocument()
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
                var result = connection.Query<XDocumentXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        [DataRow("<root><child></root>")]
        [DataRow("<root>")]
        [DataRow("not an xml")]
        [DataRow("<root/><second/>")]
        public void TestXmlToXDocumentPropertyHandlerWithMalformedXml(string text)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", text);

                // Act / Assert
                Assert.Throws<XmlException>(() =>
                    connection.Query<XDocumentTextColumnAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        #endregion
    }
}
