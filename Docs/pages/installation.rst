Installation
============

**Pre-requisite**: A Microsoft Visual Studio has already been installed and a .NET Solution and Project has already been created.

There are two ways of installing the library as standardized by Microsoft.

Nuget Package
-------------

Once the project has been opened, then do the following steps:

1. Right click on the solution and click the `Manage Nuget Packages for Solution...`.
2. Select the `Browse` tab.
3. In the `Search` field, type `RepoDb` and press `Enter`.
4. In the right side, select the project you wish the library to install to (ie: `RepoDb.SqlServer`, `RepoDb.MySql`, `RepoDb.PostgreSql`, `RepoDb.SqLite` and `RepoDb.SqlServer.BulkOperations`).
5. Click the `Install` button.

Wait for few seconds until the installation is completed.

Package Manager
---------------

Once the project has been opened, then do the following steps:

1. Select the `View -> Other Windows -> Package Manager Console`.
2. In the `Package Source` drop down field, select `All`.
3. In the `Default Project` drop down field, select the project you wish the library to install to.
4. In the `Package Manager Console`, type any of the command below and press `Enter`.

.. code-block:: c#
	:linenos:

	Install-Package RepoDb.SqlServer
	Install-Package RepoDb.MySql
	Install-Package RepoDb.PostgreSql
	Install-Package RepoDb.SqLite
	Install-Package RepoDb.SqlServer.BulkOperations

Wait for few seconds until the installation is completed.