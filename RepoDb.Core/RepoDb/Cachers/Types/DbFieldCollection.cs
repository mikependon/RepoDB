using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb;

/// <summary>
/// A class the holds the collection of column definitions of the table.
/// </summary>
public class DbFieldCollection
{
    private readonly IDbSetting dbSetting;
    private readonly IReadOnlyList<DbField> dbFields;
    private readonly Lazy<IEnumerable<Field>> lazyFields;
    private readonly Lazy<DbField> lazyIdentity;
    private readonly Lazy<Dictionary<string, DbField>> lazyMapByName;
    private readonly Lazy<Dictionary<string, DbField>> lazyMapByUnquotedName;
    private readonly Lazy<DbField> lazyPrimary;

    /// <summary>
    /// Creates a new instance of <see cref="DbFieldCollection" /> object.
    /// </summary>
    /// <param name="dbFields">A collection of column definitions of the table.</param>
    /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
    public DbFieldCollection(IEnumerable<DbField> dbFields, IDbSetting dbSetting)
    {
        this.dbSetting = dbSetting;
        this.dbFields = dbFields.AsList();

        lazyPrimary = new Lazy<DbField>(GetPrimaryDbField);
        lazyIdentity = new Lazy<DbField>(GetIdentityDbField);
        lazyMapByName = new Lazy<Dictionary<string, DbField>>(GetDbFieldsMappedByName);
        lazyMapByUnquotedName = new Lazy<Dictionary<string, DbField>>(GetDbFieldsMappedByUnquotedName);
        lazyFields = new Lazy<IEnumerable<Field>>(GetDbFieldsAsFields);
    }
    
    /// <summary>
    /// Gets a value whether the current column definition is a primary column definition.
    /// </summary>
    /// <returns>A primary column definition.</returns>
    public DbField GetPrimary() => lazyPrimary.Value;

    /// <summary>
    /// Gets a value whether the current column definition is a identity column definition.
    /// </summary>
    /// <returns>A identity column definition.</returns>
    public DbField GetIdentity() => lazyIdentity.Value;

    /// <summary>
    /// Gets a column definitions of the table.
    /// </summary>
    /// <returns>A column definitions of the table.</returns>
    public IEnumerable<DbField> GetItems() => dbFields;

    /// <summary>
    /// Get the list of <see cref="DbField" /> objects converted into an <see cref="IReadOnlyList{T}" /> of <see cref="Field" /> objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Field> GetAsFields() => lazyFields.Value;

    /// <summary>
    /// Gets a value indicating whether the current column definitions of the table is empty.
    /// </summary>
    /// <returns>A value indicating whether the column definitions of the table is empty.</returns>
    public bool IsEmpty() => dbFields.Count == 0;

    /// <summary>
    /// Gets column definition of the table based on the name of the database field.
    /// </summary>
    /// <param name="name">The name of the mapping that is equivalent to the column definition of the table.</param>
    /// <returns>A column definition of table.</returns>
    public DbField GetByName(string name)
    {
        lazyMapByName.Value.TryGetValue(name, out var dbField);

        return dbField;
    }

    /// <summary>
    /// Gets column definition of the table based on the unquotes name of the database field.
    /// </summary>
    /// <param name="name">The name of the mapping that is equivalent to the column definition of the table.</param>
    /// <returns>A column definition of table.</returns>
    public DbField GetByUnquotedName(string name)
    {
        lazyMapByUnquotedName.Value.TryGetValue(name, out var dbField);

        return dbField;
    }

    private Dictionary<string, DbField> GetDbFieldsMappedByName() => 
        dbFields.ToDictionary(df => df.Name, df => df, StringComparer.OrdinalIgnoreCase);

    private Dictionary<string, DbField> GetDbFieldsMappedByUnquotedName() =>
        dbFields.ToDictionary(df => df.Name.AsUnquoted(true, dbSetting), df => df, StringComparer.OrdinalIgnoreCase);

    private DbField GetPrimaryDbField() => dbFields.FirstOrDefault(df => df.IsPrimary);

    private DbField GetIdentityDbField() => dbFields.FirstOrDefault(df => df.IsIdentity);

    private IEnumerable<Field> GetDbFieldsAsFields() => dbFields.AsFields();
}