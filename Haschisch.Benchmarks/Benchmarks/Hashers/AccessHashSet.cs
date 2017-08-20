using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;
using Haschisch.Tests;

namespace Haschisch.Benchmarks
{
    // Compares different hash-functions for suitability for use as a key in .NETs
    // HashSet/Dictionary default implementations.
    //
    // BEWARE!!!
    // 
    // as it stands now, likely misleading
    // (although, as all algos perform almost the same, might be just doing fine)
    public class AccessHashSet
    {
        private Large[] data;

        [Params(10)]
        public int DataSize { get; set; }

        [Params(100)]
        public int ItemCount { get; set; }

        [Params(10_000)]
        public int LookupCount { get; set; }

        [GlobalSetup]
        public void InitializeData()
        {
            this.data = Data.Generate.LargeItems(this.DataSize, this.ItemCount, true);
        }

        [GlobalCleanup]
        public void ClearData()
        {
            this.data = null;
        }

        // baseline from PoC from issue
        // fastest
        // [Benchmark(Baseline = true)]
        public HashSet<Large> Murmur3x8632_WithCustomCombiner()
        {
            var set = new HashSet<Large>(Mumur3A_FromIssue_EqualityComparer.Default);
            return this.RunHashSetBenchmark(set);
        }

        // alternative implementations of the same thing, using more generic methods
        //
        // [Benchmark]
        // around 2.10 times slower
        public HashSet<Large> Murmur3x8632_ByHaschisch()
        {
            return this.RunHashSetBenchmark_ByHaschisch<Murmur3x8632Hasher.Stream>();
        }

        // [Benchmark]
        // around 3-6% slower than Murmur3x8632_WithCustomCombiner
        // seems good enough for comparison across algorithms
        // use this as baseline to have the same overhead across all algorithms
        [Benchmark(Baseline = true)]
        public HashSet<Large> Murmur3x8632_ByBlock()
        {
            return this.RunHashSetBenchmark_ByBlock<Murmur3x8632Hasher.Block>();
        }

        //[Benchmark]
        // around 10-15% slower
        public HashSet<Large> Murmur3x8632_ByStream()
        {
            return this.RunHashSetBenchmark_ByStream<Murmur3x8632Hasher.Stream>();
        }

        [Benchmark]
        public HashSet<Large> Marvin32()
        {
            return this.RunHashSetBenchmark_ByBlock<Marvin32Hasher.Block>();
        }

        [Benchmark]
        public HashSet<Large> HSip13()
        {
            return this.RunHashSetBenchmark_ByBlock<HalfSip13Hasher.Block>();
        }

        [Benchmark]
        public HashSet<Large> XXHash32()
        {
            return this.RunHashSetBenchmark_ByBlock<XXHash32Hasher.Block>();
        }

        [Benchmark]
        public HashSet<Large> XXHash64()
        {
            return this.RunHashSetBenchmark_ByBlock<XXHash64Hasher.Block>();
        }

        private HashSet<Large> RunHashSetBenchmark_ByHaschisch<T>()
            where T : struct, IStreamingHasher<int>
        {
            var set = new HashSet<Large>(HashableEqualityComparer<Large, T>.Default);
            return RunHashSetBenchmark(set);
        }

        private HashSet<Large> RunHashSetBenchmark_ByBlock<T>()
            where T : struct, IUnsafeBlockHasher<int>
        {
            var set = new HashSet<Large>(ByBlockEqualityComparer<T>.Default);
            return RunHashSetBenchmark(set);
        }

        private HashSet<Large> RunHashSetBenchmark_ByStream<T>()
            where T : struct, IStreamingHasher<int>
        {
            var set = new HashSet<Large>(ByStreamEqualityComparer<T>.Default);
            return RunHashSetBenchmark(set);
        }

        private HashSet<Large> RunHashSetBenchmark(HashSet<Large> set)
        {
            var misCnt = 0UL;

            for (var i = 0; i < this.data.Length; i++)
            {
                if (i % 4 == 0) { continue; }
                set.Add(this.data[i]);
            }

            var idx = 0;
            for (var i = 0; i < this.LookupCount; i++)
            {
                var item = this.data[idx++ % this.data.Length];
                if (!set.Contains(item))
                {
                    misCnt++;
                }
            }

            return set;
        }
    }
}
