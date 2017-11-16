del *.nupkg
..\tools\nuget.exe pack PaymentKit.csproj
..\tools\nuget.exe push *.nupkg 6c5bfb82-54c4-4eb9-a335-2f000888661d -Source https://www.nuget.org/api/v2/package
