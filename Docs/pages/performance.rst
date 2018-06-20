Performance Benchmark
=====================

.. highlight:: c#

We have measure the performance benchmarks of `RepoDb` against `Dapper` library, as we considered the most lightweight ORM currently available.

Comparisson process:

1. Created a local database.
2. A table named `Employee` has been created to a local machine SQL Server database.
3. Created a small `Console` application to `Insert` and `Query` the data. Can be downloaded.

**Note**: Modify the connection string to connect to your own local machine newly created test database.

.. highlight:: sql

Table structure:

::

	IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Employee' AND type = 'U')
	BEGIN
		DROP TABLE [dbo].[Employee];
	END
	GO

	CREATE TABLE [dbo].[Employee]
	(
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[FirstName] [nvarchar](128) NOT NULL,
		[LastName] [nvarchar](128) NOT NULL,
		[Address] [nvarchar](128) NULL,
		[Phone] [nvarchar](128) NULL,
		[Age] [int] NULL,
		[Worth] [real] NULL,
		[Salary] [decimal](18, 2) NULL,
		[DateOfBirth] [datetime] NULL,
		[Gender] [smallint] NOT NULL,
		[Email] [nvarchar](128) NULL,
		[CreatedDate] [datetime] NOT NULL,
		[UpdatedDate] [datetime] NOT NULL
	) ON [PRIMARY]

	GO

	ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_Employee_CreatedDate]  DEFAULT (getutcdate()) FOR [CreatedDate]
	GO

	ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_Employee_UpdatedDate]  DEFAULT (getutcdate()) FOR [UpdatedDate]
	GO

.. highlight:: c#

Below are the results:

For inserting 500 rows in 500 interation.

::

	Dapper.Insert: 2.2524948 for 500 rows.
	RepoDb.Insert: 2.4751717 for 500 rows.
	RepoDb.BulkInsert: 0.0312229 for 500 rows.
	
.. image:: ../images/perf_benchmark_insert_500_rows.png

For inserting 1000 rows in 1000 interation.

::

	Dapper.Insert: 4.5111358 for 1000 rows.
	RepoDb.Insert: 4.8044364 for 1000 rows.
	RepoDb.BulkInsert: 0.0221467 for 1000 rows.
	
.. image:: ../images/perf_benchmark_insert_1k_rows.png

For querying 100K rows.

::

	Dapper.Query<Employee>: 0.7167928 for 100000 rows.
	RepoDb.DbRepository.Query<Employee>: 0.6705296 for 100000 rows.
	Dapper.Query (Dynamic): 0.8864116 for 100000 rows.
	RepoDb.Connection.ExecuteQuery (Dynamic - No IL): 1.2674217 for 100000 rows.
	
.. image:: ../images/perf_benchmark_query_100k_rows.png

For querying 500K rows.

::

	Dapper.Query<Employee>: 3.7871821 for 500000 rows.
	RepoDb.DbRepository.Query<Employee>: 3.6779512 for 500000 rows.
	Dapper.Query (Dynamic): 3.7625469 for 500000 rows.
	RepoDb.Connection.ExecuteQuery (Dynamic - No IL): 6.5918586 for 500000 rows.
	
.. image:: ../images/perf_benchmark_query_500k_rows.png

For querying 1M rows.

::

	Dapper.Query<Employee>: 7.5485485 for 1000000 rows.
	RepoDb.DbRepository.Query<Employee>: 7.5019162 for 1000000 rows.
	Dapper.Query (Dynamic): 8.0423502 for 1000000 rows.
	RepoDb.Connection.ExecuteQuery (Dynamic - No IL): 13.1152323 for 1000000 rows.
	
.. image:: ../images/perf_benchmark_query_1m_rows.png

Currently, `Dapper` is much more faster in `Insert` operation, but we find `RepoDb` much faster in `Query` operation.

Our performance benchmark tool can be downloaded at https://github.com/mikependon/RepoDb/tree/master/Docs.

The team is doing its best effort to further optimize the performance of the library. Stay stuned!