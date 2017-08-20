using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    internal static class HalfSip24Steps
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize((uint k0, uint k1) key, out uint v0, out uint v1, out uint v2, out uint v3)
        {
            v0 = key.k0;
            v1 = key.k1;
            v2 = 0x6c796765 ^ key.k0;
            v3 = 0x74656462 ^ key.k1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SipCRound(ref uint v0, ref uint v1, ref uint v2, ref uint v3, uint block)
        {
            v3 ^= block;
            MixState(ref v0, ref v1, ref v2, ref v3);
            MixState(ref v0, ref v1, ref v2, ref v3);
            v0 ^= block;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SipDRound(ref uint v0, ref uint v1, ref uint v2, ref uint v3)
        {
            MixState(ref v0, ref v1, ref v2, ref v3);
            MixState(ref v0, ref v1, ref v2, ref v3);
            MixState(ref v0, ref v1, ref v2, ref v3);
            MixState(ref v0, ref v1, ref v2, ref v3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Finish(ref uint v0, ref uint v1, ref uint v2, ref uint v3, uint partialBlock, uint length)
        {
            var block = partialBlock | (length << 24);
            SipCRound(ref v0, ref v1, ref v2, ref v3, block);

            v2 ^= 0xff;
            SipDRound(ref v0, ref v1, ref v2, ref v3);

            return (int)(v1 ^ v3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MixState(ref uint v0, ref uint v1, ref uint v2, ref uint v3)
        {
            unchecked
            {
                v0 += v1;
                v2 += v3;
                v1 = BitOps.RotateLeft(v1, 5);
                v3 = BitOps.RotateLeft(v3, 8);
                v1 ^= v0;
                v3 ^= v2;
                v0 = BitOps.RotateLeft(v0, 16);
                v2 += v1;
                v0 += v3;
                v1 = BitOps.RotateLeft(v1, 13);
                v3 = BitOps.RotateLeft(v3, 7);
                v1 ^= v2;
                v3 ^= v0;
                v2 = BitOps.RotateLeft(v2, 16);
            }
        }
    }
}
