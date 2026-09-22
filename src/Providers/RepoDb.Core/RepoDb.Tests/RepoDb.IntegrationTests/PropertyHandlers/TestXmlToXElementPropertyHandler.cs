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
    /// The integration tests of <see cref="XmlToXElementPropertyHandler"/>. The <c>XML</c> column (returned by the driver as a <see cref="string"/>)
    /// and the <c>NVARCHAR(MAX)</c> column of the <c>[dbo].[CompleteTable]</c> are used to store the <see cref="XElement"/>.
    /// </summary>
    [TestClass]
    public class XmlToXElementPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(XElement));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class XElementXmlColumnAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnXml"), PropertyHandler(typeof(XmlToXElementPropertyHandler))]
            public XElement Element { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class XElementTextColumnAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(XmlToXElementPropertyHandler))]
            public XElement Element { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class XElementFluentModel
        {
            public Guid SessionId { get; set; }

            public XElement ColumnXml { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="XElement"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class XElementTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public XElement ColumnXml { get; set; }
        }

        #endregion

        #region Helpers

        private static XElementXmlColumnAttributeModel CreateModel(string xml) =>
            new XElementXmlColumnAttributeModel { SessionId = Guid.NewGuid(), Element = xml == null ? null : XElement.Parse(xml) };

        private static XElementTextColumnAttributeModel CreateTextModel(string xml) =>
            new XElementTextColumnAttributeModel { SessionId = Guid.NewGuid(), Element = xml == null ? null : XElement.Parse(xml) };

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
        public void TestXmlToXElementPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root a=\"1\"><child>text</child></root>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XElementXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(result.Element);
                Assert.IsTrue(XNode.DeepEquals(model.Element, result.Element));
                Assert.AreEqual("1", result.Element.Attribute("a").Value, StringComparer.Ordinal);
                Assert.AreEqual("text", result.Element.Element("child").Value, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestXmlToXElementPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root a=\"1\"><child>text</child></root>");

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<XElementXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.IsTrue(XNode.DeepEquals(model.Element, result.Element));
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerWritesCompactXmlText()
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
                Assert.AreEqual("<root><child a=\"1\">text</child></root>", raw, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerWritesEmptyElementsWithSpace()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateTextModel("<root><item id=\"1\"/></root>");

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, "ColumnNVarChar", model.SessionId);

                // Assert
                Assert.AreEqual("<root><item id=\"1\" /></root>", raw, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerReadsXmlText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", "<items><item id=\"1\"/><item id=\"2\"/></items>");

                // Act
                var result = connection.Query<XElementTextColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual("items", result.Element.Name.LocalName, StringComparer.Ordinal);
                Assert.AreEqual(2, result.Element.Elements("item").Count());
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerReadsXmlColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnXml", "<items><item id=\"1\"/><item id=\"2\"/></items>");

                // Act
                var result = connection.Query<XElementXmlColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual("items", result.Element.Name.LocalName, StringComparer.Ordinal);
                Assert.AreEqual(2, result.Element.Elements("item").Count());
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerWritesXmlColumnAsQueryableXml()
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
        public void TestXmlToXElementPropertyHandlerWithNamespacesAndPrefixes()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<a:root xmlns:a=\"urn:a\" xmlns=\"urn:default\"><child a:attr=\"1\">x</child><a:other/></a:root>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XElementXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                XNamespace a = "urn:a";
                XNamespace defaultNamespace = "urn:default";
                Assert.AreEqual(a + "root", result.Element.Name);
                Assert.AreEqual("x", result.Element.Element(defaultNamespace + "child").Value, StringComparer.Ordinal);
                Assert.AreEqual("1", result.Element.Element(defaultNamespace + "child").Attribute(a + "attr").Value, StringComparer.Ordinal);
                Assert.IsNotNull(result.Element.Element(a + "other"));
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerWithEscapedAndUnicodeCharacters()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<t a=\"&quot;q&quot;\">a &amp; b &lt; c 日本語 Ñandú 😀</t>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XElementXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("a & b < c 日本語 Ñandú 😀", result.Element.Value, StringComparer.Ordinal);
                Assert.AreEqual("\"q\"", result.Element.Attribute("a").Value, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerWithCData()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<t><![CDATA[<not>&xml]]></t>");

                // Act
                connection.Insert(model);
                var result = connection.Query<XElementXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("<not>&xml", result.Element.Value, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerWithLargeElement()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var root = new XElement("root", Enumerable.Range(1, 2000).Select(i => new XElement("item", new XAttribute("id", i), $"value-{i}")));
                var model = new XElementXmlColumnAttributeModel { SessionId = Guid.NewGuid(), Element = root };

                // Act
                connection.Insert(model);
                var result = connection.Query<XElementXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(2000, result.Element.Elements("item").Count());
                Assert.IsTrue(XNode.DeepEquals(model.Element, result.Element));
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10).Select(i => CreateModel($"<root id=\"{i}\"/>")).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<XElementXmlColumnAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    Assert.IsTrue(XNode.DeepEquals(model.Element, result.First(e => e.SessionId == model.SessionId).Element));
                }
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root v=\"1\"/>");
                connection.Insert(model);

                // Act
                model.Element = XElement.Parse("<root v=\"2\"/>");
                var affectedRows = connection.Update(model);
                var result = connection.Query<XElementXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual("2", result.Element.Attribute("v").Value, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root v=\"1\"/>");
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<XElementXmlColumnAttributeModel>(
                    "SELECT [SessionId], [ColumnXml] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                Assert.AreEqual("1", result.Element.Attribute("v").Value, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<XElementFluentModel>()
                    .PropertyHandler(e => e.ColumnXml, new XmlToXElementPropertyHandler());
                var model = new XElementFluentModel { SessionId = Guid.NewGuid(), ColumnXml = XElement.Parse("<root v=\"1\"/>") };

                // Act
                connection.Insert(model);
                var result = connection.Query<XElementFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("1", result.ColumnXml.Attribute("v").Value, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<XElement, XmlToXElementPropertyHandler>(true);
                var model = new XElementTypeLevelModel { SessionId = Guid.NewGuid(), ColumnXml = XElement.Parse("<root v=\"1\"/>") };

                // Act
                connection.Insert(model);
                var result = connection.Query<XElementTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("1", result.ColumnXml.Attribute("v").Value, StringComparer.Ordinal);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerWithNullElement()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, "ColumnXml", model.SessionId);
                var result = connection.Query<XElementXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Element);
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnXml]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<XElementXmlColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Element);
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", string.Empty);

                // Act
                var result = connection.Query<XElementTextColumnAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Element);
            }
        }

        [TestMethod]
        public void TestXmlToXElementPropertyHandlerUpdateToNullElement()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("<root/>");
                connection.Insert(model);

                // Act
                model.Element = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, "ColumnXml", model.SessionId);
                var result = connection.Query<XElementXmlColumnAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Element);
            }
        }

        [TestMethod]
        [DataRow("<root><child></root>")]
        [DataRow("<root>")]
        [DataRow("not an xml")]
        [DataRow("<root/><second/>")]
        public void TestXmlToXElementPropertyHandlerWithMalformedXml(string text)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", text);

                // Act / Assert
                Assert.Throws<XmlException>(() =>
                    connection.Query<XElementTextColumnAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        #endregion
    }
}
