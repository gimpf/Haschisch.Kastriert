using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public class HashByteArray_CompareHSip24
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

        [Benchmark]
        public int HalfSip24_Block()
        {
            return HashByteArrayUtil.HashWithBlock<HalfSip24Hasher.Block>(this.data);
        }

        [Benchmark]
        public int HalfSip24_Stream()
        {
            return HashByteArrayUtil.HashWithStreaming<HalfSip24Hasher.Stream>(this.data);
        }

        [Benchmark]
        public int HalfSip24_Stream_ByU32()
        {
            return HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<HalfSip24Hasher.Stream>(this.data);
        }
    }
}
