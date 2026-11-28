using System.ComponentModel;

namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Records the names of changed properties.
/// </summary>
internal sealed class PropertyRecorder
{
    #region Constructors

    /// <summary>
    /// Starts recording.
    /// </summary>
    /// <param name="source">The watched object.</param>
    public PropertyRecorder(INotifyPropertyChanged source) => source.PropertyChanged += (_,
        e) => Names.Add(e.PropertyName!);

    #endregion

    #region Properties

    /// <summary>
    /// Gets the names, in order.
    /// </summary>
    public List<string> Names { get; } = [];

    #endregion
}
