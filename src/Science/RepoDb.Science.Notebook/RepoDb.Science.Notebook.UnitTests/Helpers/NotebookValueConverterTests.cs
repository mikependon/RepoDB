using System.Globalization;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the value converter and the output trimmer.
/// </summary>
internal static class NotebookValueConverterTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunTypeTests();
        RunReadTests();
        RunWriteTests();
        RunTrimmerTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs type name scenarios.
    /// </summary>
    private static void RunTypeTests()
    {
        NumericTypesRoundTrip();
        OtherTypesRoundTrip();
        NullableTypeUsesItsValueType();
        MissingTypeIsAString();
        UnknownTypeIsAString();
        UnknownTypeNameIsAString();
    }

    /// <summary>
    /// Runs reading scenarios.
    /// </summary>
    private static void RunReadTests()
    {
        NullTextIsNull();
        ReadsNumbers();
        ReadsFractions();
        ReadsFlags();
        ReadsDates();
        ReadsGuidsAndSpans();
        ReadsBinary();
        KeepsUnreadableNumbers();
        KeepsUnreadableValues();
        KeepsTextOfUnknownTypes();
    }

    /// <summary>
    /// Runs writing scenarios.
    /// </summary>
    private static void RunWriteTests()
    {
        WritesNulls();
        WritesDatesInRoundTripForm();
        WritesSpansAndBinary();
        WritesNumbersInvariantly();
        WritesOtherValues();
    }

    /// <summary>
    /// Runs output trimming scenarios.
    /// </summary>
    private static void RunTrimmerTests()
    {
        TrimmerKeepsShortText();
        TrimmerKeepsEmptyText();
        TrimmerKeepsTheLastLines();
        TrimmerWritesASingularNote();
        TrimmerAddsToAnEarlierNote();
        TrimmerKeepsAtLeastOneLine();
        TrimmerIgnoresALookalikeNote();
        TrimmerIgnoresPartialNotes();
    }

    /// <summary>
    /// Reads a text and compares it with a value.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="typeName">The type name.</param>
    /// <param name="expected">The expected value.</param>
    /// <returns>Whether they match.</returns>
    private static bool Reads(string text,
        string typeName,
        object expected) => expected.Equals(NotebookValueConverter.FromText(text,
        typeName));

    /// <summary>
    /// Checks that types keep their names both ways.
    /// </summary>
    /// <param name="types">The types.</param>
    /// <returns>Whether they all round trip.</returns>
    private static bool RoundTrips(params Type[] types) => types
        .All(type => NotebookValueConverter.TypeOf(NotebookValueConverter.TypeNameOf(type)) == type);

    /// <summary>
    /// Tests NumericTypesRoundTrip.
    /// </summary>
    private static void NumericTypesRoundTrip()
    {
        // Act
        var result = RoundTrips(typeof(int),
            typeof(long),
            typeof(short),
            typeof(byte),
            typeof(decimal),
            typeof(double),
            typeof(float));

        // Assert
        Check(result);
    }

    /// <summary>
    /// Tests OtherTypesRoundTrip.
    /// </summary>
    private static void OtherTypesRoundTrip()
    {
        // Act
        var result = RoundTrips(typeof(bool),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(Guid),
            typeof(TimeSpan),
            typeof(byte[]),
            typeof(string));

        // Assert
        Check(result);
    }

    /// <summary>
    /// Tests NullableTypeUsesItsValueType.
    /// </summary>
    private static void NullableTypeUsesItsValueType()
    {
        // Act
        var name = NotebookValueConverter.TypeNameOf(typeof(int?));

        // Assert
        Check(name == "int");
    }

    /// <summary>
    /// Tests MissingTypeIsAString.
    /// </summary>
    private static void MissingTypeIsAString()
    {
        // Act
        var name = NotebookValueConverter.TypeNameOf(null);

        // Assert
        Check(name == "string");
    }

    /// <summary>
    /// Tests UnknownTypeIsAString.
    /// </summary>
    private static void UnknownTypeIsAString()
    {
        // Act
        var name = NotebookValueConverter.TypeNameOf(typeof(Uri));

        // Assert
        Check(name == "string");
    }

    /// <summary>
    /// Tests UnknownTypeNameIsAString.
    /// </summary>
    private static void UnknownTypeNameIsAString()
    {
        // Act
        var type = NotebookValueConverter.TypeOf("geometry");

        // Assert
        Check(type == typeof(string));
    }

    /// <summary>
    /// Tests NullTextIsNull.
    /// </summary>
    private static void NullTextIsNull()
    {
        // Act
        var value = NotebookValueConverter.FromText(null,
            "int");

        // Assert
        Check(value is null);
    }

    /// <summary>
    /// Tests ReadsNumbers.
    /// </summary>
    private static void ReadsNumbers()
    {
        // Assert
        Check(Reads("1",
            "int",
            1) && Reads("2",
                "long",
                2L));
        Check(Reads("3",
            "short",
            (short)3) && Reads("4",
                "byte",
                (byte)4));
    }

    /// <summary>
    /// Tests ReadsFractions.
    /// </summary>
    private static void ReadsFractions()
    {
        // Assert
        Check(Reads("1.5",
            "decimal",
            1.5m) && Reads("2.5",
                "double",
                2.5d) && Reads("3.5",
                    "float",
                    3.5f));
    }

    /// <summary>
    /// Tests ReadsFlags.
    /// </summary>
    private static void ReadsFlags()
    {
        // Assert
        Check(Reads("True",
            "bool",
            true));
    }

    /// <summary>
    /// Tests ReadsDates.
    /// </summary>
    private static void ReadsDates()
    {
        // Assert
        Check(Reads("1970-01-01T00:00:00.0000000Z",
            "datetime",
            DateTime.UnixEpoch));
        Check(Reads("1970-01-01T00:00:00.0000000+00:00",
            "datetimeoffset",
            DateTimeOffset.UnixEpoch));
    }

    /// <summary>
    /// Tests ReadsGuidsAndSpans.
    /// </summary>
    private static void ReadsGuidsAndSpans()
    {
        // Assert
        Check(Reads(Guid.Empty.ToString(),
            "guid",
            Guid.Empty));
        Check(Reads("00:00:05",
            "timespan",
            TimeSpan.FromSeconds(5)));
    }

    /// <summary>
    /// Tests ReadsBinary.
    /// </summary>
    private static void ReadsBinary()
    {
        // Act
        var value = NotebookValueConverter.FromText("AQI=",
            "binary");

        // Assert
        Check(value is byte[] { Length: 2 });
    }

    /// <summary>
    /// Tests KeepsUnreadableNumbers.
    /// </summary>
    private static void KeepsUnreadableNumbers()
    {
        // Assert
        Check(new[] { "int", "long", "short", "byte", "decimal", "double", "float", "bool" }
            .All(type => Reads("x",
                type,
                "x")));
    }

    /// <summary>
    /// Tests KeepsUnreadableValues.
    /// </summary>
    private static void KeepsUnreadableValues()
    {
        // Assert
        Check(new[] { "datetime", "datetimeoffset", "guid", "timespan", "binary" }
            .All(type => Reads("x",
                type,
                "x")));
    }

    /// <summary>
    /// Tests KeepsTextOfUnknownTypes.
    /// </summary>
    private static void KeepsTextOfUnknownTypes()
    {
        // Assert
        Check(Reads("x",
            "string",
            "x"));
    }

    /// <summary>
    /// Tests WritesNulls.
    /// </summary>
    private static void WritesNulls()
    {
        // Assert
        Check(NotebookValueConverter.ToText(null) is null && NotebookValueConverter.ToText(DBNull.Value) is null);
    }

    /// <summary>
    /// Tests WritesDatesInRoundTripForm.
    /// </summary>
    private static void WritesDatesInRoundTripForm()
    {
        // Assert
        Check(NotebookValueConverter.ToText(DateTime.UnixEpoch) == "1970-01-01T00:00:00.0000000Z");
        Check(NotebookValueConverter.ToText(DateTimeOffset.UnixEpoch) == "1970-01-01T00:00:00.0000000+00:00");
    }

    /// <summary>
    /// Tests WritesSpansAndBinary.
    /// </summary>
    private static void WritesSpansAndBinary()
    {
        // Assert
        Check(NotebookValueConverter.ToText(TimeSpan.FromSeconds(5)) == "00:00:05");
        Check(NotebookValueConverter.ToText(new byte[] { 1, 2 }) == "AQI=");
    }

    /// <summary>
    /// Tests WritesNumbersInvariantly.
    /// </summary>
    private static void WritesNumbersInvariantly()
    {
        // Setup
        var culture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");

        // Act
        var text = NotebookValueConverter.ToText(1.5m);
        CultureInfo.CurrentCulture = culture;

        // Assert
        Check(text == "1.5");
    }

    /// <summary>
    /// Tests WritesOtherValues.
    /// </summary>
    private static void WritesOtherValues()
    {
        // Assert
        Check(NotebookValueConverter.ToText(true) == "True");
    }

    /// <summary>
    /// Tests TrimmerKeepsShortText.
    /// </summary>
    private static void TrimmerKeepsShortText()
    {
        // Act
        var text = NotebookOutputTrimmer.LastLines("a\nb",
            2);

        // Assert
        Check(text == "a\nb");
    }

    /// <summary>
    /// Tests TrimmerKeepsEmptyText.
    /// </summary>
    private static void TrimmerKeepsEmptyText()
    {
        // Act
        var text = NotebookOutputTrimmer.LastLines("",
            2);

        // Assert
        Check(text.Length == 0);
    }

    /// <summary>
    /// Tests TrimmerKeepsTheLastLines.
    /// </summary>
    private static void TrimmerKeepsTheLastLines()
    {
        // Act
        var text = NotebookOutputTrimmer.LastLines("a\nb\nc\nd",
            2);

        // Assert
        Check(text == "... 2 earlier lines not shown" + Environment.NewLine + "c\nd");
    }

    /// <summary>
    /// Tests TrimmerWritesASingularNote.
    /// </summary>
    private static void TrimmerWritesASingularNote()
    {
        // Act
        var text = NotebookOutputTrimmer.LastLines("a\nb",
            1);

        // Assert
        Check(text.StartsWith("... 1 earlier line not shown",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests TrimmerAddsToAnEarlierNote.
    /// </summary>
    private static void TrimmerAddsToAnEarlierNote()
    {
        // Setup
        var trimmed = NotebookOutputTrimmer.LastLines("a\nb\nc",
            2);

        // Act
        var again = NotebookOutputTrimmer.LastLines(trimmed,
            1);

        // Assert
        Check(again == "... 2 earlier lines not shown" + Environment.NewLine + "c");
    }

    /// <summary>
    /// Tests TrimmerKeepsAtLeastOneLine.
    /// </summary>
    private static void TrimmerKeepsAtLeastOneLine()
    {
        // Act
        var text = NotebookOutputTrimmer.LastLines("a\nb",
            0);

        // Assert
        Check(text.EndsWith('b'));
    }

    /// <summary>
    /// Tests TrimmerIgnoresPartialNotes.
    /// </summary>
    private static void TrimmerIgnoresPartialNotes()
    {
        // Setup
        string[] lookalikes =
        [
            "and 2 earlier lines not shown",
            "... 2 later lines not shown",
            "... 2 earlier rows not shown",
            "... 2 earlier lines never shown",
            "... 2 earlier lines not printed",
            "... 2 earlier lines not shown here"
        ];

        // Act
        var results = lookalikes
            .Select(line => NotebookOutputTrimmer.LastLines(line + "\nb",
            1));

        // Assert
        Check(results
            .All(text => text.StartsWith("... 1 earlier line not shown",
                StringComparison.Ordinal)));
    }

    /// <summary>
    /// Tests TrimmerIgnoresALookalikeNote.
    /// </summary>
    private static void TrimmerIgnoresALookalikeNote()
    {
        // Act
        var text = NotebookOutputTrimmer.LastLines("... x earlier lines not shown\nb",
            1);

        // Assert
        Check(text.StartsWith("... 1 earlier line not shown",
            StringComparison.Ordinal));
    }

    #endregion
}
