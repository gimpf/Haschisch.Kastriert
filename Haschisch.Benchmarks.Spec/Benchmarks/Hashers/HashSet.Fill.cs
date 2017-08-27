using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;
using Haschisch.Tests;

namespace Haschisch.Benchmarks
{
    public class HashSet_Fill
    {
        private Large[] data;

        [Params(10)]
        public int DataSize { get; set; }

        [Params(100)]
        public int ItemCount { get; set; }

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

        [Benchmark(Baseline = true)]
        public HashSet<Large> Murmur3x8632_WithCustomCombiner()
        {
            return this.RunHashSetBenchmark(Mumur3A_FromIssue_EqualityComparer.Default);
        }

        [Benchmark]
        public HashSet<Large> Murmur3x8632()
        {
            return this.RunHashSetBenchmark_ByBlock<Murmur3x8632Hasher.Block>();
        }

        [Benchmark]
        public HashSet<Large> Murmur3x8632_ByStream()
        {
            return this.RunHashSetBenchmark_ByStream<Murmur3x8632Hasher.Stream>();
        }

        [Benchmark]
        public HashSet<Large> Murmur3x8632_ByHaschisch()
        {
            return this.RunHashSetBenchmark_ByHaschisch<Murmur3x8632Hasher.Stream>();
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

        [Benchmark]
        public HashSet<Large> SeaHash()
        {
            return this.RunHashSetBenchmark_ByBlock<SeaHasher.Block>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<Large> RunHashSetBenchmark_ByHaschisch<T>()
            where T : struct, IStreamingHasher<int>
        {
            return RunHashSetBenchmark(HashableEqualityComparer<Large, T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<Large> RunHashSetBenchmark_ByBlock<T>()
            where T : struct, IUnsafeBlockHasher<int>
        {
            return RunHashSetBenchmark(ByBlockEqualityComparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<Large> RunHashSetBenchmark_ByStream<T>()
            where T : struct, IStreamingHasher<int>
        {
            return RunHashSetBenchmark(ByStreamEqualityComparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<Large> RunHashSetBenchmark(IEqualityComparer<Large> comparer)
        {
            return new HashSet<Large>(this.data, comparer);
        }
    }
}
