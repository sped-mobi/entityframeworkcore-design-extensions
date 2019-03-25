
Clear-Host

$Models = Join-Path -Path $PWD -ChildPath "Models" -Verbose
$Context = Join-Path -Path $PWD -ChildPath "Context" -Verbose

if (Test-Path $Models)
{
    #Remove-Item -Path $Models -Recurse -Force
}

if (Test-Path $Context)
{
    #Remove-Item -Path $Context -Recurse -Force
}


Scaffold-DbContext -Connection "Server=BACKTRACK;Database=OpenLMSDB;Trusted_Connection=True;MultipleActiveResultSets=true" -Provider Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir Context -Context OpenLMSDbContext -Force -Verbose


