using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class XXHash64Steps
    {
        internal const int FullBlockSize = sizeof(ulong) * 4;
        internal const ulong Prime64n1 = 11400714785074694791UL;
        internal const ulong Prime64n2 = 14029467366897019727UL;
        internal const ulong Prime64n3 = 1609587929392839161UL;
        internal const ulong Prime64n4 = 9650029242287828579UL;
        internal const ulong Prime64n5 = 2870177450012600261UL;

        public static class Long
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Initialize(ulong seed, out ulong v1, out ulong v2, out ulong v3, out ulong v4)
            {
                v1 = seed + Prime64n1 + Prime64n2;
                v2 = seed + Prime64n2;
                v3 = seed + 0;
                v4 = seed - Prime64n1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MixStep(
                ref ulong v1, ref ulong v2, ref ulong v3, ref ulong v4, ulong x0, ulong x1, ulong x2, ulong x3)
            {
                v1 += x0 * Prime64n2;
                v2 += x1 * Prime64n2;
                v3 += x2 * Prime64n2;
                v4 += x3 * Prime64n2;
                v1 = BitOps.RotateLeft(v1, 31);
                v2 = BitOps.RotateLeft(v2, 31);
                v3 = BitOps.RotateLeft(v3, 31);
                v4 = BitOps.RotateLeft(v4, 31);
                v1 *= Prime64n1;
                v2 *= Prime64n1;
                v3 *= Prime64n1;
                v4 *= Prime64n1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ulong GetSmallState(ref ulong v1, ref ulong v2, ref ulong v3, ref ulong v4)
            {
                var h = BitOps.RotateLeft(v1, 1) +
                    BitOps.RotateLeft(v2, 7) +
                    BitOps.RotateLeft(v3, 12) +
                    BitOps.RotateLeft(v4, 18);

                v1 *= Prime64n2;
                v2 *= Prime64n2;
                v3 *= Prime64n2;
                v4 *= Prime64n2;
                v1 = BitOps.RotateLeft(v1, 31);
                v2 = BitOps.RotateLeft(v2, 31);
                v3 = BitOps.RotateLeft(v3, 31);
                v4 = BitOps.RotateLeft(v4, 31);
                v1 *= Prime64n1;
                v2 *= Prime64n1;
                v3 *= Prime64n1;
                v4 *= Prime64n1;
                h ^= v1;
                h = (h * Prime64n1) + Prime64n4;
                h ^= v2;
                h = (h * Prime64n1) + Prime64n4;
                h ^= v3;
                h = (h * Prime64n1) + Prime64n4;
                h ^= v4;
                h = (h * Prime64n1) + Prime64n4;

                return h;
            }
        }

        public static class Short
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Initialize(ulong start, ulong length, out ulong h)
            {
                h = start + length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MixFinalPartialBlock(ref ulong h, int remaining, ulong x0, ulong x1, ulong x2, ulong x3)
            {
                ulong remainder;
                if (remaining >= (3 * sizeof(ulong)))
                {
                    h = MixFinalLong(h, x0);
                    h = MixFinalLong(h, x1);
                    h = MixFinalLong(h, x2);
                    remainder = x3;
                }
                else if (remaining >= (2 * sizeof(ulong)))
                {
                    h = MixFinalLong(h, x0);
                    h = MixFinalLong(h, x1);
                    remainder = x2;
                }
                else if (remaining >= sizeof(ulong))
                {
                    h = MixFinalLong(h, x0);
                    remainder = x1;
                }
                else
                {
                    remainder = x0;
                }

                if ((remaining % sizeof(ulong)) >= sizeof(uint))
                {
                    h = MixFinalInt(h, (uint)remainder);
                    remainder = remainder >> 32;
                }

                MixFinalBytes(ref h, (uint)remainder, remaining % sizeof(uint));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void MixFinalBytes(ref ulong state, uint remainder, int byteCount)
            {
                if (byteCount > 0) { MixFinalByte(ref state, UnsafeByteOps.ToByte(ref remainder, 0)); }
                if (byteCount > 1) { MixFinalByte(ref state, UnsafeByteOps.ToByte(ref remainder, 1)); }
                if (byteCount > 2) { MixFinalByte(ref state, UnsafeByteOps.ToByte(ref remainder, 2)); }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static ulong MixFinalLong(ulong h, ulong i)
            {
                var k1 = i;
                k1 *= XXHash64Steps.Prime64n2;
                k1 = BitOps.RotateLeft(k1, 31);
                k1 *= XXHash64Steps.Prime64n1;
                h ^= k1;
                h = (BitOps.RotateLeft(h, 27) * XXHash64Steps.Prime64n1) + XXHash64Steps.Prime64n4;
                return h;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static ulong MixFinalInt(ulong h, uint i)
            {
                h ^= i * XXHash64Steps.Prime64n3;
                h = (BitOps.RotateLeft(h, 23) * XXHash64Steps.Prime64n2) + XXHash64Steps.Prime64n3;
                return h;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void MixFinalByte(ref ulong h, byte b)
            {
                h ^= b * XXHash64Steps.Prime64n5;
                h = BitOps.RotateLeft(h, 11) * XXHash64Steps.Prime64n1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ulong Finish(ref ulong h)
            {
                h ^= h >> 33;
                h *= XXHash64Steps.Prime64n2;
                h ^= h >> 29;
                h *= XXHash64Steps.Prime64n3;
                h ^= h >> 32;

                return h;
            }
        }
    }
}
