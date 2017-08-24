using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public class HashByteArray_CompareXXHash32
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

        [Benchmark]
        public int XxHash32_Block()
        {
            return HashByteArrayUtil.HashWithBlock<XXHash32Hasher.Block>(this.data);
        }

        [Benchmark]
        public int XxHash32_Stream()
        {
            return HashByteArrayUtil.HashWithStreaming<XXHash32Hasher.Stream>(this.data);
        }

        [Benchmark]
        public int XxHash32_Stream_ByU32()
        {
            return HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<XXHash32Hasher.Stream>(this.data);
        }
    }
}
