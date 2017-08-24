using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;
using Haschisch.Hashers.Adapters;

namespace Haschisch.Benchmarks
{
    public class HashByteArray_CompareSeaHash
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
        public void ClearData() { this.data = null; }

        [Benchmark(Baseline = true)]
        public int SeaHash_Block()
        {
            return HashByteArrayUtil.HashWithBlock<SeaHasher.Block>(this.data);
        }

        [Benchmark]
        public int SeaHash_Stream()
        {
            return HashByteArrayUtil.HashWithStreaming<SeaHasher.Stream>(this.data);
        }

        [Benchmark]
        public int SeaHash_Stream_ByU32()
        {
            return HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<SeaHasher.Stream>(this.data);
        }
    }
}
