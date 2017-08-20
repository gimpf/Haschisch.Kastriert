using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public class HashByteArray_CompareHSip13
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
        public int HalfSip13_Block()
        {
            return HashByteArrayUtil.HashWithBlock<HalfSip13Hasher.Block>(this.data);
        }

        [Benchmark]
        public int HalfSip13_Stream()
        {
            return HashByteArrayUtil.HashWithStreaming<HalfSip13Hasher.Stream>(this.data);
        }

        [Benchmark]
        public int HalfSip13_Stream_ByU32()
        {
            return HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<HalfSip13Hasher.Stream>(this.data);
        }
    }
}
