using RepoDb.Enumerations;
using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    internal interface IQueryBuilder<TEntity>
        where TEntity : IDataEntity
    {
        // Custom Methods
        string GetString();

        IQueryBuilder<TEntity> Trim();

        IQueryBuilder<TEntity> Space();

        IQueryBuilder<TEntity> NewLine();

        IQueryBuilder<TEntity> WriteText(string text);

        // Basic Methods

        // Select (SELECT)
        IQueryBuilder<TEntity> Select();

        // Update (UPDATE)
        IQueryBuilder<TEntity> Update();

        // Delete (DELETE)
        IQueryBuilder<TEntity> Delete();

        // Insert (INSERT)
        IQueryBuilder<TEntity> Insert();

        // Into (INTO)
        IQueryBuilder<TEntity> Into();

        // Values (VALUES)
        IQueryBuilder<TEntity> Values();

        // As (AS)
        IQueryBuilder<TEntity> As(string alias);

        // Set (SET)
        IQueryBuilder<TEntity> Set();

        // Merge (MERGE)
        IQueryBuilder<TEntity> Merge();

        // Top (TOP)
        IQueryBuilder<TEntity> Top(int value);

        // From (FROM)
        IQueryBuilder<TEntity> From();

        // From (JOIN)
        IQueryBuilder<TEntity> Join();

        // Name ([dbo].[Name])
        IQueryBuilder<TEntity> Table();

        // Using (USING)
        IQueryBuilder<TEntity> Using();

        // And (AND)
        IQueryBuilder<TEntity> And();

        // Or (OR)
        IQueryBuilder<TEntity> Or();

        // OpenParen (()
        IQueryBuilder<TEntity> OpenParen();

        // CloseParen ())
        IQueryBuilder<TEntity> CloseParen();

        // On (ON)
        IQueryBuilder<TEntity> On();

        // In
        IQueryBuilder<TEntity> In();

        // Between
        IQueryBuilder<TEntity> Between();

        // When
        IQueryBuilder<TEntity> When();

        // Not
        IQueryBuilder<TEntity> Not();

        // Matched
        IQueryBuilder<TEntity> Matched();

        // Then
        IQueryBuilder<TEntity> Then();

        // Where ([Field1] = @Field1)
        IQueryBuilder<TEntity> Where(IQueryGroup queryGroup);

        // OrderBy (ORDER BY)
        IQueryBuilder<TEntity> OrderBy(IEnumerable<IOrderField> fields);

        // GroupBy (GROUP BY)
        IQueryBuilder<TEntity> GroupBy(IEnumerable<Field> fields);

        // Having (HAVING COUNT(@Field1) > 0)
        IQueryBuilder<TEntity> HavingCount(IQueryField queryField);

        // Fields ([Id], [Name])
        IQueryBuilder<TEntity> Fields(Command command);

        // Parameters (@Id, @Name)
        IQueryBuilder<TEntity> Parameters(Command command);

        // Parameters (@Id AS [Id], @Name AS [Name])
        IQueryBuilder<TEntity> ParametersAsFields(Command command);

        // Parameters ([Id] = @Id, [Name] = @Name)
        IQueryBuilder<TEntity> FieldsAndParameters(Command command);

        // FieldsAndAliasFields ([Id] = T.[Id], [Name] = T.[Name])
        IQueryBuilder<TEntity> FieldsAndAliasFields(Command command, string alias);

        // JoinQualifiers (S.[Id] = T.[Id], S.[Name] = T.[Name])
        IQueryBuilder<TEntity> JoinQualifiers(string leftAlias, string rightAlias);

        // End (;)
        IQueryBuilder<TEntity> End();
    }
}