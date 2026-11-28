namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the conversion between notebook tables and data tables.
/// </summary>
internal static class NotebookDataTableConverterTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunToDataTable();
        RunColumnNames();
        RunToNotebookTable();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of the data table.
    /// </summary>
    private static void RunToDataTable()
    {
        NamesTheDataTable();
        GivesColumnsTheirTypes();
        ReadsTypedValues();
        ReadsEveryTypeName();
        ReadsAnUnknownTypeAsText();
        ReadsNullAsDatabaseNull();
        ReadsAnUnreadableValueAsDatabaseNull();
        CompletesAShortRowWithDatabaseNulls();
        LeavesTheRowsAdded();
    }

    /// <summary>
    /// Runs the scenarios of the column names.
    /// </summary>
    private static void RunColumnNames()
    {
        NumbersARepeatedName();
        NumbersANameThatDiffersByCaseOnly();
        SkipsASuffixThatIsTaken();
        NamesColumnsWithoutAName();
        NamesAColumnWithoutANameByItsPosition();
        SuffixesAPositionNameThatIsTaken();
    }

    /// <summary>
    /// Runs the scenarios of the notebook table.
    /// </summary>
    private static void RunToNotebookTable()
    {
        KeepsTheNameAsTitle();
        TitlesAnUnnamedTable();
        KeepsTheColumnNamesAndTypes();
        WritesValuesWithTheInvariantCulture();
        WritesDatabaseNullAsNull();
        WritesAValueWhoseTextFails();
        PassesOnAnOutOfMemoryOfAValue();
        LeavesOutDeletedRows();
        CutsRowsAtTheLimit();
        MarksACutTable();
        DoesNotMarkATableAtTheLimit();
        KeepsEveryRowWithTheLargestLimit();
    }

    /// <summary>
    /// Converts one value of a type.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <param name="text">The text.</param>
    /// <returns>The value of the data table.</returns>
    private static object Value(string typeName,
        string? text) => NotebookDataTableConverter.ToDataTable(new NotebookTable([new NotebookColumn("Value",
                typeName)],
            [[text]]),
        "T").Rows[0][0];

    /// <summary>
    /// Converts a table without rows and gets its column names.
    /// </summary>
    /// <param name="names">The names of the notebook columns.</param>
    /// <returns>The names of the data columns.</returns>
    private static string NamesOf(params string[] names) => string.Join(',',
        NotebookDataTableConverter.ToDataTable(new NotebookTable(names
                    .Select(name => new NotebookColumn(name))
                    .ToList(),
                []),
            "T").Columns
            .Cast<DataColumn>()
            .Select(column => column.ColumnName));

    /// <summary>
    /// Converts a data table whose only value fails to write its text.
    /// </summary>
    /// <param name="failure">What writing the text throws.</param>
    /// <returns>The notebook table.</returns>
    private static NotebookTable Failing(Exception failure)
    {
        var source = new DataTable();
        source.Columns.Add("Value",
            typeof(object));
        source.Rows.Add(new FailingTexts(failure));
        return NotebookDataTableConverter.ToNotebookTable(source,
            10);
    }

    /// <summary>
    /// Converts a data table of numbers.
    /// </summary>
    /// <param name="rowLimit">The row limit.</param>
    /// <returns>The notebook table.</returns>
    private static NotebookTable Numbers(int rowLimit) => NotebookDataTableConverter.ToNotebookTable(SampleTables.Numbers(),
        rowLimit);

    /// <summary>
    /// Tests NamesTheDataTable.
    /// </summary>
    private static void NamesTheDataTable()
    {
        // Act
        var table = NotebookDataTableConverter.ToDataTable(SampleTables.Customers(),
            "Customers");

        // Assert
        Check(table.TableName == "Customers");
    }

    /// <summary>
    /// Tests GivesColumnsTheirTypes.
    /// </summary>
    private static void GivesColumnsTheirTypes()
    {
        // Act
        var table = NotebookDataTableConverter.ToDataTable(SampleTables.Customers(),
            "T");

        // Assert
        Check(table.Columns[0].DataType == typeof(string) && table.Columns[1].DataType == typeof(long));
    }

    /// <summary>
    /// Tests ReadsTypedValues.
    /// </summary>
    private static void ReadsTypedValues()
    {
        // Act
        var table = NotebookDataTableConverter.ToDataTable(SampleTables.Customers(),
            "T");

        // Assert
        Check(table.Rows[1]["Orders"] is 7L);
    }

    /// <summary>
    /// Tests ReadsEveryTypeName.
    /// </summary>
    private static void ReadsEveryTypeName()
    {
        // Setup
        (string TypeName, string Text)[] values =
        [
            ("binary", "AQI="),
            ("bool", "True"),
            ("byte", "1"),
            ("datetime", "2026-11-28T10:00:00.0000000"),
            ("datetimeoffset", "2026-11-28T10:00:00.0000000+08:00"),
            ("decimal", "1.5"),
            ("double", "1.5"),
            ("float", "1.5"),
            ("guid", "0f8fad5b-d9cb-469f-a165-70867728950e"),
            ("int", "1"),
            ("long", "1"),
            ("short", "1"),
            ("timespan", "01:00:00")
        ];

        // Act
        var types = values
            .Select(value => Value(value.TypeName,
                value.Text)
                .GetType())
            .ToList();

        // Assert
        Check(types
            .SequenceEqual(values
                .Select(value => NotebookValueConverter.TypeOf(value.TypeName))));
    }

    /// <summary>
    /// Tests ReadsAnUnknownTypeAsText.
    /// </summary>
    private static void ReadsAnUnknownTypeAsText()
    {
        // Act
        var value = Value("geometry",
            "POINT");

        // Assert
        Check(value is "POINT");
    }

    /// <summary>
    /// Tests ReadsNullAsDatabaseNull.
    /// </summary>
    private static void ReadsNullAsDatabaseNull()
    {
        // Act
        var value = Value("string",
            null);

        // Assert
        Check(value is DBNull);
    }

    /// <summary>
    /// Tests ReadsAnUnreadableValueAsDatabaseNull.
    /// </summary>
    private static void ReadsAnUnreadableValueAsDatabaseNull()
    {
        // Act
        var value = Value("int",
            "twelve");

        // Assert
        Check(value is DBNull);
    }

    /// <summary>
    /// Tests CompletesAShortRowWithDatabaseNulls.
    /// </summary>
    private static void CompletesAShortRowWithDatabaseNulls()
    {
        // Setup
        var source = new NotebookTable([new NotebookColumn("A"), new NotebookColumn("B")],
            [["a"]]);

        // Act
        var table = NotebookDataTableConverter.ToDataTable(source,
            "T");

        // Assert
        Check(table.Rows[0][1] is DBNull);
    }

    /// <summary>
    /// Tests LeavesTheRowsAdded.
    /// </summary>
    private static void LeavesTheRowsAdded()
    {
        // Act
        var table = NotebookDataTableConverter.ToDataTable(SampleTables.Customers(),
            "T");

        // Assert
        Check(table.Rows[0].RowState == DataRowState.Added);
    }

    /// <summary>
    /// Tests NumbersARepeatedName.
    /// </summary>
    private static void NumbersARepeatedName()
    {
        // Act
        var names = NamesOf("Id",
            "Id");

        // Assert
        Check(names == "Id,Id_");
    }

    /// <summary>
    /// Tests NumbersANameThatDiffersByCaseOnly.
    /// </summary>
    private static void NumbersANameThatDiffersByCaseOnly()
    {
        // Act
        var names = NamesOf("Id",
            "ID");

        // Assert
        Check(names == "Id,ID_");
    }

    /// <summary>
    /// Tests SkipsASuffixThatIsTaken.
    /// </summary>
    private static void SkipsASuffixThatIsTaken()
    {
        // Act
        var names = NamesOf("Id",
            "Id_",
            "Id");

        // Assert
        Check(names == "Id,Id_,Id__");
    }

    /// <summary>
    /// Tests NamesColumnsWithoutAName.
    /// </summary>
    private static void NamesColumnsWithoutAName()
    {
        // Act
        var names = NamesOf(string.Empty,
            string.Empty);

        // Assert
        Check(names == "Column1,Column2");
    }

    /// <summary>
    /// Tests KeepsTheNameAsTitle.
    /// </summary>
    private static void KeepsTheNameAsTitle()
    {
        // Act
        var table = Numbers(10);

        // Assert
        Check(table.Title == "Numbers");
    }

    /// <summary>
    /// Tests TitlesAnUnnamedTable.
    /// </summary>
    private static void TitlesAnUnnamedTable()
    {
        // Act
        var table = NotebookDataTableConverter.ToNotebookTable(new DataTable(),
            10);

        // Assert
        Check(table.Title == "Table");
    }

    /// <summary>
    /// Tests KeepsTheColumnNamesAndTypes.
    /// </summary>
    private static void KeepsTheColumnNamesAndTypes()
    {
        // Act
        var column = Numbers(10).Columns
            .Single();

        // Assert
        Check(column.Name == "Value" && column.TypeName == "int");
    }

    /// <summary>
    /// Tests WritesValuesWithTheInvariantCulture.
    /// </summary>
    private static void WritesValuesWithTheInvariantCulture()
    {
        // Setup
        using var culture = new CultureScope();
        var source = new DataTable();
        source.Columns.Add("Price",
            typeof(double));
        source.Rows.Add(1.5);

        // Act
        var table = NotebookDataTableConverter.ToNotebookTable(source,
            10);

        // Assert
        Check(table.Rows[0][0] == "1.5");
    }

    /// <summary>
    /// Tests WritesDatabaseNullAsNull.
    /// </summary>
    private static void WritesDatabaseNullAsNull()
    {
        // Setup
        var source = new DataTable();
        source.Columns.Add("Name");
        source.Rows.Add(DBNull.Value);

        // Act
        var table = NotebookDataTableConverter.ToNotebookTable(source,
            10);

        // Assert
        Check(table.Rows[0][0] is null);
    }

    /// <summary>
    /// Tests LeavesOutDeletedRows.
    /// </summary>
    private static void LeavesOutDeletedRows()
    {
        // Setup
        var source = SampleTables.Numbers();
        source.Rows[0].Delete();

        // Act
        var table = NotebookDataTableConverter.ToNotebookTable(source,
            10);

        // Assert
        Check(table.Rows.Count == 2 && table.Rows[0][0] == "2");
    }

    /// <summary>
    /// Tests CutsRowsAtTheLimit.
    /// </summary>
    private static void CutsRowsAtTheLimit()
    {
        // Act
        var table = Numbers(2);

        // Assert
        Check(table.Rows.Count == 2);
    }

    /// <summary>
    /// Tests MarksACutTable.
    /// </summary>
    private static void MarksACutTable()
    {
        // Act
        var table = Numbers(2);

        // Assert
        Check(table.IsTruncated);
    }

    /// <summary>
    /// Tests DoesNotMarkATableAtTheLimit.
    /// </summary>
    private static void DoesNotMarkATableAtTheLimit()
    {
        // Act
        var table = Numbers(3);

        // Assert
        Check(!table.IsTruncated);
    }

    /// <summary>
    /// Tests KeepsEveryRowWithTheLargestLimit.
    /// </summary>
    private static void KeepsEveryRowWithTheLargestLimit()
    {
        // Act
        var table = Numbers(int.MaxValue);

        // Assert
        Check(table.Rows.Count == 3);
    }

    /// <summary>
    /// Tests NamesAColumnWithoutANameByItsPosition.
    /// </summary>
    private static void NamesAColumnWithoutANameByItsPosition()
    {
        // Act
        var names = NamesOf("A",
            string.Empty);

        // Assert
        Check(names == "A,Column2");
    }

    /// <summary>
    /// Tests SuffixesAPositionNameThatIsTaken.
    /// </summary>
    private static void SuffixesAPositionNameThatIsTaken()
    {
        // Act
        var names = NamesOf("Column2",
            string.Empty);

        // Assert
        Check(names == "Column2,Column2_");
    }

    /// <summary>
    /// Tests WritesAValueWhoseTextFails.
    /// </summary>
    private static void WritesAValueWhoseTextFails()
    {
        // Act
        var table = Failing(new InvalidOperationException("no text"));

        // Assert
        Check(table.Rows[0][0] == "<ToString failed: no text>");
    }

    /// <summary>
    /// Tests PassesOnAnOutOfMemoryOfAValue.
    /// </summary>
    private static void PassesOnAnOutOfMemoryOfAValue()
    {
        // Act
        Action operation = () => Failing(new OutOfMemoryException());

        // Assert
        Throws<OutOfMemoryException>(operation);
    }

    #endregion
}
