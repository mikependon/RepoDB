## Coding Standards

Below are the list of things to be considered when doing a code change. Please be noted that this is not a strict compliance we you are pushing a pull-requests to us.

Some of the coding standards here is a preference of the author itself. We are listening to any comments you made, please do let us know if you think we need to adjust the way on how we do coding.

## Class Implementation

1. Always use `ProperCase` naming convention.

	- Like this:

	```csharp
	public class QueryField
	{
		...
	}
	```

	- Not like this:

	```csharp
	public class queryField
	{
		...
	}
	```

## Properties Implementation

1. Always use `ProperCase` naming convention.

	- Like this:

	```csharp
	public IEnumerable<QueryField> QueryFields { get; set; }
	```

	- Not like this:
	
	```csharp
	public IEnumerable<QueryField> queryFields { get; set; }
	```

2. Always use the *get/set* directly.

	- Like this:

	```csharp
	public string ConnectionString { get; set; }
	```

	- Not like this:
	
	```csharp
	private string m_propertyName;
	public string propertyName
	{
		get { return m_propertyName; }
		set { m_propertyName = value; }
	}
	```

3. For *get-only* properties, always consider the usage of direct assignment (first) before doing any other implementation.

	```charp
	public string ConnectionString => DbRepository.ConnectionString;
	```

## Variables

1. Use the `var` keyword when declaring the method-level variables.

	- Like this:

	```csharp
	var field = new QueryField("Name", "Value");
	```

	- Not like this:

	```csharp
	QueryField field = new QueryField("Name", "Value");
	```

2. Use `camelCase` when declaring the method-level variables.

	- Like this:

	```csharp
	var propertyIndex = 0;
	```

	- Not like this:

	```csharp
	var propertyindex = 0;
	var ProperyIndex = 0;
	```

3. Declare a meaningful variable name.

	- Like this:

	```csharp
	var propertiesCount = properties.Count();
	```

	- Not like this:

	```csharp
	var x = properties.Count();
	```

4. Usage of prefix `m_` for private variables.

	- Like this:

	```csharp
	private IDbConnection m_activeConnection;
	```

	- Not like this:

	```csharp
	private IDbConnection _activeConnection;
	```

## Looping

1. Always use `foreach` or `for (var i)`. Do not use *Linq ForEach*.

	- Like this:

	```csharp
	foreach(var queryField in queryFields)
	{
		...
	}
	```
	
	- Not like this:

	```csharp
	queryFields.ForEach(queryField =>
	{
		...
	});
	```

	**Reason**: The author preferred the lowest level implementation as always for performance purposes.

## Coding Styles

1. Always open and close the conditional statements with curly-brackets.

	- Like this:

	```csharp
	if (true)
	{
		Process();
	}
	```

	- Not Like this:

	```csharp
	if (true)
		Process();

	if (true) Process();
	```

2. Always add an XML-comments in all public implementations.

	- *Methods*
	- *Properties*
	- *Classes*
	- *Interfaces*
	- *Enumerations*

3. Always use the `String.Concat()` `+ Concatenation`.

	- Like this:

	```csharp
	var tableName = string.Concat("[dbo].[", entityName, "]");
	```

	- Not Like this:

	```csharp
	var tableName = "[dbo].[" + entityName + "]";
	```
	
	**Reason**: The author preferred the lowest level implementation as always for performance purposes.

3. Always use the `String.Concat()` or `String.Format()` over the *String Interpolation*.

	- Like this:

	```csharp
	var tableName = string.Concat("[dbo].[", entityName, "]");
	```

	- Not Like this:

	```csharp
	var tableName = $"[dbo].[{entityName}]");
	```
	
4. Avoid the usage of `this` and `base` keywords, unless very necesarry.

	- Like this:

	```csharp
	var entities = QueryAll<T>();
	```

	- Not Like this:

	```csharp
	var entities = this.QueryAll<T>();
	```

5. Always use the `AsList()` over `ToList()`.

	- Like this:

	```csharp
	var childQueryFields = queryGroup.QueryFields.AsList();
	```

	- Not Like this:

	```csharp
	var childQueryFields = queryGroup.QueryFields.ToList();
	```

6. The shorter the better (less then 25 lines of codes per method).

## Arguments

This is an author's preference. Always use a new-lined arguments.

- Like this:

	```charp
	internal static async Task<int> MergeAllAsyncInternalBase<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<TEntity> entities,
        IEnumerable<Field> qualifiers,
        int batchSize,
        IEnumerable<Field> fields,
        int? commandTimeout = null,
        IDbTransaction transaction = null,
        ITrace trace = null,
        IStatementBuilder statementBuilder = null,
        bool skipIdentityCheck = false)
        where TEntity : class
	{
		...
	}
	```
	
- Not like this:

	```charp
	internal static async Task<int> MergeAllAsyncInternalBase<TEntity>(this IDbConnection connection, string tableName, IEnumerable<TEntity> entities, IEnumerable<Field> qualifiers, int batchSize, IEnumerable<Field> fields, int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null, bool skipIdentityCheck = false) where TEntity : class
	{
		...
	}
	```

## Regions

The regions are rich in RepoDb.

1. Create a region for the *Properties*.
	
	- Like this:

	```csharp
	#region Properties

	public string ConnectionString => DbRepository.ConnectionString;

	#endregion
	```

2. Create a region for the *Static Properties*.

	- Like this:

	```csharp
	#region Static Properties

	public static IDbConnection ActiveConnection { get; private set; }

	#endregion
	```

3. Create a region for the *Private Variables*.

	- Like this:

	```csharp
	#region Privates

	public int? m_hashCode = null;

	#endregion
	```
	
4. Create a region for the *Static Private Variables*.

	- Like this:

	```csharp
	#region Statics/Privates

	public static IDbConnection m_activeConnection = null;

	#endregion
	```

5. Create a region for the *ConstructorsVariables*.

	- Like this:

	```csharp
	#region Constructors

	public QueryGroup(QueryField queryField) :
        this(queryField?.AsEnumerable(),
            null,
            Conjunction.And,
            false)
        { }

	public QueryGroup(QueryGroup queryGroup) :
        this(null,
            queryGroup?.AsEnumerable(),
            Conjunction.And,
            false)
        { }

	#endregion
	```

6. Create a region for the *Instance Methods*.

	```csharp
	#region Methods

	public void Fix()
	{
		...
	}

	#endregion
	```

7. Create a region for the *Static Methods*.

	```csharp
	#region Methods

	public static IEnumerable<Field> Parse<T>(T instance)
	{
		...
	}

	#endregion
	```

