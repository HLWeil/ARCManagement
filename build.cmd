@echo off
setlocal
pushd "%~dp0"
dotnet tool restore
if errorlevel 1 goto :failed
dotnet run --project build\Build.fsproj -- %*
set "build_exit=%errorlevel%"
popd
exit /b %build_exit%
:failed
set "build_exit=%errorlevel%"
popd
exit /b %build_exit%
