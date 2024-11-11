# Start instances of all database engines in clean docker instances
docker compose down
docker compose up -d

$cd = (Get-Location).Path
$framework = 'net8.0'

$tests = @(
  ($cd + "\RepoDb.Core\RepoDb.Tests\RepoDb.UnitTests\RepoDb.UnitTests.csproj")
  ($cd + "\RepoDb.MySql\RepoDb.MySql.UnitTests\RepoDb.MySql.UnitTests.csproj")
  ($cd + "\RepoDb.MySqlConnector\RepoDb.MySqlConnector.UnitTests\RepoDb.MySqlConnector.UnitTests.csproj")
  ($cd + "\RepoDb.PostgreSql\RepoDb.PostgreSql.UnitTests\RepoDb.PostgreSql.UnitTests.csproj")
  ($cd + "\RepoDb.Sqlite.Microsoft\RepoDb.Sqlite.Microsoft.UnitTests\RepoDb.Sqlite.Microsoft.UnitTests.csproj")
  ($cd + "\RepoDb.SQLite.System\RepoDb.SQLite.System.UnitTests\RepoDb.SQLite.System.UnitTests.csproj")
  ($cd + "\RepoDb.SqlServer\RepoDb.SqlServer.UnitTests\RepoDb.SqlServer.UnitTests.csproj")

  ($cd + "\RepoDb.Core\RepoDb.Tests\RepoDb.IntegrationTests\RepoDb.IntegrationTests.csproj")
  ($cd + "\RepoDb.MySql\RepoDb.MySql.IntegrationTests\RepoDb.MySql.IntegrationTests.csproj")
  ($cd + "\RepoDb.MySqlConnector\RepoDb.MySqlConnector.IntegrationTests\RepoDb.MySqlConnector.IntegrationTests.csproj")
  ($cd + "\RepoDb.PostgreSql\RepoDb.PostgreSql.IntegrationTests\RepoDb.PostgreSql.IntegrationTests.csproj")
  ($cd + "\RepoDb.Sqlite.Microsoft\RepoDb.Sqlite.Microsoft.IntegrationTests\RepoDb.Sqlite.Microsoft.IntegrationTests.csproj")
  ($cd + "\RepoDb.SQLite.System\RepoDb.SQLite.System.IntegrationTests\RepoDb.SQLite.System.IntegrationTests.csproj")
  ($cd + "\RepoDb.SqlServer\RepoDb.SqlServer.IntegrationTests\RepoDb.SqlServer.IntegrationTests.csproj")

  ($cd + "\RepoDb.Extensions\RepoDb.PostgreSql.BulkOperations\RepoDb.PostgreSql.BulkOperations.IntegrationTests\RepoDb.PostgreSql.BulkOperations.IntegrationTests.csproj")
  ($cd + "\RepoDb.Extensions\RepoDb.SqlServer.BulkOperations\RepoDb.SqlServer.BulkOperations.IntegrationTests\RepoDb.SqlServer.BulkOperations.IntegrationTests.csproj")
)

# Build all test project
foreach ($test in $tests) {
  dotnet build $test -f $framework
  if(-not $?){
    Exit(1)
  }
}

$env:REPODB_SQLSERVER_CONSTR_MASTER="Server=tcp:127.0.0.1,41433;Database=master;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;"
$env:REPODB_SQLSERVER_CONSTR_REPODB="Server=tcp:127.0.0.1,41433;Database=RepoDb;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;"
$env:REPODB_SQLSERVER_CONSTR_REPODBTEST="Server=tcp:127.0.0.1,41433;Database=RepoDbTest;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;"
$env:REPODB_POSTGRESQL_CONSTR_POSTGRESDB="Server=127.0.0.1;Port=45432;Database=postgres;User Id=postgres;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"
$env:REPODB_POSTGRESQL_CONSTR="Server=127.0.0.1;Port=45432;Database=RepoDb;User Id=postgres;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"
$env:REPODB_MYSQL_CONSTR_SYS="Server=127.0.0.1;Port=43306;Database=sys;User ID=root;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"
$env:REPODB_MYSQL_CONSTR_REPODB="Server=127.0.0.1;Port=43306;Database=RepoDb;User ID=root;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"
$env:REPODB_MYSQL_CONSTR_REPODBTEST="Server=127.0.0.1;Port=43306;Database=RepoDbTest;User ID=root;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"

#And run tests on these projects
foreach ($test in $tests) {
  dotnet test $test -f $framework --no-build -p:TestingPlatformShowTestsFailure=true -p:TestingPlatformCaptureOutput=false
  if(-not $?){
    Exit(1)
  }
}
