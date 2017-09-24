using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;
using Haschisch.Tests;

namespace Haschisch.Benchmarks
{
    public class AccessHashSet
    {
        private System.Random rand;
        private Large[] data;

        [Params(30, 10_000)]
        public int ItemCount { get; set; }

        [Params(30, 100_000)]
        public int LookupCount { get; set; }

        [GlobalSetup]
        public void InitializeData()
        {
            this.rand = new System.Random(42);
            this.data = Data.Generate.LargeItems((int)(System.Math.Log10(this.ItemCount)), this.ItemCount, true);
        }

        [GlobalCleanup]
        public void ClearData()
        {
            this.data = null;
        }

        [Benchmark(Baseline = true)][BenchmarkCategory("hashset", "multiply-add", "prime", "no-seed")]
        public HashSet<Large> MultiplyAdd_ByCombiner() => this.RunHashSetBenchmark_ByCombiner<MultiplyAddReorderedCombiner>();

        [Benchmark][BenchmarkCategory("hashset", "multiply-add", "variant", "no-seed")]
        public HashSet<Large> MultiplyAdd_Naive_ByCombiner() => this.RunHashSetBenchmark_ByCombiner<MultiplyAddCombiner>();

        // baseline from PoC from issue
        [Benchmark][BenchmarkCategory("hashset", "murmur-3-32", "prime", "no-seed")]
        public HashSet<Large> Murmur3x8632_TG_CustomComparer() => this.RunHashSetBenchmark(Mumur3A_TG_EqualityComparer.Default);

        [Benchmark][BenchmarkCategory("hashset", "murmur-3-32", "prime", "per-ad-seed")]
        public HashSet<Large> Murmur3x8632_ByCombinerComparer() => this.RunHashSetBenchmark_ByCombiner<Murmur3x8632Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "murmur-3-32", "variant", "per-ad-seed")]
        public HashSet<Large> Murmur3x8632_ByBlockComparer() => this.RunHashSetBenchmark_ByBlock<Murmur3x8632Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "murmur-3-32", "variant", "per-ad-seed")]
        public HashSet<Large> Murmur3x8632_ByHaschisch() => this.RunHashSetBenchmark_ByHaschisch<Murmur3x8632Hasher.Stream>();

        [Benchmark][BenchmarkCategory("hashset", "murmur-3-32", "variant", "per-ad-seed")]
        public HashSet<Large> Murmur3x8632_ByStreamComparer() => this.RunHashSetBenchmark_ByStream<Murmur3x8632Hasher.Stream>();

        [Benchmark][BenchmarkCategory("hashset", "marvin32", "prime", "per-ad-seed")]
        public HashSet<Large> Marvin32_Combiner() => this.RunHashSetBenchmark_ByCombiner<Marvin32Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "marvin32", "variant", "per-ad-seed")]
        public HashSet<Large> Marvin32_Block() => this.RunHashSetBenchmark_ByBlock<Marvin32Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "hsip", "hsip-1-3", "prime", "per-ad-seed")]
        public HashSet<Large> HSip13_Combiner() => this.RunHashSetBenchmark_ByCombiner<HalfSip13Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "hsip", "hsip-1-3", "variant", "per-ad-seed")]
        public HashSet<Large> HSip13_Block() => this.RunHashSetBenchmark_ByBlock<HalfSip13Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "hsip", "hsip-1-3", "variant", "per-ad-seed")]
        public HashSet<Large> HSip24_Combiner() => this.RunHashSetBenchmark_ByCombiner<HalfSip24Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "sip", "sip-1-3", "prime", "per-ad-seed")]
        public HashSet<Large> Sip13_Combiner() => this.RunHashSetBenchmark_ByCombiner<Sip13Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "sip", "sip-1-3", "variant", "per-ad-seed")]
        public HashSet<Large> Sip13_Block() => this.RunHashSetBenchmark_ByBlock<Sip13Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "sip", "sip-2-4", "prime", "per-ad-seed")]
        public HashSet<Large> Sip24_Combiner() => this.RunHashSetBenchmark_ByCombiner<Sip24Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "sip", "sip-2-4", "variant", "per-ad-seed")]
        public HashSet<Large> Sip24_Block() => this.RunHashSetBenchmark_ByBlock<Sip13Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "xx32", "prime", "per-ad-seed")]
        public HashSet<Large> XXHash32_Combiner() => this.RunHashSetBenchmark_ByCombiner<XXHash32Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "xx32", "variant", "per-ad-seed")]
        public HashSet<Large> XXHash32_Block() => this.RunHashSetBenchmark_ByBlock<XXHash32Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "xx64", "prime", "per-ad-seed")]
        public HashSet<Large> XXHash64_Combiner() => this.RunHashSetBenchmark_ByCombiner<XXHash64Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "xx64", "variant", "per-ad-seed")]
        public HashSet<Large> XXHash64_Block() => this.RunHashSetBenchmark_ByBlock<XXHash64Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "sea", "prime", "per-ad-seed")]
        public HashSet<Large> SeaHash_Combiner() => this.RunHashSetBenchmark_ByCombiner<SeaHasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "sea", "variant", "no-seed")]
        public HashSet<Large> SeaHash_Block() => this.RunHashSetBenchmark_ByBlock<SeaHasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "spookyv2", "prime", "per-ad-seed")]
        public HashSet<Large> SpookyV2_Combiner() => this.RunHashSetBenchmark_ByCombiner<SpookyV2Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "city", "city32", "prime", "per-ad-seed")]
        public HashSet<Large> City32_Combiner() => this.RunHashSetBenchmark_ByCombiner<City32Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "city", "city32", "variant", "no-seed")]
        public HashSet<Large> City32_Unseeded_Combiner() => this.RunHashSetBenchmark_ByCombiner<City32OldHasher.CombinerUnseeded>();

        [Benchmark][BenchmarkCategory("hashset", "city", "city32", "variant", "no-seed")]
        public HashSet<Large> City32_NonUnrolled_Combiner() => this.RunHashSetBenchmark_ByCombiner<City32OldHasher.CombinerNonUnrolled>();

        [Benchmark][BenchmarkCategory("hashset", "city", "city32", "variant", "no-seed")]
        public HashSet<Large> City32_Block() => this.RunHashSetBenchmark_ByBlock<City32Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "city", "city64", "prime", "no-seed")]
        public HashSet<Large> City64_Combiner() => this.RunHashSetBenchmark_ByCombiner<City64Hasher.Combiner>();

        [Benchmark][BenchmarkCategory("hashset", "city", "city64", "variant", "no-seed")]
        public HashSet<Large> City64_Block() => this.RunHashSetBenchmark_ByBlock<City64Hasher.Block>();

        [Benchmark][BenchmarkCategory("hashset", "city", "city64-w-seeds", "prime", "per-ad-seed")]
        public HashSet<Large> City64WithSeeds_Combiner() => this.RunHashSetBenchmark_ByCombiner<City64WithSeedsHasher.Combiner>();

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

            for (var i = 0; i < this.LookupCount; i++)
            {
                var item = this.data[this.rand.Next(this.data.Length)];
                if (!set.Contains(item))
                {
                    misCnt++;
                }
            }

            return set;
        }
    }
}
