using System;
using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public class CombineHashCodes
    {
        private static readonly Random rnd = new Random(42);
        private static readonly int v1 = rnd.Next();
        private static readonly int v2 = rnd.Next();
        private static readonly int v3 = rnd.Next();
        private static readonly int v4 = rnd.Next();
        private static readonly int v5 = rnd.Next();
        private static readonly int v6 = rnd.Next();
        private static readonly int v7 = rnd.Next();
        private static readonly int v8 = rnd.Next();
        private static readonly int seed = rnd.Next();

        private ICombine combiner;

        [Params(4, 8, 16, 32)]
        public int Bytes { get; set; }

        // going through combiner creates some overhead, but it's constant and the same
        // for all algorithms, so we can ignore it w.r.t. relative performance
        //
        // why at all? to make the summary output of benchmarkdotnet more readable;
        // simplifies playing around with implementations
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
                case 16:
                    this.combiner = new Combiner4();
                    break;
                case 32:
                    this.combiner = new Combiner8();
                    break;
                default:
                    throw new InvalidOperationException("unsupported combine count");
            }
        }

        // clean up so that memory-diagnoser has correct results
        [GlobalCleanup]
        public void CleanupCombiner()
        {
            this.combiner = null;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("combine", "throughput", "empty", "prime", "i")]
        public int Empty() => this.combiner.Empty();

        [Benchmark][BenchmarkCategory("combine", "throughput", "multiply-add", "prime", "i")]
        public int MultiplyAddReordered_Custom() => this.combiner.MultiplyAddReordered_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "multiply-add", "variant")]
        public int MultiplyAdd_Custom() => this.combiner.MultiplyAdd_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "prime", "i")]
        public int Murmur3A_Tannergooding_Custom() => this.combiner.Murmur3A_Tannergooding_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant")]
        public int Murmur3A_TannergoodingWithSeed_Custom() => this.combiner.Murmur3A_TannergoodingWithSeed_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant")]
        public int Murmur3A_TannergoodingWithSeedArg_Custom() => this.combiner.Murmur3A_TannergoodingWithSeedArg_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant")]
        public int Murmur3A_Steps() => this.combiner.Murmur3A_Steps_Custom();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant")]
        public int Murmur3A_Combine() => this.combiner.Murmur3A_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "murmur-3-32", "variant")]
        public int Murmur3A_Block() => this.combiner.Murmur3A_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "hsip", "hsip-1-3", "prime")]
        public int HSip13_Combine() => this.combiner.HSip13_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "hsip", "hsip-1-3", "variant")]
        public int HSip13_Block() => this.combiner.HSip13_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "hsip", "hsip-2-4", "prime")]
        public int HSip24_Combine() => this.combiner.HSip24_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "sip", "sip-1-3", "prime", "i")]
        public int Sip13_Combine() => this.combiner.Sip13_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "sip", "sip-2-4", "prime")]
        public int Sip24_Combine() => this.combiner.Sip24_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "marvin32", "prime")]
        public int Marvin32_Combine() => this.combiner.Marvin32_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "marvin32", "variant")]
        public int Marvin32_Block() => this.combiner.Marvin32_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "xx32", "prime", "i")]
        public int XXHash32_Combine() => this.combiner.XXHash32_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "xx32", "variant")]
        public int XXHash32_Block() => this.combiner.XXHash32_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "xx64", "prime", "i")]
        public int XXHash64_Combine() => this.combiner.XXHash64_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "xx64", "variant")]
        public int XXHash64_Block() => this.combiner.XXHash64_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "sea", "prime", "i")]
        public int SeaHash_Combine() => this.combiner.SeaHash_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "sea", "variant")]
        public int SeaHash_Block() => this.combiner.SeaHash_Block();

        [Benchmark][BenchmarkCategory("combine", "throughput", "spookyv2", "prime")]
        public int SpookyV2_Combine() => this.combiner.SpookyV2_Combine();

        [Benchmark][BenchmarkCategory("combine", "throughput", "city32", "prime", "i")]
        public int City32_Combine() => this.combiner.City32_Combine();

        private sealed class Combiner1 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAddReordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1);
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
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1);
            public int City32_Combine() => default(City32Hasher.Combiner).Combine(v1);
        }

        private sealed class Combiner2 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAddReordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1, v2);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1, v2);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1, v2);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1, v2);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1, v2);
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
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1, v2);
            public int City32_Combine() => default(City32Hasher.Combiner).Combine(v1, v2);
        }

        private sealed class Combiner4 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAddReordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1, v2, v3, v4);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1, v2, v3, v4);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1, v2, v3, v4);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1, v2, v3, v4);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1, v2, v3, v4);
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
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1, v2, v3, v4);
            public int City32_Combine() => default(City32Hasher.Combiner).Combine(v1, v2, v3, v4);
        }

        private sealed class Combiner8 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAddReordered_Custom() => MultiplyAddReorderedCombiner.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int Murmur3A_TannergoodingWithSeedArg_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(Murmur3x8632Hasher.DefaultSeed, v1, v2, v3, v4, v5, v6, v7, v8);
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
            public int SpookyV2_Combine() => default(SpookyV2Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
            public int City32_Combine() => default(City32Hasher.Combiner).Combine(v1, v2, v3, v4, v5, v6, v7, v8);
        }

        private interface ICombine
        {
            int Empty();

            // algorithm _ algorithm-implementation _ combiner
            //
            int MultiplyAddReordered_Custom();
            int MultiplyAdd_Custom();
            int Murmur3A_Tannergooding_Custom();
            int Murmur3A_TannergoodingWithSeed_Custom();
            int Murmur3A_TannergoodingWithSeedArg_Custom();
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
            int SpookyV2_Combine();
            int City32_Combine();
        }
    }
}
