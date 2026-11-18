namespace RepoDb.Science.UnitTests;

/// <summary>
/// Simulates a forward-only input stream.
/// </summary>
internal sealed class ForwardOnlyLoadStream : MemoryStream
{
    #region Constructors

    /// <summary>
    /// Creates a forward-only stream.
    /// </summary>
    /// <param name="buffer">The input bytes.</param>
    public ForwardOnlyLoadStream(byte[] buffer) : base(buffer)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates that the input cannot seek.
    /// </summary>
    public override bool CanSeek => false;

    /// <summary>
    /// Rejects access to the current position.
    /// </summary>
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Rejects seek attempts.
    /// </summary>
    /// <param name="offset">The requested offset.</param>
    /// <param name="loc">The starting point.</param>
    /// <returns>The requested position.</returns>
    public override long Seek(long offset,
        SeekOrigin loc) => throw new NotSupportedException();

    #endregion
}
