@echo off
bin\Release\net47\Haschisch.Benchmarks.Net47.exe -j:clr_x86 -j:clr_x64 -j:clr_x64_legacy -j:core_x86 -j:core_x64 -- --join * --allcategories=prime,throughput
del Benchmark.Results.7z
7z a Benchmark.Results.7z BenchmarkDotNet.Artifacts
