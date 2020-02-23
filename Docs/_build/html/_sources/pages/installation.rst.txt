Installation
============

**Pre-requisite**: A Microsoft Visual Studio has already been installed and a .NET Solution and Project has already been created.

SQL Server
----------

In your Package Manager Console, type the following command.

.. code-block:: c#
	:linenos:

	> Install-Package RepoDb.SqlServer

Then, bootstrap your SQL Server settings once.

.. code-block:: c#
	:linenos:

	SqlServerBootstrap.Initialize();

To work with `BulkOperations`, type the following command in your Package Manager Console.

.. code-block:: c#
	:linenos:

	> Install-Package RepoDb.SqlServer.BulkOperations

SQLite
------

In your Package Manager Console, type the following command.

.. code-block:: c#
	:linenos:

	> Install-Package RepoDb.SqLite

Then, bootstrap your SQLite settings once.

.. code-block:: c#
	:linenos:

	SqLiteBootstrap.Initialize();

MySql.Data
----------

In your Package Manager Console, type the following command.

.. code-block:: c#
	:linenos:

	> Install-Package RepoDb.MySql

Then, bootstrap your MySql settings once.

.. code-block:: c#
	:linenos:

	MySqlBootstrap.Initialize();

PostgreSql
----------

In your Package Manager Console, type the following command.

.. code-block:: c#
	:linenos:

	> Install-Package RepoDb.PostgreSql

Then, bootstrap your PostgreSQL settings once.

.. code-block:: c#
	:linenos:

	PostgreSqlBootstrap.Initialize();