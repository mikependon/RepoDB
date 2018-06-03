namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a field.
    /// </summary>
    public interface IField
    {
        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        string Name { get; }
    }
}
