namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the code section.
/// </summary>
internal static class NotebookCodeSectionTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        SectionStartsAsEmptySql();
        SectionIsCode();
        NullScriptIsEmpty();
        ScriptIsTheScriptToRun();
        LanguageIsTheScriptLanguage();
        UnknownLanguageIsRejected();
        LooseSectionCanChangeLanguage();
        SwitchingToSqlGivesAName();
        SwitchingToCSharpKeepsTheName();
        SettingTheSameLanguageKeepsNoChanges();
        ValidNameIsAccepted();
        InvalidNameIsRejected();
        DuplicateNameIsRejected();
        OwnNameCanChangeCase();
        CSharpSectionCanShareAName();
        LooseSectionAcceptsAnyValidName();
        NameIsContent();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a notebook with two SQL sections.
    /// </summary>
    /// <returns>The sections.</returns>
    private static (NotebookCodeSection First, NotebookCodeSection Second) TwoSections()
    {
        var document = new NotebookDocument();
        var first = new NotebookCodeSection();
        var second = new NotebookCodeSection();
        document.Sections.Add(first);
        document.Sections.Add(second);
        return (first, second);
    }

    /// <summary>
    /// Tests SectionStartsAsEmptySql.
    /// </summary>
    private static void SectionStartsAsEmptySql()
    {
        // Act
        var section = new NotebookCodeSection();

        // Assert
        Check(section.Language == NotebookCodeLanguage.Sql && section.Name.Length == 0 && section.Script.Length == 0);
    }

    /// <summary>
    /// Tests SectionIsCode.
    /// </summary>
    private static void SectionIsCode()
    {
        // Act
        var section = new NotebookCodeSection();

        // Assert
        Check(section.Type == NotebookSectionType.Code);
    }

    /// <summary>
    /// Tests NullScriptIsEmpty.
    /// </summary>
    private static void NullScriptIsEmpty()
    {
        // Act
        var section = new NotebookCodeSection { Script = null! };

        // Assert
        Check(section.Script.Length == 0);
    }

    /// <summary>
    /// Tests ScriptIsTheScriptToRun.
    /// </summary>
    private static void ScriptIsTheScriptToRun()
    {
        // Act
        var section = new NotebookCodeSection { Script = "SELECT 1" };

        // Assert
        Check(section.ScriptToRun == "SELECT 1" && section.Preview == "SELECT 1");
    }

    /// <summary>
    /// Tests LanguageIsTheScriptLanguage.
    /// </summary>
    private static void LanguageIsTheScriptLanguage()
    {
        // Act
        var section = new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp };

        // Assert
        Check(section.ScriptLanguage == NotebookCodeLanguage.CSharp);
    }

    /// <summary>
    /// Tests UnknownLanguageIsRejected.
    /// </summary>
    private static void UnknownLanguageIsRejected()
    {
        // Act
        Action operation = () => _ = new NotebookCodeSection { Language = (NotebookCodeLanguage)9 };

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests LooseSectionCanChangeLanguage.
    /// </summary>
    private static void LooseSectionCanChangeLanguage()
    {
        // Setup
        var section = new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp };

        // Act
        section.Language = NotebookCodeLanguage.Sql;

        // Assert
        Check(section.Name.Length == 0);
    }

    /// <summary>
    /// Tests SwitchingToSqlGivesAName.
    /// </summary>
    private static void SwitchingToSqlGivesAName()
    {
        // Setup
        var document = new NotebookDocument();
        var section = new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp };
        document.Sections.Add(section);

        // Act
        section.Language = NotebookCodeLanguage.Sql;

        // Assert
        Check(section.Name == "SQLSection1");
    }

    /// <summary>
    /// Tests SwitchingToCSharpKeepsTheName.
    /// </summary>
    private static void SwitchingToCSharpKeepsTheName()
    {
        // Setup
        var (first, _) = TwoSections();

        // Act
        first.Language = NotebookCodeLanguage.CSharp;

        // Assert
        Check(first.Name == "SQLSection1");
    }

    /// <summary>
    /// Tests SettingTheSameLanguageKeepsNoChanges.
    /// </summary>
    private static void SettingTheSameLanguageKeepsNoChanges()
    {
        // Setup
        var section = new NotebookCodeSection();

        // Act
        section.Language = NotebookCodeLanguage.Sql;

        // Assert
        Check(!section.HasChanges);
    }

    /// <summary>
    /// Tests ValidNameIsAccepted.
    /// </summary>
    private static void ValidNameIsAccepted()
    {
        // Setup
        var (first, _) = TwoSections();

        // Act
        var accepted = first.TryRename("Sales");

        // Assert
        Check(accepted && first.Name == "Sales");
    }

    /// <summary>
    /// Tests InvalidNameIsRejected.
    /// </summary>
    private static void InvalidNameIsRejected()
    {
        // Setup
        var (first, _) = TwoSections();

        // Act
        var accepted = first.TryRename("No spaces");

        // Assert
        Check(!accepted && first.Name == "SQLSection1");
    }

    /// <summary>
    /// Tests DuplicateNameIsRejected.
    /// </summary>
    private static void DuplicateNameIsRejected()
    {
        // Setup
        var (first, second) = TwoSections();

        // Act
        var accepted = second.TryRename("sqlsection1");

        // Assert
        Check(!accepted && second.Name == "SQLSection2" && first.Name == "SQLSection1");
    }

    /// <summary>
    /// Tests OwnNameCanChangeCase.
    /// </summary>
    private static void OwnNameCanChangeCase()
    {
        // Setup
        var (first, _) = TwoSections();

        // Act
        var accepted = first.TryRename("sqlsection1");

        // Assert
        Check(accepted);
    }

    /// <summary>
    /// Tests CSharpSectionCanShareAName.
    /// </summary>
    private static void CSharpSectionCanShareAName()
    {
        // Setup
        var (_, second) = TwoSections();
        second.Language = NotebookCodeLanguage.CSharp;

        // Act
        var accepted = second.TryRename("SQLSection1");

        // Assert
        Check(accepted);
    }

    /// <summary>
    /// Tests LooseSectionAcceptsAnyValidName.
    /// </summary>
    private static void LooseSectionAcceptsAnyValidName()
    {
        // Setup
        var section = new NotebookCodeSection();

        // Act
        var accepted = section.TryRename("Sales");

        // Assert
        Check(accepted);
    }

    /// <summary>
    /// Tests NameIsContent.
    /// </summary>
    private static void NameIsContent()
    {
        // Setup
        var section = new NotebookCodeSection();

        // Act
        section.TryRename("Sales");

        // Assert
        Check(section.HasChanges);
    }

    #endregion
}
