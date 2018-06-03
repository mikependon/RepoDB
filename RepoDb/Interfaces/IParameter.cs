namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a parameter.
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        object Value { get; }
    }
}
