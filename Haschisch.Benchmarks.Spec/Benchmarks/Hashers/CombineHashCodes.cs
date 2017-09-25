using System;
using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public class CombineHashCodes
    {
        private static readonly Random rnd = new Random(42);
        private static int v1 = rnd.Next();
        private static int v2 = rnd.Next();
        private static int v3 = rnd.Next();
        private static int v4 = rnd.Next();
        private static int v5 = rnd.Next();
        private static int v6 = rnd.Next();
        private static int v7 = rnd.Next();
        private static int v8 = rnd.Next();
        private static int seed = rnd.Next();

        private ICombine combiner;

        [Params(4, 8, 12, 16, 20, 24, 28, 32)]
        public int Bytes { get; set; }

        // going through combiner creates some overhead, but it's constant and the same
        // for all algorithms, so we can ignore it w.r.t. relative performance
        [GlobalSetup]
        public void InitializeCombiner()
        {
            switch (this.Bytes)
            {
                case 4:
                    this.combiner = new Combiner1();
                    break;
                case 8:
                    this.combiner = new Combiner2();
                    break;
                case 12:
                    this.combiner = new Combiner3();
                    break;
                case 16:
                    this.combiner = new Combiner4();
                    break;
                case 20:
                    this.combiner = new Combiner5();
                    break;
                case 24:
                    this.combiner = new Combiner6();
                    break;
                case 28:
                    this.combiner = new Combiner7();
                    break;
                case 32:
                    this.combiner = new Combiner8();
                    break;
                default:
                    throw new InvalidOperationException("unsupported combine count");
            }
        }

        // without resetting the hash-codes on each iteration, some benchmarked implementations return
        // invalid data (for at least one runtime, typically some ryu-jit variant); looking at the result
        // it seems that city64 without seed collapses into a constant...  Funnily enough, murmur-3-32
        // only had the same behavior when instead of using static readonly fields I used literal constants
        [IterationSetup]
        public void UpdateHashCodes()
        {
            v1 = rnd.Next();
            v2 = rnd.Next();
            v3 = rnd.Next();
            v4 = rnd.Next();
            v5 = rnd.Next();
            v6 = rnd.Next();
            v7 = rnd.Next();
            v8 = rnd.Next();
            seed = rnd.Next();
        }

        // clean up so that memory-diagnoser has correct results
        [GlobalCleanup]
        public void CleanupCombiner()
        {
            this.combiner = null;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("combine", "throughput", "empty", "prime", "no-seed")]
        public int Empty() => this.combiner.Empty();

        [Benchmark][BenchmarkCategory("combine", "throughput", "multiply-add", "prime", "no-seed")]
        public int MultiplyAdd_Reordered_Custom() => this.combiner.MultiplyAdd_Reordered_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "multiply-add", "variant", "no-seed")]
        public int MultiplyAdd_Custom() => this.combiner.MultiplyAdd_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant", "no-seed")]
        public int Murmur3A_Tannergooding_Custom() => this.combiner.Murmur3A_Tannergooding_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant", "per-ad-seed")]
        public int Murmur3A_TannergoodingWithSeed_Custom() => this.combiner.Murmur3A_TannergoodingWithSeed_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant", "per-ad-seed")]
        public int Murmur3A_TannergoodingWithSeedArg_Custom() => this.combiner.Murmur3A_TannergoodingWithSeedArg_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant", "per-ad-seed")]
        public int Murmur3A_TannergoodingWithSpecialSauce_Custom() => this.combiner.Murmur3A_TannergoodingSpecialSauce_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "prime", "per-ad-seed")]
        public int Murmur3A_Steps() => this.combiner.Murmur3A_Steps_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant", "per-ad-seed")]
        public int Murmur3A_Combine() => this.combiner.Murmur3A_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant", "per-ad-seed")]
        public int Murmur3A_Block() => this.combiner.Murmur3A_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "hsip", "hsip-1-3", "prime", "per-ad-seed")]
        public int HSip13_Combine() => this.combiner.HSip13_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "hsip", "hsip-1-3", "variant", "per-ad-seed")]
        public int HSip13_Block() => this.combiner.HSip13_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "hsip", "hsip-2-4", "prime", "per-ad-seed")]
        public int HSip24_Combine() => this.combiner.HSip24_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "sip", "sip-1-3", "prime", "per-ad-seed")]
        public int Sip13_Combine() => this.combiner.Sip13_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "sip", "sip-2-4", "prime", "per-ad-seed")]
        public int Sip24_Combine() => this.combiner.Sip24_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "marvin32", "prime", "per-ad-seed")]
        public int Marvin32_Combine() => this.combiner.Marvin32_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "marvin32", "variant", "per-ad-seed")]
        public int Marvin32_Block() => this.combiner.Marvin32_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "xx32", "prime", "per-ad-seed")]
        public int XXHash32_Combine() => this.combiner.XXHash32_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "xx32", "variant", "per-ad-seed")]
        public int XXHash32_Block() => this.combiner.XXHash32_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "xx64", "prime", "per-ad-seed")]
        public int XXHash64_Combine() => this.combiner.XXHash64_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "xx64", "variant", "per-ad-seed")]
        public int XXHash64_Block() => this.combiner.XXHash64_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "sea", "prime", "per-ad-seed")]
        public int SeaHash_Combine() => this.combiner.SeaHash_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "sea", "variant", "no-seed")]
        public int SeaHash_Block() => this.combiner.SeaHash_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "sea", "variant", "per-ad-seed")]
        public int SeaHash_Experimental_Combine() => this.combiner.SeaHash_Experimental_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "spookyv2", "prime", "per-ad-seed")]
        public int SpookyV2_Combine() => this.combiner.SpookyV2_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "city", "city32", "prime", "per-ad-seed")]
        public int City32_CustomSeed_Combine() => this.combiner.City32_CustomSeed_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "city", "city32", "variant", "no-seed")]
        public int City32_OldUnseeded_Combine() => this.combiner.City32_OldUnseeded_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "city", "city32", "variant", "no-seed")]
        public int City32_OldNonUnrolled_Combine() => this.combiner.City32_OldNonUnrolled_Combine();

        // for .NET 4.7 64bit (ryujit), this benchmark produced incorrect results until the
        // stream of combined hash-codes was not fixed throughout the iteration
        [Benchmark][BenchmarkCategory("combine", "throughput", "city", "city64", "prime", "no-seed")]
        public int City64_Combine() => this.combiner.City64_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "city", "city64-w-seeds", "prime", "per-ad-seed")]
        public int City64WithSeeds_Combine() => this.combiner.City64WithSeeds_Combine();

        private sealed class Combiner1 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAdd_Reordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1);
            public int Murmur3A_TannergoodingSpecialSauce_Custom() => Murmur3A_TG_SpecialSauce_Combiner.Combine(v1);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(v1);
            public int Murmur3A_Combine() => default(Murmur3x8632Hasher.Combiner).Combine(v1);
            public int Murmur3A_Block() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(v1);
            public int HSip13_Combine() => default(HalfSip13Hasher.Combiner).Combine(v1);
            public int HSip13_Block() => GenericCombiner<HalfSip13Hasher.Block>.Combine(v1);
            public int HSip24_Combine() => default(HalfSip24Hasher.Combiner).Combine(v1);
            public int Sip13_Combine() => default(Sip13Hasher.Combiner).Combine(v1);
            public int Sip24_Combine() => default(Sip24Hasher.Combiner).Combine(v1);
            public int Marvin32_Combine() => default(Marvin32Hasher.Combiner).Combine(v1);
            public int Marvin32_Block() => GenericCombiner<Marvin32Hasher.Block>.Combine(v1);
            public int XXHash32_Combine() => default(XXHash32Hasher.Combiner).Combine(v1);
            public int XXHash32_Block() => GenericCombiner<XXHash32Hasher.Block>.Combine(v1);
            public int XXHash64_Combine() => default(XXHash64Hasher.Combiner).Combine(v1);
            public int XXHash64_Block() => GenericCombiner<XXHash64Hasher.Block>.Combine(v1);
            public int SeaHash_Combine() => default(SeaHasher.Combiner).Combine(v1);
            public int SeaHash_Block() => GenericCombiner<SeaHasher.Block>.Combine(v1);
            public int SeaHash_Experimental_Combine() => default(SeaExperimentalHasher.Combiner).Combine(v1);
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1);
            public int City32_CustomSeed_Combine() => default(City32Hasher.Combiner).Combine(v1);
            public int City32_OldUnseeded_Combine() => default(City32OldHasher.CombinerUnseeded).Combine(v1);
            public int City32_OldNonUnrolled_Combine() => default(City32OldHasher.CombinerNonUnrolled).Combine(v1);
            public int City64_Combine() => default(City64Hasher.Combiner).Combine(v1);
            public int City64WithSeeds_Combine() => default(City64WithSeedsHasher.Combiner).Combine(v1);
        }

        private sealed class Combiner2 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAdd_Reordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1, v2);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1, v2);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1, v2);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1, v2);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1, v2);
            public int Murmur3A_TannergoodingSpecialSauce_Custom() => Murmur3A_TG_SpecialSauce_Combiner.Combine(v1, v2);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(v1, v2);
            public int Murmur3A_Combine() => default(Murmur3x8632Hasher.Combiner).Combine(v1, v2);
            public int Murmur3A_Block() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(v1, v2);
            public int HSip13_Combine() => default(HalfSip13Hasher.Combiner).Combine(v1, v2);
            public int HSip13_Block() => GenericCombiner<HalfSip13Hasher.Block>.Combine(v1, v2);
            public int HSip24_Combine() => default(HalfSip24Hasher.Combiner).Combine(v1, v2);
            public int Sip13_Combine() => default(Sip13Hasher.Combiner).Combine(v1, v2);
            public int Sip24_Combine() => default(Sip24Hasher.Combiner).Combine(v1, v2);
            public int Marvin32_Combine() => default(Marvin32Hasher.Combiner).Combine(v1, v2);
            public int Marvin32_Block() => GenericCombiner<Marvin32Hasher.Block>.Combine(v1, v2);
            public int XXHash32_Combine() => default(XXHash32Hasher.Combiner).Combine(v1, v2);
            public int XXHash32_Block() => GenericCombiner<XXHash32Hasher.Block>.Combine(v1, v2);
            public int XXHash64_Combine() => default(XXHash64Hasher.Combiner).Combine(v1, v2);
            public int XXHash64_Block() => GenericCombiner<XXHash64Hasher.Block>.Combine(v1, v2);
            public int SeaHash_Combine() => default(SeaHasher.Combiner).Combine(v1, v2);
            public int SeaHash_Block() => GenericCombiner<SeaHasher.Block>.Combine(v1, v2);
            public int SeaHash_Experimental_Combine() => default(SeaExperimentalHasher.Combiner).Combine(v1, v2);
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1, v2);
            public int City32_CustomSeed_Combine() => default(City32Hasher.Combiner).Combine(v1, v2);
            public int City32_OldUnseeded_Combine() => default(City32OldHasher.CombinerUnseeded).Combine(v1, v2);
            public int City32_OldNonUnrolled_Combine() => default(City32OldHasher.CombinerNonUnrolled).Combine(v1, v2);
            public int City64_Combine() => default(City64Hasher.Combiner).Combine(v1, v2);
            public int City64WithSeeds_Combine() => default(City64WithSeedsHasher.Combiner).Combine(v1, v2);
        }

        private sealed class Combiner3 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAdd_Reordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1, v2, v3);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1, v2, v3);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1, v2, v3);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1, v2, v3);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1, v2, v3);
            public int Murmur3A_TannergoodingSpecialSauce_Custom() => Murmur3A_TG_SpecialSauce_Combiner.Combine(v1, v2, v3);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(v1, v2, v3);
            public int Murmur3A_Combine() => default(Murmur3x8632Hasher.Combiner).Combine(v1, v2, v3);
            public int Murmur3A_Block() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(v1, v2, v3);
            public int HSip13_Combine() => default(HalfSip13Hasher.Combiner).Combine(v1, v2, v3);
            public int HSip13_Block() => GenericCombiner<HalfSip13Hasher.Block>.Combine(v1, v2, v3);
            public int HSip24_Combine() => default(HalfSip24Hasher.Combiner).Combine(v1, v2, v3);
            public int Sip13_Combine() => default(Sip13Hasher.Combiner).Combine(v1, v2, v3);
            public int Sip24_Combine() => default(Sip24Hasher.Combiner).Combine(v1, v2, v3);
            public int Marvin32_Combine() => default(Marvin32Hasher.Combiner).Combine(v1, v2, v3);
            public int Marvin32_Block() => GenericCombiner<Marvin32Hasher.Block>.Combine(v1, v2, v3);
            public int XXHash32_Combine() => default(XXHash32Hasher.Combiner).Combine(v1, v2, v3);
            public int XXHash32_Block() => GenericCombiner<XXHash32Hasher.Block>.Combine(v1, v2, v3);
            public int XXHash64_Combine() => default(XXHash64Hasher.Combiner).Combine(v1, v2, v3);
            public int XXHash64_Block() => GenericCombiner<XXHash64Hasher.Block>.Combine(v1, v2, v3);
            public int SeaHash_Combine() => default(SeaHasher.Combiner).Combine(v1, v2, v3);
            public int SeaHash_Block() => GenericCombiner<SeaHasher.Block>.Combine(v1, v2, v3);
            public int SeaHash_Experimental_Combine() => default(SeaExperimentalHasher.Combiner).Combine(v1, v2, v3);
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1, v2, v3);
            public int City32_CustomSeed_Combine() => default(City32Hasher.Combiner).Combine(v1, v2, v3);
            public int City32_OldUnseeded_Combine() => default(City32OldHasher.CombinerUnseeded).Combine(v1, v2, v3);
            public int City32_OldNonUnrolled_Combine() => default(City32OldHasher.CombinerNonUnrolled).Combine(v1, v2, v3);
            public int City64_Combine() => default(City64Hasher.Combiner).Combine(v1, v2, v3);
            public int City64WithSeeds_Combine() => default(City64WithSeedsHasher.Combiner).Combine(v1, v2, v3);
        }

        private sealed class Combiner4 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAdd_Reordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1, v2, v3, v4);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1, v2, v3, v4);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1, v2, v3, v4);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1, v2, v3, v4);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1, v2, v3, v4);
            public int Murmur3A_TannergoodingSpecialSauce_Custom() => Murmur3A_TG_SpecialSauce_Combiner.Combine(v1, v2, v3, v4);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(v1, v2, v3, v4);
            public int Murmur3A_Combine() => default(Murmur3x8632Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int Murmur3A_Block() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(v1, v2, v3, v4);
            public int HSip13_Combine() => default(HalfSip13Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int HSip13_Block() => GenericCombiner<HalfSip13Hasher.Block>.Combine(v1, v2, v3, v4);
            public int HSip24_Combine() => default(HalfSip24Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int Sip13_Combine() => default(Sip13Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int Sip24_Combine() => default(Sip24Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int Marvin32_Combine() => default(Marvin32Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int Marvin32_Block() => GenericCombiner<Marvin32Hasher.Block>.Combine(v1, v2, v3, v4);
            public int XXHash32_Combine() => default(XXHash32Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int XXHash32_Block() => GenericCombiner<XXHash32Hasher.Block>.Combine(v1, v2, v3, v4);
            public int XXHash64_Combine() => default(XXHash64Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int XXHash64_Block() => GenericCombiner<XXHash64Hasher.Block>.Combine(v1, v2, v3, v4);
            public int SeaHash_Combine() => default(SeaHasher.Combiner).Combine(v1, v2, v3, v4);
            public int SeaHash_Block() => GenericCombiner<SeaHasher.Block>.Combine(v1, v2, v3, v4);
            public int SeaHash_Experimental_Combine() => default(SeaExperimentalHasher.Combiner).Combine(v1, v2, v3, v4);
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int City32_CustomSeed_Combine() => default(City32Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int City32_OldUnseeded_Combine() => default(City32OldHasher.CombinerUnseeded).Combine(v1, v2, v3, v4);
            public int City32_OldNonUnrolled_Combine() => default(City32OldHasher.CombinerNonUnrolled).Combine(v1, v2, v3, v4);
            public int City64_Combine() => default(City64Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int City64WithSeeds_Combine() => default(City64WithSeedsHasher.Combiner).Combine(v1, v2, v3, v4);
        }

        private sealed class Combiner5 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAdd_Reordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1, v2, v3, v4, v5);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1, v2, v3, v4, v5);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1, v2, v3, v4, v5);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1, v2, v3, v4, v5);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1, v2, v3, v4, v5);
            public int Murmur3A_TannergoodingSpecialSauce_Custom() => Murmur3A_TG_SpecialSauce_Combiner.Combine(v1, v2, v3, v4, v5);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(v1, v2, v3, v4, v5);
            public int Murmur3A_Combine() => default(Murmur3x8632Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int Murmur3A_Block() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(v1, v2, v3, v4, v5);
            public int HSip13_Combine() => default(HalfSip13Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int HSip13_Block() => GenericCombiner<HalfSip13Hasher.Block>.Combine(v1, v2, v3, v4, v5);
            public int HSip24_Combine() => default(HalfSip24Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int Sip13_Combine() => default(Sip13Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int Sip24_Combine() => default(Sip24Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int Marvin32_Combine() => default(Marvin32Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int Marvin32_Block() => GenericCombiner<Marvin32Hasher.Block>.Combine(v1, v2, v3, v4, v5);
            public int XXHash32_Combine() => default(XXHash32Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int XXHash32_Block() => GenericCombiner<XXHash32Hasher.Block>.Combine(v1, v2, v3, v4, v5);
            public int XXHash64_Combine() => default(XXHash64Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int XXHash64_Block() => GenericCombiner<XXHash64Hasher.Block>.Combine(v1, v2, v3, v4, v5);
            public int SeaHash_Combine() => default(SeaHasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int SeaHash_Block() => GenericCombiner<SeaHasher.Block>.Combine(v1, v2, v3, v4, v5);
            public int SeaHash_Experimental_Combine() => default(SeaExperimentalHasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int City32_CustomSeed_Combine() => default(City32Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int City32_OldUnseeded_Combine() => default(City32OldHasher.CombinerUnseeded).Combine(v1, v2, v3, v4, v5);
            public int City32_OldNonUnrolled_Combine() => default(City32OldHasher.CombinerNonUnrolled).Combine(v1, v2, v3, v4, v5);
            public int City64_Combine() => default(City64Hasher.Combiner).Combine(v1, v2, v3, v4, v5);
            public int City64WithSeeds_Combine() => default(City64WithSeedsHasher.Combiner).Combine(v1, v2, v3, v4, v5);
        }

        private sealed class Combiner6 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAdd_Reordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1, v2, v3, v4, v5, v6);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1, v2, v3, v4, v5, v6);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1, v2, v3, v4, v5, v6);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1, v2, v3, v4, v5, v6);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1, v2, v3, v4, v5, v6);
            public int Murmur3A_TannergoodingSpecialSauce_Custom() => Murmur3A_TG_SpecialSauce_Combiner.Combine(v1, v2, v3, v4, v5, v6);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(v1, v2, v3, v4, v5, v6);
            public int Murmur3A_Combine() => default(Murmur3x8632Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int Murmur3A_Block() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6);
            public int HSip13_Combine() => default(HalfSip13Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int HSip13_Block() => GenericCombiner<HalfSip13Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6);
            public int HSip24_Combine() => default(HalfSip24Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int Sip13_Combine() => default(Sip13Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int Sip24_Combine() => default(Sip24Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int Marvin32_Combine() => default(Marvin32Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int Marvin32_Block() => GenericCombiner<Marvin32Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6);
            public int XXHash32_Combine() => default(XXHash32Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int XXHash32_Block() => GenericCombiner<XXHash32Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6);
            public int XXHash64_Combine() => default(XXHash64Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int XXHash64_Block() => GenericCombiner<XXHash64Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6);
            public int SeaHash_Combine() => default(SeaHasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int SeaHash_Block() => GenericCombiner<SeaHasher.Block>.Combine(v1, v2, v3, v4, v5, v6);
            public int SeaHash_Experimental_Combine() => default(SeaExperimentalHasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int City32_CustomSeed_Combine() => default(City32Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int City32_OldUnseeded_Combine() => default(City32OldHasher.CombinerUnseeded).Combine(v1, v2, v3, v4, v5, v6);
            public int City32_OldNonUnrolled_Combine() => default(City32OldHasher.CombinerNonUnrolled).Combine(v1, v2, v3, v4, v5, v6);
            public int City64_Combine() => default(City64Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
            public int City64WithSeeds_Combine() => default(City64WithSeedsHasher.Combiner).Combine(v1, v2, v3, v4, v5, v6);
        }

        private sealed class Combiner7 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAdd_Reordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1, v2, v3, v4, v5, v6, v7);
            public int Murmur3A_TannergoodingSpecialSauce_Custom() => Murmur3A_TG_SpecialSauce_Combiner.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int Murmur3A_Combine() => default(Murmur3x8632Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int Murmur3A_Block() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int HSip13_Combine() => default(HalfSip13Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int HSip13_Block() => GenericCombiner<HalfSip13Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int HSip24_Combine() => default(HalfSip24Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int Sip13_Combine() => default(Sip13Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int Sip24_Combine() => default(Sip24Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int Marvin32_Combine() => default(Marvin32Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int Marvin32_Block() => GenericCombiner<Marvin32Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int XXHash32_Combine() => default(XXHash32Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int XXHash32_Block() => GenericCombiner<XXHash32Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int XXHash64_Combine() => default(XXHash64Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int XXHash64_Block() => GenericCombiner<XXHash64Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int SeaHash_Combine() => default(SeaHasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int SeaHash_Block() => GenericCombiner<SeaHasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7);
            public int SeaHash_Experimental_Combine() => default(SeaExperimentalHasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int City32_CustomSeed_Combine() => default(City32Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int City32_OldUnseeded_Combine() => default(City32OldHasher.CombinerUnseeded).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int City32_OldNonUnrolled_Combine() => default(City32OldHasher.CombinerNonUnrolled).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int City64_Combine() => default(City64Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
            public int City64WithSeeds_Combine() => default(City64WithSeedsHasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7);
        }

        private sealed class Combiner8 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAdd_Reordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1, v2, v3, v4, v5, v6, v7, v8);
            public int Murmur3A_TannergoodingSpecialSauce_Custom() => Murmur3A_TG_SpecialSauce_Combiner.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Murmur3A_Combine() => default(Murmur3x8632Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Murmur3A_Block() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int HSip13_Combine() => default(HalfSip13Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int HSip13_Block() => GenericCombiner<HalfSip13Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int HSip24_Combine() => default(HalfSip24Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Sip13_Combine() => default(Sip13Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Sip24_Combine() => default(Sip24Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Marvin32_Combine() => default(Marvin32Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Marvin32_Block() => GenericCombiner<Marvin32Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int XXHash32_Combine() => default(XXHash32Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int XXHash32_Block() => GenericCombiner<XXHash32Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int XXHash64_Combine() => default(XXHash64Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int XXHash64_Block() => GenericCombiner<XXHash64Hasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int SeaHash_Combine() => default(SeaHasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int SeaHash_Block() => GenericCombiner<SeaHasher.Block>.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int SeaHash_Experimental_Combine() => default(SeaExperimentalHasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int City32_CustomSeed_Combine() => default(City32Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int City32_OldUnseeded_Combine() => default(City32OldHasher.CombinerUnseeded).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int City32_OldNonUnrolled_Combine() => default(City32OldHasher.CombinerNonUnrolled).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int City64_Combine() => default(City64Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int City64WithSeeds_Combine() => default(City64WithSeedsHasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
        }

        private interface ICombine
        {
            int Empty();

            // algorithm _ algorithm-implementation _ combiner
            //
            int MultiplyAdd_Reordered_Custom();
            int MultiplyAdd_Custom();

            int Murmur3A_Tannergooding_Custom();
            int Murmur3A_TannergoodingWithSeed_Custom();
            int Murmur3A_TannergoodingWithSeedArg_Custom();
            int Murmur3A_TannergoodingSpecialSauce_Custom();
            int Murmur3A_Steps_Custom();
            int Murmur3A_Combine();
            int Murmur3A_Block();

            int HSip13_Combine();
            int HSip13_Block();
            int HSip24_Combine();
            int Sip13_Combine();
            int Sip24_Combine();

            int Marvin32_Combine();
            int Marvin32_Block();

            int XXHash32_Combine();
            int XXHash32_Block();
            int XXHash64_Combine();
            int XXHash64_Block();

            int SeaHash_Block();
            int SeaHash_Combine();
            int SeaHash_Experimental_Combine();

            int SpookyV2_Combine();

            int City32_CustomSeed_Combine();
            int City32_OldUnseeded_Combine();
            int City32_OldNonUnrolled_Combine();

            int City64_Combine();
            int City64WithSeeds_Combine();
        }
    }
}
