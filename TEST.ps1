Write-Host "[Test Execution Options]"
Write-Host "0. All Tests"
Write-Host "1. RepoDb.Core"
Write-Host "2. RepoDb.SqlServer"
Write-Host "3. RepoDb.SqlServer.BulkOperations"
Write-Host "4. RepoDb.Sqlite.Microsoft"
Write-Host "5. RepoDb.SQLite.System"
Write-Host "6. RepoDb.PostgreSql"
Write-Host "7. RepoDb.PostgreSql.BulkOperations"
Write-Host "8. RepoDb.MySql"
Write-Host "9. RepoDb.MySqlConnector"

Function Ensure-Argument ($value, $prompt_message) {
    
    if ([string]::IsNullOrEmpty($value)) {
        $value = Read-Host -Prompt $prompt_message
    } else {
        Write-Host ($prompt_message + ": " + "$value")
    }

    return $value
}

$option = Ensure-Argument $args[0] 'Option'
$current_location = (Get-Location).Path

# RepoDb.Core
if ($option -eq 1 -or $option -eq 0)
{
    Write-Host "Executing the tests for '1. RepoDb.Core'"
    dotnet test ($current_location + "\RepoDb.Core\RepoDb.Tests\RepoDb.UnitTests\RepoDb.UnitTests.csproj") -c Release -f net6.0 # -t
    dotnet test ($current_location + "\RepoDb.Core\RepoDb.Tests\RepoDb.IntegrationTests\RepoDb.IntegrationTests.csproj") -c Release -f net6.0 # -t
}

# RepoDb.SqlServer
if ($option -eq 2 -or $option -eq 0)
{
    Write-Host "Executing the tests for '2. RepoDb.SqlServer'"
    dotnet test ($current_location + "\RepoDb.SqlServer\RepoDb.SqlServer.UnitTests\RepoDb.SqlServer.UnitTests.csproj") -c Release -f net6.0 # -t
    dotnet test ($current_location + "\RepoDb.SqlServer\RepoDb.SqlServer.IntegrationTests\RepoDb.SqlServer.IntegrationTests.csproj") -c Release -f net6.0 # -t
}

# RepoDb.SqlServer.BulkOperations
if ($option -eq 3 -or $option -eq 0)
{
    Write-Host "Executing the tests for '3. RepoDb.SqlServer.BulkOperations'"
    dotnet test ($current_location + "\RepoDb.Extensions\RepoDb.SqlServer.BulkOperations\RepoDb.SqlServer.BulkOperations.IntegrationTests\RepoDb.SqlServer.BulkOperations.IntegrationTests.csproj") -c Release -f net6.0 # -t
}

# RepoDb.Sqlite.Microsoft
if ($option -eq 4 -or $option -eq 0)
{
    Write-Host "Executing the tests for '4. RepoDb.Sqlite.Microsoft'"
    dotnet test ($current_location + "\RepoDb.Sqlite.Microsoft\RepoDb.Sqlite.Microsoft.UnitTests\RepoDb.Sqlite.Microsoft.UnitTests.csproj") -c Release -f net6.0 # -t
    dotnet test ($current_location + "\RepoDb.Sqlite.Microsoft\RepoDb.Sqlite.Microsoft.IntegrationTests\RepoDb.Sqlite.Microsoft.IntegrationTests.csproj") -c Release -f net6.0 # -t
}

# RepoDb.SQLite.System
if ($option -eq 5 -or $option -eq 0)
{
    Write-Host "Executing the tests for '5. RepoDb.SQLite.System'"
    dotnet test ($current_location + "\RepoDb.SQLite.System\RepoDb.SQLite.System.UnitTests\RepoDb.SQLite.System.UnitTests.csproj") -c Release -f net6.0 # -t
    dotnet test ($current_location + "\RepoDb.SQLite.System\RepoDb.SQLite.System.IntegrationTests\RepoDb.SQLite.System.IntegrationTests.csproj") -c Release -f net6.0 # -t
}

# RepoDb.PostgreSql
if ($option -eq 6 -or $option -eq 0)
{
    Write-Host "Executing the tests for '6. RepoDb.PostgreSql'"
    dotnet test ($current_location + "\RepoDb.PostgreSql\RepoDb.PostgreSql.UnitTests\RepoDb.PostgreSql.UnitTests.csproj") -c Release -f net6.0 # -t
    dotnet test ($current_location + "\RepoDb.PostgreSql\RepoDb.PostgreSql.IntegrationTests\RepoDb.PostgreSql.IntegrationTests.csproj") -c Release -f net6.0 # -t
}

# RepoDb.PostgreSql.BulkOperations
if ($option -eq 7 -or $option -eq 0)
{
    Write-Host "Executing the tests for '7. RepoDb.PostgreSql.BulkOperations'"
    dotnet test ($current_location + "\RepoDb.Extensions\RepoDb.PostgreSql.BulkOperations\RepoDb.PostgreSql.BulkOperations.IntegrationTests\RepoDb.PostgreSql.BulkOperations.IntegrationTests.csproj") -c Release -f net6.0 # -t
}

# RepoDb.MySql
if ($option -eq 8 -or $option -eq 0)
{
    Write-Host "Executing the tests for '8. RepoDb.MySql'"
    dotnet test ($current_location + "\RepoDb.MySql\RepoDb.MySql.UnitTests\RepoDb.MySql.UnitTests.csproj") -c Release -f net6.0 # -t
    dotnet test ($current_location + "\RepoDb.MySql\RepoDb.MySql.IntegrationTests\RepoDb.MySql.IntegrationTests.csproj") -c Release -f net6.0 # -t
}

# RepoDb.MySqlConnector
if ($option -eq 9 -or $option -eq 0)
{
    Write-Host "Executing the tests for '9. RepoDb.MySqlConnector'"
    dotnet test ($current_location + "\RepoDb.MySqlConnector\RepoDb.MySqlConnector.UnitTests\RepoDb.MySqlConnector.UnitTests.csproj") -c Release -f net6.0 # -t
    dotnet test ($current_location + "\RepoDb.MySqlConnector\RepoDb.MySqlConnector.IntegrationTests\RepoDb.MySqlConnector.IntegrationTests.csproj") -c Release -f net6.0 # -t
}