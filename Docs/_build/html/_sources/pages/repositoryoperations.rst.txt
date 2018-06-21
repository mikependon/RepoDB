Repository Operations
=====================

.. highlight:: c#

A repository contain different operations to manipulate the data from the database. Below are the list of common operations widely used.

- **BatchQuery**: used to query a record from the database by batch. It uses the `SELECT` in combination of `ROW_NUMBER` and `ORDER` command of SQL.
- **BulkInsert**: used to bulk-insert the records in the database.
- **Count**: used to count all the records in the database. It uses the `SELECT`, `COUNT` command of SQL.
- **Delete**: used to delete a record in the database. It uses the `DELETE` command of SQL.
- **ExecuteNonQuery**: used to execute a non-queryable query statement in the database.
- **ExecuteQuery**: used to read certain records from the database in fast-forward access. It returns an enumerable list of `RepoDb.DataEntity` objects.
- **ExecuteScalar**: used to execute a command that returns a single-object value from the database.
- **InlineUpdate**: used to do column-based update of the records in the database. It uses the `UPDATE` command of SQL.
- **Insert**: used to insert a record in the database. It uses the `INSERT` command of SQL.
- **Merge**: used to merge a record in the database. It uses the `MERGE` command of SQL.
- **Query**: used to query a record from the database. It uses the `SELECT` command of SQL.
- **Update**: used to update a record in the database. It uses the `UPDATE` command of SQL.

All operations mentioned above has its own corresponding asynchronous operation. Usually, the asynchronous operation is only appended by `Async` keyword. Below are the list of asynchronous operations.

- **BatchQueryAsync**
- **BulkInsertAsync**
- **CountAsync**
- **DeleteAsync**
- **ExecuteNonQueryAsync**
- **ExecuteQueryAsync**
- **ExecuteScalar**
- **InlineUpdateAsync**
- **InsertAsync**
- **MergeAsync**
- **QueryAsync**
- **UpdateAsync**

BatchQuery Operation
--------------------

.. highlight:: c#

This operation is used to batching when querying a data from the database. It returns an enumerable object of `RepoDb.DataEntity` objects.

Below are the parameters:

- **where**: an expression to used to filter the data.
- **page**: a zero-based index that signifies the page number of the batch to query.
- **rowsPerBatch**: the number of rows to be returned per batch.
- **orderBy**: the list of fields to be used to sort the data during querying.
- **transaction**: the transaction object to be used when querying a data.

Below is a sample on how to query the first batch of data from the database where the number of rows per batch is 24.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	repository.BatchQuery<Order>(0, 24);

Below is the way to query by batch the data with expression.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	repository.BatchQuery<Order>(new { CustomerId = 10045, 0, 24);

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

BulkInsert Operation
--------------------

.. highlight:: c#

This operation is used to bulk-insert the entities to the database. It returns an `int` value indicating the number of rows affected by the bulk-inserting.

Below are the parameters:

- **entities**: the list of entities to be inserted.
- **transaction**: the transaction object to be used when doing bulk-insert.

Below is a sample on how to do bulk-insert.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var entities = new List<Order>();
	entities.Add(new Order()
	{
		Id = 251,
		Quantity = 2,
		ProductId = 12,
		CreatedDate = DateTime.UtcNow,
		UpdatedDate = DateTime.UtcNow
	});
	entities.Add(new Stock()
	{
		Id = 251,
		Quantity = 25,
		ProductId = 15,
		CreatedDate = DateTime.UtcNow,
		UpdatedDate = DateTime.UtcNow
	});
	var affectedRows = repository.BulkInsert(entities);

Count Operation
---------------

.. highlight:: c#

These operations are used to count the number of records from the database. It returns a value indicating the number of counted rows based on the created expression.

Below are the parameters:

- **where**: an expression to used when counting a record. If left `null`, all records from the database will be counted.
- **transaction**: the transaction object to be used when updating a data.

Below is a sample on how to count a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var rows = repository.Count<Order>();

The code snippets above will count all the `Order` records from the database.

Below is the sample way to count a records with expression

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var rows = repository.Count<Order>(new { CustomerId = 10045 });

Above code snippets will count all the `Order` records from the database where `CustomerId` is equals to `10045`.

Delete Operation
----------------

.. highlight:: c#

This operation is used to delete an existing record from the database. It returns an `int` value indicating the number of rows affected by the delete.

Below are the parameters:

- **where**: an expression to used when deleting a record.
- **transaction**: the transaction object to be used when deleting a data.

Below is a sample on how to delete a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var order = repository.Query<Order>(new { Id = "251" }).FirstOrDefault();
	if (order != null)
	{
		var affectedRows = repository.Delete(order);
	}

or by `PrimaryKey`

::

	var affectedRows = repository.Delete<Order>(order.Id);

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var affectedRows = repository.Delete<Order>(new { Id = "251" });

**Note**: Deleting a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.

ExecuteNonQuery Operation
-------------------------

.. highlight:: c#

This connection extension method is used to execute a non-queryable SQL statement query. It returns an `int` that holds the number of affected rows during the execution.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout (optional)**: the command timeout in seconds to be used when executing the query in the database.
- **commandType (optional)**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction (optional)**: the transaction object be used when executing the command.

Below is the way on how to call the operation.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var param = new
	{
		CustomerId = 10045,
		Quantity = 5,
		UpdatedDate = DateTime.UtcNow
	};
	var result = repository.ExecuteNonQuery("UPDATE [dbo].[Stock] SET Quantity = @Quantity, UpdatedDate = @UpdatedDate WHERE CustomerId = @CustomerId;", param);

ExecuteQuery Operation
----------------------

.. highlight:: c#

This connection extension method is used to execute a SQL Statement query from the database in fast-forward access. It returns an `IEnumerable` object with `dynamic` or `RepoDb.DataEntity` type as its generic type.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout (optional)**: the command timeout in seconds to be used when executing the query in the database.
- **commandType (optional)**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction (optional)**: the transaction object be used when executing the command.

Below is the way on how to call the operation.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var param = new { CustomerId = 10045 };
	var result = repository.ExecuteQuery<Order>("SELECT * FROM [dbo].[Stock] WHERE CustomerId = @CustomerId;", param);

ExecuteScalar Operation
-----------------------

.. highlight:: c#

This connection extension method is used to execute a query statement that returns a single value.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout (optional)**: the command timeout in seconds to be used when executing the query in the database.
- **commandType (optional)**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction (optional)**: the transaction object be used when executing the command.

Below is the way on how to call the operation.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var param = new { CustomerId = 10045 };
	var id = repository.ExecuteScalar("SELECT MAX([Id]) AS MaxIdByCustomerId FROM [dbo].[Stock] CustomerId = @CustomerId;", param);

InlineUpdate Operation
----------------------

.. highlight:: c#

This operation is used to do a column-based update of an existing record from the database. It returns an `int` value indicating the number of rows affected by the updates.

Below are the parameters:

- **entity**: the dynamically or entity driven data entity object that contains the target fields to be updated.
- **where**: an expression to used when updating a record.
- **transaction**: the transaction object to be used when updating a data.

Below is a sample on how to update a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var affectedRows = repository.InlineUpdate<Order>(new { Quantity = 5, UpdatedDate = DateTime.UtcNow }, new { Id = "251" });

The code snippets above will update the `Quantity` column of a order records from the dabatase where the value of `Id` column is equals to `251`.

Insert Operation
----------------

.. highlight:: c#

This operation is used to insert a record in the database. It returns an object valued by the `PrimaryKey` column. If the `PrimaryKey` column is identity, this operation will return the newly added identity column value.

Below are the parameters:

- **entity**: the entity object to be inserted.
- **transaction**: the transaction object to be used when inserting a data.

Below is a sample on how to insert a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var order = new Order()
	{
		CustomerId = 10045,
		ProductId = 12
		Quantity = 2,
		CreatedDate = DateTime.UtcNow
	};
	repository.Insert(order);

Merge Operation
---------------

.. highlight:: c#

This operation is used to merge an entity from the existing record from the database. It returns an `int` value indicating the number of rows affected by the merge.

Below are the parameters:

- **entity**: the entity object to be merged.
- **qualifiers**: the list of fields to be used as a qualifiers when merging a record.
- **transaction**: the transaction object to be used when merging a data.

Below is a sample on how to merge a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var order = repository.Query<Order>(1);
	order.Quantity = 5;
	UpdatedDate = DateTime.UtcNow;
	repository.Merge(order, Field.Parse(new { order.Id }));

**Note**: The merge is a process of updating and inserting. If the data is present in the database using the qualifiers, then the existing data will be updated, otherwise, a new data will be inserted in the database.

Query Operation
---------------

.. highlight:: c#

This operation is used to query a data from the database and returns an `IEnumerable<TEntity>` object. Below are the parameters.

- **where**: an expression to used to filter the data.
- **transaction**: the transaction object to be used when querying a data.
- **top**: the value used to return certain number of rows from the database.
- **orderBy**: the list of fields to be used to sort the data during querying.
- **cacheKey**: the key of the cache to check.

Below is a sample on how to query a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>();

Above snippet will return all the `Customer` records from the database. The data can filtered using the `where` parameter. See sample below.

Implicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customer = repository.Query<Customer>(1).FirstOrDefault();

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customer = repository.Query<Order>(new { Id = 1 }).FirstOrDefault();

Explicity way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customer = repository.Query<Customer>
	(
		new QueryGroup(new QueryField("Id", 1).AsEnumerable())
	).FirstOrDefault();

Below is the sample on how to query with multiple columns.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>(new { Id = 1, Name = "Anna Fullerton", Conjunction.Or });

Explicity way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
		new QueryGroup
		(
			new []
			{
				new QueryField("Id", Operation.Equal, 1),
				new QueryField("Name", Operation.Equal, "Anna Fullerton")
			},
			null,
			Conjunction.Or
		)
	);

When querying a data where `Id` field is greater than 50 and less than 100. See sample expressions below.

Dynamic way:

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
			QueryGroups = new[]
			{
				new { Id = { Operation = Operation.GreaterThanOrEqual, Value = 50 } },
				new { Id = { Operation = Operation.LessThanOrEqual, Value = 100 } }
			}
		}
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

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
		new QueryGroup
		(
			new []
			{
				new QueryField("Id", Operation.GreaterThanOrEqual, 50),
				new QueryField("Id", Operation.LessThanOrEqual, 100)
			}
		)
	);

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
		new QueryGroup
		(
			new QueryField("Id", Operation.Between, new [] { 50, 100 }).AsEnumerable()
		)
	);

**Note**: Querying a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.

Ordering a Query
~~~~~~~~~~~~~~~~

.. highlight:: c#

An ordering is the way of sorting the result of your query in `ascending` or `descending` order, depending on the qualifier fields. Below is a sample snippet that returns the `Stock` records ordered by `ParentId` field in ascending manner and `Name` field is in `descending` manner.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var orderBy = new
	{
		CustomerId = Order.Ascending,
		Name = Order.Descending
	};
	var orders = repository.Query<Order>(new { CustomerId = new { Operation = Operation.GreaterThan, Value = 1 } }, orderBy: OrderField.Parse(orderBy));

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var orderBy = new []
	{
		new OrderField("CustomerId", Order.Ascending),
		new OrderField("Name", Order.Descending)
	};
	var orders = repository.Query<Order>(new { CustomerId = new { Operation = Operation.GreaterThan, Value = 1 } }, orderBy: orderBy);

The `RepodDb.OrderField` is an object that is being used to order a query result. The `Parse` method is used to convert the `dynamic` object to become an `OrderField` instances.

**Note:** When composing a dynamic ordering object, the value of the properties should be equal to `RepoDb.Enumerations.Order` values (`Ascending` or `Descending`). Otherwise, an exception will be thrown during `OrderField.Parse` operation.

Limiting a Query Result
~~~~~~~~~~~~~~~~~~~~~~~

.. highlight:: c#

A top parameter is used to limit the result when querying a data from the database. Below is a sample way on how to use the top parameter.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var orders = repository.Query<Order>(new { CustomerId = new { Operation = Operation.GreaterThan, Value = 1 } }, top: 100);

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var orders = repository.Query<Order>(new { CustomerId = new { Operation = Operation.GreaterThan, Value = 1 } }, top: 100);

Update Operation
----------------

.. highlight:: c#

This operation is used to update an existing record from the database. It returns an `int` value indicating the number of rows affected by the updates.

Below are the parameters:

- **entity**: the entity object to be updated.
- **where**: an expression to used when updating a record.
- **transaction**: the transaction object to be used when updating a data.

Below is a sample on how to update a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var order = repository.Query<Order>(new { Id = 251 }).FirstOrDefault();
	if (order != null)
	{
		order.Quantity = 5;
		order.UpdateDate = DateTime.UtcNow;
		var affectedRows = repository.Update(order);
	}

Dynamic way (column-based update), or see InlineUpdate documentation:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var affectedRows = repository.InlineUpdate<Order>(new { Quantity = 5, UpdatedDate = DateTime.UtcNow }, new { Id = 251 });

**Note**:  Updating a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.
