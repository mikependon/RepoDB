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

	Dapper.Insert: 4.934 secs for 500 rows.
	RepoDb.Insert: 4.857 secs for 500 rows.
	RepoDb.BulkInsert: 0.011 secs for 500 rows.
	
.. image:: ../images/performance_insert_500_rows.png

For inserting 1000 rows in 1000 interation.

::

	Dapper.Insert: 11.241 secs for 1000 rows.
	RepoDb.Insert: 10.706 secs for 1000 rows.
	RepoDb.BulkInsert: 0.012 secs for 1000 rows.
	
.. image:: ../images/performance_insert_1k_rows.png

For querying 100K rows.

::

	Dapper.Query<Employee>: 0.661 secs for 100000 rows.
	RepoDb.DbRepository.Query<Employee>: 0.596 secs for 100000 rows.
	Dapper.Query (Dynamic): 0.636 secs for 100000 rows.
	
.. image:: ../images/performance_query_100k_rows.png

For querying 500K rows.

::

	Dapper.Query<Employee>: 3.168 secs for 500000 rows.
	RepoDb.DbRepository.Query<Employee>: 3.055 secs for 500000 rows.
	Dapper.Query (Dynamic): 3.276 secs for 500000 rows.
	
.. image:: ../images/performance_query_500k_rows.png

For querying 1M rows.

::

	Dapper.Query<Employee>: 6.277 secs for 1000000 rows.
	RepoDb.DbRepository.Query<Employee>: 6.048 secs for 1000000 rows.
	Dapper.Query (Dynamic): 5.991 secs for 1000000 rows.
	
.. image:: ../images/performance_query_1m_rows.png

Our performance benchmark tool can be downloaded at https://github.com/mikependon/RepoDb/tree/master/Docs.

The team is doing its best effort to further optimize the performance of the library. Stay stuned!

**Note**: I personally had discovered a more optimal way than the current performance of RepoDb. I can even make the performance fast enough with additional 40% performance gain (10 secs to be 6 secs) to be exact. I just could not release it as the code is a bit buggy at the 'Guid, Double, Decimal, Binary and other SQL Data Types'.