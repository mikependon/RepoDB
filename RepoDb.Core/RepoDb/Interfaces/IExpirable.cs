using System;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark the class to be expirable.
    /// </summary>
    public interface IExpirable
    {
        /// <summary>
        /// Gets the created timestamp of this class.
        /// </summary>
        DateTime CreatedDate { get; }

        /// <summary>
        /// Gets or sets the expiration date of this class.
        /// </summary>
        DateTime Expiration { get; set; }

        /// <summary>
        /// Identifies whether this class is expired.
        /// </summary>
        /// <returns>A boolean value that indicate whether this class is expired.</returns>
        bool IsExpired();
    }
}
