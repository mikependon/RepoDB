using RepoDb.Enumerations;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a field for ordering.
    /// </summary>
    public interface IOrderField : IField
    {
        /// <summary>
        /// Gets the order direction of the field.
        /// </summary>
        Order Order { get; }

        /// <summary>
        /// Gets the value of the <i>RepoDb.Attributes.TextAttribute.Text</i> thas was implemented on the ordering direction.
        /// </summary>
        /// <returns>The string containing the text value of the ordering direction.</returns>
        string GetOrderText();
    }
}