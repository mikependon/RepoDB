# RepoDb
A dynamic ORM .Net Library used to create an entity-based repository classes when accessing data	 from the database.

## Class Entity

It is required that you have your entity classes inherited the `RepoDb.DataEntity` class. It is also advisable that you must create an explicit interface that implements the `IDataEntity` interface. See example below.

Entity Interface:
```
public interface IStock : IDataEntity

{
	int Id { get; set; }
	string Name { get; set; }
	...
	DateTime CreatedDate { get; set; }
}
```
Entity Class:
```
public class Stock : DataEntity, IStock
{
	public int Id { get; set; }
	public string Name { get; set; }
	...
	public DateTime CreatedDate { get; set; }
}
```
By default, `RepoDb` is using the class name as the default mapped object in the database. This means that your class `Stock` above is automatically mapped to `[dbo].[Stock]` object from your database.

### Map  Attribute

You can change the target mapped object by specifying the `Map` attribute on the class level. See sample below.
```
[Map("[dbo].[StockTable]", CommandType.Text)]
public class Stock : DataEntity, IStock
```
Above class `Stock` is being mapped at `[dbo].[StockTable]` of your database. This attribute has second parameter called `commandType` of `System.Data` namespace.

If you specify the `CommandType` parameter, the library will then use the class object to be executed under that command type. See Microsoft documentation [here](https://msdn.microsoft.com/en-us/library/system.data.commandtype%28v=vs.110%29.aspx). 

### Primary Attribute

The `Primary` attribute is necessary in order for the `RepoDb` to identity which property of you class signify as the primary column of your mapped object. See sample below.
```
[Primary]
public int Id { get; set; }
```
This attribute accepts a `System.Boolean`  parameter that tells whether the primary property is an identity column or not. If sets to `true`, the value of the `Id` column of the newly added record from the database will be returned after calling an `Insert` operation.

By default, `RepoDb` has a built-in mechanism when identifying the primary key. If the `Primary` attribute is defined, the first occurrence will automatically be qualified as the primary property.

If the `Primary` attribute is not defined, then `RepoDb` will do the following identification.

  - Search for `Id` property. If present, this automatically overrule the mechanism.
  - If there is no `Id` property from the class, the class name plus the word `Id` will be evaluated. This means that on your `Stock` class, the property `StockId` will be identified.
  - If both properties above `Id` and `StockId` is not defined, then the mechanism will evaluate the `Map` attribute mapped object plus the `Id` word. This means that on your `Map("[dbo].[StockTable]` mapping, the `StockTableId` will be evalulated.

### Ignore Attribute

This attribute is necessary to command `RepoDb` which properties of your entity class is being ignored in certain operations. It accepts an argument `RepoDb.Enumerations.Command` enumeration. The following are the values of the `Command` operation.

 - None
 - Select
 - Insert
 - Update
 - Delete
 - Create
 - Drop
 - Alter
 - Execute

Below is a way on how to marked the property `Id` to be ignored during `Update` operation.
```
[Ignore(Command.Update)]
public int Id { get; set; }
```
On the other hand, you can also cast multiple ignore commands on a single property. Below is the sample on how to marked the `CreatedDate` property to be ignored during `Insert` and `Update` operation. *(This is applicable to a property with default values from the database)*
```
[Ignore(Command.Insert | Command.Update)]
public int CreatedDate { get; set; }
```
Currently, the library is only using the `None`, `Select`, `Insert`, `Update`, `Delete` commands on its certain operations.

## Repository

The library contains two base repository objects, the `BaseRepository<TEntity, TDbConnection>` and `DbRepository<TDbConnection>`. The latter is the heart of the `RepoDb` as it contains all the operations that is being used by all other repositories within or outside the library.

This mean that the `BaseRepository` is only abstracting the operations of the `DbRepository` object in all areas.

Both classes accept the following parameters on there respective constructors.

 - **connectionString** - the connection string to connect to.
 - **commandTimeout (optional)** - the command timeout in seconds is being used by the repository to the value of the `DbCommand.CommandTimeout` everytime there is an operation.
 - **cache (optional)** - the cache object to be used by the repository. By default, the repository is using the `RepoDb.MemoryCache` object.
 - **trace (optional)** - the trace object to be used by the repository. The default is `null`.
 - **statementBuilder (optional)** - the statement builder object to be used by the repository. By default, the repository is using the `RepoDb.SqlDbStatementBuilder` object.

### Creating a Repository

If you are creating an entity-based repository, then you can inherit from `BaseRepository<TEntity, TDbConnection>` repository. See sample below.
```
public StockRepository : BaseRepository<Stock, SqlConnection>
{
	public StockRepository(string connectionString)
		: base(connectionString)
}
```
The class above named `StockRepository` is an entity based repository for an entity `Stock`. It also uses `SqlConnection`  as the database connection object. You can pass any type of database connection here if you are accessing different database like Oracle, PostgreSQL, OleDb databases and others.

On the other hand, if you are creating a shared repository, then you can inherit from `DbRepository<TDbConnection>` repository. See sample below.
```
public SharedRepository : DbRepository<SqlConnection>
{
	public SharedRepository(string connectionString)
		: base(connectionString)
}
```
Since there is no entity type defined on the `SharedRepository` above, then all operations of that repository can be used anywhere in the solution by any entities.

We also recommend that you should create your own contract interface when creating a repository. With interfaces defined on your repository, you can easily inject it anywhere in your solution if you are using any Dependency Injection library.

To create an interface for repository, you  should implement the `IBaseRepository<TEntity, TDbConnection>` or `IDbRepository<TDbConnection>` interface on your contract interface. The said interfaces can be found at `RepoDb.Interfaces` namespace. See sample below.
```
public interface IStockRepository : IBaseRepository<TEntity, TDbConnection>
{
	...
}

public interface ISharedRepository : IDbRepository<TDbConnection>
{
	...
}
```
and implement it on your custom repositories as shown below.
```
public StockRepository : BaseRepository<Stock, SqlConnection>, IStockRepository
{
	public StockRepository(string connectionString)
		: base(connectionString)
}

public SharedRepository : DbRepository<SqlConnection>, ISharedRepository
{
	public SharedRepository(string connectionString)
		: base(connectionString)
}
```

## Creating a Connection

The repository object is used to create a connection object  for the caller to be able to used to manually manipulate the data.

 - **CreateConnection** - returns a connection object.

Below is the way on how to create a connection.
```
var stockRepository = new StockRepository(connectionString);
var connection = stockRepository.CreateConnection();
```
The library has created certain extension methods on the connection object. Below are the list of extension methods.

 - **EnsureOpen** - used to ensure that the connection is open. Returns the instance of the connection object.
 - **ExecuteReader** - used to read certain records from the database in fast-forward access.
 - **ExecuteNonQuery** - used to execute a non-queryable query statement in the database.
 - **ExecuteScalar** - used to execute a command that returns a single-object value from the database. 

### EnsureOpen

This operation is used to ensure that the current connection object is open. The underlying call of the method is the `IDbConnection.Open` method. It returns a connection object instance (self instance).

Below is the way on how to use the operation.
```
var stockRepository = new StockRepository(connectionString);
using (var connection = stockRepository.CreateConnection().EnsureOpen())
{
	...
}
```

### ExecuteReader

This connection extension method is very important if you wish to execute a query from the database in fast-forward access. It returns an `IEnumerable` object with `dynamic` or `object` type as its generic type.

Below are the parameters:

 - **commandText** - the SQL statement to be used for execution.
 - **param** - the parameters to be used for the execution. It could be an entity class or a dynamic object.
 - **commandTimeout** - the command timeout in seconds to be used when executing the query in the database.
 - **commandType** - the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
 - **transaction** - the transaction object be used when executing the command.
 - **trace** - the trace object to be used on this operation.

Below is the way on how to call the operation.
```
var stockRepository = new StockRepository(connectionString);
using (var connection = stockRepository.CreateConnection().EnsureOpen())
{
	var param = new { Name = "GOOGL" };
	var result = connection.ExecuteReader<Stock>("SELECT * FROM [dbo].[Stock] WHERE Name = @Name;", param);
}
```

### ExecuteNonQuery

This connection extension method is used to execute a non-queryable query statement. It returns an `int` that holds the number of affected rows during the execution.

Below are the parameters:

 - **commandText** - the SQL statement to be used for execution.
 - **param** - the parameters to be used for the execution.
 - **commandTimeout** - the command timeout in seconds to be used when executing the query in the database.
 - **commandType** - the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
 - **transaction** - the transaction object be used when executing the command.
 - **trace** - the trace object to be used on this operation.

Below is the way on how to call the operation.
```
var stockRepository = new StockRepository(connectionString);
using (var connection = stockRepository.CreateConnection().EnsureOpen())
{
	var param = new
	{
		Name = "GOOGL",
		Motto = "Do not be evil.",
		UpdatedDate = DateTime.UtcNow
	};
	var result = connection.ExecuteNonQuery("UPDATE [dbo].[Stock] SET Motto = @Motto, UpdatedDate = @UpdatedDate WHERE Name = @Name;", param);
}
```

### ExecuteScalar

This connection extension method is used to execute a query statement that returns single value.

Below are the parameters:

 - **commandText** - the SQL statement to be used for execution.
 - **param** - the parameters to be used for the execution.
 - **commandTimeout** - the command timeout in seconds to be used when executing the query in the database.
 - **commandType** - the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
 - **transaction** - the transaction object be used when executing the command.
 - **trace** - the trace object to be used on this operation.

Below is the way on how to call the operation.
```
var stockRepository = new StockRepository(connectionString);
using (var connection = stockRepository.CreateConnection().EnsureOpen())
{
	var param = new { Name = "GOOGL" };
	var id = connection.ExecuteScalar("SELECT [Id] FROM [dbo].[Stock] Name = @Name;", param);
}
```

## Expression Tree

The expression is tree is very important for you to be able to maximize the usage of the library. Expression tree defines the best way possible of doing an expression by composing it via dynamic objects or static object called `IQueryGroup`.

Certain operations uses expression tree to compose the SQL Statement on the fly prior the execution back to the database.

Below are the objects useful for composing the expression tree.

 - **QueryGroup** - used to group an expression.
 - **QueryField** - holds the field/value pair values of the expressions.
 - **Conjunction** - an enumeration that holds the value whether the expression is on `And` or `Or` operation.
 - **Operation** - an enumeration that holds the value what kind of operation is going to be executed on certain expression. It holds the value of like `Equal`, `NotEqual`, `Between`, `GreaterThan` and etc.

TODO: To be continued by Michael Pendon

## Operations

The repository consist of different operations to manipulate the data from your database. Below are the list of the common operations widely used.

 - **Query** - used to query a record from the database. It uses the `SELECT` command of SQL.
 - **Insert** - used to insert a record in the database. It uses the `INSERT` command of SQL.
 - **Update** - used to update a record in the database. It uses the `UPDATE` command of SQL.
 - **Delete** - used to delete a record in the database. It uses the `DELETE` command of SQL.
 - **Merge** - used to merge a record in the database. It uses the `MERGE` command of SQL.
 - **BulkInsert** - used to bulk-insert the records in the database.
 - **ExecuteReader** - used to read certain records from the database in fast-forward access.
 - **ExecuteNonQuery** - used to execute a non-queryable query statement in the database.
 - **ExecuteScalar** - used to execute a command that returns a single-object value from the database.

On the other hand, the library has extension methods on the `IDbConnection` object level that you  can used to execute. Below are the 3 common methods wide used.

 - **ExecuteReader** - used to read certain records from the database in fast-forward access.
 - **ExecuteNonQuery** - used to execute a non-executable query in the database.
 - **ExecuteScalar** - used to execute a command that returns a single-object value from the database.

All operations mentioned above has its corresponding asynchronous operation. Usually, the asynchronous operation is only appended by `Async` keyword. Below are the list of asynchronous operations.

  - **QueryAsync**
  - **InsertAsync**
  - **UpdateAsync**
  - **DeleteAsync**
  - **MergeAsync**
  - **BulkInsertAsync**
  - **ExecuteReaderAsync**
  - **ExecuteNonQueryAsync**
  - **ExecuteScalar**

## Query Operation

This operation is used to query a data from the database and returns an `IEnumerable<TEntity>` object. Below are the parameters.

  - **where** - an expression to used to filter the data.
  - **transaction** - the transaction object to be used when querying a data.
  - **top** - the value used to return certain number of rows from the database.
  - **orderBy** - the list of fields to be used to sort the data during querying.
  - **cacheKey** - the key of the cache to check.

Below is a sample on how to query a data.
```
var stockRepository = new StockRepository(connectionString);
var stocks = stockRepository.Query();
```
Above snippet will return all the `Stock` records from the database. You can filter the data using the `where` parameter. See sample below.

Implicit way:
```
var stocks = stockRepository.Query(1);
```
Dynamic way:
```
var stocks = stockRepository.Query(new { Id = 1 });
```
Explicity way:
```
var stocks = stockRepository.Query(new QueryGroup(new QueryField("Id", 1).AsEnumerable()));
```
Below is the sample if you wish to query with multiple columns.
```
var stocks = stockRepository.Query(new { Id = 1, Name = "AAPL" });
```
Explicity way:
```
var stocks = stockRepository.Query(
	new QueryGroup(
		new [] {
			new QueryField("Id", 1),
			new QueryField("Id", Operation.Equal, "AAPL")
		}
	));
```
There are scenarios that you would like query some records with `Id` greater than 50 and less than 100. You can only achieve it using `expression tree` and `explicit` way.

See sample expressions below.

Dynamic way:
```
var stocks = stockRepository.Query(
	new
	{
		Id = new { Operation = Operation.Between, Value = new int[] { 50, 100 } }
	});
```
or

```
var stocks = stockRepository.Query(
	new
	{
		QueryGroups = new[]
		{
			new { Id = { Operation = Operation.GreaterThanOrEqual, Value = 50 } },
			new { Id = { Operation = Operation.LessThanOrEqual, Value = 100 } }
		}
	});
```
or
```
var stocks = stockRepository.Query(
	new
	{
		Id = new
		{
			Operation = Operation.All,
			Value = new object[]
			{
				new { Operation = Operation.GreaterThanOrEqual, Value = 50 },
				new { Operation = Operation.LessThanOrEqual, Value = 100 }
			} 
		}
	});
```
Explicit way:
```
var stocks = stockRepository.Query(
	new QueryGroup(
		new [] {
			new QueryField("Id", Operation.GreaterThanOrEqual, 50),
			new QueryField("Id", Operation.LessThanOrEqual, 100)
		}
	));
```
or
```
var stocks = stockRepository.Query(
	new QueryGroup(
		new QueryField("Id", Operation.Between, new [] { 50, 100 }).AsEnumerable()
	)
);
```
**Note**: Querying a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.

## Insert Operation

This operation is used to insert a record in the database. It returns an object valued by the `PrimaryKey` column. If the `PrimaryKey` column is identity, this operation will return the newly added identity column value. Below are the parameters:

 - **entity** - the entity object to be inserted.
 - **transaction** - the transaction object to be used when inserting a data.

Below is a sample on how to insert a data.
```
var stockRepository = new StockRepository(connectionString);
var stock = new Stock()
	{
		Name = "GOOGL",
		CreatedDate = DateTime.UtcNow
	};
repository.Insert(stock);
```

## Update Operation

This operation is used to update an existing record from the database. It returns an `int` value indicating the number of rows affected by the updates. Below are the parameters:

 - **entity** - the entity object to be updated.
 - **where** - an expression to used when updating a record.
 - **transaction** - the transaction object to be used when updating a data.

Below is a sample on how to update a data.
```
var stockRepository = new StockRepository(connectionString);
var stock = stockRepository.Query(new { Name = "GOOGL" }).FirstOrDefault();
if (stock != null)
{
	stock.Motto = "Do not be evil.";
	stock.UpdateDate = DateTime.UtcNow;
	var affectedRows = repository.Update(stock);
}
```
Dynamic way (soon to be supported above 1.0.9 version):
```
var stockRepository = new StockRepository(connectionString);
var affectedRows = stockRepository.Update(
new
{
	Motto = "Do not be evil."
	UpdatedDate = DateTime.UtcNow
},
new
{
	Name = "GOOGL"
});
```
**Note**:  Updating a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.

## Delete Operation

This operation is used to delete an existing record from the database. It returns an `int` value indicating the number of rows affected by the delete. Below are the parameters:

 - **where** - an expression to used when deleting a record.
 - **transaction** - the transaction object to be used when deleting a data.

Below is a sample on how to delete a data.
```
var stockRepository = new StockRepository(connectionString);
var stock = stockRepository.Query(new { Name = "GOOGL" }).FirstOrDefault();
if (stock != null)
{
	var affectedRows = stockRepository.Delete(stock);
}
```
or by `PrimaryKey`
```
var affectedRows = stockRepository.Delete(stock.Id);
```
Dynamic way:
```
var stockRepository = new StockRepository(connectionString);
var affectedRows = stockRepository.Delete(new { Name = "GOOGL" });
```
**Note**:  Deleting a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.

## Merge Operation

This operation is used to merge an entity from the existing record from the database. It returns an `int` value indicating the number of rows affected by the merge. Below are the parameters:

 - **entity** - the entity object to be merged.
 - **qualifiers** - the list of fields to be used as a qualifiers when merging a record.
 - **transaction** - the transaction object to be used when merging a data.

Below is a sample on how to merge a data.
```
var stockRepository = new StockRepository(connectionString);
var stock = stockRepository.Query(1);
stock.Motto = "Do not be evil all the time.";
UpdatedDate = DateTime.UtcNow;
stockRepository.Merge(stock, Field.Parse(new { stock.Name }));
```
or by creating a new entity with existing qualifier value.
```
var stock = new Stock()
{
	Name = "GOOGL"
	Motto = "Do not be evil all the time.",
	UpdatedDate = DateTime.UtcNow
};
stockRepository.Merge(stock, Field.Parse(new { stock.Name }));
```
**Note**:  If the `qualifiers` are not defined, the library will automatically used the `PrimaryKey` as the default qualifier. If however the `PrimaryKey` is not defined in the entity, a `PrimaryFieldNotFoundException` will be thrown back to the caller.

Please also note that merging is a process of updating and inserting. If the data is present in the database using the qualifiers, then the existing data will be updated, otherwise, a new data will be inserted in the database.

## BulkInsert Operation

This operation is used to bulk-insert the entities to the database. It returns an `int` value indicating the number of rows affected by the bulk-inserting. Below are the parameters:

 - **entities** - the list of entities to be inserted.
 - **transaction** - the transaction object to be used when doing bulk-insert.

Below is a sample on how to do bulk-insert.
```
var stockRepository = new StockRepository(connectionString);
var entities = new List<Stock>();
entities.Add(new Stock()
{
	Name = "GOOGL"
	Motto = "Do not be evil all the time.",
	CreatedDate = DateTime.UtcNow,
	UpdatedDate = DateTime.UtcNow
});
entities.Add(new Stock()
{
	Name = "MSFT"
	Motto = "Make it all easy.",
	CreatedDate = DateTime.UtcNow,
	UpdatedDate = DateTime.UtcNow
});
var affectedRows = stockRepository.BulkInsert(entities);
```

## ExecuteReader Operation

This connection extension method is very important if you wish to execute a query from the database in fast-forward access. It returns an `IEnumerable` object with `dynamic` or `object` type as its generic type.

Below are the parameters:

 - **commandText** - the SQL statement to be used for execution.
 - **param** - the parameters to be used for the execution. It could be an entity class or a dynamic object.
 - **commandTimeout** - the command timeout in seconds to be used when executing the query in the database.
 - **commandType** - the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
 - **transaction** - the transaction object be used when executing the command.

Below is the way on how to call the operation.
```
var stockRepository = new StockRepository(connectionString);
var param = new { Name = "GOOGL" };
var result = stockRepository .ExecuteReader<Stock>("SELECT * FROM [dbo].[Stock] WHERE Name = @Name;", param);
```

## ExecuteNonQuery Operation

This connection extension method is used to execute a non-queryable query statement. It returns an `int` that holds the number of affected rows during the execution.

Below are the parameters:

 - **commandText** - the SQL statement to be used for execution.
 - **param** - the parameters to be used for the execution.
 - **commandTimeout** - the command timeout in seconds to be used when executing the query in the database.
 - **commandType** - the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
 - **transaction** - the transaction object be used when executing the command.

Below is the way on how to call the operation.
```
var stockRepository = new StockRepository(connectionString);
var param = new
{
	Name = "GOOGL",
	Motto = "Do not be evil.",
	UpdatedDate = DateTime.UtcNow
};
var result = stockRepository .ExecuteNonQuery("UPDATE [dbo].[Stock] SET Motto = @Motto, UpdatedDate = @UpdatedDate WHERE Name = @Name;", param);
```

## ExecuteScalar Operation

This connection extension method is used to execute a query statement that returns single value.

Below are the parameters:

 - **commandText** - the SQL statement to be used for execution.
 - **param** - the parameters to be used for the execution.
 - **commandTimeout** - the command timeout in seconds to be used when executing the query in the database.
 - **commandType** - the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
 - **transaction** - the transaction object be used when executing the command.

Below is the way on how to call the operation.
```
var stockRepository = new StockRepository(connectionString);
var param = new { Name = "GOOGL" };
var id = stockRepository .ExecuteScalar("SELECT [Id] FROM [dbo].[Stock] Name = @Name;", param);
```
