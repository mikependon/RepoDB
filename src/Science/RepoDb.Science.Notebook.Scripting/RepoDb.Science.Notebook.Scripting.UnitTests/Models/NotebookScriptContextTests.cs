namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the values the scripts share.
/// </summary>
internal static class NotebookScriptContextTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunValues();
        RunTypedValues();
        RunNames();
        RunSqlResults();
        RunClear();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of the values.
    /// </summary>
    private static void RunValues()
    {
        ConstructorRejectsNullMessages();
        GetReadsAStoredValue();
        SetReplacesAValue();
        SetStoresNull();
        SetRejectsABlankName();
        SetRejectsANullName();
        GetOfAnUnknownNameSaysToSetIt();
        GetIsCaseSensitive();
        GetUsesTheTextsOfTheHost();
    }

    /// <summary>
    /// Runs the scenarios of the typed values.
    /// </summary>
    private static void RunTypedValues()
    {
        TypedGetReadsTheValue();
        TypedGetReadsNullAsAReference();
        TypedGetReadsNullAsANullableValue();
        TypedGetRefusesAnotherType();
        TypedGetRefusesNullAsAValue();
        TypedGetUsesTheTextsOfTheHost();
        TypedGetOfAnUnknownNameFails();
    }

    /// <summary>
    /// Runs the scenarios of the names.
    /// </summary>
    private static void RunNames()
    {
        NamesStartEmpty();
        NamesHoldTheStoredValues();
        HasFindsAStoredValue();
        HasMissesAnUnknownValue();
        RemoveTakesAValueAway();
        RemoveOfAnUnknownValueIsFalse();
    }

    /// <summary>
    /// Runs the scenarios of the SQL results.
    /// </summary>
    private static void RunSqlResults()
    {
        LoadRejectsNull();
        FromSqlResultGivesEveryTable();
        FromSqlResultIgnoresCase();
        FromSqlResultGivesTheSameTableUntilTheNextLoad();
        FromSqlResultOfAnUnknownSectionSaysToRunIt();
        FromSqlResultUsesTheTextsOfTheHost();
        LoadNamesATableByItsTitle();
        LoadLeavesATableWithoutATitleUnnamed();
        LoadReplacesTheEarlierResults();
        LoadKeepsTheValues();
    }

    /// <summary>
    /// Runs the scenarios of the clearing.
    /// </summary>
    private static void RunClear()
    {
        ClearRemovesTheValues();
        ClearRemovesTheSqlResults();
    }

    /// <summary>
    /// Creates an empty context.
    /// </summary>
    /// <param name="messages">The texts, or null for the default ones.</param>
    /// <returns>The context.</returns>
    private static NotebookScriptContext Empty(NotebookScriptingMessages? messages = null) => new(messages ?? NotebookScriptingMessages.Default);

    /// <summary>
    /// Creates a context that holds one value.
    /// </summary>
    /// <param name="messages">The texts, or null for the default ones.</param>
    /// <returns>The context, where Total is one.</returns>
    private static NotebookScriptContext WithTotal(NotebookScriptingMessages? messages = null)
    {
        var context = Empty(messages);
        context.Set("Total",
            1);
        return context;
    }

    /// <summary>
    /// Creates a context that holds the result of a section.
    /// </summary>
    /// <param name="title">The title of the table.</param>
    /// <returns>The context.</returns>
    private static NotebookScriptContext Loaded(string title = "")
    {
        var context = Empty();
        context.Load(SampleTables.Results("Section1",
            SampleTables.Customers(title)));
        return context;
    }

    /// <summary>
    /// Tests ConstructorRejectsNullMessages.
    /// </summary>
    private static void ConstructorRejectsNullMessages()
    {
        // Act
        Action operation = () => _ = new NotebookScriptContext(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests GetReadsAStoredValue.
    /// </summary>
    private static void GetReadsAStoredValue()
    {
        // Setup
        var context = WithTotal();

        // Act
        var value = context.Get("Total");

        // Assert
        Check(value is 1);
    }

    /// <summary>
    /// Tests SetReplacesAValue.
    /// </summary>
    private static void SetReplacesAValue()
    {
        // Setup
        var context = WithTotal();

        // Act
        context.Set("Total",
            2);

        // Assert
        Check(context.Get("Total") is 2);
    }

    /// <summary>
    /// Tests SetStoresNull.
    /// </summary>
    private static void SetStoresNull()
    {
        // Setup
        var context = Empty();

        // Act
        context.Set("Nothing",
            null);

        // Assert
        Check(context.Get("Nothing") is null);
    }

    /// <summary>
    /// Tests SetRejectsABlankName.
    /// </summary>
    private static void SetRejectsABlankName()
    {
        // Setup
        var context = Empty();

        // Act/Assert
        Throws<ArgumentException>(() => context.Set(" ",
            1));
    }

    /// <summary>
    /// Tests SetRejectsANullName.
    /// </summary>
    private static void SetRejectsANullName()
    {
        // Setup
        var context = Empty();

        // Act/Assert
        Throws<ArgumentNullException>(() => context.Set(null!,
            1));
    }

    /// <summary>
    /// Tests GetOfAnUnknownNameSaysToSetIt.
    /// </summary>
    private static void GetOfAnUnknownNameSaysToSetIt()
    {
        // Setup
        var context = Empty();

        // Act
        var error = Throws<InvalidOperationException>(() => context.Get("Missing"));

        // Assert
        Check(error.Message == NotebookScriptingMessages.Default.ValueMissing("Missing"));
    }

    /// <summary>
    /// Tests GetIsCaseSensitive.
    /// </summary>
    private static void GetIsCaseSensitive()
    {
        // Setup
        var context = WithTotal();

        // Act/Assert
        Throws<InvalidOperationException>(() => context.Get("total"));
    }

    /// <summary>
    /// Tests GetUsesTheTextsOfTheHost.
    /// </summary>
    private static void GetUsesTheTextsOfTheHost()
    {
        // Setup
        var context = Empty(new MarkedMessages());

        // Act
        var error = Throws<InvalidOperationException>(() => context.Get("V"));

        // Assert
        Check(error.Message == "no value V");
    }

    /// <summary>
    /// Tests TypedGetReadsTheValue.
    /// </summary>
    private static void TypedGetReadsTheValue()
    {
        // Setup
        var context = WithTotal();

        // Act
        var value = context.Get<int>("Total");

        // Assert
        Check(value == 1);
    }

    /// <summary>
    /// Tests TypedGetReadsNullAsAReference.
    /// </summary>
    private static void TypedGetReadsNullAsAReference()
    {
        // Setup
        var context = Empty();
        context.Set("Nothing",
            null);

        // Act
        var value = context.Get<string>("Nothing");

        // Assert
        Check(value is null);
    }

    /// <summary>
    /// Tests TypedGetReadsNullAsANullableValue.
    /// </summary>
    private static void TypedGetReadsNullAsANullableValue()
    {
        // Setup
        var context = Empty();
        context.Set("Nothing",
            null);

        // Act
        var value = context.Get<int?>("Nothing");

        // Assert
        Check(value is null);
    }

    /// <summary>
    /// Tests TypedGetRefusesAnotherType.
    /// </summary>
    private static void TypedGetRefusesAnotherType()
    {
        // Setup
        var context = WithTotal();

        // Act
        var error = Throws<InvalidCastException>(() => context.Get<string>("Total"));

        // Assert
        Check(error.Message == "The value 'Total' is a Int32, not a String.");
    }

    /// <summary>
    /// Tests TypedGetRefusesNullAsAValue.
    /// </summary>
    private static void TypedGetRefusesNullAsAValue()
    {
        // Setup
        var context = Empty();
        context.Set("Nothing",
            null);

        // Act/Assert
        Throws<InvalidCastException>(() => context.Get<int>("Nothing"));
    }

    /// <summary>
    /// Tests TypedGetUsesTheTextsOfTheHost.
    /// </summary>
    private static void TypedGetUsesTheTextsOfTheHost()
    {
        // Setup
        var context = WithTotal(new MarkedMessages());

        // Act
        var error = Throws<InvalidCastException>(() => context.Get<string>("Total"));

        // Assert
        Check(error.Message == "wrong type Total");
    }

    /// <summary>
    /// Tests TypedGetOfAnUnknownNameFails.
    /// </summary>
    private static void TypedGetOfAnUnknownNameFails()
    {
        // Setup
        var context = Empty();

        // Act/Assert
        Throws<InvalidOperationException>(() => context.Get<int>("Missing"));
    }

    /// <summary>
    /// Tests NamesStartEmpty.
    /// </summary>
    private static void NamesStartEmpty()
    {
        // Act
        var names = Empty().Names;

        // Assert
        Check(names.Count == 0);
    }

    /// <summary>
    /// Tests NamesHoldTheStoredValues.
    /// </summary>
    private static void NamesHoldTheStoredValues()
    {
        // Act
        var names = WithTotal().Names;

        // Assert
        Check(names
            .SequenceEqual(["Total"]));
    }

    /// <summary>
    /// Tests HasFindsAStoredValue.
    /// </summary>
    private static void HasFindsAStoredValue()
    {
        // Act
        var has = WithTotal()
            .Has("Total");

        // Assert
        Check(has);
    }

    /// <summary>
    /// Tests HasMissesAnUnknownValue.
    /// </summary>
    private static void HasMissesAnUnknownValue()
    {
        // Act
        var has = Empty()
            .Has("Total");

        // Assert
        Check(!has);
    }

    /// <summary>
    /// Tests RemoveTakesAValueAway.
    /// </summary>
    private static void RemoveTakesAValueAway()
    {
        // Setup
        var context = WithTotal();

        // Act
        var removed = context.Remove("Total");

        // Assert
        Check(removed && !context.Has("Total"));
    }

    /// <summary>
    /// Tests RemoveOfAnUnknownValueIsFalse.
    /// </summary>
    private static void RemoveOfAnUnknownValueIsFalse()
    {
        // Act
        var removed = Empty()
            .Remove("Total");

        // Assert
        Check(!removed);
    }

    /// <summary>
    /// Tests LoadRejectsNull.
    /// </summary>
    private static void LoadRejectsNull()
    {
        // Setup
        var context = Empty();

        // Act/Assert
        Throws<ArgumentNullException>(() => context.Load(null!));
    }

    /// <summary>
    /// Tests FromSqlResultGivesEveryTable.
    /// </summary>
    private static void FromSqlResultGivesEveryTable()
    {
        // Setup
        var context = Empty();

        // Act
        context.Load(SampleTables.Results("S",
            SampleTables.Customers(),
            SampleTables.Customers()));

        // Assert
        Check(context.FromSqlResult("S").Length == 2);
    }

    /// <summary>
    /// Tests FromSqlResultIgnoresCase.
    /// </summary>
    private static void FromSqlResultIgnoresCase()
    {
        // Setup
        var context = Loaded();

        // Act
        var tables = context.FromSqlResult("SECTION1");

        // Assert
        Check(tables.Length == 1);
    }

    /// <summary>
    /// Tests FromSqlResultGivesTheSameTableUntilTheNextLoad.
    /// </summary>
    private static void FromSqlResultGivesTheSameTableUntilTheNextLoad()
    {
        // Setup
        var context = Loaded();

        // Act
        var table = context.FromSqlResult("Section1")[0];

        // Assert
        Check(ReferenceEquals(table,
            context.FromSqlResult("Section1")[0]));
    }

    /// <summary>
    /// Tests FromSqlResultOfAnUnknownSectionSaysToRunIt.
    /// </summary>
    private static void FromSqlResultOfAnUnknownSectionSaysToRunIt()
    {
        // Setup
        var context = Empty();

        // Act
        var error = Throws<InvalidOperationException>(() => context.FromSqlResult("Missing"));

        // Assert
        Check(error.Message == NotebookScriptingMessages.Default.SqlResultMissing("Missing"));
    }

    /// <summary>
    /// Tests FromSqlResultUsesTheTextsOfTheHost.
    /// </summary>
    private static void FromSqlResultUsesTheTextsOfTheHost()
    {
        // Setup
        var context = Empty(new MarkedMessages());

        // Act
        var error = Throws<InvalidOperationException>(() => context.FromSqlResult("S"));

        // Assert
        Check(error.Message == "no result S");
    }

    /// <summary>
    /// Tests LoadNamesATableByItsTitle.
    /// </summary>
    private static void LoadNamesATableByItsTitle()
    {
        // Act
        var context = Loaded("Customers");

        // Assert
        Check(context.FromSqlResult("Section1")[0].TableName == "Customers");
    }

    /// <summary>
    /// Tests LoadLeavesATableWithoutATitleUnnamed.
    /// </summary>
    private static void LoadLeavesATableWithoutATitleUnnamed()
    {
        // Act
        var context = Loaded();

        // Assert
        Check(context.FromSqlResult("Section1")[0].TableName.Length == 0);
    }

    /// <summary>
    /// Tests LoadReplacesTheEarlierResults.
    /// </summary>
    private static void LoadReplacesTheEarlierResults()
    {
        // Setup
        var context = Loaded();

        // Act
        context.Load(SampleTables.Results("Other",
            SampleTables.Customers()));

        // Assert
        Throws<InvalidOperationException>(() => context.FromSqlResult("Section1"));
    }

    /// <summary>
    /// Tests LoadKeepsTheValues.
    /// </summary>
    private static void LoadKeepsTheValues()
    {
        // Setup
        var context = WithTotal();

        // Act
        context.Load(SampleTables.Results("Other"));

        // Assert
        Check(context.Get("Total") is 1);
    }

    /// <summary>
    /// Tests ClearRemovesTheValues.
    /// </summary>
    private static void ClearRemovesTheValues()
    {
        // Setup
        var context = WithTotal();

        // Act
        context.Clear();

        // Assert
        Check(context.Names.Count == 0);
    }

    /// <summary>
    /// Tests ClearRemovesTheSqlResults.
    /// </summary>
    private static void ClearRemovesTheSqlResults()
    {
        // Setup
        var context = Loaded();

        // Act
        context.Clear();

        // Assert
        Throws<InvalidOperationException>(() => context.FromSqlResult("Section1"));
    }

    #endregion
}
