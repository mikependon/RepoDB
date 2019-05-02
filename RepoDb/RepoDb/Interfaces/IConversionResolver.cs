namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interfaced used to mark a class to become a conversion resolver.
    /// </summary>
    /// <typeparamref name="TInput">The type of the input value.</typeparamref>
    /// <typeparamref name="TResult">The type of the result value.</typeparamref>
    public interface IResolver<TInput, TResult>
    {
        /// <summary>
        /// Resolves an input value to a target result type.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <returns>The resolved value.</returns>
        TResult Resolve(TInput input);
    }
}
