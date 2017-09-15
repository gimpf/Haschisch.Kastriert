using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

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

--quick              accept somewhat lower accuracy for quicker runs
--quick-and-dirty    accept very low accuracy for quick bench runs

-- BENCHMARK-SWITCHER-OPTIONS

where BENCHMARK-SWITCHER-OPTIONS is
<BenchmarkTypeSelection> [--join] [--category=<CAT>] [--allcategories=<CATS>] [--anycategories=<CATS>]

<BenchmarkTypeSelection>: * | HashByteArray | CombineHashCode | AccessHashSet
<CAT>: sea, xx32, xx64, hsip, hsip-1-3, hsip-2-3, sip, sip-1-3, sip-2-4, city32, marvin32, murmur-3-32, spookyv2
       prime, variant
       array, combine, throughput, hashset

Examples:
-j:clr_x64 --quick-and-dirty -- * --join --allcategories=sea
-j:clr_x64 --quick -- * --join --allcategories=sea,throughput
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
                .With(ExecutionValidator.FailOnError)
                .With(jobs.ToArray());

            if (args.Contains("--"))
            {
                BenchmarkSwitcher
                    .FromAssembly(typeof(HashByteArray).Assembly)
                    .Run(args.SkipWhile(x => x != "--").Skip(1).ToArray(), cfg);
            }
        }
    }
}
