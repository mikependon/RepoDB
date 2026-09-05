# 🛡️ Security Policy

RepoDB takes security seriously, but is honest about what that means in practice: RepoDB is primarily maintained by a single individual, not a security team or a vendor with an SLA (see our [Support Policy](src/Shared/RepoDb.Docs/support-policy.md) and the [Governance](src/Shared/RepoDb.Docs/support-policy.md#governance) section). This document explains what is and isn't covered, how to report a vulnerability, and how we mitigate the security risks specific to a data-access library.

## 📦 Supported Versions

Only the **latest published release** of each RepoDB package (see the [package list](PACKAGES.md)) receives security fixes. There is no formal backport policy — fixes ship forward as a new version, not as a patch to an older major/minor line (see the [versioning notice](README.md#-enterprise-notice) in the root README).

If you're running an older version, upgrade to the latest before reporting — the issue may already be fixed.

## 🚨 Reporting a Vulnerability

**Do not open a public GitHub issue for a suspected vulnerability.** Instead:

1. 📧 Email **[michael.c.pendon@outlook.com](mailto:michael.c.pendon@outlook.com)** with a subject line starting with `[SECURITY]`.
2. 📝 Include the affected package(s) and version(s), the database provider involved (if relevant), a minimal reproduction or proof of concept, and the impact you believe it has.
3. ⏱️ You'll get an acknowledgment during the hours defined in the [Support Policy](src/Shared/RepoDb.Docs/support-policy.md) (CET timezone; single maintainer — there is no 24/7 security desk).

There is no bug bounty program. Coordinated disclosure is appreciated: please give us a reasonable window to ship a fix before disclosing publicly, and we'll credit you in the release notes if you'd like.

## 🔎 Known Risk Areas and How We Mitigate Them

RepoDB's design surfaces a few security-relevant areas worth calling out explicitly, rather than leaving them implicit:

- **💉 Raw SQL execution** — [ExecuteQuery](http://repodb.net/operation/executequery), [ExecuteNonQuery](http://repodb.net/operation/executenonquery), and related methods run SQL text you provide. RepoDB parameterizes every fluent operation ([Insert](http://repodb.net/operation/insert), [Query](http://repodb.net/operation/query), [Update](http://repodb.net/operation/update), [Delete](http://repodb.net/operation/delete), [Merge](http://repodb.net/operation/merge), etc.) by default, but the raw-SQL methods are only as safe as the SQL you hand them. Always pass user input as parameters (e.g. `@Name`), never by string-concatenating it into the command text.
- **🔑 No credential handling** — RepoDB never stores, logs, or transmits your connection string or credentials. You own the `IDbConnection` and its connection string; RepoDB only consumes it for the lifetime of the call. Review the [Telemetry](README.md#telemetry) feature separately if you enable it, since it publishes operation metadata (not connection strings or row-level data) to a collector you configure.
- **🪞 Reflection-based provider internals** — A small number of bulk-operation code paths use reflection against non-public members of underlying ADO.NET provider types (documented in [Limitations](src/Shared/RepoDb.Docs/limitations.md), e.g. `SqlBulkCopy` internals). These are reviewed and covered by tests, but they depend on the internal shape of a specific provider driver version. Pin your driver package version in production and re-run your test suite after upgrading it.
- **📦 Third-party provider dependencies** — RepoDB depends on the official ADO.NET drivers for each provider (Npgsql, MySql.Data, MySqlConnector, Oracle.ManagedDataAccess.Core, Net.IBM.Data.Db2, ClickHouse.Driver, FirebirdSql.Data.FirebirdClient, Vertica.Data, Sap.Data.Hana.Net.v6.0, etc. — see [Credits](README.md#credits)). RepoDB does not currently run automated dependency-vulnerability scanning (e.g. Dependabot/CodeQL) across this repository. Track and update these driver dependencies in your own project independently; a CVE in a driver is not necessarily reflected in a new RepoDB release.
- **✅ Continuous testing, not a security audit** — Every provider has a dedicated unit/integration test suite that runs in CI on every pull request and release (see [Packages and Build Status](PACKAGES.md)). This catches regressions and behavioral bugs, but RepoDB has not undergone a formal third-party security audit or penetration test.

## 🗺️ Scope

This policy covers the RepoDB libraries in this repository (core, provider packages, and bulk-operations add-ons). It does not cover the [documentation site](https://github.com/mikependon/RepoDb.NET) repository or third-party packages that merely integrate with RepoDB — report those upstream.
