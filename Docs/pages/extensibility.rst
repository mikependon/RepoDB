Extensibility
=============

RepoDb is a hybrid-ORM for RDBMS. It has complete support for SQL Server databases and can be extended to support various data providers like SqLite, MySql, PostgreSQL and other RDBMS.

Below are the objects that can be extended.

- Caches
- Database Helpers
- Inline Hints
- Database Settings
- Repositories
- Resolvers
- Statement Builders
- Traces

Current implemented extension.

- RepoDb.MySql - `https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql/RepoDb.MySql <https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql/RepoDb.MySql>`_
- RepoDb.SqLite - `https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql/RepoDb.SqLite <https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql/RepoDb.SqLite>`_
- RepoDb.PostgreSql - `https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql/RepoDb.PostgreSql <https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql/RepoDb.PostgreSql>`_ (in-progress)

**Note**: RepoDb cannot be extended for NoSQL databases.