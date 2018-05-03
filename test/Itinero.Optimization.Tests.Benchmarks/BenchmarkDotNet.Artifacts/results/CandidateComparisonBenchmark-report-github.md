``` ini

BenchmarkDotNet=v0.10.14, OS=ubuntu 16.04
Intel Core i7-6560U CPU 2.20GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (CoreCLR 4.6.0.0, CoreFX 4.6.26018.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (CoreCLR 4.6.0.0, CoreFX 4.6.26018.01), 64bit RyuJIT


```
|                           Method |     Mean |    Error |   StdDev |
|--------------------------------- |---------:|---------:|---------:|
| CompareStructsGenericIComparable | 156.1 ns | 3.187 ns | 8.054 ns |
|   CompareClassGenericIComparable | 154.9 ns | 3.150 ns | 8.622 ns |
