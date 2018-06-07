``` ini

BenchmarkDotNet=v0.10.14, OS=ubuntu 16.04
Intel Core i7-6560U CPU 2.20GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.1.300
  [Host]     : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT


```
|    Method |     Mean |     Error |    StdDev |
|---------- |---------:|----------:|----------:|
| SolveBR17 | 16.05 ms | 0.3128 ms | 0.3477 ms |
