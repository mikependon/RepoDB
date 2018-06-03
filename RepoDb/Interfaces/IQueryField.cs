using RepoDb.Enumerations;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a query field. The query field is useful when composing the query group tree expressions.
    /// </summary>
    public interface IQueryField
    {
        /// <summary>
        /// Gets the associated field object.
        /// </summary>
        IField Field { get; }

        /// <summary>
        /// Gets the operation used by this instance.
        /// </summary>
        Operation Operation { get; }

        /// <summary>
        /// Gets the associated parameter object.
        /// </summary>
        IParameter Parameter { get; }

        /// <summary>
        /// Gets the text value of <i>RepoDb.Attributes.TextAttribute</i> implemented at the <i>Operation</i> property value of this instance.
        /// </summary>
        /// <returns>A string instance containing the value of the <i>RepoDb.Attributes.TextAttribute</i> text property.</returns>
        string GetOperationText();
    }
}
