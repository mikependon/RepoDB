using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Writes table projections as XML.
/// </summary>
internal static class XmlSaveHelper
{
    #region Public Methods

    /// <summary>
    /// Writes an XML projection to an open stream.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="stream">The output stream.</param>
    /// <param name="rootElement">The root element name.</param>
    /// <param name="rowElement">The row element name.</param>
    /// <param name="orientation">The selected layout.</param>
    /// <param name="indented">Whether to indent the output.</param>
    public static void Write(DataTable table,
        Stream stream,
        string rootElement,
        string rowElement,
        DataOrientation orientation,
        bool indented)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(stream);
        Validate(table,
            rootElement,
            rowElement,
            orientation);
        using var writer = XmlWriter.Create(stream,
            new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false,
                    true),
                Indent = indented,
                NewLineHandling = NewLineHandling.Entitize,
                OmitXmlDeclaration = true
            });
        writer.WriteStartElement(rootElement);
        writer.WriteAttributeString("producer",
            "RepoDB");
        writer.WriteAttributeString("orientation",
            orientation.ToString());
        WriteProjection(writer,
            table,
            rowElement,
            orientation);
        writer.WriteEndElement();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validates the layout and any names used as elements.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="rootElement">The root element name.</param>
    /// <param name="rowElement">The row element name.</param>
    /// <param name="orientation">The selected layout.</param>
    private static void Validate(DataTable table,
        string rootElement,
        string rowElement,
        DataOrientation orientation)
    {
        TableSaveHelper.ValidateOrientation(orientation);
        XmlConvert.VerifyNCName(rootElement);
        XmlConvert.VerifyNCName(rowElement);
        if (orientation is DataOrientation.Records or DataOrientation.Index or DataOrientation.Table)
        {
            foreach (DataColumn column in table.Columns)
            {
                XmlConvert.VerifyNCName(column.ColumnName);
            }
        }
    }

    /// <summary>
    /// Writes the selected projection.
    /// </summary>
    /// <param name="writer">The XML writer.</param>
    /// <param name="table">The source table.</param>
    /// <param name="rowElement">The row element name.</param>
    /// <param name="orientation">The selected layout.</param>
    private static void WriteProjection(XmlWriter writer,
        DataTable table,
        string rowElement,
        DataOrientation orientation)
    {
        switch (orientation)
        {
            case DataOrientation.Columns:
                WriteColumns(writer,
                    table);
                break;
            case DataOrientation.Split:
                WriteSplit(writer,
                    table,
                    rowElement);
                break;
            case DataOrientation.Table:
                WriteTable(writer,
                    table,
                    rowElement);
                break;
            default:
                WriteRows(writer,
                    table,
                    rowElement,
                    positional: orientation == DataOrientation.Values,
                    indexName: orientation == DataOrientation.Index ? "index" : null);
                break;
        }
    }

    /// <summary>
    /// Writes column names, row positions, and values separately.
    /// </summary>
    /// <param name="writer">The XML writer.</param>
    /// <param name="table">The source table.</param>
    /// <param name="rowElement">The row element name.</param>
    private static void WriteSplit(XmlWriter writer,
        DataTable table,
        string rowElement)
    {
        writer.WriteStartElement("columns");
        foreach (DataColumn column in table.Columns)
        {
            writer.WriteElementString("column",
                column.ColumnName);
        }
        writer.WriteEndElement();
        writer.WriteStartElement("index");
        var index = 0;
        foreach (var _ in InspectionHelper.Rows(table))
        {
            writer.WriteElementString("value",
                XmlConvert.ToString(index++));
        }
        writer.WriteEndElement();
        writer.WriteStartElement("data");
        WriteRows(writer,
            table,
            rowElement,
            positional: true);
        writer.WriteEndElement();
    }

    /// <summary>
    /// Writes columns containing indexed values.
    /// </summary>
    /// <param name="writer">The XML writer.</param>
    /// <param name="table">The source table.</param>
    private static void WriteColumns(XmlWriter writer,
        DataTable table)
    {
        foreach (DataColumn column in table.Columns)
        {
            writer.WriteStartElement("column");
            writer.WriteAttributeString("name",
                column.ColumnName);
            var index = 0;
            foreach (var row in InspectionHelper.Rows(table))
            {
                writer.WriteStartElement("value");
                WriteIndex(writer,
                    "index",
                    index++);
                WriteValue(writer,
                    row[column]);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }

    /// <summary>
    /// Writes a schema followed by named records.
    /// </summary>
    /// <param name="writer">The XML writer.</param>
    /// <param name="table">The source table.</param>
    /// <param name="rowElement">The row element name.</param>
    private static void WriteTable(XmlWriter writer,
        DataTable table,
        string rowElement)
    {
        var indexName = TableSaveHelper.IndexName(table);
        writer.WriteStartElement("schema");
        WriteSchemaField(writer,
            indexName,
            "integer");
        foreach (DataColumn column in table.Columns)
        {
            WriteSchemaField(writer,
                column.ColumnName,
                TableSaveHelper.SchemaType(column));
        }
        writer.WriteElementString("primaryKey",
            indexName);
        writer.WriteEndElement();
        writer.WriteStartElement("data");
        WriteRows(writer,
            table,
            rowElement,
            positional: false,
            indexName);
        writer.WriteEndElement();
    }

    /// <summary>
    /// Writes one field descriptor.
    /// </summary>
    /// <param name="writer">The XML writer.</param>
    /// <param name="name">The field name.</param>
    /// <param name="type">The serialized field type.</param>
    private static void WriteSchemaField(XmlWriter writer,
        string name,
        string type)
    {
        writer.WriteStartElement("field");
        writer.WriteAttributeString("name",
            name);
        writer.WriteAttributeString("type",
            type);
        writer.WriteEndElement();
    }

    /// <summary>
    /// Writes the current rows in their original order.
    /// </summary>
    /// <param name="writer">The XML writer.</param>
    /// <param name="table">The source table.</param>
    /// <param name="rowElement">The row element name.</param>
    /// <param name="positional">Whether values use a common element name.</param>
    /// <param name="indexName">The index attribute name, if any.</param>
    private static void WriteRows(XmlWriter writer,
        DataTable table,
        string rowElement,
        bool positional,
        string? indexName = null)
    {
        var index = 0;
        foreach (var row in InspectionHelper.Rows(table))
        {
            writer.WriteStartElement(rowElement);
            if (indexName is not null)
            {
                WriteIndex(writer,
                    indexName,
                    index);
            }
            WriteFields(writer,
                row,
                positional);
            writer.WriteEndElement();
            index++;
        }
    }

    /// <summary>
    /// Writes the fields of one row.
    /// </summary>
    /// <param name="writer">The XML writer.</param>
    /// <param name="row">The source row.</param>
    /// <param name="positional">Whether values use a common element name.</param>
    private static void WriteFields(XmlWriter writer,
        DataRow row,
        bool positional)
    {
        foreach (DataColumn column in row.Table.Columns)
        {
            writer.WriteStartElement(positional ? "value" : column.ColumnName);
            WriteValue(writer,
                row[column]);
            writer.WriteEndElement();
        }
    }

    /// <summary>
    /// Writes a row position attribute.
    /// </summary>
    /// <param name="writer">The XML writer.</param>
    /// <param name="name">The attribute name.</param>
    /// <param name="index">The row position.</param>
    private static void WriteIndex(XmlWriter writer,
        string name,
        int index) =>
        writer.WriteAttributeString(name,
            XmlConvert.ToString(index));

    /// <summary>
    /// Writes a scalar or the XML Schema nil attribute.
    /// </summary>
    /// <param name="writer">The XML writer.</param>
    /// <param name="value">The cell value.</param>
    private static void WriteValue(XmlWriter writer,
        object value)
    {
        if (value is DBNull)
        {
            writer.WriteAttributeString("xsi",
                "nil",
                XmlSchema.InstanceNamespace,
                "true");
            return;
        }
        var text = value switch
        {
            float number => XmlConvert.ToString(number),
            double number => XmlConvert.ToString(number),
            TimeSpan duration => XmlConvert.ToString(duration),
            _ => TableSaveHelper.FormatValue(value)
        };
        writer.WriteString(text);
    }

    #endregion
}
