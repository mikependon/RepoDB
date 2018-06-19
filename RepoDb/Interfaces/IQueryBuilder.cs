using RepoDb.Enumerations;
using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to make a query builder object used to compose a SQL Query Statement.
    /// </summary>
    /// <typeparam name="TEntity">An entity where the SQL Query Statement is bound to.</typeparam>
    public interface IQueryBuilder<TEntity>
        where TEntity : DataEntity
    {
        // Custom Methods

        /// <summary>
        /// Gets the string that corresponds to the composed SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        string GetString();

        /// <summary>
        /// Clears the current composed SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Clear();

        /// <summary>
        /// Append a space to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Space();

        /// <summary>
        /// Appends a new-line to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> NewLine();

        /// <summary>
        /// Writes a custom text to the SQL Query Statement.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> WriteText(string text);

        // Basic Methods

        /// <summary>
        /// Appends a word DELETE word to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Delete();

        /// <summary>
        /// Appends a character ";" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> End();

        /// <summary>
        /// Appends a word COUNT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Count();

        /// <summary>
        /// Appends a word COUNT_BIG to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> CountBig();

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Field(Field field);

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement by command.
        /// </summary>
        /// <param name="command">The mapped command where to get all the fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Fields(Command command);

        /// <summary>
        /// Append a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Fields(IEnumerable<Field> fields);

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement by command.
        /// </summary>
        /// <param name="command">The mapped command where to get all the fields and parameters to be stringified.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> FieldsAndParameters(Command command);

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> FieldsAndParameters(IEnumerable<Field> fields);

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement by command with aliases.
        /// </summary>
        /// <param name="command">The mapped command where to get all the fields and parameters to be stringified.</param>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> FieldsAndAliasFields(Command command, string alias);

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement by command with aliases.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> FieldsAndAliasFields(IEnumerable<Field> fields, string alias);

        /// <summary>
        /// Appends a word FROM to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> From();

        /// <summary>
        /// Appends a word GROUP BY and a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> GroupBy(IEnumerable<Field> fields);

        /// <summary>
        /// Appends a word HAVING COUNT and a conditional field to the SQL Query Statement.
        /// </summary>
        /// <param name="queryField">The conditional field object used for composition.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> HavingCount(QueryField queryField);

        /// <summary>
        /// Appends a word INSERT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Insert();

        /// <summary>
        /// Appends a word INTO to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Into();

        /// <summary>
        /// Appends a word VALUES to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Values();

        /// <summary>
        /// Appends a word ORDER BY and the stringified fields to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="orderBy">The list of order fields to be stringified.</param>
        /// <param name="alias">The aliases to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> OrderBy(IEnumerable<OrderField> orderBy = null, string alias = null);

        /// <summary>
        /// Appends a word AS to the SQL Query Statement with alias.
        /// </summary>
        /// <param name="alias">The alias to be prepended.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> As(string alias = null);

        /// <summary>
        /// Appends a word WITH to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> With();

        /// <summary>
        /// Appends a word SET to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Set();

        /// <summary>
        /// Appends a word JOIN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Join();

        /// <summary>
        /// Appends a stringified field as a joined qualifier to the SQL Query Statement with left and right aliases.
        /// </summary>
        /// <param name="field">The field to be stringified.</param>
        /// <param name="leftAlias">The left alias.</param>
        /// <param name="rightAlias">The right alias.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> JoinQualifiers(Field field, string leftAlias, string rightAlias);

        /// <summary>
        /// Appends a word MERGE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Merge();

        /// <summary>
        /// Appends the mapped entity name to the SQL Query Statement based on the mapped command.
        /// </summary>
        /// <param name="command">The command where the mapping is defined.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Table(Command command);

        /// <summary>
        /// Append the mapped properpties name to the SQL Query Statement based on the mapped command.
        /// </summary>
        /// <param name="command">The command where the mapping is defined.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Parameters(Command command);

        /// <summary>
        /// Append the stringified field parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Parameters(IEnumerable<Field> fields);

        /// <summary>
        /// Append the stringified parameter as fields to the SQL Query Statement based on the mapped command.
        /// </summary>
        /// <param name="command">The command where the mapping is defined.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> ParametersAsFields(Command command);

        /// <summary>
        /// Append the stringified parameter as fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> ParametersAsFields(IEnumerable<Field> fields);

        /// <summary>
        /// Appends a word SELECT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Select();

        /// <summary>
        /// Appends a word TOP and row number to the SQL Query Statement.
        /// </summary>
        /// <param name="rows">The row number to be appended.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Top(int? rows = 0);

        /// <summary>
        /// Appends a word UPDATE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Update();

        /// <summary>
        /// Appends a word USING to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Using();

        /// <summary>
        /// Appends a word WHERE and the stringified values of the Query Group to the SQL Query Statement.
        /// </summary>
        /// <param name="queryGroup">The query group to be stringified.</param>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Where(QueryGroup queryGroup);

        /// <summary>
        /// Appends a word ROW_NUMBER to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> RowNumber();

        /// <summary>
        /// Appends a word OVER to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Over();

        /// <summary>
        /// Appends a word AND to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> And();

        /// <summary>
        /// Appends a word OR to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Or();

        /// <summary>
        /// Appends a character "(" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> OpenParen();

        /// <summary>
        /// Appends a character ")" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> CloseParen();

        /// <summary>
        /// Appends a word ON to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> On();

        /// <summary>
        /// Appends a word IN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> In();

        /// <summary>
        /// Appends a word BETWEEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Between();

        /// <summary>
        /// Appends a word WHEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> When();

        /// <summary>
        /// Appends a word NOT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Not();

        /// <summary>
        /// Appends a word MATCHED to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Matched();

        /// <summary>
        /// Appends a word THEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Then();

        /// <summary>
        /// Appends a word CASE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        IQueryBuilder<TEntity> Case();
    }
}