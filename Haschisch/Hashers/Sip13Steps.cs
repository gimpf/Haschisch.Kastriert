using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    internal static class Sip13Steps
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize((ulong k0, ulong k1) key, out ulong v0, out ulong v1, out ulong v2, out ulong v3)
        {
            v0 = 0x736f6d6570736575UL ^ key.k0;
            v1 = 0x646f72616e646f6dUL ^ key.k1;
            v2 = 0x6c7967656e657261UL ^ key.k0;
            v3 = 0x7465646279746573UL ^ key.k1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SipCRound(ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3, ulong block)
        {
            v3 ^= block;
            MixState(ref v0, ref v1, ref v2, ref v3);
            v0 ^= block;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Finish(ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3, ulong partialBlock, ulong length)
        {
            var block = partialBlock | (length << 56);
            SipCRound(ref v0, ref v1, ref v2, ref v3, block);

            v2 ^= 0xff;
            SipDRound(ref v0, ref v1, ref v2, ref v3);

            var sipHash = (v0 ^ v1) ^ (v2 ^ v3);
            return (long)sipHash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SipDRound(ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3)
        {
            MixState(ref v0, ref v1, ref v2, ref v3);
            MixState(ref v0, ref v1, ref v2, ref v3);
            MixState(ref v0, ref v1, ref v2, ref v3);
        }

        private static void MixState(ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3)
        {
            unchecked
            {
                v0 += v1;
                v2 += v3;
                v1 = BitOps.RotateLeft(v1, 13);
                v3 = BitOps.RotateLeft(v3, 16);
                v1 ^= v0;
                v3 ^= v2;
                v0 = BitOps.RotateLeft(v0, 32);
                v2 += v1;
                v0 += v3;
                v1 = BitOps.RotateLeft(v1, 17);
                v3 = BitOps.RotateLeft(v3, 21);
                v1 ^= v2;
                v3 ^= v0;
                v2 = BitOps.RotateLeft(v2, 32);
            }
        }
    }
}
