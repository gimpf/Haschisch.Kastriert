using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public class HashByteArray_CompareMurmur3x8632
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
        public int Murmur3x8632_Block()
        {
            return HashByteArrayUtil.HashWithBlock<Murmur3x8632Hasher.Block>(this.data);
        }

        [Benchmark]
        public int Murmur3x8632_Stream()
        {
            return HashByteArrayUtil.HashWithStreaming<Murmur3x8632Hasher.Stream>(this.data);
        }

        [Benchmark]
        public int Murmur3x8632_Stream_ByU32()
        {
            return HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<Murmur3x8632Hasher.Stream>(this.data);
        }
    }
}
