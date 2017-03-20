dotnet restore
dotnet build ./src/Itinero.Optimization -f netstandard1.3
dotnet build ./test/Itinero.Optimization.Test
dotnet build ./test/Itinero.Optimization.Test.Functional
dotnet build ./test/Itinero.Optimization.Test.Runner