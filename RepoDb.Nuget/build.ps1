## Get the directories
Set-Location -Path "C:\Users\MichaelP\Source\Repos\GitHub\RepoDb\RepoDb.Nuget"
$currentLocation = (Get-Location).Path
$repoDbLocation = (Get-Item $currentLocation).Parent.FullName + "\RepoDb"
$repoDbStandardLocation = (Get-Item $currentLocation).Parent.FullName + "\RepoDb.Core"

## Write the locations
Write-Host "[Information]"
Write-Host ""
Write-Host "RepoDb (NETFramework):`t" + $repoDbLocation
Write-Host "RepoDb (NETStandard):`t" + $repoDbStandardLocation

## Verify from the user
#Write-Host ""
#$answer = Read-Host "Please verify the directories above. Proceed (Y/N)?"
#if ($answer.toString().toUpper() -ne "Y") {
#    Write-Host "User did not proceed."
#    return
#}

## Set the 'MSBuild' exe location
$msBuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

## Build the (NETFramework)
$filename = $repoDbLocation + "\RepoDb.sln"
Write-Host ""
Write-Host "----------------------------------------"
Write-Host "Building the RepoDb (Standard) solution"
Write-Host ""
& $msBuild $filename "/p:configuration=Release" "/t:Clean,Build"
Write-Host ""
Write-Host "Build completed for RepoDb (NETFramework) solution" -ForegroundColor Green

## Build the (NETStandard)
$filename = $repoDbStandardLocation + "\RepoDb.Core.sln"
Write-Host ""
Write-Host "----------------------------------------"
Write-Host "Building the RepoDb (NETStandard) solution"
Write-Host ""
& dotnet msbuild $filename "/p:configuration=Release" "/t:Clean,Build"
Write-Host ""
Write-Host "Build completed for RepoDb (NETStandard) solution" -ForegroundColor Green

## Copy the files for 'NETFramework'
$path = $repoDbLocation + "\RepoDb\bin\Release"
$dropoff = $currentLocation + "\lib\net40"
Write-Host ""
Write-Host "----------------------------------------"
Write-Host "Copying the (NETFramework) release files"
Write-Host "From:`t" + $path
Write-Host "To:`t`t" + $dropoff
Copy-item -Path ($path + "\RepoDb.dll") -Destination ($dropoff + "\RepoDb.dll") -Force -Recurse -Verbose
Copy-item -Path ($path + "\RepoDb.xml") -Destination ($dropoff + "\RepoDb.xml") -Force -Recurse -Verbose
Write-Host "Files for (NETFramework) has been copied to dropoff path." -ForegroundColor Green

## Copy the files for 'NETStandard'
$path = $repoDbStandardLocation + "\RepoDb\bin\Release\netstandard1.5"
$dropoff = $currentLocation + "\lib\netstandard1.3"
Write-Host ""
Write-Host "----------------------------------------"
Write-Host "Copying the (NETStandard) release files"
Write-Host "From:`t" + $path
Write-Host "To:`t`t" + $dropoff
Copy-item -Path ($path + "\RepoDb.dll") -Destination ($dropoff + "\RepoDb.dll") -Force -Recurse -Verbose
Copy-item -Path ($path + "\RepoDb.xml") -Destination ($dropoff + "\RepoDb.xml") -Force -Recurse -Verbose
Write-Host "Files for (NETStandard) has been copied to dropoff path." -ForegroundColor Green

## Execute the 'Nuget' pack
$nuget = $currentLocation + "\nuget.exe"
Write-Host ""
Write-Host "----------------------------------------"
Write-Host "Executing Nuget Pack Command"
& $nuget "pack"
Write-Host "The Nuget package has been created successfully." -ForegroundColor Green