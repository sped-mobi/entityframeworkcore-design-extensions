

$OutputDir = Join-Path -Path $PSScriptRoot -ChildPath "Data" -Verbose

Remove-Item -Path $OutputDir -Force -Recurse -Verbose

New-Item -ItemType Directory -Path $OutputDir -Verbose


Add-Migration -Name InitialMigres -OutputDir $OutputDir -Verbose
















