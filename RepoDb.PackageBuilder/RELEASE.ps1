# dotnet build .\RepoDb.Solutions.sln -c Release -v n -p:Version='0.0.0-prefix',FileVersion='0.0.0.0',AssemblyVersion='0.0.0.0'

Function Ensure-Argument ($value, $prompt_message) {
    
    if ([string]::IsNullOrEmpty($value)) {
        $value = Read-Host -Prompt $prompt_message
    } else {
        Write-Host ($prompt_message + ": " + "$value")
    }

    return $value
}

$package_version = Ensure-Argument $args[0] 'Package Version'
$file_version = Ensure-Argument $args[1] 'File Version'
$filename = ((Get-Location).Path + "\RepoDb.Core\RepoDb.Solutions.sln")
dotnet build $filename -c Release -v n -p:Version=$package_version,FileVersion=$file_version,AssemblyVersion=$file_version