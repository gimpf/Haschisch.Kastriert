using System;
using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public class CombineHashCodes
    {
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
        public int Empty() => this.combiner.Empty();

        [Benchmark]
        public int MultiplyAddReordered_Custom() => this.combiner.MultiplyAddReordered_Custom();

        [Benchmark]
        public int MultiplyAdd_Custom() => this.combiner.MultiplyAdd_Custom();

        [Benchmark]
        public int Murmur3A_Tannergooding_Custom() => this.combiner.Murmur3A_Tannergooding_Custom();

        [Benchmark]
        public int Murmur3A_TannergoodingWithSeed_Custom() => this.combiner.Murmur3A_TannergoodingWithSeed_Custom();

        [Benchmark]
        public int Murmur3A_Steps_Custom() => this.combiner.Murmur3A_Steps_Custom();

        [Benchmark]
        public int Murmur3A_Combine_Generic() => this.combiner.Murmur3A_Combine_Generic();

        [Benchmark]
        public int Murmur3A_Block_Generic() => this.combiner.Murmur3A_Block_Generic();

        [Benchmark]
        public int HSip13_Combine_Generic() => this.combiner.HSip13_Combine_Generic();

        [Benchmark]
        public int HSip13_Block_Generic() => this.combiner.HSip13_Block_Generic();

        [Benchmark]
        public int HSip24_Combine_Generic() => this.combiner.HSip24_Combine_Generic();

        [Benchmark]
        public int Sip13_Combine_Generic() => this.combiner.Sip13_Combine_Generic();

        [Benchmark]
        public int Sip24_Combine_Generic() => this.combiner.Sip24_Combine_Generic();

        [Benchmark]
        public int Marvin32_Combine_Generic() => this.combiner.Marvin32_Combine_Generic();

        [Benchmark]
        public int Marvin32_Block_Generic() => this.combiner.Marvin32_Block_Generic();

        [Benchmark]
        public int XXHash32_Combine_Generic() => this.combiner.XXHash32_Combine_Generic();

        [Benchmark]
        public int XXHash32_Block_Generic() => this.combiner.XXHash32_Block_Generic();

        [Benchmark]
        public int XXHash64_Combine_Generic() => this.combiner.XXHash64_Combine_Generic();

        [Benchmark]
        public int XXHash64_Block_Generic() => this.combiner.XXHash64_Block_Generic();

        [Benchmark]
        public int SeaHash_Combine_Generic() => this.combiner.SeaHash_Combine_Generic();

        [Benchmark]
        public int SeaHash_Block_Generic() => this.combiner.SeaHash_Block_Generic();

        [Benchmark]
        public int SpookyV2_Combine_Generic() => this.combiner.SpookyV2_Combine_Generic();

        private sealed class Combiner1 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAddReordered_Custom() => MultiplyAddReorderedCombiner.Combine(1);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(1);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(1);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(1);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(1);
            public int Murmur3A_Combine_Generic() => default(Murmur3x8632Hasher.Combiner).Combine(1);
            public int Murmur3A_Block_Generic() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(1);
            public int HSip13_Combine_Generic() => default(HalfSip13Hasher.Combiner).Combine(1, 2);
            public int HSip13_Block_Generic() => GenericCombiner<HalfSip13Hasher.Block>.Combine(1);
            public int HSip24_Combine_Generic() => default(HalfSip24Hasher.Combiner).Combine(1);
            public int Sip13_Combine_Generic() => default(Sip13Hasher.Combiner).Combine(1);
            public int Sip24_Combine_Generic() => default(Sip24Hasher.Combiner).Combine(1);
            public int Marvin32_Combine_Generic() => default(Marvin32Hasher.Combiner).Combine(1);
            public int Marvin32_Block_Generic() => GenericCombiner<Marvin32Hasher.Block>.Combine(1);
            public int XXHash32_Combine_Generic() => default(XXHash32Hasher.Combiner).Combine(1, 2);
            public int XXHash32_Block_Generic() => GenericCombiner<XXHash32Hasher.Block>.Combine(1);
            public int XXHash64_Combine_Generic() => default(XXHash64Hasher.Combiner).Combine(1, 2);
            public int XXHash64_Block_Generic() => GenericCombiner<XXHash64Hasher.Block>.Combine(1);
            public int SeaHash_Combine_Generic() => default(SeaHasher.Combiner).Combine(1);
            public int SeaHash_Block_Generic() => GenericCombiner<SeaHasher.Block>.Combine(1);
            public int SpookyV2_Combine_Generic() => default(SpookyV2Hasher.Combiner).Combine(1);
        }

        private sealed class Combiner2 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAddReordered_Custom() => MultiplyAddReorderedCombiner.Combine(1, 2);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(1, 2);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(1, 2);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(1, 2);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(1, 2);
            public int Murmur3A_Combine_Generic() => default(Murmur3x8632Hasher.Combiner).Combine(1, 2);
            public int Murmur3A_Block_Generic() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(1, 2);
            public int HSip13_Combine_Generic() => default(HalfSip13Hasher.Combiner).Combine(1, 2);
            public int HSip13_Block_Generic() => GenericCombiner<HalfSip13Hasher.Block>.Combine(1, 2);
            public int HSip24_Combine_Generic() => default(HalfSip24Hasher.Combiner).Combine(1, 2);
            public int Sip13_Combine_Generic() => default(Sip13Hasher.Combiner).Combine(1, 2);
            public int Sip24_Combine_Generic() => default(Sip24Hasher.Combiner).Combine(1, 2);
            public int Marvin32_Combine_Generic() => default(Marvin32Hasher.Combiner).Combine(1, 2);
            public int Marvin32_Block_Generic() => GenericCombiner<Marvin32Hasher.Block>.Combine(1, 2);
            public int XXHash32_Combine_Generic() => default(XXHash32Hasher.Combiner).Combine(1, 2);
            public int XXHash32_Block_Generic() => GenericCombiner<XXHash32Hasher.Block>.Combine(1, 2);
            public int XXHash64_Combine_Generic() => default(XXHash64Hasher.Combiner).Combine(1, 2);
            public int XXHash64_Block_Generic() => GenericCombiner<XXHash64Hasher.Block>.Combine(1, 2);
            public int SeaHash_Combine_Generic() => default(SeaHasher.Combiner).Combine(1, 2);
            public int SeaHash_Block_Generic() => GenericCombiner<SeaHasher.Block>.Combine(1, 2);
            public int SpookyV2_Combine_Generic() => default(SpookyV2Hasher.Combiner).Combine(1, 2);
        }

        private sealed class Combiner4 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAddReordered_Custom() => MultiplyAddReorderedCombiner.Combine(1, 2, 3, 4);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(1, 2, 3, 4);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(1, 2, 3, 4);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(1, 2, 3, 4);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(1, 2, 3, 4);
            public int Murmur3A_Combine_Generic() => default(Murmur3x8632Hasher.Combiner).Combine(1, 2, 3, 4);
            public int Murmur3A_Block_Generic() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(1, 2, 3, 4);
            public int HSip13_Combine_Generic() => default(HalfSip13Hasher.Combiner).Combine(1, 2, 3, 4);
            public int HSip13_Block_Generic() => GenericCombiner<HalfSip13Hasher.Block>.Combine(1, 2, 3, 4);
            public int HSip24_Combine_Generic() => default(HalfSip24Hasher.Combiner).Combine(1, 2, 3, 4);
            public int Sip13_Combine_Generic() => default(Sip13Hasher.Combiner).Combine(1, 2, 3, 4);
            public int Sip24_Combine_Generic() => default(Sip24Hasher.Combiner).Combine(1, 2, 3, 4);
            public int Marvin32_Combine_Generic() => default(Marvin32Hasher.Combiner).Combine(1, 2, 3, 4);
            public int Marvin32_Block_Generic() => GenericCombiner<Marvin32Hasher.Block>.Combine(1, 2, 3, 4);
            public int XXHash32_Combine_Generic() => default(XXHash32Hasher.Combiner).Combine(1, 2, 3, 4);
            public int XXHash32_Block_Generic() => GenericCombiner<XXHash32Hasher.Block>.Combine(1, 2, 3, 4);
            public int XXHash64_Combine_Generic() => default(XXHash64Hasher.Combiner).Combine(1, 2, 3, 4);
            public int XXHash64_Block_Generic() => GenericCombiner<XXHash64Hasher.Block>.Combine(1, 2, 3, 4);
            public int SeaHash_Combine_Generic() => default(SeaHasher.Combiner).Combine(1, 2, 3, 4);
            public int SeaHash_Block_Generic() => GenericCombiner<SeaHasher.Block>.Combine(1, 2, 3, 4);
            public int SpookyV2_Combine_Generic() => default(SpookyV2Hasher.Combiner).Combine(1, 2, 3, 4);
        }

        private sealed class Combiner8 : ICombine
        {
            public int Empty() => 0;

            public int MultiplyAddReordered_Custom() => MultiplyAddReorderedCombiner.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int MultiplyAdd_Custom() => MultiplyAddCombiner.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Murmur3A_Tannergooding_Custom() => Murmur3A_TG_Combiner.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Murmur3A_TannergoodingWithSeed_Custom() => Murmur3A_TG_WithSeed_Combiner.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Murmur3A_Steps_Custom() => Murmur3AStepsCombiner.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Murmur3A_Combine_Generic() => default(Murmur3x8632Hasher.Combiner).Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Murmur3A_Block_Generic() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int HSip13_Combine_Generic() => default(HalfSip13Hasher.Combiner).Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int HSip13_Block_Generic() => GenericCombiner<HalfSip13Hasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int HSip24_Combine_Generic() => default(HalfSip24Hasher.Combiner).Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Sip13_Combine_Generic() => default(Sip13Hasher.Combiner).Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Sip24_Combine_Generic() => default(Sip24Hasher.Combiner).Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Marvin32_Combine_Generic() => default(Marvin32Hasher.Combiner).Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Marvin32_Block_Generic() => GenericCombiner<Marvin32Hasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int XXHash32_Combine_Generic() => default(XXHash32Hasher.Combiner).Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int XXHash32_Block_Generic() => GenericCombiner<XXHash32Hasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int XXHash64_Combine_Generic() => default(XXHash64Hasher.Combiner).Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int XXHash64_Block_Generic() => GenericCombiner<XXHash64Hasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int SeaHash_Combine_Generic() => default(SeaHasher.Combiner).Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int SeaHash_Block_Generic() => GenericCombiner<SeaHasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int SpookyV2_Combine_Generic() => default(SpookyV2Hasher.Combiner).Combine(1, 2, 3, 4, 5, 6, 7, 8);
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
            int Murmur3A_Steps_Custom();
            int Murmur3A_Combine_Generic();
            int Murmur3A_Block_Generic();
            int HSip13_Combine_Generic();
            int HSip13_Block_Generic();
            int HSip24_Combine_Generic();
            int Sip13_Combine_Generic();
            int Sip24_Combine_Generic();
            int Marvin32_Combine_Generic();
            int Marvin32_Block_Generic();
            int XXHash32_Combine_Generic();
            int XXHash32_Block_Generic();
            int XXHash64_Combine_Generic();
            int XXHash64_Block_Generic();
            int SeaHash_Block_Generic();
            int SeaHash_Combine_Generic();
            int SpookyV2_Combine_Generic();
        }
    }
}
