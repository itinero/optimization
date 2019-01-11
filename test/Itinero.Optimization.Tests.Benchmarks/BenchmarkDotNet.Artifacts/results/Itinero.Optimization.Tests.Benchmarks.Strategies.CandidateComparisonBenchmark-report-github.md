``` ini

BenchmarkDotNet=v0.11.3, OS=ubuntu 16.04
Intel Core i7-6560U CPU 2.20GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.100
  [Host]     : .NET Core 2.1.6 (CoreCLR 4.6.27019.06, CoreFX 4.6.27019.05), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.6 (CoreCLR 4.6.27019.06, CoreFX 4.6.27019.05), 64bit RyuJIT


```
|                           Method |     Mean |     Error |    StdDev |
|--------------------------------- |---------:|----------:|----------:|
| CompareStructsGenericIComparable | 11.31 ns | 0.1568 ns | 0.1466 ns |
|   CompareClassGenericIComparable | 11.33 ns | 0.0968 ns | 0.0905 ns |
