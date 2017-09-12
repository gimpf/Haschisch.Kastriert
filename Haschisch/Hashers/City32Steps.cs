using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class City32Steps
    {
        private const ulong K0 = 0xc3a5c85c97cb3127UL;
        private const ulong K1 = 0xb492b66fbe98f273UL;
        private const ulong K2 = 0x9ae16a3b2f90404fUL;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static uint Hash(ref byte s, uint len)
        {
            if (len <= 24)
            {
                if (len <= 12)
                {
                    return len <= 4 ? Hash_Len0to4(ref s, len) : Hash_Len5to12(ref s, len);
                }

                return Hash_Len13to24(ref s, len);
            }

            return Hash_Gt24(ref s, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static uint Hash_Len0to4(ref byte s, uint len)
        {
            var b = 0u;
            var c = 9u;

            if (len > 0)
            {
                sbyte v = Unsafe.As<byte, sbyte>(ref s);
                b = (uint)(b * Murmur3x8632Steps.C1 + v);
                c ^= b;
            }

            if (len > 1)
            {
                sbyte v = Unsafe.As<byte, sbyte>(ref Unsafe.Add(ref s, 1));
                b = (uint)(b * Murmur3x8632Steps.C1 + v);
                c ^= b;
            }

            if (len > 2)
            {
                sbyte v = Unsafe.As<byte, sbyte>(ref Unsafe.Add(ref s, 2));
                b = (uint)(b * Murmur3x8632Steps.C1 + v);
                c ^= b;
            }

            if (len > 3)
            {
                sbyte v = Unsafe.As<byte, sbyte>(ref Unsafe.Add(ref s, 3));
                b = (uint)(b * Murmur3x8632Steps.C1 + v);
                c ^= b;
            }

            var h = Murmur3x8632Steps.MixStep(len, c);
            h = Murmur3x8632Steps.MixStep(b, h);
            return Murmur3x8632Steps.FMix32(h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static uint Hash_Len5to12(ref byte s, uint len)
        {
            var a = len;
            var b = len * 5;
            var c = 9u;
            var d = b;

            a += Fetch32(ref s, 0);
            b += Fetch32(ref s, len - 4);
            c += Fetch32(ref s, (len >> 1) & 4);

            var h = Murmur3x8632Steps.MixStep(a, d);
            h = Murmur3x8632Steps.MixStep(b, h);
            h = Murmur3x8632Steps.MixStep(c, h);
            return Murmur3x8632Steps.FMix32(h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static uint Hash_Len13to24(ref byte s, uint len)
        {
            var a = Fetch32(ref s, (len >> 1) - 4);
            var b = Fetch32(ref s, 4);
            var c = Fetch32(ref s, len - 8);
            var d = Fetch32(ref s, len >> 1);
            var e = Fetch32(ref s, 0);
            var f = Fetch32(ref s, len - 4);

            var h = len;
            h = Murmur3x8632Steps.MixStep(a, h);
            h = Murmur3x8632Steps.MixStep(b, h);
            h = Murmur3x8632Steps.MixStep(c, h);
            h = Murmur3x8632Steps.MixStep(d, h);
            h = Murmur3x8632Steps.MixStep(e, h);
            h = Murmur3x8632Steps.MixStep(f, h);

            return Murmur3x8632Steps.FMix32(h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe uint Hash_Gt24(ref byte s, uint len)
        {
            Initialize_Gt24(ref s, len, out var h, out var g, out var f);

            uint iters = (len - 1) / 20;
            uint offset = 0;
            do
            {
                var a0 = Fetch32(ref s, offset + 0);
                var a1 = Fetch32(ref s, offset + 4);
                var a2 = Fetch32(ref s, offset + 8);
                var a3 = Fetch32(ref s, offset + 12);
                var a4 = Fetch32(ref s, offset + 16);

                Update_Gt24(ref h, ref g, ref f, a0, a1, a2, a3, a4);

                offset += 20;
            } while (--iters != 0);

            return Finish(ref h, ref g, ref f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Initialize_Gt24(ref byte s, uint len, out uint h, out uint g, out uint f)
        {
            h = len;
            g = Murmur3x8632Steps.C1 * len;
            f = g;

            var a0 = Fetch32(ref s, len - 4);
            var a1 = Fetch32(ref s, len - 8);
            var a2 = Fetch32(ref s, len - 16);
            var a3 = Fetch32(ref s, len - 12);
            var a4 = Fetch32(ref s, len - 20);

            // this starts with a franken-murmur-3-32...
            a0 = BitOps.RotateRight(a0 * Murmur3x8632Steps.C1, 17) * Murmur3x8632Steps.C2;
            a1 = BitOps.RotateRight(a1 * Murmur3x8632Steps.C1, 17) * Murmur3x8632Steps.C2;
            a2 = BitOps.RotateRight(a2 * Murmur3x8632Steps.C1, 17) * Murmur3x8632Steps.C2;
            a3 = BitOps.RotateRight(a3 * Murmur3x8632Steps.C1, 17) * Murmur3x8632Steps.C2;
            a4 = BitOps.RotateRight(a4 * Murmur3x8632Steps.C1, 17) * Murmur3x8632Steps.C2;

            h ^= a0;
            h = BitOps.RotateRight(h, 19);
            h = h * 5 + 0xe6546b64;

            h ^= a2;
            h = BitOps.RotateRight(h, 19);
            h = h * 5 + 0xe6546b64;

            g ^= a1;
            g = BitOps.RotateRight(g, 19);
            g = g * 5 + 0xe6546b64;

            g ^= a3;
            g = BitOps.RotateRight(g, 19);
            g = g * 5 + 0xe6546b64;

            f += a4;
            f = BitOps.RotateRight(f, 19);
            f = f * 5 + 0xe6546b64;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Update_Gt24(
            ref uint h, ref uint g, ref uint f, uint a0, uint a1, uint a2, uint a3, uint a4)
        {
            // still franken
            a0 = BitOps.RotateRight(a0 * Murmur3x8632Steps.C1, 17) * Murmur3x8632Steps.C2;
            a2 = BitOps.RotateRight(a2 * Murmur3x8632Steps.C1, 17) * Murmur3x8632Steps.C2;
            a3 = BitOps.RotateRight(a3 * Murmur3x8632Steps.C1, 17) * Murmur3x8632Steps.C2;

            h ^= a0;
            h = BitOps.RotateRight(h, 18);
            h = h * 5 + 0xe6546b64;

            f += a1;
            f = BitOps.RotateRight(f, 19);
            f = f * Murmur3x8632Steps.C1;

            g += a2;
            g = BitOps.RotateRight(g, 18);
            g = g * 5 + 0xe6546b64;

            h ^= a3 + a1;
            h = BitOps.RotateRight(h, 19);
            h = h * 5 + 0xe6546b64;

            g ^= a4;
            g = ByteOps.SwapBytes(g) * 5;

            h += a4 * 5;
            h = ByteOps.SwapBytes(h);

            f += a0;

            Rotate(ref f, ref h, ref g);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Finish(ref uint h, ref uint g, ref uint f)
        {
            g = BitOps.RotateRight(g, 11) * Murmur3x8632Steps.C1;
            g = BitOps.RotateRight(g, 17) * Murmur3x8632Steps.C1;
            f = BitOps.RotateRight(f, 11) * Murmur3x8632Steps.C1;
            f = BitOps.RotateRight(f, 17) * Murmur3x8632Steps.C1;
            h = BitOps.RotateRight(h + g, 19);
            h = h * 5 + 0xe6546b64;
            h = BitOps.RotateRight(h, 17) * Murmur3x8632Steps.C1;
            h = BitOps.RotateRight(h + f, 19);
            h = h * 5 + 0xe6546b64;
            h = BitOps.RotateRight(h, 17) * Murmur3x8632Steps.C1;
            return h;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static uint Fetch32(ref byte s, uint idx) =>
            Unsafe.As<byte, uint>(ref Unsafe.Add<byte>(ref s, (int)idx));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static void Rotate(ref uint a, ref uint b, ref uint c)
        {
            var t = c;
            c = b;
            b = a;
            a = t;
        }
    }
}
