namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the rules of the section collection.
/// </summary>
internal static class NotebookSectionCollectionTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunRuleTests();
        RunNameTests();
        RunOwnershipTests();
        RunDataTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the rules of adding sections.
    /// </summary>
    private static void RunRuleTests()
    {
        ImportsAreDetected();
        SecondImportsSectionIsRejected();
        NullSectionIsRejected();
        SameSectionTwiceIsRejected();
        SectionOfAnotherNotebookIsRejected();
        ImportsCanBeReplaced();
        SectionCanReplaceItself();
        ReplacingWithSecondImportsIsRejected();
        RejectedSectionIsNotOwned();
    }

    /// <summary>
    /// Runs naming scenarios.
    /// </summary>
    private static void RunNameTests()
    {
        SqlSectionsAreNamed();
        UsableNameIsKept();
        DuplicateNameIsReplaced();
        CSharpSectionIsNotNamed();
        ReplacementIsNamed();
        LoadKeepsLaterValidNames();
        LoadRenamesDuplicates();
        LoadKeepsEveryImportsSection();
    }

    /// <summary>
    /// Runs ownership and change scenarios.
    /// </summary>
    private static void RunOwnershipTests()
    {
        RemovedSectionCanBeAddedElsewhere();
        ClearedSectionsAreReleased();
        ReplacedSectionIsReleased();
        MovedSectionKeepsItsPlace();
        EveryStructureChangeIsAnnounced();
        SectionEditsAreAnnounced();
        OtherSectionPropertiesAreNotAnnounced();
        RemovedSectionEditsAreNotAnnounced();
    }

    /// <summary>
    /// Runs data scenarios.
    /// </summary>
    private static void RunDataTests()
    {
        ImportsScriptIsEmptyWithoutImports();
        ImportsScriptComesFromTheSection();
        SqlResultsAreKeyedByName();
        SqlResultsLeaveOutOtherSections();
        LogLimitAppliesToSections();
        LogLimitAppliesToNewSections();
        LogLimitIsAtLeastOne();
    }

    /// <summary>
    /// Creates a notebook and records its announced changes.
    /// </summary>
    /// <returns>The notebook and the record.</returns>
    private static (NotebookDocument Document, PropertyRecorder Recorder) Watched()
    {
        var document = new NotebookDocument();
        return (document, new PropertyRecorder(document));
    }

    /// <summary>
    /// Creates a notebook with sections.
    /// </summary>
    /// <param name="sections">The sections.</param>
    /// <returns>The notebook.</returns>
    private static NotebookDocument With(params NotebookSection[] sections)
    {
        var document = new NotebookDocument();
        foreach (var section in sections)
        {
            document.Sections.Add(section);
        }
        return document;
    }

    /// <summary>
    /// Creates a SQL section with a name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The section.</returns>
    private static NotebookCodeSection Named(string name)
    {
        var section = new NotebookCodeSection();
        section.SetName(name);
        return section;
    }

    /// <summary>
    /// Tests ImportsAreDetected.
    /// </summary>
    private static void ImportsAreDetected()
    {
        // Setup
        var document = new NotebookDocument();
        var before = document.Sections.HasImports;

        // Act
        document.Sections.Add(new NotebookImportsSection());

        // Assert
        Check(!before && document.Sections.HasImports);
    }

    /// <summary>
    /// Tests SecondImportsSectionIsRejected.
    /// </summary>
    private static void SecondImportsSectionIsRejected()
    {
        // Setup
        var document = With(new NotebookImportsSection());

        // Act
        Action operation = () => document.Sections.Add(new NotebookImportsSection());

        // Assert
        Throws<InvalidOperationException>(operation);
    }

    /// <summary>
    /// Tests NullSectionIsRejected.
    /// </summary>
    private static void NullSectionIsRejected()
    {
        // Setup
        var document = new NotebookDocument();

        // Act
        Action operation = () => document.Sections.Add(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests SameSectionTwiceIsRejected.
    /// </summary>
    private static void SameSectionTwiceIsRejected()
    {
        // Setup
        var section = new NotebookHeaderSection();
        var document = With(section);

        // Act
        Action operation = () => document.Sections.Add(section);

        // Assert
        Throws<InvalidOperationException>(operation);
    }

    /// <summary>
    /// Tests SectionOfAnotherNotebookIsRejected.
    /// </summary>
    private static void SectionOfAnotherNotebookIsRejected()
    {
        // Setup
        var section = new NotebookHeaderSection();
        With(section);
        var other = new NotebookDocument();

        // Act
        Action operation = () => other.Sections.Add(section);

        // Assert
        Throws<InvalidOperationException>(operation);
    }

    /// <summary>
    /// Tests ImportsCanBeReplaced.
    /// </summary>
    private static void ImportsCanBeReplaced()
    {
        // Setup
        var document = With(new NotebookImportsSection());
        var replacement = new NotebookImportsSection();

        // Act
        document.Sections[0] = replacement;

        // Assert
        Check(document.Sections
            .Single() == replacement);
    }

    /// <summary>
    /// Tests SectionCanReplaceItself.
    /// </summary>
    private static void SectionCanReplaceItself()
    {
        // Setup
        var section = new NotebookCodeSection();
        var document = With(section);

        // Act
        document.Sections[0] = section;

        // Assert
        Check(section.Owner == document.Sections && section.Name == "SQLSection1");
    }

    /// <summary>
    /// Tests ReplacingWithSecondImportsIsRejected.
    /// </summary>
    private static void ReplacingWithSecondImportsIsRejected()
    {
        // Setup
        var document = With(new NotebookImportsSection(),
            new NotebookHeaderSection());

        // Act
        Action operation = () => document.Sections[1] = new NotebookImportsSection();

        // Assert
        Throws<InvalidOperationException>(operation);
    }

    /// <summary>
    /// Tests RejectedSectionIsNotOwned.
    /// </summary>
    private static void RejectedSectionIsNotOwned()
    {
        // Setup
        var document = With(new NotebookImportsSection());
        var rejected = new NotebookImportsSection();

        // Act
        Throws<InvalidOperationException>(() => document.Sections.Add(rejected));

        // Assert
        Check(rejected.Owner is null);
    }

    /// <summary>
    /// Tests SqlSectionsAreNamed.
    /// </summary>
    private static void SqlSectionsAreNamed()
    {
        // Setup
        var first = new NotebookCodeSection();
        var second = new NotebookCodeSection();

        // Act
        With(first,
            second);

        // Assert
        Check(first.Name == "SQLSection1" && second.Name == "SQLSection2");
    }

    /// <summary>
    /// Tests UsableNameIsKept.
    /// </summary>
    private static void UsableNameIsKept()
    {
        // Setup
        var section = Named("Sales");

        // Act
        With(section);

        // Assert
        Check(section.Name == "Sales");
    }

    /// <summary>
    /// Tests DuplicateNameIsReplaced.
    /// </summary>
    private static void DuplicateNameIsReplaced()
    {
        // Setup
        var second = Named("SALES");

        // Act
        With(Named("Sales"),
            second);

        // Assert
        Check(second.Name == "SQLSection1");
    }

    /// <summary>
    /// Tests CSharpSectionIsNotNamed.
    /// </summary>
    private static void CSharpSectionIsNotNamed()
    {
        // Setup
        var section = new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp };

        // Act
        With(section);

        // Assert
        Check(section.Name.Length == 0);
    }

    /// <summary>
    /// Tests ReplacementIsNamed.
    /// </summary>
    private static void ReplacementIsNamed()
    {
        // Setup
        var document = With(new NotebookCodeSection());
        var replacement = new NotebookCodeSection();

        // Act
        document.Sections[0] = replacement;

        // Assert
        Check(replacement.Name == "SQLSection1");
    }

    /// <summary>
    /// Tests LoadKeepsLaterValidNames.
    /// </summary>
    private static void LoadKeepsLaterValidNames()
    {
        // Setup
        var document = new NotebookDocument();
        var unnamed = new NotebookCodeSection();
        var named = Named("SQLSection1");

        // Act
        document.Sections.Load([unnamed, named]);

        // Assert
        Check(named.Name == "SQLSection1" && unnamed.Name == "SQLSection2");
    }

    /// <summary>
    /// Tests LoadRenamesDuplicates.
    /// </summary>
    private static void LoadRenamesDuplicates()
    {
        // Setup
        var document = new NotebookDocument();
        var first = Named("Sales");
        var second = Named("Sales");

        // Act
        document.Sections.Load([first, second]);

        // Assert
        Check(first.Name == "Sales" && second.Name == "SQLSection1");
    }

    /// <summary>
    /// Tests LoadKeepsEveryImportsSection.
    /// </summary>
    private static void LoadKeepsEveryImportsSection()
    {
        // Setup
        var document = new NotebookDocument();

        // Act
        document.Sections.Load([new NotebookImportsSection(), new NotebookImportsSection()]);

        // Assert
        Check(document.Sections.Count == 2);
    }

    /// <summary>
    /// Tests RemovedSectionCanBeAddedElsewhere.
    /// </summary>
    private static void RemovedSectionCanBeAddedElsewhere()
    {
        // Setup
        var section = new NotebookHeaderSection();
        var document = With(section);

        // Act
        document.Sections.Remove(section);
        var other = With(section);

        // Assert
        Check(section.Owner == other.Sections);
    }

    /// <summary>
    /// Tests ClearedSectionsAreReleased.
    /// </summary>
    private static void ClearedSectionsAreReleased()
    {
        // Setup
        var section = new NotebookHeaderSection();
        var document = With(section);

        // Act
        document.Sections.Clear();

        // Assert
        Check(section.Owner is null && document.Sections.Count == 0);
    }

    /// <summary>
    /// Tests ReplacedSectionIsReleased.
    /// </summary>
    private static void ReplacedSectionIsReleased()
    {
        // Setup
        var section = new NotebookHeaderSection();
        var document = With(section);

        // Act
        document.Sections[0] = new NotebookHeaderSection();

        // Assert
        Check(section.Owner is null);
    }

    /// <summary>
    /// Tests MovedSectionKeepsItsPlace.
    /// </summary>
    private static void MovedSectionKeepsItsPlace()
    {
        // Setup
        var first = new NotebookHeaderSection();
        var document = With(first,
            new NotebookHeaderSection());

        // Act
        document.Sections.Move(0,
            1);

        // Assert
        Check(document.Sections[1] == first && first.Owner == document.Sections);
    }

    /// <summary>
    /// Tests EveryStructureChangeIsAnnounced.
    /// </summary>
    private static void EveryStructureChangeIsAnnounced()
    {
        // Setup
        var (document, recorder) = Watched();

        // Act
        document.Sections.Add(new NotebookHeaderSection());
        document.Sections.Add(new NotebookHeaderSection());
        document.Sections.Move(0,
            1);
        document.Sections[0] = new NotebookHeaderSection();
        document.Sections.RemoveAt(0);
        document.Sections.Clear();

        // Assert
        Check(recorder.Names
            .Count(name => name == nameof(NotebookDocument.HasChanges)) == 6);
    }

    /// <summary>
    /// Tests SectionEditsAreAnnounced.
    /// </summary>
    private static void SectionEditsAreAnnounced()
    {
        // Setup
        var section = new NotebookHeaderSection();
        var document = With(section);
        var recorder = new PropertyRecorder(document);

        // Act
        section.Title = "A";

        // Assert
        Check(recorder.Names
            .Single() == nameof(NotebookDocument.HasChanges));
    }

    /// <summary>
    /// Tests OtherSectionPropertiesAreNotAnnounced.
    /// </summary>
    private static void OtherSectionPropertiesAreNotAnnounced()
    {
        // Setup
        var section = new NotebookHeaderSection();
        var document = With(section);
        var recorder = new PropertyRecorder(document);

        // Act
        section.IsCollapsed = true;

        // Assert
        Check(recorder.Names.Count == 0);
    }

    /// <summary>
    /// Tests RemovedSectionEditsAreNotAnnounced.
    /// </summary>
    private static void RemovedSectionEditsAreNotAnnounced()
    {
        // Setup
        var section = new NotebookHeaderSection();
        var document = With(section);
        document.Sections.Remove(section);
        var recorder = new PropertyRecorder(document);

        // Act
        section.Title = "A";

        // Assert
        Check(recorder.Names.Count == 0);
    }

    /// <summary>
    /// Tests ImportsScriptIsEmptyWithoutImports.
    /// </summary>
    private static void ImportsScriptIsEmptyWithoutImports()
    {
        // Act
        var document = new NotebookDocument();

        // Assert
        Check(document.Sections.ImportsScript.Length == 0);
    }

    /// <summary>
    /// Tests ImportsScriptComesFromTheSection.
    /// </summary>
    private static void ImportsScriptComesFromTheSection()
    {
        // Act
        var document = With(new NotebookImportsSection { Script = "using A;" });

        // Assert
        Check(document.Sections.ImportsScript == "using A;");
    }

    /// <summary>
    /// Tests SqlResultsAreKeyedByName.
    /// </summary>
    private static void SqlResultsAreKeyedByName()
    {
        // Setup
        var section = Named("Sales");
        section.RestoreResults("",
            [SampleTables.Categories()],
            "");

        // Act
        var results = With(section).Sections.GetSqlResults();

        // Assert
        Check(results["SALES"].Count == 1);
    }

    /// <summary>
    /// Tests SqlResultsLeaveOutOtherSections.
    /// </summary>
    private static void SqlResultsLeaveOutOtherSections()
    {
        // Setup
        var code = new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp };
        code.RestoreResults("",
            [SampleTables.Categories()],
            "");

        // Act
        var results = With(code,
            new NotebookCodeSection()).Sections.GetSqlResults();

        // Assert
        Check(results.Count == 0);
    }

    /// <summary>
    /// Tests LogLimitAppliesToSections.
    /// </summary>
    private static void LogLimitAppliesToSections()
    {
        // Setup
        var section = new NotebookCodeSection();
        var document = With(section,
            new NotebookHeaderSection());

        // Act
        document.LogLimit = 5;

        // Assert
        Check(section.Log.Limit == 5 && document.LogLimit == 5);
    }

    /// <summary>
    /// Tests LogLimitAppliesToNewSections.
    /// </summary>
    private static void LogLimitAppliesToNewSections()
    {
        // Setup
        var document = new NotebookDocument { LogLimit = 3 };
        var section = new NotebookCodeSection();

        // Act
        document.Sections.Add(section);

        // Assert
        Check(section.Log.Limit == 3);
    }

    /// <summary>
    /// Tests LogLimitIsAtLeastOne.
    /// </summary>
    private static void LogLimitIsAtLeastOne()
    {
        // Act
        var document = new NotebookDocument { LogLimit = -1 };

        // Assert
        Check(document.LogLimit == 1);
    }

    #endregion
}
