using System.Data;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using RepoDb.Science.UnitTests.Helpers;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Exercises the DataTable loaders.
/// </summary>
internal static class LoadingTests
{
    #region Public Methods

    /// <summary>
    /// Runs the loading tests.
    /// </summary>
    public static void Run()
    {
        RunCsvTests();
        RunJsonTests();
        RunXmlTests();
        RunSchemaTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs CSV loading scenarios.
    /// </summary>
    private static void RunCsvTests()
    {
        CsvReadsQuotedMultilineFields();
        CsvInfersOnlyUnambiguousTypes();
        CsvPreservesDecimalsThatWouldRound();
        CsvRetainsHeaderWithoutData();
        CsvRejectsInvalidRecords();
        CsvRejectsInvalidHeaders();
        CsvRejectsWrongFieldCount();
        CsvLoadsFromPath();
    }

    /// <summary>
    /// Runs JSON loading scenarios.
    /// </summary>
    private static void RunJsonTests()
    {
        JsonDiscoversLateColumnsAndMissingValues();
        JsonKeepsQuotedAndMixedValuesAsText();
        JsonMapsNestedScalarsAcrossRepeatedCollections();
        JsonAllowsEmptyParentCollections();
        JsonRejectsMappedRepeatingValues();
        JsonPreservesNumbersThatWouldRound();
        JsonSkipsUnselectedStructures();
        JsonRefillsAcrossLargeUtf8TokensAndAcceptsBom();
        JsonInfersOnForwardOnlyInput();
        JsonRestartsInferenceAtCurrentPosition();
        JsonRejectsTruncatedIgnoredProperties();
        JsonRejectsMissingCollection();
        JsonRejectsFractionalValuesInIntegerColumns();
        JsonLoadsFromPath();
    }

    /// <summary>
    /// Runs XML loading scenarios.
    /// </summary>
    private static void RunXmlTests()
    {
        XmlLoadsFlatNamespacedRecords();
        XmlInfersTypesAndReadsNil();
        XmlMapsNestedScalarsAcrossRepeatedCollections();
        XmlSkipsUnprojectedNestedElements();
        XmlRejectsRepeatingScalarMappings();
        XmlRejectsDtds();
        XmlUsesDeclaredXsdTypes();
        XmlRejectsValuesOutsideXsd();
        XmlLoadsFromPath();
    }

    /// <summary>
    /// Runs schema loading scenarios.
    /// </summary>
    private static void RunSchemaTests()
    {
        SchemaTakesPrecedenceAndPreservesReceiver();
        SchemaReportsFieldConversionErrors();
        SchemaEnforcesConstraints();
    }

    /// <summary>
    /// Verifies that CSV reads quoted multiline fields.
    /// </summary>
    private static void CsvReadsQuotedMultilineFields()
    {
        using var stream = Input("Id;Note\r\n00123;\"first; line\r\nsecond \"\"quoted\"\"\"\r\n");
        var table = new DataTable()
            .LoadCsv(stream,
                separator: ';');
        AssertHelper.Check(string.Equals((string)table.Rows[0]["Id"],
            "00123",
            StringComparison.Ordinal));
        AssertHelper.Check(string.Equals((string)table.Rows[0]["Note"],
            "first; line\r\nsecond \"quoted\"",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies that CSV infers only unambiguous types.
    /// </summary>
    private static void CsvInfersOnlyUnambiguousTypes()
    {
        using var stream = Input("Id,Amount,Active\n00123,1,true\n00456,2.5,false");
        var table = new DataTable()
            .LoadCsv(stream,
                inferTypes: true);
        AssertHelper.Check(table.Columns["Id"]!.DataType == typeof(string));
        AssertHelper.Check((decimal)table.Rows[1]["Amount"] == 2.5m);
        AssertHelper.Check(table.Columns["Active"]!.DataType == typeof(bool));
    }

    /// <summary>
    /// Verifies that CSV preserves decimals outside exact decimal precision.
    /// </summary>
    private static void CsvPreservesDecimalsThatWouldRound()
    {
        const string value = "0.123456789012345678901234567891";
        using var stream = Input("Amount\n" + value);
        var table = new DataTable()
            .LoadCsv(stream,
                inferTypes: true);
        AssertHelper.Check(string.Equals((string)table.Rows[0]["Amount"],
            value,
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies that CSV retains header without data.
    /// </summary>
    private static void CsvRetainsHeaderWithoutData()
    {
        using var stream = Input("Id,Name");
        var table = new DataTable()
            .LoadCsv(stream,
                inferTypes: true);
        AssertHelper.Check(table.Columns.Count == 2);
        AssertHelper.Check(table.Rows.Count == 0);
    }

    /// <summary>
    /// Verifies that CSV rejects invalid records.
    /// </summary>
    private static void CsvRejectsInvalidRecords()
    {
        using var stream = Input("Id,Name\n1,\"unclosed");
        AssertHelper.Throws<FormatException>(() => new DataTable()
            .LoadCsv(stream));
    }

    /// <summary>
    /// Verifies that CSV rejects invalid headers.
    /// </summary>
    private static void CsvRejectsInvalidHeaders()
    {
        using var stream = Input("Id,id\n1,2");
        AssertHelper.Throws<FormatException>(() => new DataTable()
            .LoadCsv(stream));
    }

    /// <summary>
    /// Verifies that CSV rejects wrong field count.
    /// </summary>
    private static void CsvRejectsWrongFieldCount()
    {
        using var stream = Input("Id,Name\n1");
        AssertHelper.Throws<FormatException>(() => new DataTable()
            .LoadCsv(stream));
    }

    /// <summary>
    /// Verifies that CSV loads from path.
    /// </summary>
    private static void CsvLoadsFromPath()
    {
        AssertHelper.Check(LoadFile("Id\n1",
            path => new DataTable()
                .LoadCsv(path)).Rows.Count == 1);
    }

    /// <summary>
    /// Verifies that the schema takes precedence and preserves receiver.
    /// </summary>
    private static void SchemaTakesPrecedenceAndPreservesReceiver()
    {
        using var stream = Input("Id\n42");
        using var schema = IdSchema();
        schema.Rows.Add(7);
        using var result = schema.LoadCsv(stream,
            inferTypes: true);
        AssertHelper.Check((int)result.Rows[0]["Id"] == 42);
        AssertHelper.Check((int)schema.Rows[0]["Id"] == 7);
        AssertHelper.Check(stream.CanRead);
    }

    /// <summary>
    /// Verifies that the schema reports field conversion errors.
    /// </summary>
    private static void SchemaReportsFieldConversionErrors()
    {
        using var stream = Input("Id\n1\ninvalid");
        using var schema = IdSchema();
        var error = AssertHelper.Throws<FormatException>(() => schema.LoadCsv(stream));
        AssertHelper.Check(error.Message
            .Contains("row 2, column 'Id'",
                StringComparison.Ordinal));
        AssertHelper.Check(schema.Rows.Count == 0);
    }

    /// <summary>
    /// Verifies that the schema enforces constraints.
    /// </summary>
    private static void SchemaEnforcesConstraints()
    {
        using var stream = Input("Id\n1\n1");
        using var schema = IdSchema();
        schema.PrimaryKey = [schema.Columns["Id"]!];
        AssertHelper.Throws<ConstraintException>(() => schema.LoadCsv(stream));
    }

    /// <summary>
    /// Verifies that JSON discovers late columns and missing values.
    /// </summary>
    private static void JsonDiscoversLateColumnsAndMissingValues()
    {
        using var stream = Input("""[{"A":1},{"A":2.5,"B":true},{"B":null}]""");
        var table = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check((decimal)table.Rows[0]["A"] == 1);
        AssertHelper.Check((bool)table.Rows[1]["B"]);
        AssertHelper.Check(table.Rows[0].IsNull("B") && table.Rows[2].IsNull("B"));
    }

    /// <summary>
    /// Verifies that JSON keeps quoted and mixed values as text.
    /// </summary>
    private static void JsonKeepsQuotedAndMixedValuesAsText()
    {
        using var stream = Input("""[{"Id":"00123","Mixed":1},{"Id":"00456","Mixed":"late"}]""");
        var table = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check(string.Equals((string)table.Rows[0]["Id"],
            "00123",
            StringComparison.Ordinal));
        AssertHelper.Check(string.Equals((string)table.Rows[0]["Mixed"],
            "1",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies that JSON maps nested scalars across repeated collections.
    /// </summary>
    private static void JsonMapsNestedScalarsAcrossRepeatedCollections()
    {
        using var stream = Input("""
            {"customers":[{"orders":[{"address":{"city":"Paris"}}]},{"orders":[{"address":{"city":"Oslo"}}]}]}
            """);
        var table = new DataTable()
            .LoadJson(stream,
                collectionPath: "customers.orders",
                columns: new Dictionary<string, string>(StringComparer.Ordinal) { ["City"] = "address.city" });
        AssertHelper.Check(table.Rows.Count == 2);
        AssertHelper.Check(string.Equals((string)table.Rows[1]["City"],
            "Oslo",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies that an empty intermediate array produces an empty table.
    /// </summary>
    private static void JsonAllowsEmptyParentCollections()
    {
        using var stream = Input("""{"customers":[]}""");
        var table = new DataTable()
            .LoadJson(stream,
                collectionPath: "customers.orders");
        AssertHelper.Check(table.Rows.Count == 0);
    }

    /// <summary>
    /// Verifies that a scalar mapping cannot select an array.
    /// </summary>
    private static void JsonRejectsMappedRepeatingValues()
    {
        using var stream = Input("""[{"orders":[{"Id":1}]}]""");
        AssertHelper.Throws<JsonException>(() => new DataTable()
            .LoadJson(stream,
                columns: new Dictionary<string, string>(StringComparer.Ordinal) { ["Id"] = "orders.Id" }));
    }

    /// <summary>
    /// Verifies that JSON preserves decimals outside exact decimal precision.
    /// </summary>
    private static void JsonPreservesNumbersThatWouldRound()
    {
        using var stream = Input("""[{"Inexact":1e-29,"Exact":10e-29}]""");
        var table = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check(string.Equals((string)table.Rows[0]["Inexact"],
            "1e-29",
            StringComparison.Ordinal));
        AssertHelper.Check((decimal)table.Rows[0]["Exact"] == 0.0000000000000000000000000001m);
    }

    /// <summary>
    /// Verifies that JSON skips unselected structures.
    /// </summary>
    private static void JsonSkipsUnselectedStructures()
    {
        using var stream = Input("""{"meta":[{"x":[2]}],"data":[{"nested":{"x":1},"array":[1],"Id":4}]}""");
        var table = new DataTable()
            .LoadJson(stream,
                collectionPath: "data");
        AssertHelper.Check(table.Columns.Count == 1);
        AssertHelper.Check((long)table.Rows[0]["Id"] == 4L);
    }

    /// <summary>
    /// Verifies that JSON refills across large UTF-8 tokens and accepts BOM.
    /// </summary>
    private static void JsonRefillsAcrossLargeUtf8TokensAndAcceptsBom()
    {
        var name = new string('é',
            6000);
        using var stream = Input("\uFEFF[{\"Name\":\"" + name + "\"}]");
        var table = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check(string.Equals((string)table.Rows[0]["Name"],
            name,
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies that JSON infers on forward only input.
    /// </summary>
    private static void JsonInfersOnForwardOnlyInput()
    {
        using var stream = new ForwardOnlyLoadStream(Encoding.UTF8.GetBytes("""[{"Id":3}]"""));
        var table = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check((long)table.Rows[0]["Id"] == 3L);
        AssertHelper.Check(stream.CanRead);
    }

    /// <summary>
    /// Verifies that JSON restarts inference at current position.
    /// </summary>
    private static void JsonRestartsInferenceAtCurrentPosition()
    {
        using var stream = Input("""prefix[{"Id":8}]""");
        stream.Position = 6;
        var table = new DataTable()
            .LoadJson(stream);
        AssertHelper.Check((long)table.Rows[0]["Id"] == 8L);
    }

    /// <summary>
    /// Verifies that JSON rejects truncated ignored properties.
    /// </summary>
    private static void JsonRejectsTruncatedIgnoredProperties()
    {
        using var stream = Input("""[{"extra":{"broken":""");
        AssertHelper.Throws<JsonException>(() => new DataTable()
            .LoadJson(stream));
    }

    /// <summary>
    /// Verifies that JSON rejects missing collection.
    /// </summary>
    private static void JsonRejectsMissingCollection()
    {
        using var stream = Input("""{"other":[]}""");
        AssertHelper.Throws<JsonException>(() => new DataTable()
            .LoadJson(stream,
                collectionPath: "records"));
    }

    /// <summary>
    /// Verifies that JSON rejects fractional values in integer columns.
    /// </summary>
    private static void JsonRejectsFractionalValuesInIntegerColumns()
    {
        using var stream = Input("""[{"Id":1.5}]""");
        using var schema = IdSchema();
        AssertHelper.Throws<FormatException>(() => schema.LoadJson(stream));
    }

    /// <summary>
    /// Verifies that JSON loads from path.
    /// </summary>
    private static void JsonLoadsFromPath()
    {
        AssertHelper.Check(LoadFile("""[{"Id":1}]""",
            path => new DataTable()
                .LoadJson(path)).Rows.Count == 1);
    }

    /// <summary>
    /// Verifies that XML loads flat namespaced records.
    /// </summary>
    private static void XmlLoadsFlatNamespacedRecords()
    {
        using var stream = Input("""<rows xmlns="urn:data"><row><Id>00123</Id></row><row><New>late</New></row></rows>""");
        var table = new DataTable()
            .LoadXml(stream,
                "row");
        AssertHelper.Check(string.Equals((string)table.Rows[0]["Id"],
            "00123",
            StringComparison.Ordinal));
        AssertHelper.Check(table.Rows[0].IsNull("New"));
        AssertHelper.Check(stream.CanRead);
    }

    /// <summary>
    /// Verifies that XML infers types and reads nil.
    /// </summary>
    private static void XmlInfersTypesAndReadsNil()
    {
        using var stream = Input("""
            <rows xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
              <row><Id>1</Id></row><row><Id xsi:nil="true"/></row>
            </rows>
            """);
        var table = new DataTable()
            .LoadXml(stream,
                "row",
                inferTypes: true);
        AssertHelper.Check((long)table.Rows[0]["Id"] == 1L);
        AssertHelper.Check(table.Rows[1].IsNull("Id"));
    }

    /// <summary>
    /// Verifies that XML maps nested scalars across repeated collections.
    /// </summary>
    private static void XmlMapsNestedScalarsAcrossRepeatedCollections()
    {
        using var stream = Input("""
            <root><group><row><address><city>Paris</city></address></row></group>
            <group><row><address><city>Oslo</city></address></row></group></root>
            """);
        var table = new DataTable()
            .LoadXml(stream,
                "root/group/row",
                columns: new Dictionary<string, string>(StringComparer.Ordinal) { ["City"] = "address/city" });
        AssertHelper.Check(table.Rows.Count == 2);
        AssertHelper.Check(string.Equals((string)table.Rows[1]["City"],
            "Oslo",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies that nested structures are skipped by default.
    /// </summary>
    private static void XmlSkipsUnprojectedNestedElements()
    {
        using var stream = Input("""<rows><row><extra><a><b>ignore</b></a></extra><Id>7</Id></row></rows>""");
        var table = new DataTable()
            .LoadXml(stream,
                "row");
        AssertHelper.Check(table.Columns.Count == 1);
        AssertHelper.Check(string.Equals((string)table.Rows[0]["Id"],
            "7",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies that XML rejects repeating scalar mappings.
    /// </summary>
    private static void XmlRejectsRepeatingScalarMappings()
    {
        using var stream = Input("""<rows><row><Id>1</Id><Id>2</Id></row></rows>""");
        AssertHelper.Throws<XmlException>(() => new DataTable()
            .LoadXml(stream,
                "row",
                columns: new Dictionary<string, string>(StringComparer.Ordinal) { ["Id"] = "Id" }));
    }

    /// <summary>
    /// Verifies that XML rejects DTDs.
    /// </summary>
    private static void XmlRejectsDtds()
    {
        using var stream = Input("""<!DOCTYPE rows [<!ENTITY x "1">]><rows><row><Id>&x;</Id></row></rows>""");
        AssertHelper.Throws<XmlException>(() => new DataTable()
            .LoadXml(stream,
                "row"));
    }

    /// <summary>
    /// Verifies that XML uses declared XSD types.
    /// </summary>
    private static void XmlUsesDeclaredXsdTypes()
    {
        using var stream = Input("""
            <rows xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
              <row><Id>9</Id><Code>123</Code><Flag xsi:nil="true"/></row>
            </rows>
            """);
        var table = new DataTable()
            .LoadXml(stream,
                "row",
                inferTypes: true,
                schemas: XmlSchema());
        AssertHelper.Check((int)table.Rows[0]["Id"] == 9);
        AssertHelper.Check(table.Columns["Code"]!.DataType == typeof(string));
        AssertHelper.Check(table.Columns["Flag"]!.DataType == typeof(bool) && table.Rows[0].IsNull("Flag"));
    }

    /// <summary>
    /// Verifies that XML rejects values outside XSD.
    /// </summary>
    private static void XmlRejectsValuesOutsideXsd()
    {
        using var stream = Input("""<rows><row><Id>not an integer</Id></row></rows>""");
        AssertHelper.Throws<XmlSchemaValidationException>(() => new DataTable()
            .LoadXml(stream,
                "row",
                schemas: XmlSchema()));
    }

    /// <summary>
    /// Verifies that XML loads from path.
    /// </summary>
    private static void XmlLoadsFromPath()
    {
        AssertHelper.Check(LoadFile("<rows><row><Id>1</Id></row></rows>",
            path => new DataTable()
                .LoadXml(path,
                    "row")).Rows.Count == 1);
    }

    /// <summary>
    /// Creates a UTF-8 input stream.
    /// </summary>
    /// <param name="content">The input text.</param>
    /// <returns>The input stream.</returns>
    private static MemoryStream Input(string content) => new(Encoding.UTF8.GetBytes(content));

    /// <summary>
    /// Creates an integer column schema.
    /// </summary>
    /// <returns>The empty schema.</returns>
    private static DataTable IdSchema()
    {
        var schema = new DataTable();
        schema.Columns.Add("Id",
            typeof(int));
        return schema;
    }

    /// <summary>
    /// Exercises a loader against a temporary input file.
    /// </summary>
    /// <param name="content">The input text.</param>
    /// <param name="load">The path-based loader.</param>
    /// <returns>The loaded table.</returns>
    private static DataTable LoadFile(string content,
        Func<string, DataTable> load)
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path,
                content);
            return load(path);
        }
        finally
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// Creates the XML validation schema.
    /// </summary>
    /// <returns>The declared XML schema.</returns>
    private static XmlSchemaSet XmlSchema()
    {
        using var input = Input("""
            <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
              <xs:element name="rows"><xs:complexType><xs:sequence>
                <xs:element name="row" maxOccurs="unbounded"><xs:complexType><xs:sequence>
                  <xs:element name="Id" type="xs:int"/>
                  <xs:element name="Code" type="xs:string" minOccurs="0"/>
                  <xs:element name="Flag" type="xs:boolean" minOccurs="0" nillable="true"/>
                </xs:sequence></xs:complexType></xs:element>
              </xs:sequence></xs:complexType></xs:element>
            </xs:schema>
            """);
        using var reader = XmlReader.Create(input);
        var schemas = new XmlSchemaSet();
        schemas.Add(null,
            reader);
        return schemas;
    }

    #endregion
}
