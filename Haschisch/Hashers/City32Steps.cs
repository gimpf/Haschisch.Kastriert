using System.Runtime.CompilerServices;
using Haschisch.Util;

using UBO = Haschisch.Util.UnsafeByteOps;
using CS = Haschisch.Hashers.CitySteps;

namespace Haschisch.Hashers
{
    public static class City32Steps
    {
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
        internal unsafe static uint Hash_Len4(uint s)
        {
            var b = 0u;
            var c = 9u;

            var v = (sbyte)s;
            b = (uint)(b * Murmur3x8632Steps.C1 + v);
            c ^= b;

            v = (sbyte)(s >> 8);
            b = (uint)(b * Murmur3x8632Steps.C1 + v);
            c ^= b;

            v = (sbyte)(s >> 16);
            b = (uint)(b * Murmur3x8632Steps.C1 + v);
            c ^= b;

            v = (sbyte)(s >> 24);
            b = (uint)(b * Murmur3x8632Steps.C1 + v);
            c ^= b;

            var h = Murmur3x8632Steps.MixStep(4, c);
            h = Murmur3x8632Steps.MixStep(b, h);
            return Murmur3x8632Steps.FMix32(h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static uint Hash_Len5to12(ref byte s, uint len)
        {
            var a0 = UBO.ToUInt32(ref s, 0);
            var a1 = UBO.ToUInt32(ref s, len - 4);
            var a2 = UBO.ToUInt32(ref s, (len >> 1) & 4);

            return Hash_Len5to12(a0, a1, a2, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static uint Hash_Len5to12(uint a0, uint a1, uint a2, uint len)
        {
            var a = len;
            var b = len * 5;
            var c = 9u;
            var d = b;

            a += a0;
            b += a1;
            c += a2;

            var h = Murmur3x8632Steps.MixStep(a, d);
            h = Murmur3x8632Steps.MixStep(b, h);
            h = Murmur3x8632Steps.MixStep(c, h);
            return Murmur3x8632Steps.FMix32(h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static uint Hash_Len13to24(ref byte s, uint len)
        {
            var a = UBO.ToUInt32(ref s, (len >> 1) - 4);
            var b = UBO.ToUInt32(ref s, 4);
            var c = UBO.ToUInt32(ref s, len - 8);
            var d = UBO.ToUInt32(ref s, len >> 1);
            var e = UBO.ToUInt32(ref s, 0);
            var f = UBO.ToUInt32(ref s, len - 4);

            return Hash_Len13to24(a, b, c, d, e, f, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static uint Hash_Len13to24(uint a, uint b, uint c, uint d, uint e, uint f, uint len)
        {
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
                var a0 = UBO.ToUInt32(ref s, offset + 0);
                var a1 = UBO.ToUInt32(ref s, offset + 4);
                var a2 = UBO.ToUInt32(ref s, offset + 8);
                var a3 = UBO.ToUInt32(ref s, offset + 12);
                var a4 = UBO.ToUInt32(ref s, offset + 16);

                Update_Gt24(ref h, ref g, ref f, a0, a1, a2, a3, a4);

                offset += 20;
            } while (--iters != 0);

            return Finish(ref h, ref g, ref f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Initialize_Gt24(ref byte s, uint len, out uint h, out uint g, out uint f)
        {
            var a0 = UBO.ToUInt32(ref s, len - 4);
            var a1 = UBO.ToUInt32(ref s, len - 8);
            var a2 = UBO.ToUInt32(ref s, len - 16); // was this a type in the reference implementation, or
            var a3 = UBO.ToUInt32(ref s, len - 12); // is this is super-special-hero-hack actually improving things?
            var a4 = UBO.ToUInt32(ref s, len - 20);
            Initialize_Gt24(a0, a1, a2, a3, a4, len, out h, out g, out f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Initialize_Gt24(uint last4, uint last8, uint last16, uint last12, uint last20, uint len, out uint h, out uint g, out uint f)
        {
            h = len;
            g = Murmur3x8632Steps.C1 * len;
            f = g;

            var a0 = last4;
            var a1 = last8;
            var a2 = last16;
            var a3 = last12;
            var a4 = last20;

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
        public static unsafe void Update_Gt24(
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

            CS.Permute3(ref f, ref h, ref g);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Finish(ref uint h, ref uint g, ref uint f)
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
    }
}
