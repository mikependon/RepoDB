using RepoDb.Options;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface that is used to mark a class to be a property handler.
    /// </summary>
    /// <typeparam name="TInput">Usually refers to the type of the database column. The input type for the getter; the output type for the setter.</typeparam>
    /// <typeparam name="TResult">Usually refers to the type of the data entity type property. The input type for the setter; the output type for the getter.</typeparam>
    public interface IPropertyHandler<TInput, TResult>
    {
        // TODO: Change to the proper type

        /// <summary>
        /// The method that is being invoked when the outbound transformation is triggered (i.e.: BatchQuery, ExecuteQuery and Query).
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <param name="options">The object that contains optional values in the current execution context.</param>
        /// <returns>An instance of the <typeparamref name="TInput"/> generic type.</returns>
        TResult Get(TInput input,
            PropertyHandlerOptions options);

        /// <summary>
        /// The method that is being invoked when the inbound transformation is triggered (i.e.: Insert, Merge and Update).
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <param name="options">The object that contains optional values in the current execution context.</param>
        /// <returns>An instance of the <typeparamref name="TInput"/> generic type.</returns>
        TInput Set(TResult input,
            PropertyHandlerOptions options);
    }
}
