dotnet restore
dotnet build

cd ./test/Itinero.Optimization.Test.Runner
dotnet run -c release
cd ../..

cd ./test/Itinero.Optimization.Test.Functional
dotnet run -c release
cd ../..