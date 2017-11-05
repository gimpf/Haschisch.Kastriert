@echo off
bin\Release\net47\Haschisch.Benchmarks.Net47.exe -j:clr_x86 -j:clr_x64 -j:clr_x64_legacy -- CombineHashCodes --allcategories=prime
del Benchmark.CLR.7z
7z a Benchmark.CLR.7z BenchmarkDotNet.Artifacts
del bin\Release\netcoreapp2.0\publish\Haschisch.Benchmarks.Spec.dll.config
dotnet bin\Release\netcoreapp2.0\publish\Haschisch.Benchmarks.dll -j:core_x64 -- CombineHashCodes --allcategories=prime
del Benchmark.Core.7z
7z a Benchmark.Core.7z BenchmarkDotNet.Artifacts
