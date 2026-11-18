using System.Data;
using System.Text;
using System.Text.Json;
using RepoDb.Science.Helpers;
using RepoDb.Science.UnitTests.Helpers;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Exercises the watermarked JSON and XML loaders.
/// </summary>
internal static class ProjectionLoadingTests
{
    #region Public Methods

    /// <summary>
    /// Runs the orientation tests.
    /// </summary>
    public static void Run()
    {
        JsonRoundTripsAllLayouts();
        XmlRoundTripsAllLayouts();
        ValuesUseCallerColumnOrder();
        TableUsesDeclaredAndCallerTypes();
        JsonFindsWatermarkAfterData();
        JsonSortsColumnIndexesAndRetainsMissingCells();
        XmlSortsIndexRows();
        JsonReplaysForwardOnlyInput();
        XmlReplaysFromCurrentPosition();
        XmlFallsBackForOtherProducer();
        JsonFallsBackForOtherProducer();
        JsonFallsBackWithoutProducer();
        JsonRejectsUnsupportedReaderOrientation();
        TableResolvesIntegerExtensionTypes();
        EmptyProjectedTablesRetainDeclaredColumns();
        JsonRequiresWatermarkMetadataAndPayload();
        XmlRequiresOrientation();
        JsonRejectsMalformedProjections();
        XmlRejectsMalformedProjections();
        XmlRestoresSchemaScalarTypes();
        JsonRestoresTypedScalars();
        JsonConvertsNumericBooleanThroughCallerSchema();
        JsonWidensMixedFractionsToText();
        CsvInfersSignedValuesOnForwardOnlyStreams();
        XmlSchemaConversionsPreserveLexicalText();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads every JSON orientation without an explicit selector.
    /// </summary>
    private static void JsonRoundTripsAllLayouts()
    {
        foreach (var orientation in Enum.GetValues<DataOrientation>())
        {
            using var original = Sample();
            using var stream = new MemoryStream();
            original.SaveJson(stream,
                orientation);
            stream.Position = 0;
            using var read = new DataTable()
                .LoadJson(stream);
            CheckRows(read,
                orientation == DataOrientation.Values ? "Column2" : "Name");
        }
    }

    /// <summary>
    /// Loads every XML orientation with custom XML element names.
    /// </summary>
    private static void XmlRoundTripsAllLayouts()
    {
        foreach (var orientation in Enum.GetValues<DataOrientation>())
        {
            using var original = Sample();
            using var stream = new MemoryStream();
            original.SaveXml(stream,
                rootElement: "customers",
                rowElement: "customer",
                orientation: orientation,
                indented: true);
            stream.Position = 0;
            using var read = new DataTable()
                .LoadXml(stream,
                    inferTypes: true);
            CheckRows(read,
                orientation == DataOrientation.Values ? "Column2" : "Name");
        }
    }

    /// <summary>
    /// Maps positional values to the caller's columns in both formats.
    /// </summary>
    private static void ValuesUseCallerColumnOrder()
    {
        using var original = Sample();
        using var schema = new DataTable();
        schema.Columns.Add("Identifier",
            typeof(int));
        schema.Columns.Add("Description");
        using var json = new MemoryStream();
        using var xml = new MemoryStream();
        original.SaveJson(json,
            DataOrientation.Values);
        original.SaveXml(xml,
            orientation: DataOrientation.Values);
        json.Position = 0;
        xml.Position = 0;
        using var fromJson = schema.LoadJson(json);
        using var fromXml = schema.LoadXml(xml);
        AssertHelper.Check((int)fromXml.Rows[0]["Identifier"] == 101);
        CheckRows(fromJson,
            "Description");
        CheckRows(fromXml,
            "Description");
    }

    /// <summary>
    /// Restores types from Table and honors explicit column types.
    /// </summary>
    private static void TableUsesDeclaredAndCallerTypes()
    {
        using var original = Sample();
        using var stream = new MemoryStream();
        original.SaveJson(stream,
            DataOrientation.Table);
        stream.Position = 0;
        using var inferred = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check(inferred.Columns["Id"]!.DataType == typeof(int));
        using var schema = new DataTable();
        schema.Columns.Add("Id",
            typeof(string));
        stream.Position = 0;
        using var supplied = schema.LoadJson(stream);
        AssertHelper.Check(supplied.Columns.Count == 1);
        AssertHelper.Check(string.Equals((string)supplied.Rows[0]["Id"],
            "101",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Finds root metadata regardless of JSON property order.
    /// </summary>
    private static void JsonFindsWatermarkAfterData()
    {
        using var stream = Input("""{"data":[{"A":4}],"orientation":"Records","extra":{"nested":[1]},"producer":"RepoDB"}""");
        using var table = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check((long)table.Rows[0]["A"] == 4L);
    }

    /// <summary>
    /// Restores a sparse Columns layout in ascending index order.
    /// </summary>
    private static void JsonSortsColumnIndexesAndRetainsMissingCells()
    {
        using var stream = Input(Json("Columns",
            """{"A":{"8":1,"2":6},"B":{"8":true}}"""));
        using var table = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check((long)table.Rows[0]["A"] == 6L);
        AssertHelper.Check(table.Rows[0].IsNull("B"));
    }

    /// <summary>
    /// Reorders XML Index rows by their saved position.
    /// </summary>
    private static void XmlSortsIndexRows()
    {
        using var stream = Input(Xml("Index",
            """<r index="8"><A>1</A></r><r index="2"><A>6</A></r>"""));
        using var table = new DataTable()
            .LoadXml(stream);
        AssertHelper.Check(string.Equals((string)table.Rows[0]["A"],
            "6",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Loads a marked JSON document from a forward-only stream.
    /// </summary>
    private static void JsonReplaysForwardOnlyInput()
    {
        using var stream = new ForwardOnlyLoadStream(Encoding.UTF8.GetBytes(Json("Records",
            """[{"A":4}]""")));
        using var table = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check((long)table.Rows[0]["A"] == 4L);
        AssertHelper.Check(stream.CanRead);
    }

    /// <summary>
    /// Loads marked XML from the stream's current position.
    /// </summary>
    private static void XmlReplaysFromCurrentPosition()
    {
        using var stream = Input("prefix" + Xml("Records",
            "<item><A>4</A></item>"));
        stream.Position = 6;
        using var table = new DataTable()
            .LoadXml(stream);
        AssertHelper.Check(table.Rows.Count == 1);
        AssertHelper.Check(stream.CanRead);
    }

    /// <summary>
    /// Keeps the flat XML loader for other producers.
    /// </summary>
    private static void XmlFallsBackForOtherProducer()
    {
        using var stream = Input("""<rows producer="Other" orientation="Values"><row><A>4</A></row></rows>""");
        using var table = new DataTable()
            .LoadXml(stream);
        AssertHelper.Check(table.Columns.Contains("A"));
    }

    /// <summary>
    /// Keeps the flat JSON loader for other producers.
    /// </summary>
    private static void JsonFallsBackForOtherProducer()
    {
        using var stream = Input("""{"producer":"Other","orientation":"Values","data":[{"A":4}]}""");
        AssertHelper.Throws<JsonException>(() => new DataTable()
            .LoadJson(stream));
    }

    /// <summary>
    /// Keeps the flat loader for absent or non-string producer metadata.
    /// </summary>
    private static void JsonFallsBackWithoutProducer()
    {
        foreach (var input in new[] { """{"data":[]}""", """{"producer":null,"data":[]}""", """{"producer":1,"data":[]}""" })
        {
            using var stream = Input(input);
            AssertHelper.Throws<JsonException>(() => new DataTable()
                .LoadJson(stream));
        }
    }

    /// <summary>
    /// Rejects invalid orientations passed directly to the projection reader.
    /// </summary>
    private static void JsonRejectsUnsupportedReaderOrientation()
    {
        using var stream = Input(Json("Records",
            "[]"));
        var target = new ProjectionLoadTarget(null,
            false,
            null);
        AssertHelper.Throws<ArgumentOutOfRangeException>(() => JsonProjectionLoadHelper.Read(stream,
            (DataOrientation)100,
            target));
    }

    /// <summary>
    /// Restores the declared integer widths of JSON Table fields.
    /// </summary>
    private static void TableResolvesIntegerExtensionTypes()
    {
        foreach (var (dtype, expected) in new (string, Type)[]
        {
            ("Int8", typeof(sbyte)),
            ("UInt8", typeof(byte)),
            ("Int16", typeof(short)),
            ("UInt16", typeof(ushort)),
            ("Int32", typeof(int)),
            ("UInt32", typeof(uint)),
            ("Int64", typeof(long)),
            ("UInt64", typeof(ulong))
        })
        {
            AssertHelper.Check(ProjectionLoadHelper.SchemaType("integer",
                dtype) == expected);
        }
    }

    /// <summary>
    /// Preserves declared columns when a projected table has no rows.
    /// </summary>
    private static void EmptyProjectedTablesRetainDeclaredColumns()
    {
        using var json = Input(Json("Split",
            """{"columns":["A"],"index":[],"data":[]}"""));
        using var xml = Input(Xml("Table",
            """<schema><field name="index" type="integer"/><field name="A" type="boolean"/><primaryKey>index</primaryKey></schema><data/>"""));
        using var fromJson = new DataTable()
            .LoadJson(json);
        using var fromXml = new DataTable()
            .LoadXml(xml);
        AssertHelper.Check(fromJson.Columns.Contains("A") && fromJson.Rows.Count == 0);
        AssertHelper.Check(fromXml.Columns["A"]!.DataType == typeof(bool) && fromXml.Rows.Count == 0);
    }

    /// <summary>
    /// Rejects missing or repeated metadata sections in marked JSON.
    /// </summary>
    private static void JsonRequiresWatermarkMetadataAndPayload()
    {
        foreach (var input in new[]
        {
            """{"producer":"RepoDB","data":[]}""",
            """{"producer":"RepoDB","orientation":"Records"}""",
            """{"producer":"RepoDB","orientation":"Records","data":[],"data":[]}"""
        })
        {
            using var stream = Input(input);
            AssertHelper.Throws<FormatException>(() => new DataTable()
                .LoadJson(stream));
        }
    }

    /// <summary>
    /// Rejects marked XML with no orientation.
    /// </summary>
    private static void XmlRequiresOrientation()
    {
        using var stream = Input("""<r producer="RepoDB"/>""");
        AssertHelper.Throws<FormatException>(() => new DataTable()
            .LoadXml(stream));
    }

    /// <summary>
    /// Rejects incorrect JSON layout structures.
    /// </summary>
    private static void JsonRejectsMalformedProjections()
    {
        foreach (var (orientation, payload) in new (string, string)[]
        {
            ("Unknown", """[]"""),
            ("Records", """{}"""),
            ("Records", """[{"A":1,"A":2}]"""),
            ("Records", """[{"A":[]}]"""),
            ("Index", """{"-1":{"A":1}}"""),
            ("Index", """{"0":{"A":1},"00":{"A":2}}"""),
            ("Columns", """{"A":{"0":1,"00":2}}"""),
            ("Columns", """{"A":{},"A":{}}"""),
            ("Columns", """{" ":{"0":1}}"""),
            ("Split", """{"wrong":[]}"""),
            ("Split", """{"columns":[1],"index":[],"data":[]}"""),
            ("Split", """{"columns":["A","A"],"index":[],"data":[]}"""),
            ("Split", """{"columns":["A"],"index":[-1],"data":[[1]]}"""),
            ("Split", """{"columns":["A"],"index":[2147483648],"data":[[1]]}"""),
            ("Split", """{"columns":["A"],"index":[],"data":[[1]]}"""),
            ("Split", """{"columns":["A"],"index":[0],"data":[]}"""),
            ("Split", """{"columns":["A"],"index":[0],"data":[[]]}"""),
            ("Values", """[[1],[1,2]]"""),
            ("Table", """{"schema":{"fields":[{"name":"A","type":"unknown"}],"primaryKey":["A"]},"data":[]}"""),
            ("Table", """{"schema":{"fields":[{"name":"i","type":"integer"},{"name":"i","type":"integer"}],"primaryKey":["i"]},"data":[]}"""),
            ("Table", """{"schema":{"fields":[{"name":"i","type":"integer"}],"primaryKey":["other"]},"data":[]}"""),
            ("Table", """{"schema":{"fields":[{"name":"i","type":"integer"}],"primaryKey":[]},"data":[]}"""),
            ("Table", """{"schema":{"fields":[{"name":"i","type":"integer"}],"primaryKey":["i"]},"data":[{}]}"""),
        })
        {
            using var stream = Input(Json(orientation,
                payload));
            AssertHelper.Throws<FormatException>(() => new DataTable()
                .LoadJson(stream));
        }
    }

    /// <summary>
    /// Rejects incorrect XML layout structures.
    /// </summary>
    private static void XmlRejectsMalformedProjections()
    {
        foreach (var (orientation, payload) in new (string, string)[]
        {
            ("Unknown", string.Empty),
            ("Records", """<r><A>1</A><A>2</A></r>"""),
            ("Index", """<r><A>1</A></r>"""),
            ("Index", """<r index="0"/><r index="0"/>"""),
            ("Columns", """<column><value index="0">1</value></column>"""),
            ("Columns", """<column name="A"/><column name="A"/>"""),
            ("Columns", """<column name="A"><value index="0">1</value><value index="00">2</value></column>"""),
            ("Split", """<wrong/>"""),
            ("Split", string.Empty),
            ("Split", """<columns><column>A</column><column>A</column></columns><index/><data/>"""),
            ("Split", """<columns><column>A</column></columns><index/><data><r><value>1</value></r></data>"""),
            ("Split", """<columns><column>A</column></columns><index><value>0</value></index><data/>"""),
            ("Split", """<columns/><index/><data/><extra/>"""),
            ("Values", """<r><value>1</value></r><r/>"""),
            ("Table", """<schema><field type="integer"/></schema><data/>"""),
            ("Table", """<schema><field name="i" type="integer"/><field name="i" type="integer"/><primaryKey>i</primaryKey></schema><data/>"""),
            ("Table", """<schema><field name="i" type="integer"/></schema><data/>"""),
            ("Table", """<schema><field name="i" type="integer"/><primaryKey>wrong</primaryKey></schema><data/>"""),
        })
        {
            using var stream = Input(Xml(orientation,
                payload));
            AssertHelper.Throws<FormatException>(() => new DataTable()
                .LoadXml(stream));
        }
    }

    /// <summary>
    /// Restores types declared in an XML Table schema.
    /// </summary>
    private static void XmlRestoresSchemaScalarTypes()
    {
        using var table = new DataTable();
        table.Columns.Add("Amount",
            typeof(decimal));
        table.Columns.Add("Start",
            typeof(DateTime));
        table.Columns.Add("Duration",
            typeof(TimeSpan));
        table.Rows.Add(12.5m,
            new DateTime(2026,
                1,
                1),
            TimeSpan.FromMinutes(-3));
        using var stream = new MemoryStream();
        table.SaveXml(stream,
            orientation: DataOrientation.Table);
        stream.Position = 0;
        using var read = new DataTable()
            .LoadXml(stream);
        AssertHelper.Check((decimal)read.Rows[0]["Amount"] == 12.5m);
        AssertHelper.Check((DateTime)read.Rows[0]["Start"] == (DateTime)table.Rows[0]["Start"]);
        AssertHelper.Check((TimeSpan)read.Rows[0]["Duration"] == TimeSpan.FromMinutes(-3));
    }

    /// <summary>
    /// Reads scalar types from a supplied schema and accepts both duration formats.
    /// </summary>
    private static void JsonRestoresTypedScalars()
    {
        var key = Guid.Empty.ToString();
        using var stream = Input(Json("Records",
            $$"""
            [
              {"Key":"{{key}}","At":"2026-01-01T08:00:00+01:00","Duration":"PT3M","Amount":1.5},
              {"Duration":"00:04:00","Amount":2.0}
            ]
            """));
        using var schema = new DataTable();
        schema.Columns.Add("Key",
            typeof(Guid));
        schema.Columns.Add("At",
            typeof(DateTimeOffset));
        schema.Columns.Add("Duration",
            typeof(TimeSpan));
        schema.Columns.Add("Amount",
            typeof(double));
        using var table = schema.LoadJson(stream);
        AssertHelper.Check((Guid)table.Rows[0]["Key"] == Guid.Empty);
        AssertHelper.Check((DateTimeOffset)table.Rows[0]["At"] ==
            DateTimeOffset.Parse("2026-01-01T08:00:00+01:00",
                System.Globalization.CultureInfo.InvariantCulture));
        AssertHelper.Check((double)table.Rows[0]["Amount"] == 1.5);
        CheckDurations(schema,
            table);
    }

    /// <summary>
    /// Checks duration forms and rejects empty duration text.
    /// </summary>
    /// <param name="schema">The caller's schema.</param>
    /// <param name="table">The loaded durations.</param>
    private static void CheckDurations(DataTable schema,
        DataTable table)
    {
        AssertHelper.Check((TimeSpan)table.Rows[0]["Duration"] == TimeSpan.FromMinutes(3));
        AssertHelper.Check((TimeSpan)table.Rows[1]["Duration"] == TimeSpan.FromMinutes(4));
        using var stream = Input(Json("Records",
            """[{"Duration":""}]"""));
        AssertHelper.Throws<FormatException>(() => schema.LoadJson(stream));
    }

    /// <summary>
    /// Converts a nonzero number to a boolean when requested by the schema.
    /// </summary>
    private static void JsonConvertsNumericBooleanThroughCallerSchema()
    {
        using var stream = Input(Json("Records",
            """[{"Flag":1.5}]"""));
        using var schema = new DataTable();
        schema.Columns.Add("Flag",
            typeof(bool));
        using var result = schema.LoadJson(stream);
        AssertHelper.Check((bool)result.Rows[0]["Flag"]);
    }

    /// <summary>
    /// Falls back to text when a fractional column later contains a string.
    /// </summary>
    private static void JsonWidensMixedFractionsToText()
    {
        using var stream = Input(Json("Records",
            """[{"A":1.5},{"A":"literal"}]"""));
        using var table = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check(table.Columns["A"]!.DataType == typeof(string));
    }

    /// <summary>
    /// Replays CSV without seeking and distinguishes signed numbers from text.
    /// </summary>
    private static void CsvInfersSignedValuesOnForwardOnlyStreams()
    {
        using var stream = new ForwardOnlyLoadStream(Encoding.UTF8.GetBytes("A,B,C\n+2,0,\"\"\n-3,-004,a"));
        using var table = new DataTable()
            .LoadCsv(stream,
                inferTypes: true);
        AssertHelper.Check((long)table.Rows[1]["A"] == -3L);
        AssertHelper.Check(table.Columns["B"]!.DataType == typeof(string));
    }

    /// <summary>
    /// Uses XML lexical text when the caller overrides a schema type.
    /// </summary>
    private static void XmlSchemaConversionsPreserveLexicalText()
    {
        using var xsd = Input("""
            <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
              <xs:element name="row"><xs:complexType><xs:sequence>
                <xs:element name="A" type="xs:int"/>
              </xs:sequence></xs:complexType></xs:element>
            </xs:schema>
            """);
        using var reader = System.Xml.XmlReader.Create(xsd);
        var schemas = new System.Xml.Schema.XmlSchemaSet();
        schemas.Add(null,
            reader);
        using var input = Input("<row><A>004</A></row>");
        using var schema = new DataTable();
        schema.Columns.Add("A",
            typeof(string));
        using var table = schema.LoadXml(input,
            schemas: schemas);
        AssertHelper.Check(string.Equals((string)table.Rows[0]["A"],
            "004",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Checks the sample records and their null values.
    /// </summary>
    /// <param name="table">The loaded table.</param>
    /// <param name="name">The name column.</param>
    private static void CheckRows(DataTable table,
        string name)
    {
        AssertHelper.Check(table.Rows.Count == 3);
        AssertHelper.Check(string.Equals((string)table.Rows[0][name],
            "Ada",
            StringComparison.Ordinal));
        AssertHelper.Check(table.Rows[1].IsNull(name));
    }

    /// <summary>
    /// Creates a typed three-row sample.
    /// </summary>
    private static DataTable Sample()
    {
        var table = new DataTable();
        table.Columns.Add("Id",
            typeof(int));
        table.Columns.Add("Name");
        table.Rows.Add(101,
            "Ada");
        table.Rows.Add(102,
            DBNull.Value);
        table.Rows.Add(103,
            "Linus");
        return table;
    }

    /// <summary>
    /// Creates a watermarked JSON document.
    /// </summary>
    /// <param name="orientation">The orientation name.</param>
    /// <param name="data">The JSON payload.</param>
    private static string Json(string orientation,
        string data) =>
        $$"""{"producer":"RepoDB","orientation":"{{orientation}}","data":{{data}}}""";

    /// <summary>
    /// Creates a watermarked XML document.
    /// </summary>
    /// <param name="orientation">The orientation name.</param>
    /// <param name="data">The XML payload.</param>
    private static string Xml(string orientation,
        string data) =>
        $"""<root producer="RepoDB" orientation="{orientation}">{data}</root>""";

    /// <summary>
    /// Creates a UTF-8 stream.
    /// </summary>
    /// <param name="text">The document text.</param>
    private static MemoryStream Input(string text) => new(Encoding.UTF8.GetBytes(text));

    #endregion
}
