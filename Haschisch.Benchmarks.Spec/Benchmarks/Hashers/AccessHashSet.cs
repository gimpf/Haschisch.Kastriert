using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;
using Haschisch.Tests;

namespace Haschisch.Benchmarks
{
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

        [Benchmark(Baseline = true)][BenchmarkCategory("hashset", "multiply-add", "prime")]
        public HashSet<Large> MultiplyAdd_ByCombiner() => this.RunHashSetBenchmark_ByCombiner<MultiplyAddReorderedCombiner>();

        [Benchmark][BenchmarkCategory("hashset", "multiply-add", "variant")]
        public HashSet<Large> MultiplyAdd_Naive_ByCombiner() => this.RunHashSetBenchmark_ByCombiner<MultiplyAddCombiner>();

        // baseline from PoC from issue
        [Benchmark][BenchmarkCategory("hashset", "murmur-3-32", "prime")]
        public HashSet<Large> Murmur3x8632_TG_CustomComparer() => this.RunHashSetBenchmark(Mumur3A_TG_EqualityComparer.Default);

        [Benchmark][BenchmarkCategory("hashset", "murmur-3-32", "prime")]
        public HashSet<Large> Murmur3x8632_ByCombinerComparer() => this.RunHashSetBenchmark_ByCombiner<Murmur3x8632Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "murmur-3-32", "variant")]
        public HashSet<Large> Murmur3x8632_ByBlockComparer() => this.RunHashSetBenchmark_ByBlock<Murmur3x8632Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "murmur-3-32", "variant")]
        public HashSet<Large> Murmur3x8632_ByHaschisch() => this.RunHashSetBenchmark_ByHaschisch<Murmur3x8632Hasher.Stream>();

        [Benchmark][BenchmarkCategory("hashset", "murmur-3-32", "variant")]
        public HashSet<Large> Murmur3x8632_ByStreamComparer() => this.RunHashSetBenchmark_ByStream<Murmur3x8632Hasher.Stream>();

        [Benchmark][BenchmarkCategory("hashset", "marvin32", "prime")]
        public HashSet<Large> Marvin32_Combiner() => this.RunHashSetBenchmark_ByCombiner<Marvin32Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "marvin32", "variant")]
        public HashSet<Large> Marvin32_Block() => this.RunHashSetBenchmark_ByBlock<Marvin32Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "hsip", "hsip-1-3", "prime")]
        public HashSet<Large> HSip13_Combiner() => this.RunHashSetBenchmark_ByCombiner<HalfSip13Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "hsip", "hsip-1-3", "variant")]
        public HashSet<Large> HSip13_Block() => this.RunHashSetBenchmark_ByBlock<HalfSip13Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "hsip", "hsip-1-3", "variant")]
        public HashSet<Large> HSip24_Combiner() => this.RunHashSetBenchmark_ByCombiner<HalfSip24Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "sip", "sip-1-3", "prime")]
        public HashSet<Large> Sip13_Combiner() => this.RunHashSetBenchmark_ByCombiner<Sip13Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "sip", "sip-1-3", "variant")]
        public HashSet<Large> Sip13_Block() => this.RunHashSetBenchmark_ByBlock<Sip13Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "sip", "sip-2-4", "prime")]
        public HashSet<Large> Sip24_Combiner() => this.RunHashSetBenchmark_ByCombiner<Sip24Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "sip", "sip-2-4", "variant")]
        public HashSet<Large> Sip24_Block() => this.RunHashSetBenchmark_ByBlock<Sip13Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "xx32", "prime")]
        public HashSet<Large> XXHash32_Combiner() => this.RunHashSetBenchmark_ByCombiner<XXHash32Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "xx32", "variant")]
        public HashSet<Large> XXHash32_Block() => this.RunHashSetBenchmark_ByBlock<XXHash32Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "xx64", "prime")]
        public HashSet<Large> XXHash64_Combiner() => this.RunHashSetBenchmark_ByCombiner<XXHash64Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "xx64", "variant")]
        public HashSet<Large> XXHash64_Block() => this.RunHashSetBenchmark_ByBlock<XXHash64Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "sea", "prime")]
        public HashSet<Large> SeaHash_Combiner() => this.RunHashSetBenchmark_ByCombiner<SeaHasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "sea", "variant")]
        public HashSet<Large> SeaHash_Block() => this.RunHashSetBenchmark_ByBlock<SeaHasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "spookyv2", "prime")]
        public HashSet<Large> SpookyV2_Combiner() => this.RunHashSetBenchmark_ByCombiner<SpookyV2Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "city32", "prime")]
        public HashSet<Large> City32_Combiner() => this.RunHashSetBenchmark_ByCombiner<City32Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "city32", "variant")]
        public HashSet<Large> City32_Unseeded_Combiner() => this.RunHashSetBenchmark_ByCombiner<City32OldHasher.CombinerUnseeded>();

        [Benchmark][BenchmarkCategory("hashset", "city32", "variant")]
        public HashSet<Large> City32_NonUnrolled_Combiner() => this.RunHashSetBenchmark_ByCombiner<City32OldHasher.CombinerNonUnrolled>();

        [Benchmark][BenchmarkCategory("hashset", "city32", "variant")]
        public HashSet<Large> City32_Block() => this.RunHashSetBenchmark_ByBlock<City32Hasher.Block>();

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
            var misCnt = 0UL;

            var set = new HashSet<Large>(comparer);

            for (var i = 0; i < this.data.Length; i++)
            {
                if ((i & ~3) == 0) { continue; }
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
