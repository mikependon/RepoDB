using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Computes recursive exponentially weighted means from current rows.
/// </summary>
public sealed class DataTableEwm
{
    #region Private Variables

    private readonly double _alpha;
    private readonly string _column;
    private readonly int _minPeriods;
    private readonly DataTable _source;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a recursive exponential window.
    /// </summary>
    /// <param name="source">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="alpha">The smoothing factor greater than zero and at most one.</param>
    /// <param name="minPeriods">The positive minimum observation count.</param>
    internal DataTableEwm(DataTable source,
        string column,
        double alpha,
        int minPeriods)
    {
        WindowHelper.Validate(source,
            column,
            int.MaxValue,
            minPeriods);
        ArgumentOutOfRangeException.ThrowIfLessThan(minPeriods,
            1);
        if (!double.IsFinite(alpha) || alpha <= 0 || alpha > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(alpha));
        }
        _alpha = alpha;
        _column = column;
        _minPeriods = minPeriods;
        _source = source;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Computes recursive means, ignoring missing observations.
    /// </summary>
    /// <param name="outputColumn">The output name, or the input name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The weighted result table.</returns>
    public DataTable Mean(string? outputColumn = null,
        bool inPlace = false)
    {
        return WindowHelper.Exponential(_source,
            _column,
            _alpha,
            _minPeriods,
            outputColumn ?? _column,
            inPlace);
    }

    #endregion
}
