namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the shared behavior of sections and the header section.
/// </summary>
internal static class NotebookSectionTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        NewSectionHasNoChanges();
        EditingMakesChanges();
        RevertingRemovesChanges();
        MarkSavedAcceptsTheContent();
        MarkSavedRaisesHasChanges();
        CollapsingIsNotAContentChange();
        CollapsingRaisesItsProperty();
        CollapsingAgainRaisesNothing();
        EditingRaisesContentProperties();
        SettingTheSameValueRaisesNothing();
        SectionWithoutListenersCanChange();
        HeaderStartsEmpty();
        HeaderIsAHeader();
        HeaderTurnsNullIntoEmpty();
        HeaderPreviewsItsTitle();
        HeaderPreviewsItsDescriptionWithoutATitle();
        HeaderDescriptionIsContent();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests NewSectionHasNoChanges.
    /// </summary>
    private static void NewSectionHasNoChanges()
    {
        // Act
        var section = new NotebookHeaderSection();

        // Assert
        Check(!section.HasChanges);
    }

    /// <summary>
    /// Tests EditingMakesChanges.
    /// </summary>
    private static void EditingMakesChanges()
    {
        // Setup
        var section = new NotebookHeaderSection();

        // Act
        section.Title = "A";

        // Assert
        Check(section.HasChanges);
    }

    /// <summary>
    /// Tests RevertingRemovesChanges.
    /// </summary>
    private static void RevertingRemovesChanges()
    {
        // Setup
        var section = new NotebookHeaderSection { Title = "A" };
        section.MarkSaved();
        section.Title = "B";

        // Act
        section.Title = "A";

        // Assert
        Check(!section.HasChanges);
    }

    /// <summary>
    /// Tests MarkSavedAcceptsTheContent.
    /// </summary>
    private static void MarkSavedAcceptsTheContent()
    {
        // Setup
        var section = new NotebookHeaderSection { Title = "A" };

        // Act
        section.MarkSaved();

        // Assert
        Check(!section.HasChanges);
    }

    /// <summary>
    /// Tests MarkSavedRaisesHasChanges.
    /// </summary>
    private static void MarkSavedRaisesHasChanges()
    {
        // Setup
        var section = new NotebookHeaderSection();
        var recorder = new PropertyRecorder(section);

        // Act
        section.MarkSaved();

        // Assert
        Check(recorder.Names
            .Single() == nameof(NotebookSection.HasChanges));
    }

    /// <summary>
    /// Tests CollapsingIsNotAContentChange.
    /// </summary>
    private static void CollapsingIsNotAContentChange()
    {
        // Setup
        var section = new NotebookHeaderSection();

        // Act
        section.IsCollapsed = true;

        // Assert
        Check(section.IsCollapsed && !section.HasChanges);
    }

    /// <summary>
    /// Tests CollapsingRaisesItsProperty.
    /// </summary>
    private static void CollapsingRaisesItsProperty()
    {
        // Setup
        var section = new NotebookHeaderSection();
        var recorder = new PropertyRecorder(section);

        // Act
        section.IsCollapsed = true;

        // Assert
        Check(recorder.Names
            .Single() == nameof(NotebookSection.IsCollapsed));
    }

    /// <summary>
    /// Tests CollapsingAgainRaisesNothing.
    /// </summary>
    private static void CollapsingAgainRaisesNothing()
    {
        // Setup
        var section = new NotebookHeaderSection { IsCollapsed = true };
        var recorder = new PropertyRecorder(section);

        // Act
        section.IsCollapsed = true;

        // Assert
        Check(recorder.Names.Count == 0);
    }

    /// <summary>
    /// Tests EditingRaisesContentProperties.
    /// </summary>
    private static void EditingRaisesContentProperties()
    {
        // Setup
        var section = new NotebookHeaderSection();
        var recorder = new PropertyRecorder(section);

        // Act
        section.Title = "A";

        // Assert
        Check(string.Join(',',
            recorder.Names) == "Title,Preview,HasChanges");
    }

    /// <summary>
    /// Tests SettingTheSameValueRaisesNothing.
    /// </summary>
    private static void SettingTheSameValueRaisesNothing()
    {
        // Setup
        var section = new NotebookHeaderSection { Title = "A" };
        var recorder = new PropertyRecorder(section);

        // Act
        section.Title = "A";

        // Assert
        Check(recorder.Names.Count == 0);
    }

    /// <summary>
    /// Tests SectionWithoutListenersCanChange.
    /// </summary>
    private static void SectionWithoutListenersCanChange()
    {
        // Setup
        var section = new NotebookHeaderSection();

        // Act
        section.IsCollapsed = true;

        // Assert
        Check(section.IsCollapsed);
    }

    /// <summary>
    /// Tests HeaderStartsEmpty.
    /// </summary>
    private static void HeaderStartsEmpty()
    {
        // Act
        var section = new NotebookHeaderSection();

        // Assert
        Check(section.Title.Length == 0 && section.Description.Length == 0 && section.Preview.Length == 0);
    }

    /// <summary>
    /// Tests HeaderIsAHeader.
    /// </summary>
    private static void HeaderIsAHeader()
    {
        // Act
        var section = new NotebookHeaderSection();

        // Assert
        Check(section.Type == NotebookSectionType.Header);
    }

    /// <summary>
    /// Tests HeaderTurnsNullIntoEmpty.
    /// </summary>
    private static void HeaderTurnsNullIntoEmpty()
    {
        // Act
        var section = new NotebookHeaderSection { Title = null!, Description = null! };

        // Assert
        Check(section.Title.Length == 0 && section.Description.Length == 0);
    }

    /// <summary>
    /// Tests HeaderPreviewsItsTitle.
    /// </summary>
    private static void HeaderPreviewsItsTitle()
    {
        // Act
        var section = new NotebookHeaderSection { Title = "Title", Description = "Text" };

        // Assert
        Check(section.Preview == "Title");
    }

    /// <summary>
    /// Tests HeaderPreviewsItsDescriptionWithoutATitle.
    /// </summary>
    private static void HeaderPreviewsItsDescriptionWithoutATitle()
    {
        // Act
        var section = new NotebookHeaderSection { Description = "Text" };

        // Assert
        Check(section.Preview == "Text");
    }

    /// <summary>
    /// Tests HeaderDescriptionIsContent.
    /// </summary>
    private static void HeaderDescriptionIsContent()
    {
        // Setup
        var section = new NotebookHeaderSection();

        // Act
        section.Description = "Text";

        // Assert
        Check(section.HasChanges);
    }

    #endregion
}
