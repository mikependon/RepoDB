DB Helper
=========

A feature which allows developer to implement and override the database helper methods used by the library.

**How is it being used?**

When any of the operation of the `Repository` or `DbConnection` extended methods has been called, the library 
is pre-touching the databases to pre-cached all the schemas for performance purposes. 

The classes that are mainly used to pre-touched the database is implemented through `IDbHelper`.

DbHelperMapper
--------------

This class is used to map the `Type` of database provider into an instance of `IDbHelper` object.

By default, the `SqlDbHelper` class is provided by the library which is mainly used for SQL Server DB providers.

A code below is called in the static constructor of this class.

.. highlight:: c#

::

	static DbHelperMapper()
	{
		// By default, map the Sql
		Add(typeof(SqlConnection), new SqlDbHelper());
	}

If however a custom `IDbHelper` has been introduced to be a helper method for other databases, let us say `Oracle`, then it can
also be mapped using this class.

A code below is a simple call to map a customized `IDbHelper` class named `OracleDbHelper` into an `Oracle` DB provider.

::

	DbHelperMapper.Add(typeof(OracleConnection), new OracleDbHelper(), true);

The last `boolean` argument is used to override an existing mapping (if present). Otherwise, an exception will be thrown.

IDbHelper
---------

An interface used to mark the class to become a database helper. Below is a sample code that implements this interface.

.. highlight:: c#

::

	public class CustomDbHelper : IDbHelper
	{
		public CustomDbHelper()
		{
			DbTypeResolver = new SqlDbTypeNameToClientTypeResolver();
		}

		public IResolver<string, Type> DbTypeResolver { get; }

		public IEnumerable<DbField> GetFields(string connectionString, string tableName)
		{
			...
		}
	}

Once the class `CustomDbHelper` has been mapped to a specific DB Provider, then the library will automatically use it in most operations.

SqlDbHelper
-----------

This class is used to retrieve the list of columns from a SQL Server database table. By default, the library has mapped this class into a `SqlConnection` DB provider.

Below is the implementation of this class.

.. highlight:: c#

::

	public class SqlDbHelper : IDbHelper
    {
		public SqlDbHelper()
		{
			DbTypeResolver = new SqlDbTypeNameToClientTypeResolver();
		}

		public IEnumerable<DbField> GetFields(string connectionString, string tableName)
        {
			/* Querying the INFORMATION_SCHEMA and convert it back to DbField objects */
		}

		public IResolver<string, Type> DbTypeResolver { get; }
	}

Click `here <https://github.com/mikependon/RepoDb/blob/master/RepoDb/RepoDb/DbHelpers/SqlDbHelper.cs>`_ to see the actual class implementation.