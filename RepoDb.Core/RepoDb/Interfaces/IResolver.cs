namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interfaced that is used to mark a class to be a resolver.
    /// </summary>
    /// <typeparamref name="TResult">The type of the result value.</typeparamref>
    public interface IResolver<TResult>
    {
        /// <summary>
        /// Resolves an input value to a target result type.
        /// </summary>
        /// <returns>The resolved value.</returns>
        TResult Resolve();
    }

    /// <summary>
    /// An interfaced that is used to mark a class to be a resolver.
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

    /// <summary>
    /// An interfaced that is used to mark a class to be a resolver.
    /// </summary>
    /// <typeparamref name="TInput1">The type of the first input value.</typeparamref>
    /// <typeparamref name="TInput2">The type of the second input value.</typeparamref>
    /// <typeparamref name="TResult">The type of the result value.</typeparamref>
    public interface IResolver<TInput1, TInput2, TResult>
    {
        /// <summary>
        /// Resolves an input value to a target result type.
        /// </summary>
        /// <param name="input1">The first input value.</param>
        /// <param name="input2">The second input value.</param>
        /// <returns>The resolved value.</returns>
        TResult Resolve(TInput1 input1,
            TInput2 input2);
    }

    /// <summary>
    /// An interfaced that is used to mark a class to be a resolver.
    /// </summary>
    /// <typeparamref name="TInput1">The type of the first input value.</typeparamref>
    /// <typeparamref name="TInput2">The type of the second input value.</typeparamref>
    /// <typeparamref name="TInput3">The type of the third input value.</typeparamref>
    /// <typeparamref name="TResult">The type of the result value.</typeparamref>
    public interface IResolver<TInput1, TInput2, TInput3, TResult>
    {
        /// <summary>
        /// Resolves an input value to a target result type.
        /// </summary>
        /// <param name="input1">The first input value.</param>
        /// <param name="input2">The second input value.</param>
        /// <param name="input3">The third input value.</param>
        /// <returns>The resolved value.</returns>
        TResult Resolve(TInput1 input1,
            TInput2 input2,
            TInput3 input3);
    }

    /// <summary>
    /// An interfaced that is used to mark a class to be a resolver.
    /// </summary>
    /// <typeparamref name="TInput1">The type of the first input value.</typeparamref>
    /// <typeparamref name="TInput2">The type of the second input value.</typeparamref>
    /// <typeparamref name="TInput3">The type of the third input value.</typeparamref>
    /// <typeparamref name="TInput4">The type of the fourth input value.</typeparamref>
    /// <typeparamref name="TResult">The type of the result value.</typeparamref>
    public interface IResolver<TInput1, TInput2, TInput3, TInput4, TResult>
    {
        /// <summary>
        /// Resolves an input value to a target result type.
        /// </summary>
        /// <param name="input1">The first input value.</param>
        /// <param name="input2">The second input value.</param>
        /// <param name="input3">The third input value.</param>
        /// <param name="input4">The fourth input value.</param>
        /// <returns>The resolved value.</returns>
        TResult Resolve(TInput1 input1,
            TInput2 input2,
            TInput3 input3,
            TInput4 input4);
    }

    /// <summary>
    /// An interfaced that is used to mark a class to be a resolver.
    /// </summary>
    /// <typeparamref name="TInput1">The type of the first input value.</typeparamref>
    /// <typeparamref name="TInput2">The type of the second input value.</typeparamref>
    /// <typeparamref name="TInput3">The type of the third input value.</typeparamref>
    /// <typeparamref name="TInput4">The type of the fourth input value.</typeparamref>
    /// <typeparamref name="TInput5">The type of the firth input value.</typeparamref>
    /// <typeparamref name="TResult">The type of the result value.</typeparamref>
    public interface IResolver<TInput1, TInput2, TInput3, TInput4, TInput5, TResult>
    {
        /// <summary>
        /// Resolves an input value to a target result type.
        /// </summary>
        /// <param name="input1">The first input value.</param>
        /// <param name="input2">The second input value.</param>
        /// <param name="input3">The third input value.</param>
        /// <param name="input4">The fourth input value.</param>
        /// <param name="input5">The fifth input value.</param>
        /// <returns>The resolved value.</returns>
        TResult Resolve(TInput1 input1,
            TInput2 input2,
            TInput3 input3,
            TInput4 input4,
            TInput5 input5);
    }

    /// <summary>
    /// An interfaced that is used to mark a class to be a resolver.
    /// </summary>
    /// <typeparamref name="TInput1">The type of the first input value.</typeparamref>
    /// <typeparamref name="TInput2">The type of the second input value.</typeparamref>
    /// <typeparamref name="TInput3">The type of the third input value.</typeparamref>
    /// <typeparamref name="TInput4">The type of the fourth input value.</typeparamref>
    /// <typeparamref name="TInput5">The type of the firth input value.</typeparamref>
    /// <typeparamref name="TInput6">The type of the sixth input value.</typeparamref>
    /// <typeparamref name="TResult">The type of the result value.</typeparamref>
    public interface IResolver<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TResult>
    {
        /// <summary>
        /// Resolves an input value to a target result type.
        /// </summary>
        /// <param name="input1">The first input value.</param>
        /// <param name="input2">The second input value.</param>
        /// <param name="input3">The third input value.</param>
        /// <param name="input4">The fourth input value.</param>
        /// <param name="input5">The fifth input value.</param>
        /// <param name="input6">The sixth input value.</param>
        /// <returns>The resolved value.</returns>
        TResult Resolve(TInput1 input1,
            TInput2 input2,
            TInput3 input3,
            TInput4 input4,
            TInput5 input5,
            TInput6 input6);
    }

    /// <summary>
    /// An interfaced that is used to mark a class to be a resolver.
    /// </summary>
    /// <typeparamref name="TInput1">The type of the first input value.</typeparamref>
    /// <typeparamref name="TInput2">The type of the second input value.</typeparamref>
    /// <typeparamref name="TInput3">The type of the third input value.</typeparamref>
    /// <typeparamref name="TInput4">The type of the fourth input value.</typeparamref>
    /// <typeparamref name="TInput5">The type of the firth input value.</typeparamref>
    /// <typeparamref name="TInput6">The type of the sixth input value.</typeparamref>
    /// <typeparamref name="TInput7">The type of the sixth input value.</typeparamref>
    /// <typeparamref name="TResult">The type of the result value.</typeparamref>
    public interface IResolver<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TResult>
    {
        /// <summary>
        /// Resolves an input value to a target result type.
        /// </summary>
        /// <param name="input1">The first input value.</param>
        /// <param name="input2">The second input value.</param>
        /// <param name="input3">The third input value.</param>
        /// <param name="input4">The fourth input value.</param>
        /// <param name="input5">The fifth input value.</param>
        /// <param name="input6">The sixth input value.</param>
        /// <param name="input7">The seventh input value.</param>
        /// <returns>The resolved value.</returns>
        TResult Resolve(TInput1 input1,
            TInput2 input2,
            TInput3 input3,
            TInput4 input4,
            TInput5 input5,
            TInput6 input6,
            TInput6 input7);
    }
}
