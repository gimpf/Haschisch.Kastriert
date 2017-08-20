using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public class HashByteArray_CompareXXHash64
    {
        private byte[] data;

        [Params(4, 32, 256, 1024, 4096)]
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
        public int XxHash64_Block()
        {
            return HashByteArrayUtil.HashWithBlock<XXHash64Hasher.Block>(this.data);
        }

        [Benchmark]
        public int XxHash64_Stream()
        {
            return HashByteArrayUtil.HashWithStreaming<XXHash64Hasher.Stream>(this.data);
        }

        [Benchmark]
        public int XxHash64_Stream_ByU32()
        {
            return HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<XXHash64Hasher.Stream>(this.data);
        }
    }
}
