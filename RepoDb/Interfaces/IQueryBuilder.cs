using RepoDb.Enumerations;
using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    internal interface IQueryBuilder<TEntity>
        where TEntity : IDataEntity
    {
        // Custom Methods
        string GetString();

        // Trim

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
        IQueryBuilder<TEntity> As();

        // Set (SET)
        IQueryBuilder<TEntity> Set();

        // Merge (MERGE)
        IQueryBuilder<TEntity> Merge(string alias);

        // Top (TOP)
        IQueryBuilder<TEntity> Top(int value);

        // From (FROM)
        IQueryBuilder<TEntity> From();

        // From (JOIN)
        IQueryBuilder<TEntity> Join();

        // Name ([dbo].[Name])
        IQueryBuilder<TEntity> Table();

        // Using (USING)
        IQueryBuilder<TEntity> Using(string alias);

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

        // Where ([Field1] = @Field1)
        IQueryBuilder<TEntity> Where(IQueryGroup queryGroup);

        // OrderBy (ORDER BY)
        IQueryBuilder<TEntity> OrderBy(IEnumerable<IOrderField> fields);

        // GroupBy (GROUP BY)
        IQueryBuilder<TEntity> GroupBy(IEnumerable<Field> fields);

        // Having (HAVING COUNT(@Field1) > 0)
        IQueryBuilder<TEntity> HavingCount(IQueryField queryField);

        // Fields ([Id], [Name])
        IQueryBuilder<TEntity> Fields();

        // Parameters (@Id, @Name)
        IQueryBuilder<TEntity> Parameters();

        // Parameters (@Id AS [Id], @Name AS [Name])
        IQueryBuilder<TEntity> ParametersAsFields(Command command);

        // Parameters ([Id] = @Id, [Name] = @Name)
        IQueryBuilder<TEntity> FieldsAndParameters(Command command);

        // JoinQualifiers (S.[Id] = T.[Id], S.[Name] = T.[Name])
        IQueryBuilder<TEntity> JoinQualifiers(string leftAlias, string rightAlias);

        // End (;)
        IQueryBuilder<TEntity> End();
    }
}