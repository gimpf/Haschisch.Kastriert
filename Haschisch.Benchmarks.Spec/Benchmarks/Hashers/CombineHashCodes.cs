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
        public int SimpleMultiplyAdd() =>
            this.combiner.CustomSimpleMultiplyAdd();

        [Benchmark]
        public int CustomMurmur3FromIssue() =>
            this.combiner.CustomFromIssue();

        [Benchmark]
        public int HSip13() =>
            this.combiner.HSip13();

        [Benchmark]
        public int Marvin32() =>
            this.combiner.Marvin32();

        [Benchmark]
        public int Murmur3A() =>
            this.combiner.Murmur3A();

        [Benchmark]
        public int XXHash32() =>
            this.combiner.XXHash32();

        [Benchmark]
        public int XXHash64() =>
            this.combiner.XXHash64();

        [Benchmark]
        public int CustomMurmur3A() =>
            this.combiner.CustomMurmur();

        private sealed class Combiner1 : ICombine
        {
            public int Empty() => 0;

            public int CustomSimpleMultiplyAdd() => SimpleMixCombiner.CombineSimple(1);
            public int CustomFromIssue() => HashCode.Combine(1);
            public int CustomMurmur() => Murmur3Combiner.Combine(1);

            public int HSip13() => GenericCombiner<HalfSip13Hasher.Block>.Combine(1);
            public int Marvin32() => GenericCombiner<Marvin32Hasher.Block>.Combine(1);
            public int Murmur3A() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(1);
            public int XXHash32() => GenericCombiner<XXHash32Hasher.Block>.Combine(1);
            public int XXHash64() => GenericCombiner<XXHash64Hasher.Block>.Combine(1);
        }

        private sealed class Combiner2 : ICombine
        {
            public int Empty() => 0;

            public int CustomSimpleMultiplyAdd() => SimpleMixCombiner.CombineSimple(1, 2);
            public int CustomFromIssue() => HashCode.Combine(1, 2);
            public int CustomMurmur() => Murmur3Combiner.Combine(1, 2);

            public int HSip13() => GenericCombiner<HalfSip13Hasher.Block>.Combine(1, 2);
            public int Marvin32() => GenericCombiner<Marvin32Hasher.Block>.Combine(1, 2);
            public int Murmur3A() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(1, 2);
            public int XXHash32() => GenericCombiner<XXHash32Hasher.Block>.Combine(1, 2);
            public int XXHash64() => GenericCombiner<XXHash64Hasher.Block>.Combine(1, 2);
        }

        private sealed class Combiner4 : ICombine
        {
            public int Empty() => 0;

            public int CustomSimpleMultiplyAdd() => SimpleMixCombiner.CombineSimple(1, 2, 3, 4);
            public int CustomFromIssue() => HashCode.Combine(1, 2, 3, 4);
            public int CustomMurmur() => Murmur3Combiner.Combine(1, 2, 3, 4);

            public int HSip13() => GenericCombiner<HalfSip13Hasher.Block>.Combine(1, 2, 3, 4);
            public int Marvin32() => GenericCombiner<Marvin32Hasher.Block>.Combine(1, 2, 3, 4);
            public int Murmur3A() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(1, 2, 3, 4);
            public int XXHash32() => GenericCombiner<XXHash32Hasher.Block>.Combine(1, 2, 3, 4);
            public int XXHash64() => GenericCombiner<XXHash64Hasher.Block>.Combine(1, 2, 3, 4);
        }

        private sealed class Combiner8 : ICombine
        {
            public int Empty() => 0;

            public int CustomSimpleMultiplyAdd() => SimpleMixCombiner.CombineSimple(1, 2, 3, 4, 5, 6, 7, 8);
            public int CustomFromIssue() => HashCode.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int CustomMurmur() => Murmur3Combiner.Combine(1, 2, 3, 4, 5, 6, 7, 8);

            public int HSip13() => GenericCombiner<HalfSip13Hasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Marvin32() => GenericCombiner<Marvin32Hasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int Murmur3A() => GenericCombiner<Murmur3x8632Hasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int XXHash32() => GenericCombiner<XXHash32Hasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
            public int XXHash64() => GenericCombiner<XXHash64Hasher.Block>.Combine(1, 2, 3, 4, 5, 6, 7, 8);
        }

        private interface ICombine
        {
            int Empty();

            int CustomSimpleMultiplyAdd();
            int CustomFromIssue();
            int CustomMurmur();

            int HSip13();
            int Marvin32();
            int Murmur3A();
            int XXHash32();
            int XXHash64();
        }
    }
}
