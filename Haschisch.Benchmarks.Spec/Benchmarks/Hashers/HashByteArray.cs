using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    // Compares the runtime performance of various hash implementations, not
    // taking hash-quality into account at all.
    public class HashByteArray
    {
        private byte[] data;

        [Params(4, 8, 16, 32, 256, 1024, 4096, 16 * 1024)]
        public int Bytes { get; set; }

        [GlobalSetup]
        public void InitializeData()
        {
            this.data = new byte[this.Bytes];
            new System.Random(42).NextBytes(this.data);
        }

        [GlobalCleanup]
        public void ClearData()
        {
            // BenchmarkDotNet will do a full GC between runs
            // To get accurate data, free this data, as otherwise it
            // will become available for GC in Setup, messing around
            // with mem-results.
            this.data = null;
        }

        [Benchmark]
        public int SeaHash_Block()
        {
            return HashByteArrayUtil.HashWithBlock<SeaHasher.Block>(this.data);
        }

        [Benchmark]
        public int Murmur3x8632_Block()
        {
            return HashByteArrayUtil.HashWithBlock<Murmur3x8632Hasher.Block>(this.data);
        }

        [Benchmark]
        public int XxHash32_Block()
        {
            return HashByteArrayUtil.HashWithBlock<XXHash32Hasher.Block>(this.data);
        }

        [Benchmark]
        public int XxHash64_Block()
        {
            return HashByteArrayUtil.HashWithBlock<XXHash64Hasher.Block>(this.data);
        }

        [Benchmark]
        public int HalfSip13_Block()
        {
            return HashByteArrayUtil.HashWithBlock<HalfSip13Hasher.Block>(this.data);
        }

        [Benchmark]
        public int HalfSip24_ByBlock()
        {
            return HashByteArrayUtil.HashWithBlock<HalfSip24Hasher.Block>(this.data);
        }

        [Benchmark]
        public int Sip13_ByBlock()
        {
            return HashByteArrayUtil.HashWithBlock<Sip13Hasher.Block>(this.data);
        }

        [Benchmark]
        public int Sip24_ByBlock()
        {
            return HashByteArrayUtil.HashWithBlock<Sip24Hasher.Block>(this.data);
        }

        [Benchmark]
        public int Marvin32_Block()
        {
            return HashByteArrayUtil.HashWithBlock<Marvin32Hasher.Block>(this.data);
        }

        [Benchmark]
        public int SpookyV2_Block()
        {
            return HashByteArrayUtil.HashWithBlock<SpookyV2Hasher.Block>(this.data);
        }
    }
}
