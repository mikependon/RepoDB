Repository Operations
=====================

.. highlight:: c#

A repository contain different operations to manipulate the data from the database. All operations that repository has are just abstracted methods from the `System.Data.IDbConnection` object extended methods.

Below are the list of the abstracted operations of the repository.

- **BatchQuery**: is used to query a record from the database by batch. It returns an enumerable list of `DataEntity` object.
- **BulkInsert**: is used to bulk-insert the records in the database. It returns the number of inserted rows from the database.
- **Count**: is used to count all the records from the database. It returns the number of rows from the database based on the given query expression.
- **Delete**: is used to delete a record from the database. It returns the number of deleted rows from the database.
- **DeleteAll**: is used to delete all the records in the database. It returns the number of deleted rows from the database.
- **ExecuteNonQuery**: is used to execute a non-queryable SQL statement. It returns the number of affected from the database.
- **ExecuteQuery**: is used to read certain records from the database in fast-forward access. It returns an enumerable list of `dynamic` or `DataEntity` object.
- **ExecuteReader**: is used to execute a SQL statement query in the database in fast-forward access. This method returns an instance of `System.Data.IDataReader` object.
- **ExecuteScalar**: is used to execute a query statement that returns single value of type `System.Object`.
- **InlineInsert**: is used to do column-based insert of the records in the database. It returns the number of inserted rows from the database.
- **InlineMerge**: is used to do column-based merge of the records in the database. It returns the number of merged rows from the database.
- **InlineUpdate**: is used to do column-based update of the records in the database. It returns the number of updated rows from the database.
- **Insert**: is used to insert a record in the database. It returns the number of inserted rows from the database.
- **Merge**: is used to merge a record in the database. It returns the number of merged rows from the database.
- **Query**: is used to query a record from the database. It returns an enumerable list of `DataEntity` object.
- **Truncate**: is used to truncate a table in the database.
- **Update**: is used to update a record in the database. It returns the number of updated rows from the database.

All operations mentioned above has its own corresponding asynchronous operation. Usually, the asynchronous operation is only appended by `Async` keyword. Below are the list of asynchronous operations.

- **BatchQueryAsync**
- **BulkInsertAsync**
- **CountAsync**
- **DeleteAsync**
- **DeleteAllAsync**
- **ExecuteNonQueryAsync**
- **ExecuteQueryAsync**
- **ExecuteScalar**
- **InlineInsertAsync**
- **InlineMergeAsync**
- **InlineUpdateAsync**
- **InsertAsync**
- **MergeAsync**
- **QueryAsync**
- **TruncateAsync**
- **UpdateAsync**

BatchQuery Operation
--------------------

.. highlight:: c#

Query the data from the database by batch based on the given query expression. The batching will vary on the page number and number of rows per batch defined by this operation. This operation is useful for paging purposes.

Below are the parameters:

- **where**: The query expression or primary key value to be used by this operation.
- **page**: the page of the batch to be used by this operation.
- **rowsPerBatch**: the number of rows per batch to be used by this operation.
- **orderBy**: the order definition of the fields to be used by this operation.
- **transaction (optional)**: the transaction to be used by this operation.
- **returns**: an enumerable list of `DataEntity` objects.

Below is a sample on how to query the first batch of data from the database where the number of rows per batch is 24.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		repository.BatchQuery<Order>(0, 24);
	}

Below is the way to query by batch the data with expression.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		repository.BatchQuery<Order>(new { CustomerId = 10045 }, 0, 24);
	}

Batching is very important when you are lazy-loading the data from the database. Below is a sample event listener for scroll (objects), doing the batch queries and post-process the data.

::

	var scroller = <Any Customimzed Scroller Object>
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var page = 0;
	var rowsPerBatch = 24;

	scroller.ScrollToEnd += (o, e) =>
	{
		var result = repository.BatchQuery<Order>(new { CustomerId = 10045 }, page, rowsPerBatch);
		Process(result);
		page++;
	};

	void Process(IEnumerable<Order> orders)
	{
		// Process the orders (display on the page)
	}
	
	void Dispose()
	{
		connection.Dispose();
	}

BulkInsert Operation
--------------------

.. highlight:: c#

Bulk-inserting the list of `DataEntity` objects in the database.

Below are the parameters:

- **entities**: the list of entities to be inserted.
- **transaction (optional)**: the transaction object to be used when doing bulk-insert.
- **returns**: an instance of integer that holds the number of rows affected by the execution.

Below is a sample on how to do bulk-insert.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var entities = new List<Order>();
		entities.Add(new Order()
		{
			Quantity = 2,
			ProductId = 12,
			CreatedDate = DateTime.UtcNow,
			UpdatedDate = DateTime.UtcNow
		});
		entities.Add(new Order()
		{
			Quantity = 25,
			ProductId = 15,
			CreatedDate = DateTime.UtcNow,
			UpdatedDate = DateTime.UtcNow
		});
		var affectedRows = repository.BulkInsert(entities);
	}

Count Operation
---------------

.. highlight:: c#

Counts the number of rows from the database based on the given query expression.

Below are the parameters:

- **where**: The query expression or primary key value to be used by this operation.
- **transaction (optional)**: the transaction to be used by this operation.
- **returns**: an integer value for the number of rows counted from the database based on the given query expression.

Below is a sample on how to count a data.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var rows = repository.Count<Customer>();
	}

The code snippets above will count all the `Customer` records from the database.

Below is the sample way to count a records with expression

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var rows = repository.Count<Customer>(new { Id = new { Operation = Operation.GreaterThanOrEqual, Value = 10045 } });

Above code snippets will count all the `Customer` records from the database where `Id` is greater than or equals to `10045`.

Delete Operation
----------------

.. highlight:: c#

Deletes a data in the database based on the given query expression. It returns an instance of integer that holds the number of rows affected by the execution.

Below are the parameters:

- **where**: The query expression or primary key value to be used by this operation. When is set to `NULL`, it deletes all the data from the database.
- **transaction (optional)**: the transaction object to be used when deleting a data.
- **returns**: an instance of integer that holds the number of rows affected by the execution.

Below is a sample on how to delete a data.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var customer = repository.Query<Customer>(251).FirstOrDefault();
		if (customer != null)
		{
			var affectedRows = repository.Delete<Customer>(customer);
		}
	}

or by `PrimaryKey`

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var affectedRows = repository.Delete<Customer>(251);
	}
	
Deleting a by passing a `DataEntity` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.

**Note**: By leaving the `WHERE` parameter to blank would delete all records. Exactly the same operation as `DeleteAll`.

DeleteAll Operation
-------------------

.. highlight:: c#

Deletes all data in the database based on the target `DataEntity`.

Below are the parameters:

- **transaction (optional)**: the transaction object to be used when deleting a data.
- **returns**: an instance of integer that holds the number of rows affected by the execution.

Below is a sample on how to delete all the data.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var customer = repository.DeleteAll<Customer>();
	}

ExecuteNonQuery Operation
-------------------------

.. highlight:: c#

Executes a query from the database. It uses the underlying `ExecuteNonQuery` method of the `System.Data.IDbCommand` object and returns the number of affected rows during the execution.

Below are the parameters:

- **commandText**: The command text to be used on the execution.
- **param (optional)**: The dynamic object to be used as parameter. This object must contain all the values for all the parameters defined in the `CommandText` property.
- **commandType (optional)**: the command type to be used on the execution.
- **transaction (optional)**: the transaction to be used on the execution (if present).

Below is the way on how to call the operation.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var commandText = @"UPDATE [dbo].[Order] SET Quantity = @Quantity, UpdatedDate = @UpdatedDate WHERE (CustomerId = @CustomerId);";
		var result = repository.ExecuteNonQuery(commandText, new
		{
			CustomerId = 10045,
			Quantity = 5,
			UpdatedDate = DateTime.UtcNow
		});
	}

ExecuteQuery Operation
----------------------

.. highlight:: c#

Executes a query from the database. It uses the underlying `ExecuteReader` method of the `System.Data.IDbCommand` object and converts the result back to an enumerable list of `DataEntity` object.

Below are the parameters:

- **commandText**: The command text to be used on the execution.
- **param (optional)**: The dynamic object to be used as parameter. This object must contain all the values for all the parameters defined in the `CommandText` property.
- **commandType (optional)**: the command type to be used on the execution.
- **transaction (optional)**: the transaction to be used on the execution (if present).

Below is the way on how to call the operation.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var result = repository.ExecuteQuery<Order>("SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId;", new
		{
			CustomerId = 10045
		});
	}
	
ExecuteScalar Operation
-----------------------

.. highlight:: c#

Executes a query from the database. It uses the underlying `ExecuteScalar` method of the `System.Data.IDbCommand` object and returns the first occurence value (first column of first row) of the execution.

Below are the parameters:

- **commandText**: The command text to be used on the execution.
- **param (optional)**: The dynamic object to be used as parameter. This object must contain all the values for all the parameters defined in the `CommandText` property.
- **commandType (optional)**: the command type to be used on the execution.
- **transaction (optional)**: the transaction to be used on the execution (if present).

Below is the way on how to call the operation.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var id = repository.ExecuteScalar("SELECT MAX([Id]) AS MaxId FROM [dbo].[Order]");
	}

InlineInsert Operation
----------------------

.. highlight:: c#

Inserts a data in the database by targetting certain fields only.

Below are the parameters:

- **entity**: the object that contains the targetted columns to be inserted.
- **overrideIgnore (optional)**: set to `true` if to allow the insert operation on the properties with `RepoDb.Attributes.IgnoreAttribute` defined.
- **transaction (optional)**: the transaction object to be used when updating a data.
- **returns**: the value of the `PrimaryKey` of the newly inserted `DataEntity` object. Returns `NULL` if the `PrimaryKey` property is not present.

Below is a sample on how to update a data.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var id = repository.InlineInsert<Order>(new
		{
			CustomerId = 10045,
			ProductId = 35,
			Quantity = 5,
			CreatedDate = DateTime.UtcNow
		});
	}

The code snippets above will insert the `CustomerId`, `ProductId`, `Quantity` and `CreatedDate` columns of the `Order` table. It will set the other columns to `NULL`.

InlineMerge Operation
---------------------

.. highlight:: c#

Merges a data in the database by targetting certain fields only.

Below are the parameters:

- **entity**: the object that contains the targetted columns to be inserted.
- **qualifiers**: the list of the qualifier fields to be used by the inline merge operation on a SQL Statement.
- **overrideIgnore (optional)**: set to `true` if to allow the insert operation on the properties with `RepoDb.Attributes.IgnoreAttribute` defined.
- **transaction (optional)**: the transaction object to be used when updating a data.
- **returns**: an instance of integer that holds the number of rows affected by the execution.

Below is a sample on how to do inline merge.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var affectedRows = repository.InlineMerge<Order>(new
		{
			Id = 10045,
			ProductId = 35,
			Quantity = 5,
			UpdatedDate = DateTime.UtcNow
		},
		Field.From("Id"));
	}

The code snippets above will merge the `Order` record into the database by inserting the value of the `ProductId`, `Quantity` and `UpdatedDate` columns if the record with `Id` equals to `10045` is not yet in the database. Otherwise, it will update the existing records.

**Note**: It is necessary to define the qualifier fields, and the qualifier fields must be present on the dynamic object passed at `entity` parameter. Please also note that the `Merge` operation is only using the `Equal` operation when merging the data in the database. Other operations of like (`GreaterThan`, `LessThan`) is not supported. One can create a advance SQL Statement or Stored Procedure for merging process and call the `ExecuteNonQuery` method instead.

InlineUpdate Operation
----------------------

.. highlight:: c#

Updates a data in the database by targetting certain fields only.

Below are the parameters:

- **entity**: the object that contains the targetted columns to be inserted.
- **where**: The query expression or primary key value to be used by this operation.
- **overrideIgnore (optional)**: set to `true` if to allow the insert operation on the properties with `RepoDb.Attributes.IgnoreAttribute` defined.
- **commandTimeout (optional)**: the command timeout in seconds to be used on the execution.
- **transaction (optional)**: the transaction object to be used when updating a data.
- **trace (optional)**: the trace object to be used by this operation.
- **statementBuilder (optional)**: the statement builder object to be used by this operation.
- **returns**: an instance of integer that holds the number of rows affected by the execution.

Below is a sample on how to do inline merge.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var affectedRows = repository.InlineUpdate<Customer>(new
		{
			Name = "Anna Fullerton",
			UpdatedDate = DateTime.UtcNow
		},
		new { Id = 10045 });
	}

The code snippets above will update the `Name` field to `Anna Fullerton` and the `UpdatedDate` field to `DateTime.UtcNow` of the `Customer` record where the `Id` is equals to `10045`.

Please note, that in the `InlineUpdate` operation, only the fields defined at the `entity` parameters are being included in the context.

The codes above will generate the SQL Statement below.

::

	UPDATE [dbo].[Customer] SET Name = @Name, UpdateDate = @UpdatedDate WHERE Id = @Id;

Insert Operation
----------------

.. highlight:: c#

Inserts a data in the database.

Below are the parameters:

- **entity**: the entity object to be inserted.
- **transaction (optional)**: the transaction object to be used when inserting a data.
- **returns**: an instance of integer that holds the number of rows affected by the execution.

Below is a sample on how to insert a data.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var order = new Order()
		{
			CustomerId = 10045,
			ProductId = 12
			Quantity = 2,
			CreatedDate = DateTime.UtcNow
		};
		repository.Insert(order);
	}

Merge Operation
---------------

.. highlight:: c#

Merges an existing `DataEntity` object in the database.

Below are the parameters:

- **entity**: the entity object to be merged.
- **qualifiers**: the list of fields to be used as the qualifiers when merging a record.
- **transaction (optional)**: the transaction object to be used when merging a data.
- **returns**: an instance of integer that holds the number of rows affected by the execution.

Below is a sample on how to merge a data.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var order = repository.Query<Order>(1);
		order.Quantity = 5;
		UpdatedDate = DateTime.UtcNow;
		repository.Merge(order, Field.Parse(new { order.Id }));
	}

**Note**: The merge is a process of updating and inserting. If the data is present in the database using the qualifiers, then the existing data will be updated, otherwise, a new data will be inserted in the database.

Query Operation
---------------

.. highlight:: c#

Query a data from the database based on the given query expression.

- **where**: The query expression or primary key value to be used by this operation.
- **top**: the value used to return certain number of rows from the database.
- **orderBy**: the list of fields to be used to sort the data during querying.
- **cacheKey**: the key of the cache to check.
- **transaction (optional)**: the transaction object to be used when querying a data.
- **returns**: an enumerable list of `DataEntity` object.

Below is a sample on how to query a data.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var customers = repository.Query<Customer>();
	}

Above snippet will return all the `Customer` records from the database. The data can filtered using the `where` parameter. See sample below.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var customer = repository.Query<Order>(105).FirstOrDefault();
	}

Below is the sample on how to query with multiple columns.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>(new { Id = 1, Name = "Anna Fullerton", Conjunction.Or });

When querying a data where `Id` field is greater than 50 and less than 100. See sample expressions below.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
		new { Id = new { Operation = Operation.Between, Value = new int[] { 50, 100 } } }
	);

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
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
		}
	);

**Note**: Querying a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.

Ordering the Result
~~~~~~~~~~~~~~~~~~~

.. highlight:: c#

An ordering is the way of sorting the result of your query in `Ascending` or `Descending` order, depending on the qualifier fields.

Below is a sample snippet that returns the `Customer` records ordered by `ParentId` field in ascending manner and `Name` field is in `descending` manner.

Dynamic way:

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var orderBy = new
		{
			Name = Order.Descending
		};
		var customers = repository.Query<Customer>(new { Id = new { Operation = Operation.In, Value = new [] { 100, 200 } } }, orderBy: OrderField.Parse(orderBy));
		customers.ToList().ForEach(customer =>
		{
			// Process each Customer here
		});
	}

The `RepodDb.OrderField` is an object that is being used to order a query result. The `Parse` method is used to convert the `dynamic` object to become an `OrderField` instances.

**Note:** When composing a dynamic ordering object, the value of the properties should be equal to `RepoDb.Enumerations.Order` values (`Ascending` or `Descending`). Otherwise, an exception will be thrown during `OrderField.Parse` operation.

Limiting the Query Result
~~~~~~~~~~~~~~~~~~~~~~~~~

.. highlight:: c#

A top parameter is used to limit the result when querying a data from the database.

Below is a sample way on how to use the top parameter.

Dynamic way:

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var customers = repository.Query<Customer>(new { CustomerId = new { Operation = Operation.GreaterThan, Value = 1 } }, top: 100);
		customers.ToList().ForEach(customer =>
		{
			// Process each Customer here
		});
	}

Truncate Operation
------------------

.. highlight:: c#

Truncates a table from the database.

Below is a sample on how to truncate a table.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.Truncate<Customer>();
	}

Update Operation
----------------

.. highlight:: c#

Updates a data in the database based on the given query expression.

Below are the parameters:

- **entity**: the entity object to be updated.
- **where**: The query expression or primary key value to be used by this operation.
- **transaction (optional)**: the transaction object to be used when updating a data.
- **returns**: an instance of integer that holds the number of rows affected by the execution.

Below is a sample on how to update a data.

::

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var order = repository.Query<Order>(105).FirstOrDefault();
		if (order != null)
		{
			order.Quantity = 5;
			order.UpdateDate = DateTime.UtcNow;
			var affectedRows = repository.Update(order);
		}
	}

**Note**:  Updating a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.
