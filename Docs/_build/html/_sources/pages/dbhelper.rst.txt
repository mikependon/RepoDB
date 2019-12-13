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

By default, the `SqlServerDbHelper` class is provided by the library which is mainly used for SQL Server DB providers.

A code below is called in the static constructor of this class.

.. code-block:: c#
	:linenos:

	static DbHelperMapper()
	{
		// By default, map the Sql
		Add(typeof(SqlConnection), new SqlServerDbHelper());
	}

If however a custom `IDbHelper` has been introduced to be a helper method for other databases, let us say `MySql`, then it can
also be mapped using this class.

A code below is a simple call to map a customized `IDbHelper` class named `OracleDbHelper` into a `MySql` DB provider.

.. code-block:: c#
	:linenos:

	DbHelperMapper.Add(typeof(MySqlConnection), new MySqlDbHelper(), true);

The last `boolean` argument is used to override an existing mapping (if present). Otherwise, an exception will be thrown.

IDbHelper
---------

An interface used to mark the class to become a database helper. Below is a sample code that implements this interface.

.. code-block:: c#
	:linenos:

	public class CustomDbHelper : IDbHelper
	{
		public CustomDbHelper()
		{
			DbTypeResolver = new SqlDbTypeNameToClientTypeResolver();
		}

		public IResolver<string, Type> DbTypeResolver { get; }

		public IEnumerable<DbField> GetFields(IDbConnection connection, string tableName, IDbTransaction transaction = null)
		{
			...
		}

		public Task<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection, string tableName, IDbTransaction transaction = null)
		{
			...
		}

		public object GetScopeIdentity(IDbConnection connection, IDbTransaction transaction = null)
		{
			...
		}

		public Task<object> GetScopeIdentityAsync(IDbConnection connection, IDbTransaction transaction = null)
		{
			...
		}
	}

To map the `IDbHelper`, simply call the method below.

.. code-block:: c#
	:linenos:

	DbHelperMapper.Map(typeof(SqlConnection), new CustomDbHelper());

Once the class `CustomDbHelper` has been mapped to a specific DB Provider, then the library will automatically use it in all operations for that DB Provider.

SqlServerDbHelper
-----------------

This class is pre-registered as a default `IDbHelper` for SQL Server database (via `SqlConnection` object).

Click `here <https://github.com/mikependon/RepoDb/blob/master/RepoDb.Core/RepoDb/_SqlServer/DbHelpers/SqlServerDbHelper.cs>`_ to see the actual class implementation.
