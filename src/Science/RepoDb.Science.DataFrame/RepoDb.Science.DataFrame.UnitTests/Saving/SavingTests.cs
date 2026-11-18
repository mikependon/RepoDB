using System.Data;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using RepoDb.Science.UnitTests.Helpers;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Exercises the DataTable save methods.
/// </summary>
internal static class SavingTests
{
    #region Public Methods

    /// <summary>
    /// Runs the saving tests.
    /// </summary>
    public static void Run()
    {
        RunProjectionTests();
        RunFormatTests();
        RunFileTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs tests for all output layouts.
    /// </summary>
    private static void RunProjectionTests()
    {
        JsonWritesRecords();
        XmlWritesRecords();
        JsonWritesSplit();
        XmlWritesSplit();
        JsonWritesIndex();
        XmlWritesIndex();
        JsonWritesColumns();
        XmlWritesColumns();
        JsonWritesValues();
        XmlWritesValues();
        JsonWritesTable();
        XmlWritesTable();
    }

    /// <summary>
    /// Runs format and validation tests.
    /// </summary>
    private static void RunFormatTests()
    {
        CsvEscapesFieldsAndLeavesStreamOpen();
        CsvPreservesSingleEmptyFields();
        JsonRecordsRoundTripNullAndText();
        XmlRecordsRoundTripNullAndText();
        JsonIndexesOnlyCurrentRows();
        JsonTableDisambiguatesIndexName();
        JsonTableDescribesNullableColumns();
        XmlRejectsInvalidFieldNames();
        XmlColumnsAllowNamesAsAttributeValues();
        JsonRejectsInvalidOrientation();
        XmlRejectsInvalidOrientation();
    }

    /// <summary>
    /// Runs path overload tests.
    /// </summary>
    private static void RunFileTests()
    {
        FileSavesPreserveDestinationOnFailure();
        CsvSavesToPath();
        JsonSavesToPath();
        XmlSavesToPath();
    }

    /// <summary>
    /// Verifies the JSON Records projection.
    /// </summary>
    private static void JsonWritesRecords() =>
        CheckJson(DataOrientation.Records,
            """[{"Id":101,"Name":"Ada"},{"Id":102,"Name":"Grace"},{"Id":103,"Name":"Linus"}]""");

    /// <summary>
    /// Verifies the XML Records projection.
    /// </summary>
    private static void XmlWritesRecords() =>
        CheckXml(DataOrientation.Records,
            """<customer><Id>101</Id><Name>Ada</Name></customer><customer><Id>102</Id><Name>Grace</Name></customer><customer><Id>103</Id><Name>Linus</Name></customer>""");

    /// <summary>
    /// Verifies the JSON Split projection.
    /// </summary>
    private static void JsonWritesSplit() =>
        CheckJson(DataOrientation.Split,
            """{"columns":["Id","Name"],"index":[0,1,2],"data":[[101,"Ada"],[102,"Grace"],[103,"Linus"]]}""");

    /// <summary>
    /// Verifies the XML Split projection.
    /// </summary>
    private static void XmlWritesSplit() =>
        CheckXml(DataOrientation.Split,
            """<columns><column>Id</column><column>Name</column></columns><index><value>0</value><value>1</value><value>2</value></index><data><customer><value>101</value><value>Ada</value></customer><customer><value>102</value><value>Grace</value></customer><customer><value>103</value><value>Linus</value></customer></data>""");

    /// <summary>
    /// Verifies the JSON Index projection.
    /// </summary>
    private static void JsonWritesIndex() =>
        CheckJson(DataOrientation.Index,
            """{"0":{"Id":101,"Name":"Ada"},"1":{"Id":102,"Name":"Grace"},"2":{"Id":103,"Name":"Linus"}}""");

    /// <summary>
    /// Verifies the XML Index projection.
    /// </summary>
    private static void XmlWritesIndex() =>
        CheckXml(DataOrientation.Index,
            """<customer index="0"><Id>101</Id><Name>Ada</Name></customer><customer index="1"><Id>102</Id><Name>Grace</Name></customer><customer index="2"><Id>103</Id><Name>Linus</Name></customer>""");

    /// <summary>
    /// Verifies the JSON Columns projection.
    /// </summary>
    private static void JsonWritesColumns() =>
        CheckJson(DataOrientation.Columns,
            """{"Id":{"0":101,"1":102,"2":103},"Name":{"0":"Ada","1":"Grace","2":"Linus"}}""");

    /// <summary>
    /// Verifies the XML Columns projection.
    /// </summary>
    private static void XmlWritesColumns() =>
        CheckXml(DataOrientation.Columns,
            """<column name="Id"><value index="0">101</value><value index="1">102</value><value index="2">103</value></column><column name="Name"><value index="0">Ada</value><value index="1">Grace</value><value index="2">Linus</value></column>""");

    /// <summary>
    /// Verifies the JSON Values projection.
    /// </summary>
    private static void JsonWritesValues() =>
        CheckJson(DataOrientation.Values,
            """[[101,"Ada"],[102,"Grace"],[103,"Linus"]]""");

    /// <summary>
    /// Verifies the XML Values projection.
    /// </summary>
    private static void XmlWritesValues() =>
        CheckXml(DataOrientation.Values,
            """<customer><value>101</value><value>Ada</value></customer><customer><value>102</value><value>Grace</value></customer><customer><value>103</value><value>Linus</value></customer>""");

    /// <summary>
    /// Verifies the JSON Table projection.
    /// </summary>
    private static void JsonWritesTable() =>
        CheckJson(DataOrientation.Table,
            """{"schema":{"fields":[{"name":"index","type":"integer"},{"name":"Id","type":"integer"},{"name":"Name","type":"string"}],"primaryKey":["index"],"pandas_version":"1.4.0"},"data":[{"index":0,"Id":101,"Name":"Ada"},{"index":1,"Id":102,"Name":"Grace"},{"index":2,"Id":103,"Name":"Linus"}]}""");

    /// <summary>
    /// Verifies the XML Table projection.
    /// </summary>
    private static void XmlWritesTable() =>
        CheckXml(DataOrientation.Table,
            """<schema><field name="index" type="integer"/><field name="Id" type="integer"/><field name="Name" type="string"/><primaryKey>index</primaryKey></schema><data><customer index="0"><Id>101</Id><Name>Ada</Name></customer><customer index="1"><Id>102</Id><Name>Grace</Name></customer><customer index="2"><Id>103</Id><Name>Linus</Name></customer></data>""");

    /// <summary>
    /// Verifies CSV escaping and stream ownership.
    /// </summary>
    private static void CsvEscapesFieldsAndLeavesStreamOpen()
    {
        using var table = new DataTable();
        table.Columns.Add("Id");
        table.Columns.Add("Note;Text");
        table.Rows.Add("001",
            "first; line\r\nsecond \"quoted\"");
        using var stream = new MemoryStream();
        table.SaveCsv(stream,
            separator: ';');
        AssertHelper.Check(stream.CanWrite);
        stream.Position = 0;
        using var read = new DataTable()
            .LoadCsv(stream,
                separator: ';');
        AssertHelper.Check(string.Equals((string)read.Rows[0][1],
            "first; line\r\nsecond \"quoted\"",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies that a single empty field still produces a record.
    /// </summary>
    private static void CsvPreservesSingleEmptyFields()
    {
        using var table = new DataTable();
        table.Columns.Add("Value");
        table.Rows.Add(DBNull.Value);
        using var stream = new MemoryStream();
        table.SaveCsv(stream);
        stream.Position = 0;
        using var read = new DataTable()
            .LoadCsv(stream);
        AssertHelper.Check(read.Rows.Count == 1);
        AssertHelper.Check((string)read.Rows[0][0] == string.Empty);
    }

    /// <summary>
    /// Verifies the default Records layout and nullable values.
    /// </summary>
    private static void JsonRecordsRoundTripNullAndText()
    {
        using var table = Customers();
        table.Rows[0]["Name"] = DBNull.Value;
        table.Rows[1]["Name"] = "Grace \"G\"";
        using var stream = new MemoryStream();
        table.SaveJson(stream);
        AssertHelper.Check(stream.CanWrite);
        stream.Position = 0;
        using var read = table.LoadJson(stream,
            collectionPath: "data");
        AssertHelper.Check(read.Rows[0].IsNull("Name"));
        AssertHelper.Check(string.Equals((string)read.Rows[1]["Name"],
            "Grace \"G\"",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies the default Records layout, escaping, and stream ownership.
    /// </summary>
    private static void XmlRecordsRoundTripNullAndText()
    {
        using var table = Customers();
        table.Rows[0]["Name"] = DBNull.Value;
        table.Rows[1]["Name"] = "<Grace> & Friends\r\n";
        using var stream = new MemoryStream();
        table.SaveXml(stream);
        AssertHelper.Check(stream.CanWrite);
        stream.Position = 0;
        using var read = table.LoadXml(stream,
            "row");
        AssertHelper.Check(read.Rows[0].IsNull("Name"));
        AssertHelper.Check(string.Equals((string)read.Rows[1]["Name"],
            "<Grace> & Friends\r\n",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies that exported row positions skip deleted rows.
    /// </summary>
    private static void JsonIndexesOnlyCurrentRows()
    {
        using var table = Customers();
        table.AcceptChanges();
        table.Rows[0].Delete();
        using var stream = new MemoryStream();
        table.SaveJson(stream,
            DataOrientation.Index);
        stream.Position = 0;
        using var document = JsonDocument.Parse(stream);
        var data = document.RootElement.GetProperty("data");
        AssertHelper.Check(data.GetProperty("0")
            .GetProperty("Id")
            .GetInt32() == 102);
        AssertHelper.Check(table.Rows[0].RowState == DataRowState.Deleted);
    }

    /// <summary>
    /// Verifies that the generated index leaves a user column intact.
    /// </summary>
    private static void JsonTableDisambiguatesIndexName()
    {
        using var table = new DataTable();
        table.Columns.Add("index",
            typeof(string));
        table.Rows.Add("data");
        using var stream = new MemoryStream();
        table.SaveJson(stream,
            DataOrientation.Table);
        stream.Position = 0;
        using var document = JsonDocument.Parse(stream);
        var record = document.RootElement
            .GetProperty("data")
            .GetProperty("data")[0];
        AssertHelper.Check(record.GetProperty("index_")
            .GetInt32() == 0);
        AssertHelper.Check(string.Equals(record.GetProperty("index")
                .GetString(),
            "data",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies that nullable columns declare their pandas dtypes.
    /// </summary>
    private static void JsonTableDescribesNullableColumns()
    {
        using var table = new DataTable();
        table.Columns.Add("Amount",
            typeof(int));
        table.Rows.Add(DBNull.Value);
        using var stream = new MemoryStream();
        table.SaveJson(stream,
            DataOrientation.Table);
        stream.Position = 0;
        using var document = JsonDocument.Parse(stream);
        var data = document.RootElement.GetProperty("data");
        var field = data
            .GetProperty("schema")
            .GetProperty("fields")[1];
        AssertHelper.Check(string.Equals(field.GetProperty("extDtype")
                .GetString(),
            "Int32",
            StringComparison.Ordinal));
        AssertHelper.Check(data.GetProperty("data")[0]
            .GetProperty("Amount").ValueKind == JsonValueKind.Null);
    }

    /// <summary>
    /// Verifies early validation for XML named fields.
    /// </summary>
    private static void XmlRejectsInvalidFieldNames()
    {
        using var table = new DataTable();
        table.Columns.Add("invalid name");
        using var stream = new MemoryStream();
        AssertHelper.Throws<XmlException>(() => table.SaveXml(stream));
        AssertHelper.Check(stream.Length == 0);
    }

    /// <summary>
    /// Verifies column names are escaped in the Columns layout.
    /// </summary>
    private static void XmlColumnsAllowNamesAsAttributeValues()
    {
        using var table = new DataTable();
        table.Columns.Add("Name & city");
        table.Rows.Add("A");
        using var stream = new MemoryStream();
        table.SaveXml(stream,
            orientation: DataOrientation.Columns);
        stream.Position = 0;
        var root = XElement.Load(stream);
        AssertHelper.Check(string.Equals((string?)root.Element("column")?.Attribute("name"),
            "Name & city",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Verifies validation before writing a JSON document.
    /// </summary>
    private static void JsonRejectsInvalidOrientation()
    {
        using var table = Customers();
        using var stream = new MemoryStream();
        AssertHelper.Throws<ArgumentOutOfRangeException>(() => table.SaveJson(stream,
            (DataOrientation)99));
        AssertHelper.Check(stream.Length == 0);
    }

    /// <summary>
    /// Verifies validation before writing an XML document.
    /// </summary>
    private static void XmlRejectsInvalidOrientation()
    {
        using var table = Customers();
        using var stream = new MemoryStream();
        AssertHelper.Throws<ArgumentOutOfRangeException>(() => table.SaveXml(stream,
            orientation: (DataOrientation)99));
        AssertHelper.Check(stream.Length == 0);
    }

    /// <summary>
    /// Verifies that saving invalid values preserves existing output.
    /// </summary>
    private static void FileSavesPreserveDestinationOnFailure()
    {
        WithPath(path =>
        {
            using var table = new DataTable();
            table.Columns.Add("Value",
                typeof(double));
            table.Rows.Add(double.NaN);
            File.WriteAllText(path,
                "original");
            AssertHelper.Throws<ArgumentException>(() => table.SaveJson(path));
            AssertHelper.Check(string.Equals(File.ReadAllText(path),
                "original",
                StringComparison.Ordinal));
        });
    }

    /// <summary>
    /// Verifies the CSV path overload.
    /// </summary>
    private static void CsvSavesToPath()
    {
        WithPath(path =>
        {
            using var table = Customers();
            table.SaveCsv(path);
            using var read = table.LoadCsv(path);
            AssertHelper.Check(read.Rows.Count == 3);
        });
    }

    /// <summary>
    /// Verifies the JSON path overload and indentation.
    /// </summary>
    private static void JsonSavesToPath()
    {
        WithPath(path =>
        {
            using var table = Customers();
            table.SaveJson(path,
                indented: true);
            using var read = table.LoadJson(path,
                collectionPath: "data");
            AssertHelper.Check(read.Rows.Count == 3);
            AssertHelper.Check(File.ReadAllText(path)
                .Contains('\n'));
        });
    }

    /// <summary>
    /// Verifies the XML path overload with custom element names.
    /// </summary>
    private static void XmlSavesToPath()
    {
        WithPath(path =>
        {
            using var table = Customers();
            table.SaveXml(path,
                rootElement: "customers",
                rowElement: "customer",
                indented: true);
            using var read = table.LoadXml(path,
                "customers/customer");
            AssertHelper.Check(read.Rows.Count == 3);
            AssertHelper.Check(File.ReadAllText(path)
                .Contains('\n'));
        });
    }

    /// <summary>
    /// Compares JSON metadata and payload with the expected projection.
    /// </summary>
    /// <param name="orientation">The output layout.</param>
    /// <param name="payload">The expected JSON payload.</param>
    private static void CheckJson(DataOrientation orientation,
        string payload)
    {
        using var table = Customers();
        using var stream = new MemoryStream();
        table.SaveJson(stream,
            orientation);
        stream.Position = 0;
        using var actual = JsonDocument.Parse(stream);
        using var expected = JsonDocument.Parse(payload);
        var root = actual.RootElement;
        AssertHelper.Check(string.Equals(root.GetProperty("producer")
                .GetString(),
            "RepoDB",
            StringComparison.Ordinal));
        AssertHelper.Check(string.Equals(root.GetProperty("orientation")
                .GetString(),
            orientation.ToString(),
            StringComparison.Ordinal));
        AssertHelper.Check(JsonElement.DeepEquals(root.GetProperty("data"),
            expected.RootElement));
    }

    /// <summary>
    /// Compares XML metadata and payload with the expected projection.
    /// </summary>
    /// <param name="orientation">The output layout.</param>
    /// <param name="payload">The expected XML children.</param>
    private static void CheckXml(DataOrientation orientation,
        string payload)
    {
        using var table = Customers();
        using var stream = new MemoryStream();
        table.SaveXml(stream,
            rootElement: "customers",
            rowElement: "customer",
            orientation: orientation);
        stream.Position = 0;
        var actual = XElement.Load(stream);
        var expected = XElement.Parse($"""<customers producer="RepoDB" orientation="{orientation}">{payload}</customers>""");
        AssertHelper.Check(XNode.DeepEquals(actual,
            expected));
    }

    /// <summary>
    /// Creates the table used by the projection tests.
    /// </summary>
    private static DataTable Customers()
    {
        var table = new DataTable("Customers");
        table.Columns.Add("Id",
            typeof(int)).AllowDBNull = false;
        table.Columns.Add("Name",
            typeof(string));
        table.Rows.Add(101,
            "Ada");
        table.Rows.Add(102,
            "Grace");
        table.Rows.Add(103,
            "Linus");
        return table;
    }

    /// <summary>
    /// Runs a test with a temporary destination.
    /// </summary>
    /// <param name="test">The path test.</param>
    private static void WithPath(Action<string> test)
    {
        var path = Path.GetTempFileName();
        try
        {
            test(path);
        }
        finally
        {
            File.Delete(path);
        }
    }

    #endregion
}
