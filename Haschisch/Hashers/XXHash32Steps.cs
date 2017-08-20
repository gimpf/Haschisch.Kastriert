using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    internal static class XXHash32Steps
    {
        public const int FullBlockSize = 4 * sizeof(uint);
        internal const uint Prime32n1 = 2654435761U;
        internal const uint Prime32n2 = 2246822519U;
        internal const uint Prime32n3 = 3266489917U;
        internal const uint Prime32n4 = 668265263U;
        internal const uint Prime32n5 = 374761393U;

        public static class Long
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Initialize(uint seed, out uint v1, out uint v2, out uint v3, out uint v4)
            {
                v1 = seed + Prime32n1 + Prime32n2;
                v2 = seed + Prime32n2;
                v3 = seed + 0;
                v4 = seed - Prime32n1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MixStep(ref uint v1, ref uint v2, ref uint v3, ref uint v4, uint x0, uint x1, uint x2, uint x3)
            {
                v1 += x0 * Prime32n2;
                v2 += x1 * Prime32n2;
                v3 += x2 * Prime32n2;
                v4 += x3 * Prime32n2;
                v1 = BitOps.RotateLeft(v1, 13);
                v2 = BitOps.RotateLeft(v2, 13);
                v3 = BitOps.RotateLeft(v3, 13);
                v4 = BitOps.RotateLeft(v4, 13);
                v1 *= Prime32n1;
                v2 *= Prime32n1;
                v3 *= Prime32n1;
                v4 *= Prime32n1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static uint GetSmallStateStart(ref uint v1, ref uint v2, ref uint v3, ref uint v4) =>
                BitOps.RotateLeft(v1, 1) +
                BitOps.RotateLeft(v2, 7) +
                BitOps.RotateLeft(v3, 12) +
                BitOps.RotateLeft(v4, 18);
        }

        public static class Short
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Initialize(uint start, uint length, out uint state)
            {
                state = start + length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MixFinalPartialBlock(ref uint state, int remaining, uint x0, uint x1, uint x2, uint x3)
            {
                uint remainder;
                if (remaining >= 3 * sizeof(uint))
                {
                    MixFinalInt(ref state, x0);
                    MixFinalInt(ref state, x1);
                    MixFinalInt(ref state, x2);
                    remainder = x3;
                }
                else if (remaining >= 2 * sizeof(uint))
                {
                    MixFinalInt(ref state, x0);
                    MixFinalInt(ref state, x1);
                    remainder = x2;
                }
                else if (remaining >= sizeof(uint))
                {
                    MixFinalInt(ref state, x0);
                    remainder = x1;
                }
                else
                {
                    remainder = x0;
                }

                MixFinalBytes(ref state, remainder, remaining % sizeof(uint));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MixFinalInt(ref uint state, uint i)
            {
                state += i * Prime32n3;
                state = BitOps.RotateLeft(state, 17);
                state *= Prime32n4;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MixFinalBytes(ref uint state, uint remainder, int byteCount)
            {
                if (byteCount > 0) { MixFinalByte(ref state, UnsafeByteOps.ToByte(ref remainder, 0)); }
                if (byteCount > 1) { MixFinalByte(ref state, UnsafeByteOps.ToByte(ref remainder, 1)); }
                if (byteCount > 2) { MixFinalByte(ref state, UnsafeByteOps.ToByte(ref remainder, 2)); }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void MixFinalByte(ref uint state, byte b)
            {
                state += b * Prime32n5;
                state = BitOps.RotateLeft(state, 11) * Prime32n1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static uint Finish(ref uint state)
            {
                state ^= state >> 15;
                state *= Prime32n2;
                state ^= state >> 13;
                state *= Prime32n3;
                state ^= state >> 16;

                return state;
            }
        }
    }
}
