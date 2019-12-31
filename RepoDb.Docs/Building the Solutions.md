## Building the Solutions

In this page, we will guide you on how to build the *RepoDb* Solutions.

- [Building the RepoDb.Core](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Building%20the%20Solutions.md#building-the-repodbcore)
- [Building the RepoDb.SqLite](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Building%20the%20Solutions.md#building-the-repodbsqlite)
- [Building the RepoDb.MySql](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Building%20the%20Solutions.md#building-the-repodbmysql)

## Install the Git

To install, please follow this [guide](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).

## Create the 'src' folder

1. Open your command prompt.
2. Type the command below.

	```
	> mkdir c:\src
	> cd c:\src
	```

## Clone the Repository

1. Navigate to https://github.com/mikependon/RepoDb.
2. Click the *Clone or download* button.
3. Copy the provided URL.

	URL = https://github.com/mikependon/RepoDb.git

4. Open your command prompt.
5. Type the command below.

	```
	> cd c:\src
	> git clone https://github.com/mikependon/RepoDb.git
	```

--------

## Building the RepoDb.Core

Please follow the steps below to build and run the *RepoDb.Core* solutions.

### Pre-requisites

Atleast, the following softwares must be installed in your local machine.

- Microsoft Visual Studio 2017
- Microsoft SQL Server 2016
- Microsoft SSMS 2017

### Building the Solution

1. Open the *RepoDb.Core* folder.
2. Double click the *RepoDb.Core.sln*.
3. Right-click the *RepoDb.Core* solution and click the *Rebuild Solution* context menu.

### Running the RepoDb.IntegrationTests

1. Open the Microsoft SQL Server Management Studio 2017. Enter the values below.

	- Server type = *Database Engine*
	- Server name = *.*
	- Authentication = *Windows Authentication*

2. Expand the *Security* node.
3. Right-click the *Logins* node and click the *New Login...* context menu. Enter the values below.

	- Login name = *michael*
	- SQL Server authentication = *checked*
	- Password = *Password123*

4. In your Microsoft Visual Studio's, click the *Test* menu and click the *Windows* > *Text Explorer*.

	- Alternatively, press the `Ctrl + E, T`.

5. In the *Test Explorer* window, click the *Group By Class*.
6. In the *Test Explorer* window, right-click the *RepoDb.IntegrationTests* and then click the *Run Selected Tests* context menu.

### Running the RepoDb.UnitTests

1. Do the items *1* to *5* of the previous section.
2. In the *Test Explorer* window, right-click the *RepoDb.UnitTests* and then click the *Run Selected Tests* context menu.

--------

## Building the RepoDb.SqLite

Please follow the steps below to build and run the *RepoDb.SqLite* solutions.

### Pre-requisites

The *SQLite* and *SQLite Studio* must be installed. To install, please follow this [guide](https://www.sqlitetutorial.net/download-install-sqlite/).

### Building the Solution

1. Open the *RepoDb.SqLite* folder.
2. Double click the *RepoDb.SqLite.sln*.
3. Right-click the *RepoDb.SqLite* solution and click the *Rebuild Solution* context menu.

### Running the RepoDb.SqLite.IntegrationTests

1. Open the SQLite Studio.
2. Click the *Database* > *Add a Database* menu.

	- Database type = *SQLite 3*
	- File = *C:\SqLite\Databases\RepoDb.db*
	- Name (on the list) = *RepoDb*
	- Permanent (keep it in configuration) = *checked*

3. In your Microsoft Visual Studio's, click the *Test* menu and click the *Windows* > *Text Explorer*.

	- Alternatively, press the *Ctrl + E, T*.

5. In the *Test Explorer* window, click the *Group By Class*.
6. In the *Test Explorer* window, right-click the *RepoDb.SqLite.IntegrationTests* and then click the *Run Selected Tests* context menu.

### Running the RepoDb.SqLite.UnitTests

1. Do the items *1* to *5* of the previous section.
2. In the *Test Explorer* window, right-click the *RepoDb.SqLite.UnitTests* and then click the *Run Selected Tests* context menu.

--------

## Building the RepoDb.MySql

Please follow the steps below to build and run the *RepoDb.MySql* solutions.

### Pre-requisites

The *MySql* and *MySQL WorkBench CE 8.0* must be installed. To install, please follow this [guide](https://dev.mysql.com/doc/refman/8.0/en/windows-installation.html).

### Building the Solution

1. Open the *RepoDb.MySql* folder.
2. Double click the *RepoDb.MySql.sln*.
3. Right-click the *RepoDb.MySql* solution and click the *Rebuild Solution* context menu.

### Running the RepoDb.MySql.IntegrationTests

1. Open the MySQL Workbench CE 8.0.
2. Double click the *Local instance MySQL80*.
3. Click the *View* > *Panels* > *Show Sidebar*.
4. In the *Navigator* window *Schemas* tab, right-click the canvass and click the *Create Schema...* context menu. Enter the values below.

	- Name = *RepoDb*
	- Charset/Collation = *Default Charset*, *Default Collation*

5. Click the *Apply* button.
6. In the *Navigator* window *Administration* tab, under *MANAGEMENT* section, click the *Users and Privileges*.
7. Click *Add Account* button. Enter the value below.

	- Login Name type = *User*
	- Authentication Type = *Standard*
	- Limit to Hosts Matching = *%*
	- Password = *Password123*

8. Click the *Schema Privileges* tab.
9. Click the *Add Entry...* button.
10. In the new window, choose the *Selected schema* and select *RepoDb*.
11. In the section group of *Object Rights* and *DDL Rights*, select all the entries.
12. Click the *Apply* button.
13. In the *Test Explorer* window, click the *Group By Class*.
14. In the *Test Explorer* window, right-click the *RepoDb.MySql.IntegrationTests* and then click the *Run Selected Tests* context menu.

### Running the RepoDb.MySql.UnitTests

1. Do the items *1* to *13* of the previous section.
2. In the *Test Explorer* window, right-click the *RepoDb.MySql.UnitTests* and then click the *Run Selected Tests* context menu.
