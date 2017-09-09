using BenchmarkDotNet.Attributes;
using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public class HashByteArray_CompareSip24
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
        public int Sip24_Block()
        {
            return HashByteArrayUtil.HashWithBlock<Sip24Hasher.Block>(this.data);
        }

        [Benchmark]
        public int Sip24_Stream()
        {
            return HashByteArrayUtil.HashWithStreaming<Sip24Hasher.Stream>(this.data);
        }

        [Benchmark]
        public int Sip24_Stream_ByU32()
        {
            return HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<Sip24Hasher.Stream>(this.data);
        }
    }
}
