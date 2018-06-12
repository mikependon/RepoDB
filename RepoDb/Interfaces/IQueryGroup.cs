using RepoDb.Enumerations;
using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a query group. The query group is a widely-used object for 
    /// defining the groupings for the query expressions
    /// </summary>
    public interface IQueryGroup
    {
        /// <summary>
        /// Gets the conjunction used by this object..
        /// </summary>
        Conjunction Conjunction { get; }

        /// <summary>
        /// Gets the list of fields being grouped by this object.
        /// </summary>
        IEnumerable<IQueryField> QueryFields { get; }

        /// <summary>
        /// Gets the list of child query groups being grouped by this object.
        /// </summary>
        IEnumerable<IQueryGroup> QueryGroups { get; }

        /// <summary>
        /// Gets all the child query groups associated on the current instance.
        /// </summary>
        /// <returns>An enumerable list of child query groups.</returns>
        IEnumerable<IQueryField> GetAllQueryFields();

        /// <summary>
        /// Gets the text value of <i>RepoDb.Attributes.TextAttribute</i> implemented at the <i>Conjunction</i> property value of this instance.
        /// </summary>
        /// <returns>A string instance containing the value of the <i>RepoDb.Attributes.TextAttribute</i> text property.</returns>
        string GetConjunctionText();

        /// <summary>
        /// Gets the stringified format of the current instance. A formatted string for field-operation-parameter will be returned conjuncted by
        /// the value of the <i>Conjunction</i> property. Example, if the (Field=FirstName and the Operation=Like and the Conjunction=AND) then the
        /// following stringified string will be returned: (FirstName NOT LIKE @FirstName AND ....).
        /// </summary>
        /// <returns>A stringified formatted-text of the current instance.</returns>
        string GetString();
    }
}
