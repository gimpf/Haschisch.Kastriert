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

        [Benchmark][BenchmarkCategory("array", "throughput", "sea", "prime", "no-seed")]
        public int SeaHash_Block() => HashByteArrayUtil.HashWithBlock<SeaHasher.Block>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "sea", "variant", "no-seed")]
        public int SeaHash_Stream() => HashByteArrayUtil.HashWithStreaming<SeaHasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "sea", "variant", "no-seed")]
        public int SeaHash_Stream_ByU32() => HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<SeaHasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "murmur-3-32", "prime", "per-ad-seed")]
        public int Murmur3x8632_Block() => HashByteArrayUtil.HashWithBlock<Murmur3x8632Hasher.Block>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "murmur-3-32", "variant", "per-ad-seed")]
        public int Murmur3x8632_Stream() => HashByteArrayUtil.HashWithStreaming<Murmur3x8632Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "murmur-3-32", "variant", "per-ad-seed")]
        public int Murmur3x8632_Stream_ByU32() => HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<Murmur3x8632Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "xx32", "prime", "per-ad-seed")]
        public int XxHash32_Block() => HashByteArrayUtil.HashWithBlock<XXHash32Hasher.Block>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "xx32", "variant", "per-ad-seed")]
        public int XxHash32_Stream() => HashByteArrayUtil.HashWithStreaming<XXHash32Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "xx32", "variant", "per-ad-seed")]
        public int XxHash32_Stream_ByU32() => HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<XXHash32Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "xx64", "prime", "per-ad-seed")]
        public int XxHash64_Block() => HashByteArrayUtil.HashWithBlock<XXHash64Hasher.Block>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "xx64", "variant", "per-ad-seed")]
        public int XxHash64_Stream() => HashByteArrayUtil.HashWithStreaming<XXHash64Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "xx64", "variant", "per-ad-seed")]
        public int XxHash64_Stream_ByU32() => HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<XXHash64Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "hsip", "hsip-1-3", "prime")]
        public int HalfSip13_Block() => HashByteArrayUtil.HashWithBlock<HalfSip13Hasher.Block>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "hsip", "hsip-1-3", "variant", "per-ad-seed")]
        public int HalfSip13_Stream() => HashByteArrayUtil.HashWithStreaming<HalfSip13Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "hsip", "hsip-1-3", "variant", "per-ad-seed")]
        public int HalfSip13_Stream_ByU32() => HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<HalfSip13Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "hsip", "hsip-2-4", "prime", "per-ad-seed")]
        public int HalfSip24_Block() => HashByteArrayUtil.HashWithBlock<HalfSip24Hasher.Block>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "hsip", "hsip-2-4", "variant", "per-ad-seed")]
        public int HalfSip24_Stream() => HashByteArrayUtil.HashWithStreaming<HalfSip24Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "hsip", "hsip-2-4", "variant", "per-ad-seed")]
        public int HalfSip24_Stream_ByU32() => HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<HalfSip24Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "sip", "sip-1-3", "prime", "per-ad-seed")]
        public int Sip13_Block() => HashByteArrayUtil.HashWithBlock<Sip13Hasher.Block>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "sip", "sip-1-3", "variant", "per-ad-seed")]
        public int Sip13_Stream() => HashByteArrayUtil.HashWithStreaming<Sip13Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "sip", "sip-1-3", "variant", "per-ad-seed")]
        public int Sip13_Stream_ByU32() => HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<Sip13Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "sip", "sip-2-4", "prime", "per-ad-seed")]
        public int Sip24_Block() => HashByteArrayUtil.HashWithBlock<Sip24Hasher.Block>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "sip", "sip-2-4", "variant", "per-ad-seed")]
        public int Sip24_Stream() => HashByteArrayUtil.HashWithStreaming<Sip24Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "sip", "sip-2-4", "variant", "per-ad-seed")]
        public int Sip24_Stream_ByU32() => HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<Sip24Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "marvin32", "prime", "per-ad-seed")]
        public int Marvin32_Block() => HashByteArrayUtil.HashWithBlock<Marvin32Hasher.Block>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "marvin32", "variant", "per-ad-seed")]
        public int Marvin32_Stream() => HashByteArrayUtil.HashWithStreaming<Marvin32Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "marvin32", "variant", "per-ad-seed")]
        public int Marvin32_Stream_ByU32() => HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<Marvin32Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "spookyv2", "prime", "per-ad-seed")]
        public int SpookyV2_Block() => HashByteArrayUtil.HashWithBlock<SpookyV2Hasher.Block>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "spookyv2", "variant", "per-ad-seed")]
        public int SpookyV2_Stream() => HashByteArrayUtil.HashWithStreaming<SpookyV2Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "spookyv2", "variant", "per-ad-seed")]
        public int SpookyV2_Stream_ByU32() => HashByteArrayUtil.HashWithStreamingByBlockedU32Unsafe<SpookyV2Hasher.Stream>(this.data);

        [Benchmark][BenchmarkCategory("array", "throughput", "city32", "prime", "no-seed")]
        public int City32_Block() => HashByteArrayUtil.HashWithBlock<City32Hasher.Block>(this.data);
    }
}
