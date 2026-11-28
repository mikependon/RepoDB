namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the text and value helpers.
/// </summary>
internal static class NotebookTextTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        ComposeSeparatesParts();
        ComposeTellsNullFromEmpty();
        FirstLineSkipsBlankLines();
        FirstLineOfBlankTextIsEmpty();
        DefinedReturnsAKnownValue();
        DefinedRejectsAnUnknownValue();
        ElapsedReadsSeconds();
        ElapsedIgnoresNegativeSeconds();
        ElapsedIgnoresHugeSeconds();
        ElapsedIgnoresNotANumber();
        HeightKeepsAUsualHeight();
        HeightIsCapped();
        HeightIgnoresNegativeValues();
        HeightIgnoresInfinity();
        NameCanStartWithALetter();
        NameCanStartWithAnUnderscore();
        NameCannotStartWithADigit();
        NameCannotHaveSpaces();
        NameCannotBeEmpty();
        NameCannotBeNull();
        NextNameStartsAtOne();
        NextNameSkipsTakenNames();
        LimiterKeepsASmallTable();
        LimiterCutsALargeTable();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests ComposeSeparatesParts.
    /// </summary>
    private static void ComposeSeparatesParts()
    {
        // Act
        var first = NotebookText.Compose(["ab", "c"]);
        var second = NotebookText.Compose(["a", "bc"]);

        // Assert
        Check(first != second);
    }

    /// <summary>
    /// Tests ComposeTellsNullFromEmpty.
    /// </summary>
    private static void ComposeTellsNullFromEmpty()
    {
        // Act
        var missing = NotebookText.Compose([null]);
        var empty = NotebookText.Compose([""]);

        // Assert
        Check(missing != empty);
    }

    /// <summary>
    /// Tests FirstLineSkipsBlankLines.
    /// </summary>
    private static void FirstLineSkipsBlankLines()
    {
        // Act
        var line = NotebookText.FirstLine("\r\n   \n  SELECT 1  \nFROM t");

        // Assert
        Check(line == "SELECT 1");
    }

    /// <summary>
    /// Tests FirstLineOfBlankTextIsEmpty.
    /// </summary>
    private static void FirstLineOfBlankTextIsEmpty()
    {
        // Act
        var line = NotebookText.FirstLine(" \n ");

        // Assert
        Check(line.Length == 0);
    }

    /// <summary>
    /// Tests DefinedReturnsAKnownValue.
    /// </summary>
    private static void DefinedReturnsAKnownValue()
    {
        // Act
        var value = NotebookValues.Defined(NotebookCodeLanguage.CSharp,
            "value");

        // Assert
        Check(value == NotebookCodeLanguage.CSharp);
    }

    /// <summary>
    /// Tests DefinedRejectsAnUnknownValue.
    /// </summary>
    private static void DefinedRejectsAnUnknownValue()
    {
        // Act
        Action operation = () => NotebookValues.Defined((NotebookCodeLanguage)9,
            "value");

        // Assert
        Check(Throws<ArgumentOutOfRangeException>(operation).ParamName == "value");
    }

    /// <summary>
    /// Tests ElapsedReadsSeconds.
    /// </summary>
    private static void ElapsedReadsSeconds()
    {
        // Act
        var elapsed = NotebookValues.Elapsed(1.5);

        // Assert
        Check(elapsed == TimeSpan.FromSeconds(1.5));
    }

    /// <summary>
    /// Tests ElapsedIgnoresNegativeSeconds.
    /// </summary>
    private static void ElapsedIgnoresNegativeSeconds()
    {
        // Act
        var elapsed = NotebookValues.Elapsed(-1);

        // Assert
        Check(elapsed == TimeSpan.Zero);
    }

    /// <summary>
    /// Tests ElapsedIgnoresHugeSeconds.
    /// </summary>
    private static void ElapsedIgnoresHugeSeconds()
    {
        // Act
        var elapsed = NotebookValues.Elapsed(double.MaxValue);

        // Assert
        Check(elapsed == TimeSpan.Zero);
    }

    /// <summary>
    /// Tests ElapsedIgnoresNotANumber.
    /// </summary>
    private static void ElapsedIgnoresNotANumber()
    {
        // Act
        var elapsed = NotebookValues.Elapsed(double.NaN);

        // Assert
        Check(elapsed == TimeSpan.Zero);
    }

    /// <summary>
    /// Tests HeightKeepsAUsualHeight.
    /// </summary>
    private static void HeightKeepsAUsualHeight()
    {
        // Act
        var height = NotebookValues.Height(120);

        // Assert
        Check(height == 120);
    }

    /// <summary>
    /// Tests HeightIsCapped.
    /// </summary>
    private static void HeightIsCapped()
    {
        // Act
        var height = NotebookValues.Height(9000);

        // Assert
        Check(height == NotebookValues.MaxHeight);
    }

    /// <summary>
    /// Tests HeightIgnoresNegativeValues.
    /// </summary>
    private static void HeightIgnoresNegativeValues()
    {
        // Act
        var height = NotebookValues.Height(-5);

        // Assert
        Check(height == 0);
    }

    /// <summary>
    /// Tests HeightIgnoresInfinity.
    /// </summary>
    private static void HeightIgnoresInfinity()
    {
        // Act
        var height = NotebookValues.Height(double.PositiveInfinity);

        // Assert
        Check(height == 0);
    }

    /// <summary>
    /// Tests NameCanStartWithALetter.
    /// </summary>
    private static void NameCanStartWithALetter()
    {
        // Act
        var valid = NotebookSectionNames.IsValid("Sales_2");

        // Assert
        Check(valid);
    }

    /// <summary>
    /// Tests NameCanStartWithAnUnderscore.
    /// </summary>
    private static void NameCanStartWithAnUnderscore()
    {
        // Act
        var valid = NotebookSectionNames.IsValid("_sales");

        // Assert
        Check(valid);
    }

    /// <summary>
    /// Tests NameCannotStartWithADigit.
    /// </summary>
    private static void NameCannotStartWithADigit()
    {
        // Act
        var valid = NotebookSectionNames.IsValid("2Sales");

        // Assert
        Check(!valid);
    }

    /// <summary>
    /// Tests NameCannotHaveSpaces.
    /// </summary>
    private static void NameCannotHaveSpaces()
    {
        // Act
        var valid = NotebookSectionNames.IsValid("My Sales");

        // Assert
        Check(!valid);
    }

    /// <summary>
    /// Tests NameCannotBeEmpty.
    /// </summary>
    private static void NameCannotBeEmpty()
    {
        // Act
        var valid = NotebookSectionNames.IsValid("");

        // Assert
        Check(!valid);
    }

    /// <summary>
    /// Tests NameCannotBeNull.
    /// </summary>
    private static void NameCannotBeNull()
    {
        // Act
        var valid = NotebookSectionNames.IsValid(null);

        // Assert
        Check(!valid);
    }

    /// <summary>
    /// Tests NextNameStartsAtOne.
    /// </summary>
    private static void NextNameStartsAtOne()
    {
        // Act
        var name = NotebookSectionNames.Next(new HashSet<string>(StringComparer.OrdinalIgnoreCase));

        // Assert
        Check(name == "SQLSection1");
    }

    /// <summary>
    /// Tests NextNameSkipsTakenNames.
    /// </summary>
    private static void NextNameSkipsTakenNames()
    {
        // Setup
        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "sqlsection1", "SQLSection2" };

        // Act
        var name = NotebookSectionNames.Next(taken);

        // Assert
        Check(name == "SQLSection3");
    }

    /// <summary>
    /// Tests LimiterKeepsASmallTable.
    /// </summary>
    private static void LimiterKeepsASmallTable()
    {
        // Setup
        var table = SampleTables.Categories();

        // Act
        var limited = NotebookTableLimiter.Limit([table],
            2);

        // Assert
        Check(ReferenceEquals(limited[0],
            table));
    }

    /// <summary>
    /// Tests LimiterCutsALargeTable.
    /// </summary>
    private static void LimiterCutsALargeTable()
    {
        // Setup
        var table = new NotebookTable([new("Id")],
            [["1"], ["2"]],
            false,
            "T");

        // Act
        var limited = NotebookTableLimiter.Limit([table],
            1)[0];

        // Assert
        Check(limited.Rows.Count == 1 && limited.IsTruncated && limited.Title == "T");
    }

    #endregion
}
