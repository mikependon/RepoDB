namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// A base class for all execution context.
    /// </summary>
    internal abstract class CommandExecutionContext
    {
        /// <summary>
        /// The execution command text.
        /// </summary>
        public string CommandText { get; set; }
    }
}
