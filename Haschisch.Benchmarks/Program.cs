using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Haschisch.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"Supported options:
-j:<JobId>          activate benchmarking platforms
    clr_x86
    clr_x64
    core_x64
    mono_x64

-f                   accept somewhat lower accuracy for quicker runs
--quick-and-dirty    accept very low accuracy for quick bench runs

-c:<BenchmarkName>    activate a specific benchmark
    algo_block     Compare algorithms when hashing byte-arrays
    algo_combiner  Compare alg's when combining hash-codes
    algo_hashset   Compare alg's when accessing hash-sets

Example:
-j:clr_x64 --quick-and-dirty -c:algo_block
");
                return;
            }

            // Benchmarking Section -----
            //
            var jobs = new List<Job>();
            if (args.Contains("-j:clr_x86"))
            {
                jobs.Add(new Job("legacy-x86-32", Job.LegacyJitX86));
            }

            if (args.Contains("-j:clr_x64"))
            {
                jobs.Add(new Job("ryujit-x86-64", Job.Clr)
                {
                    Env = { Jit = Jit.RyuJit, Platform = Platform.X64 }
                });
            }

            if (args.Contains("-j:core_x64"))
            {
                jobs.Add(new Job("core-x86-64", Job.Core)
                {
                    Env = { Platform = Platform.X64 }
                });
            }

            if (args.Contains("-j:mono_x64"))
            {
                jobs.Add(new Job("mono-x86-64", Job.Mono)
                {
                    Env = { Platform = Platform.X64 }
                });
            }

            if (args.Contains("--quick"))
            {
                jobs = jobs
                    .Select(j => j.WithMinIterationTime(75 * TimeInterval.Millisecond).WithMaxRelativeError(0.075).WithId(j.Id))
                    .ToList();
            }

            if (args.Contains("--quick-and-dirty"))
            {
                jobs = jobs
                    .Select(j => j.WithMinIterationTime(10 * TimeInterval.Millisecond).WithMaxRelativeError(0.2).WithId(j.Id))
                    .ToList();
            }

            var cfg = ManualConfig
                .Create(DefaultConfig.Instance)
                .With(new ThroughputColumn())
                .With(jobs.ToArray());

            Summary summary;

            // Suite 1: Compare hash algorithm performance.
            //
            // Compare all interesting algorithms to each other using their fastest
            // implementation from this solution, which is usually their block-hasher
            // that uses unsafe array access, inlined-static methods for implementation which
            // access their hasher-state using local variables and make some funny copies
            // of those state-vars to help the jitter to generate not the worst possible code
            if (args.Contains("-c:algo_block")) { summary = BenchmarkRunner.Run<HashByteArray>(cfg); }

            // Suite 2: Compare algorithm performance for combining Int32 hash-codes
            //
            // This is mostly an exercise in very small initialization overhead and manually inlining
            // all code, as any kind of abstraction in .NET _costs_.
            if (args.Contains("-c:algo_combiner")) { summary = BenchmarkRunner.Run<CombineHashCodes>(cfg); }

            // Suite 3: Compare hash algorithm performance when used for .NET HashSet accesses
            //
            // A highly misleading benchmark as of now.
            if (args.Contains("-c:algo_hashset")) { summary = BenchmarkRunner.Run<HashSet_Complex>(cfg); }
            if (args.Contains("-c:algo_hashset_fill")) { summary = BenchmarkRunner.Run<HashSet_Fill>(cfg); }
            if (args.Contains("-c:algo_hashset_copy")) { summary = BenchmarkRunner.Run<HashSet_CopyCtor>(cfg); }
            if (args.Contains("-c:algo_hashset_remove")) { summary = BenchmarkRunner.Run<HashSet_Remove>(cfg); }
            if (args.Contains("-c:algo_hashset_lookup")) { summary = BenchmarkRunner.Run<HashSet_Lookup>(cfg); }

            // Suite 4, Part V: Comparing implementations for the same hash algorithm.
            if (args.Contains("-c:sip13")) { summary = BenchmarkRunner.Run<HashByteArray_CompareSip13>(cfg); }
            if (args.Contains("-c:sip24")) { summary = BenchmarkRunner.Run<HashByteArray_CompareSip24>(cfg); }
            if (args.Contains("-c:hsip13")) { summary = BenchmarkRunner.Run<HashByteArray_CompareHSip13>(cfg); }
            if (args.Contains("-c:hsip24")) { summary = BenchmarkRunner.Run<HashByteArray_CompareHSip24>(cfg); }
            if (args.Contains("-c:marvin32")) { summary = BenchmarkRunner.Run<HashByteArray_CompareMarvin32>(cfg); }
            if (args.Contains("-c:murmur3a")) { summary = BenchmarkRunner.Run<HashByteArray_CompareMurmur3x8632>(cfg); }
            if (args.Contains("-c:xx32")) { summary = BenchmarkRunner.Run<HashByteArray_CompareXXHash32>(cfg); }
            if (args.Contains("-c:xx64")) { summary = BenchmarkRunner.Run<HashByteArray_CompareXXHash64>(cfg); }
            if (args.Contains("-c:sea")) { summary = BenchmarkRunner.Run<HashByteArray_CompareSeaHash>(cfg); }
        }
    }
}
