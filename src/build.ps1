Param ([string]$Version = "0.1-debug")
$ErrorActionPreference = "Stop"
pushd $(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

dotnet clean
dotnet msbuild /t:Restore /t:Build /p:Configuration=Release /p:Version=$Version

popd
