using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Benchmarks
{
    public static class HashByteArrayUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int HashWithBlock<T>(byte[] data) where T : struct, IBlockHasher<int> =>
            default(T).Hash(data, 0, data.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int HashWithStreaming<T>(byte[] data)
            where T : struct, IStreamingHasher<int>
        {
            var hasher = default(T);
            hasher.Initialize();

            var bytewiseStart = 0;
            if (hasher.AllowUnsafeWrite)
            {
                bytewiseStart = hasher.UnsafeWrite(ref data[0], data.Length);
            }
            else
            {
                bytewiseStart = data.Length - (data.Length % 8);
                WriteBlocks(ref hasher, ref data[0], bytewiseStart);
            }

            for (int i = bytewiseStart, len = data.Length; i < len; i++)
            {
                hasher.Write8(data[i]);
            }

            return hasher.Finish();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int HashWithStreamingByByte<T>(byte[] data)
            where T : struct, IStreamingHasher<int>
        {
            var hasher = default(T);
            hasher.Initialize();
            for (var i = 0; i < data.Length; i++)
            {
                hasher.Write8(data[i]);
            }
            return hasher.Finish();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int HashWithStreamingByBlockedU32Unsafe<T>(byte[] data)
            where T : struct, IStreamingHasher<int>
        {
            var hasher = default(T);
            hasher.Initialize();
            WriteBlocked(ref data[0], data.Length);
            return hasher.Finish();

            void WriteBlocked(ref byte dataPtr, int length)
            {
                var multiBlockEnd = length - (length % (4 * sizeof(uint)));
                var end = length - (length % sizeof(uint));

                for (var i = 0; i < multiBlockEnd; i += 4 * sizeof(uint))
                {
                    hasher.Write32(Unsafe.As<byte, int>(ref Unsafe.Add(ref dataPtr, i)));
                    hasher.Write32(Unsafe.As<byte, int>(ref Unsafe.Add(ref dataPtr, i + sizeof(uint))));
                    hasher.Write32(Unsafe.As<byte, int>(ref Unsafe.Add(ref dataPtr, i + (2 * sizeof(uint)))));
                    hasher.Write32(Unsafe.As<byte, int>(ref Unsafe.Add(ref dataPtr, i + (3 * sizeof(uint)))));
                }

                for (var i = multiBlockEnd; i < end; i += sizeof(uint))
                {
                    hasher.Write32(Unsafe.As<byte, int>(ref Unsafe.Add(ref dataPtr, i)));
                }

                for (var i = end; i < length; i++)
                {
                    hasher.Write8(Unsafe.Add(ref dataPtr, i));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteBlocks<T>(ref T hasher, ref byte dataRaw, int fullBlockEnd)
            where T : struct, IStreamingHasherSink
        {
            for (var i = 0; i < fullBlockEnd; i += sizeof(ulong))
            {
                hasher.Write64(Unsafe.As<byte, long>(ref Unsafe.Add(ref dataRaw, i)));
            }
        }
    }
}
