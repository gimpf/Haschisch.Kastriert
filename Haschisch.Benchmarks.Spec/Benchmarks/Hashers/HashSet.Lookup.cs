using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;
using Haschisch.Tests;

namespace Haschisch.Benchmarks
{
    public class HashSet_Lookup
    {
        private Large[] data;

        [Params(10)]
        public int DataSize { get; set; }

        [Params(100)]
        public int ItemCount { get; set; }

        [Params(100_000)]
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

        // overhead from random number generation
        [Benchmark]
        public HashSet<Large> Murmur3x8632_WithCustomCombiner_Empty()
        {
            return this.RunEmpty(Mumur3A_TG_EqualityComparer.Default);
        }

        // baseline from PoC from issue, fastest
        [Benchmark(Baseline = true)]
        public HashSet<Large> Murmur3x8632_TG_CustomComparer()
        {
            return this.RunHashSetBenchmark(Mumur3A_TG_EqualityComparer.Default);
        }

        [Benchmark]
        public HashSet<Large> Murmur3x8632_ByCombinerComparer()
        {
            return this.RunHashSetBenchmark_ByCombiner<Murmur3x8632Hasher.Combiner>();
        }

        [Benchmark]
        public HashSet<Large> Murmur3x8632_ByBlockComparer()
        {
            return this.RunHashSetBenchmark_ByBlock<Murmur3x8632Hasher.Block>();
        }

        [Benchmark]
        public HashSet<Large> Murmur3x8632_ByHaschisch()
        {
            return this.RunHashSetBenchmark_ByHaschisch<Murmur3x8632Hasher.Stream>();
        }

        [Benchmark]
        public HashSet<Large> Murmur3x8632_ByStreamComparer()
        {
            return this.RunHashSetBenchmark_ByStream<Murmur3x8632Hasher.Stream>();
        }

        [Benchmark]
        public HashSet<Large> Marvin32_Combiner()
        {
            return this.RunHashSetBenchmark_ByCombiner<Marvin32Hasher.Combiner>();
        }

        [Benchmark]
        public HashSet<Large> Marvin32_Block()
        {
            return this.RunHashSetBenchmark_ByBlock<Marvin32Hasher.Block>();
        }

        [Benchmark]
        public HashSet<Large> HSip13_Combiner()
        {
            return this.RunHashSetBenchmark_ByCombiner<HalfSip13Hasher.Combiner>();
        }

        [Benchmark]
        public HashSet<Large> HSip13_Block()
        {
            return this.RunHashSetBenchmark_ByBlock<HalfSip13Hasher.Block>();
        }

        [Benchmark]
        public HashSet<Large> HSip24_Combiner()
        {
            return this.RunHashSetBenchmark_ByCombiner<HalfSip24Hasher.Combiner>();
        }

        [Benchmark]
        public HashSet<Large> Sip13_Combiner()
        {
            return this.RunHashSetBenchmark_ByCombiner<Sip13Hasher.Combiner>();
        }

        [Benchmark]
        public HashSet<Large> Sip24_Combiner()
        {
            return this.RunHashSetBenchmark_ByCombiner<Sip24Hasher.Combiner>();
        }

        [Benchmark]
        public HashSet<Large> XXHash32_Combiner()
        {
            return this.RunHashSetBenchmark_ByCombiner<XXHash32Hasher.Combiner>();
        }

        [Benchmark]
        public HashSet<Large> XXHash32_Block()
        {
            return this.RunHashSetBenchmark_ByBlock<XXHash32Hasher.Block>();
        }

        [Benchmark]
        public HashSet<Large> XXHash64_Combiner()
        {
            return this.RunHashSetBenchmark_ByCombiner<XXHash64Hasher.Combiner>();
        }

        [Benchmark]
        public HashSet<Large> XXHash64_Block()
        {
            return this.RunHashSetBenchmark_ByBlock<XXHash64Hasher.Block>();
        }

        [Benchmark]
        public HashSet<Large> SeaHash_Combiner()
        {
            return this.RunHashSetBenchmark_ByCombiner<SeaHasher.Combiner>();
        }

        [Benchmark]
        public HashSet<Large> SeaHash_Block()
        {
            return this.RunHashSetBenchmark_ByBlock<SeaHasher.Block>();
        }

        [Benchmark]
        public HashSet<Large> SpookyV2_Combiner()
        {
            return this.RunHashSetBenchmark_ByCombiner<SpookyV2Hasher.Combiner>();
        }

        [Benchmark]
        public HashSet<Large> City32_Combiner()
        {
            return this.RunHashSetBenchmark_ByCombiner<City32Hasher.Combiner>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<Large> RunHashSetBenchmark_ByCombiner<T>()
            where T : struct, IHashCodeCombiner =>
            RunHashSetBenchmark(ByCombinerEqualityComparer<T>.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<Large> RunHashSetBenchmark_ByBlock<T>()
            where T : struct, IUnsafeBlockHasher<int> =>
            RunHashSetBenchmark(ByBlockEqualityComparer<T>.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<Large> RunHashSetBenchmark_ByStream<T>()
            where T : struct, IStreamingHasher<int> =>
            RunHashSetBenchmark(ByStreamEqualityComparer<T>.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<Large> RunHashSetBenchmark_ByHaschisch<T>()
            where T : struct, IStreamingHasher<int> =>
            RunHashSetBenchmark(HashableEqualityComparer<Large, T>.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<Large> RunHashSetBenchmark(IEqualityComparer<Large> comparer)
        {
            var set = new HashSet<Large>(this.data, comparer);
            var rnd = new System.Random(42);
            var len = this.data.Length;

            var found = 0;
            for (int i = 0, count = this.LookupCount; i < count; i++)
            {
                found = set.Contains(this.data[rnd.Next(len)]) ? found + 1 : found;
            }

            return set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<Large> RunEmpty(IEqualityComparer<Large> comparer)
        {
            var set = new HashSet<Large>(this.data, comparer);
            var rnd = new System.Random(42);

            var found = 0;
            for (int i = 0, count = this.LookupCount; i < count; i++)
            {
                found = rnd.Next(count) == 0 ? found + 1 : found;
            }

            return set;
        }
    }
}
