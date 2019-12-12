DB Setting
==========

Is used to define a DB Provider specific setting.

IDbSetting
----------

Is an interfaced used to mark the class to become a DB Provider setting.

DbSettingMapper
---------------

A class used to map the specific DB Provider Setting into the target DB Provider.

.. code-block:: c#
	:linenos:

	DbSettingMapper.Map(typeof(SqlConnection), new SqlServerDbSetting());

Why DbSetting?
--------------

The RepoDb library is very extensible. When the other Db Provider (beside SQL Server) has been supported, specific setting value needs to be provisioned in order to target the specific DB Provider.

One example is the quotation when creating a SQL Statement. Quoting in `SQL Server` is done via `[` and `]` characters, however it is `\`` character in `MySql`.

Creating a DbSetting
--------------------

Below is the code on how to implement a `DbSetting` object for `MySql` DB Provider.

.. code-block:: c#
	:linenos:

	public class MySqlDbSetting : IDbSetting
	{
		public string ClosingQuote { get; } = "`";
		public string OpeningQuote { get; } = "`";
	}

And below for `SQL Server` DB Provider.

.. code-block:: c#
	:linenos:

	public class SqlServerDbSetting : IDbSetting
	{
		public string ClosingQuote { get; } = "[";
		public string OpeningQuote { get; } = "]";
	}

Then, specific setting must be injected properly targetting the proper DB Provider.

.. code-block:: c#
	:linenos:

	DbSettingMapper.Map(typeof(MySqlConnection), new MySqlDbSetting());
	DbSettingMapper.Map(typeof(SqlConnection), new SqlServerDbSetting());

Click `here <https://github.com/mikependon/RepoDb/blob/master/RepoDb.Core/RepoDb/_SqlServer/DbSettings/SqlServerDbSetting.cs>`_ to see the actual class implementation.