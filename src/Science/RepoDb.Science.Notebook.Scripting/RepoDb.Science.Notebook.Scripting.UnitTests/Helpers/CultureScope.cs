using System.Globalization;

namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Uses a culture that writes a comma as the decimal separator until disposed.
/// </summary>
internal sealed class CultureScope : IDisposable
{
    #region Private Variables

    private readonly CultureInfo _previous;

    #endregion

    #region Constructors

    /// <summary>
    /// Changes the culture.
    /// </summary>
    public CultureScope()
    {
        _previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gives the culture back.
    /// </summary>
    public void Dispose() => CultureInfo.CurrentCulture = _previous;

    #endregion
}
