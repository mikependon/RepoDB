param(
    [switch]$NoRestore
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../..'))
$toolDirectory = Join-Path $repositoryRoot 'artifacts/Science/tools'
$reportDirectory = Join-Path $repositoryRoot 'artifacts/Science/Coverage'
$toolName = if ($env:OS -eq 'Windows_NT') { 'dotnet-coverage.exe' } else { 'dotnet-coverage' }
$coverageTool = Join-Path $toolDirectory $toolName
$project = Join-Path $PSScriptRoot 'RepoDb.Science.DataFrame.UnitTests.csproj'
$settings = Join-Path $PSScriptRoot 'coverage.settings.xml'

if (-not (Test-Path -LiteralPath $coverageTool)) {
    & dotnet tool install dotnet-coverage --version 18.12.0 --tool-path $toolDirectory
    if ($LASTEXITCODE -ne 0) { throw 'Coverage tool installation failed.' }
}

$buildArguments = @('build', $project, '--nologo', '-p:TreatWarningsAsErrors=true')
if ($NoRestore) { $buildArguments += '--no-restore' }
& dotnet @buildArguments
if ($LASTEXITCODE -ne 0) { throw 'DataFrame test build failed.' }

foreach ($framework in @('net10.0', 'net11.0')) {
    $assembly = Join-Path $PSScriptRoot "bin/Debug/$framework/RepoDb.Science.DataFrame.UnitTests.dll"
    $report = Join-Path $reportDirectory "$framework.cobertura.xml"
    & $coverageTool collect dotnet $assembly --settings $settings --output $report --output-format cobertura --nologo
    if ($LASTEXITCODE -ne 0) { throw "DataFrame tests or coverage collection failed for $framework." }

    [xml]$coverage = Get-Content -LiteralPath $report -Raw
    $lineCount = [int]$coverage.coverage.'lines-valid'
    $branchCount = [int]$coverage.coverage.'branches-valid'
    if ($lineCount -eq 0 -or $branchCount -eq 0) { throw "Coverage report is empty for $framework." }
    if ($coverage.coverage.'lines-covered' -ne $coverage.coverage.'lines-valid' -or
        $coverage.coverage.'branches-covered' -ne $coverage.coverage.'branches-valid') {
        throw "Library coverage is below 100% for $framework. Inspect $report."
    }
    Write-Output "$framework : 100% lines ($lineCount/$lineCount), 100% branches ($branchCount/$branchCount)."
}
