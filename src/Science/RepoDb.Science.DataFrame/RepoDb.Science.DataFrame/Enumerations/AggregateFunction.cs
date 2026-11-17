namespace RepoDb.Science;

/// <summary>
/// Defines supported summary functions.
/// </summary>
public enum AggregateFunction
{
    /// <summary>
    /// Adds present numeric values.
    /// </summary>
    Sum,
    /// <summary>
    /// Averages present numeric values.
    /// </summary>
    Mean,
    /// <summary>
    /// Finds the minimum numeric value.
    /// </summary>
    Min,
    /// <summary>
    /// Finds the maximum numeric value.
    /// </summary>
    Max,
    /// <summary>
    /// Counts present values.
    /// </summary>
    Count
}
