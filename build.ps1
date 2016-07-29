# restore and builds all projects as release.
# creates NuGet package at \src\SplitSharp\bin\Release
dotnet restore .\src\SplitSharp\ 
dotnet restore .\tests\SplitSharp.Tests\ 
dotnet pack .\src\SplitSharp\  --configuration release -o artifacts
dotnet build .\tests\SplitSharp.Tests\  --configuration release 
