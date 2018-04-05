RepoDb

A dynamic ORM library used to create an Entity-Based Repositories when accessing data from the database.

What's with RepoDb?

- Initial Release for RepoDb (.Net Extension)
- Supported Dynamic DbConnection
- Supported Mapping Command Type (Entity Level)
- Supported ExecuteScalar, ExecuteNonQuery, ExecuteReader (DataEntity and ExpandoObject)
- Supported Primary Attribute (IsIdentity)
- Supported BaseRepository that uses the functionality of the DbRepository
- Supported the GetSqlStatement (Entity Level)
- Supported the dynamic-object-driven approach when doing a Query, Delete, Update and Merge
- Supported Merge Method (row Level only)
- Supported Bulk Insert (SqlBulkCopy only)
- Supported to get the Connection at all Repositories
- Supported Transaction Handlers
- Supported Async methods
- Supported DbRepository property at the BaseRepository
- Supported Dynamic object returns for ExecuteReaderEx
- Supported the execution guards at the DbRepository (Queryable, Insertable, Deletable, Mergeable, Updateable)
- Supported Type Mapper which you can define the equation of System.Type to System.Data.DbType

How to use?

Below are some guidelines on how to use the RepoDb.

---------------------

Creating an Entity

- Map attribute defines which object to use in the database. The value of this attribute could be the actual name of the table or a stored procedure
- Map attribute second parameter is a CommandType. It tells whether the object is for TableDirect, StoredProcedure or Text base execution.
- Ignore attribute is a property level attribute used to signify whether the property will be ignore on a certain commands (SELECT, DELETE, UPDATE, INSERT, MERGE)
- Primary attribute is used to flag a property as the primary field of the table (or a result set).

	public interface IStock : IDataEntity
	{
		int Id { get; set; }
		
		string ProductId { get; set; }

		string Code { get; set; }

		string Name { get; set; }
		
		int Quantity { get; set; }

		DateTime CreatedDate { get; set; }

		DateTime ModifiedDate { get; set; }
	}

	[Map("[dbo].[Stock]")]
	public class Stock : DataEntity
	{
		[Ignore(Command.Insert | Command.Update)]
		[Primary]
		public int Id { get; set; }
		
		public string ProductId { get; set; }

		[Ignore(Command.Update)]
		public string Code { get; set; }
		
		public string Name { get; set; }
		
		public int Quantity { get; set; }

		[Ignore(Command.Insert | Command.Update)]
		public DateTime CreatedDate { get; set; }

		[Ignore(Command.Insert)]
		public DateTime ModifiedDate { get; set; }
	}

---------------------

Creating a repository

- The class must inherit the BaseRepository<TEntity, TDbConnection> or DbRepository<TDbConnection>.
- The TEntity dynamic parameter type is used to define which entity objects to used on this repository.
- The TDbConnection dynamic parameter type is used to define the type of Connection object to be used when connecting to the database (SqlServer, Oracle, SqlLite, etc etc).
- The constructor of the BaseRepository accepts two parameters, the ConnectionString and the CommandTimeout.

	public class StockRepository : BaseRepository<Stock, SqlConnection>
	{
		public StockRepository(ISettings settings)
			: base(settings.ConnectionString)
		{
		}
	}

---------------------

Querying a data

- The first query will return the Stock data from the database where the Id is equals to 256;
- The second query will return the Stock data from the database where the Id is greater than or equals to 50;

	var settings = new Settings();
	var stockRepository = new StockRepository(settings.ConnectionString);

Dynamic approach
	
	var stock = stockRepository.Query({ Id = 256 });

Explicit approach
	
	var stock = stockRepository.Query(new List<IQueryField>() { new QueryField("Id", Operation.GreaterThanOrEqual, 50) });

---------------------

Updating a data

	var settings = new Settings();
	var stockRepository = new StockRepository(settings.ConnectionString);
	var stock = stockRepository.Query({ Id = 256 });
	stock.Name = $"{stock.Name} - some updates";

Default approach, uses the PrimaryKey
	
	var affectedRows = stockRepository.Update(stock);

Dynamic approach
	
	var affectedRows = stockRepository.Update(stock, new { Id = 256 });

Explicit approach
	
	var affectedRows = stockRepository.Update(stock, new List<IQueryField>() { new QueryField("Id", Operation.GreaterThanOrEqual, 50) });

---------------------

Deleting a data

	var settings = new Settings();
	var stockRepository = new StockRepository(settings.ConnectionString);
	var stock = stockRepository.Query({ Id = 256 });

Dynamic approach
	
	var affectedRows = stockRepository.Delete({ Id = 256 });

Explicit approach

	var affectedRows = stockRepository.Delete(stock, new List<IQueryField>() { new QueryField("Id", Operation.GreaterThanOrEqual, 256) });

---------------------

Merging a data

	var settings = new Settings();
	var stockRepository = new StockRepository(settings.ConnectionString);
	var stock = stockRepository.Query({ Id = 256 });
	stock.Name = $"{stock.Name} - some merge updates";

Default approach, uses the PrimaryKey as the qualifiers
	
	var affectedRows = stockRepository.Merge(stock);

Explicit approach, uses the ProductId and Code fields as the qualifiers

	var affectedRows = stockRepository.Merge(stock, Field.From("ProductId", "Code"));

---------------------

Bulk inserting a data

	var settings = new Settings();
	var stockRepository = new StockRepository(settings.ConnectionString);
	var stocks = new List<Stock>();

	stocks.Add(new Stock() { .... });
	.
	.
	.
	stocks.Add(new Stock() { .... });
	var affectedRows = stockRepository.BulkInsert(stocks);
	
---------------------

You can as well use the DbRepository object to avoid the binding on the Entity level repository. With this object, you can dynamically manipulate (CRUD) all the objects or entities that you wish to manipulate from the database.

Explicit ExecutionNonQuery
	
	var settings = new Settings();
	var repository = new DbRepository<SqlConnection>(settings.ConnectionString, settings.CommandTimeout);
	
Delete a Stock with Id equals to 1
	
	var stocks = repository.CreateConnection().ExecuteNonQuery("DELETE FROM [dbo].[Stock] WHERE (Id = @Id);", new { Id = 1 });
	
Explicit ExecuteReader

	var settings = new Settings();
	var repository = new DbRepository<SqlConnection>(settings.ConnectionString, settings.CommandTimeout);
	
Suppose you have a Product table, it will return a Product with Name equals "Dairy Milk"
	
	var product = repository.Query<Product>({ Name = "Dairy Milk" });
	
Get the stock of the Product milk in dynamic approach
	
	var stock = repository.Query<Stock>({ ProductId = product.Id });
	
Use a SQL Statement directly to return the result at the desired object
	
	var productsByDate = repository.CreateConnection().ExecuteReader<Product>("SELECT * FROM [dbo].[Product] WHERE (TransactionDate = @TransactionDate);", new { TransactionDate = DateTime.Parse("2018-01-01 00:00:00.000") });
	
Use the extended version of the ExecuteReader named ExecuteReaderEx to return the dynamic objects

	var stocks = repository.CreateConnection().ExecuteReader("SELECT * FROM [dbo].[Stock];");
	stocks.ForEach(stock => {
		var current = (dynamic)product;
		var name = current.Name;
	});

---------------------

The transactions is also supported with RepoDb

	var settings = new Settings();
	var repository = new DbRepository<SqlConnection>(settings.ConnectionString, settings.CommandTimeout);
	using (var connection = repository.CreateConnection())
	{
		using (var transaction = connection.BeginTransaction())
		{
			repository.ExecuteNonQuery("DELETE FROM [dbo].[Stock] WHERE CreatedDate > @CreatedDate);", new { CreatedDate = DateTime.Parse("2017-01-01") });
			var stocks = repository.Query<Stock>({ ProductId = 12 });
			stocks.ForEach(stock => {
				stock.Quantity = 100;
			});
			repository.Merge<Stock>(stocks);
			transaction.RollBack(); // RollBack Everything
		}
	}
	
---------------------

Lastly, the RepoDb also supports the Threading by calling each "Async" method on the repository and on the DbConnection extensions. Below are the following methods that are Multi-Threaded.

- QueryAsync
- DeleteAsync
- InsertAsync
- UpdateAsync
- MergeAsync
- BulkInsertAsync
- ExecuteNonQueryAsync
- ExecuteScalarAsync
- ExecuteReaderAsync