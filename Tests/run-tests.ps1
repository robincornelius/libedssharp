param(
    [ValidateSet('x64', 'x86', 'All')]
    [string]$Platform = 'All',
    [ValidateSet('Debug', 'Release', 'All')]
    [string]$Configuration = 'All',
    [switch]$NoBuild
)
$ErrorActionPreference = 'Stop'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuild)
{
    throw 'Install MSBuild and the .NET Framework 4.8 targeting pack.'
}
$architectures = if ($Platform -eq 'All') { @('x64', 'x86') } else { @($Platform) }
$configurations = if ($Configuration -eq 'All') { @('Debug', 'Release') } else { @($Configuration) }
foreach ($architecture in $architectures)
{
    foreach ($variant in $configurations)
    {
        if (-not $NoBuild)
        {
            $solutionPlatform = if ($architecture -eq 'x64') { 'x64' } else { 'Any CPU' }
            & $msbuild "$PSScriptRoot/../EDSEditor.sln" /t:Restore /p:RestorePackagesConfig=true "/p:Platform=$solutionPlatform" /nologo /v:minimal
            if ($LASTEXITCODE -ne 0)
            {
                throw 'Restore failed.'
            }
            & $msbuild "$PSScriptRoot/../EDSEditor.sln" /t:Build "/p:Platform=$solutionPlatform" "/p:Configuration=$variant" /nologo /v:minimal
            if ($LASTEXITCODE -ne 0)
            {
                throw "Build failed: $architecture $variant"
            }
        }
        $runner = if ($architecture -eq 'x64') { 'xunit.console.exe' } else { 'xunit.console.x86.exe' }
        $assembly = if ($architecture -eq 'x64') { "$PSScriptRoot/bin/x64/$variant/Tests.dll" } else { "$PSScriptRoot/bin/$variant/Tests.dll" }
        & "$PSScriptRoot/../packages/xunit.runner.console.2.9.3/tools/net48/$runner" $assembly -nologo
        if ($LASTEXITCODE -ne 0)
        {
            throw "EDS tests failed: $architecture $variant. Review the test output."
        }
    }
}
