dotnet pack CoreUtils.csproj -c Release /p:Version=1.0.5
nuget add "bin\Release\CoreUtils.1.0.5.nupkg" -Source "D:\DEV\Nuget"