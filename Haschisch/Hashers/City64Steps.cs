using System.Runtime.CompilerServices;
using Haschisch.Util;

using UBO = Haschisch.Util.UnsafeByteOps;
using CS = Haschisch.Hashers.CitySteps;

namespace Haschisch.Hashers
{
    public static class City64Steps
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static ulong HashWithSeed(ref byte s, uint len, ulong seed) =>
            HashWithSeeds(ref s, len, CS.K2, seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static ulong HashWithSeeds(ref byte s, uint len, ulong seed0, ulong seed1) =>
            Hash_Len16(Hash(ref s, len) - seed0, seed1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static ulong MixSeed(ulong hash, ulong seed) =>
            Hash_Len16(hash - CS.K2, seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static ulong MixSeeds(ulong hash, ulong seed0, ulong seed1) =>
            Hash_Len16(hash - seed0, seed1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static ulong Hash(ref byte s, uint len)
        {
            if (len <= 32)
            {
                if (len <= 16)
                {
                    return Hash_Len0to16(ref s, len);
                }
                else
                {
                    return Hash_Len17to32(ref s, len);
                }
            }
            else if (len <= 64)
            {
                return Hash_Len33to64(ref s, len);
            }

            return Hash_Gt64(ref s, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static ulong Hash_Len0to16(ref byte s, uint len)
        {
            if (len >= 8)
            {
                ulong a = UBO.ToUInt64(ref s, 0);
                ulong b = UBO.ToUInt64(ref s, len - 8);
                return Hash_Len8to16(a, b, len);
            }

            if (len >= 4)
            {
                uint a = UBO.ToUInt32(ref s, 0);
                uint b = UBO.ToUInt32(ref s, len - 4);
                return Hash_Len4to7(a, b, len);
            }

            if (len > 0)
            {
                return Hash_Len1to3(ref s, len);
            }

            return CS.K2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static ulong Hash_Len1to3(ref byte s, uint len)
        {
            byte a = Unsafe.Add<byte>(ref s, 0);
            byte b = Unsafe.Add<byte>(ref s, (int)(len >> 1));
            byte c = Unsafe.Add<byte>(ref s, (int)(len - 1));
            uint y = (uint)(a) + ((uint)(b) << 8);
            uint z = len + ((uint)(c) << 2);
            return CS.ShiftMix(y * CS.K2 ^ z * CS.K0) * CS.K2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static ulong Hash_Len4to7(uint a, uint b, uint len)
        {
            ulong mul = CS.K2 + len * 2;
            return Hash_Len16(len + ((ulong)a << 3), b, mul);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static ulong Hash_Len8to16(ulong a, ulong b, uint len)
        {
            a += CS.K2;
            ulong mul = CS.K2 + len * 2;
            ulong c = BitOps.RotateRight(b, 37) * mul + a;
            ulong d = (BitOps.RotateRight(a, 25) + b) * mul;
            return Hash_Len16(c, d, mul);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong Hash_Len16(ulong u, ulong v, ulong mul)
        {
            ulong a = (u ^ v) * mul;
            a ^= (a >> 47);
            ulong b = (v ^ a) * mul;
            b ^= (b >> 47);
            b *= mul;
            return b;
        }

        // same as above, but with constant multiplier
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong Hash_Len16(ulong low, ulong high)
        {
            ulong a = (low ^ high) * CS.KMul;
            a ^= (a >> 47);
            ulong b = (high ^ a) * CS.KMul;
            b ^= (b >> 47);
            b *= CS.KMul;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static ulong Hash_Len17to32(ref byte s, uint len)
        {
            ulong a = UBO.ToUInt64(ref s, 0);
            ulong b = UBO.ToUInt64(ref s, 8);
            ulong c = UBO.ToUInt64(ref s, len - 8);
            ulong d = UBO.ToUInt64(ref s, len - 16);

            return Hash_Len17to32(a, b, c, d, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static ulong Hash_Len17to32(ulong a, ulong b, ulong c, ulong d, uint len)
        {
            ulong mul = CS.K2 + len * 2;
            a = a * CS.K1;
            c = c * mul;
            d = d * CS.K2;
            return Hash_Len16(
                BitOps.RotateRight(a + b, 43) + BitOps.RotateRight(c, 30) + d,
                a + BitOps.RotateRight(b + CS.K2, 18) + c,
                mul);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WeakHashLen32WithSeeds(
            ulong w, ulong x, ulong y, ulong z, ulong a, ulong b, out ulong low, out ulong high)
        {
            a += w;
            b = BitOps.RotateRight(b + a + z, 21);
            var c = a;
            a += x;
            a += y;
            b += BitOps.RotateRight(a, 44);

            low = a + z;
            high = b + c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WeakHashLen32WithSeeds(ref byte s, ulong a, ulong b, out ulong low, out ulong high) =>
            WeakHashLen32WithSeeds(
                UBO.ToUInt64(ref s, 0),
                UBO.ToUInt64(ref s, 8),
                UBO.ToUInt64(ref s, 16),
                UBO.ToUInt64(ref s, 24),
                a,
                b,
                out low,
                out high);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static ulong Hash_Len33to64(ref byte s, uint len)
        {
            ulong mul = CS.K2 + len * 2;
            ulong a = UBO.ToUInt64(ref s, 0) * CS.K2;
            ulong b = UBO.ToUInt64(ref s, 8);
            ulong c = UBO.ToUInt64(ref s, len - 24);
            ulong d = UBO.ToUInt64(ref s, len - 32);
            ulong e = UBO.ToUInt64(ref s, 16) * CS.K2;
            ulong f = UBO.ToUInt64(ref s, 24) * 9;
            ulong g = UBO.ToUInt64(ref s, len - 8);
            ulong h = UBO.ToUInt64(ref s, len - 16) * mul;
            ulong u = BitOps.RotateRight(a + g, 43) + (BitOps.RotateRight(b, 30) + c) * 9;
            ulong v = ((a + g) ^ d) + f + 1;
            ulong w = ByteOps.SwapBytes((u + v) * mul) + h;
            ulong x = BitOps.RotateRight(e + f, 42) + c;
            ulong y = (ByteOps.SwapBytes((v + w) * mul) + g) * mul;
            ulong z = e + f + c;
            a = ByteOps.SwapBytes((x + z) * mul + y) + b;
            b = CS.ShiftMix((z + a) * mul + d + h) * mul;
            return b + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static ulong Hash_Gt64(ref byte s, uint len)
        {
            ulong x = UBO.ToUInt64(ref s, len - 40);
            ulong y = UBO.ToUInt64(ref s, len - 16) + UBO.ToUInt64(ref s, len - 56);
            ulong z = Hash_Len16(
                UBO.ToUInt64(ref s, len - 48) + len,
                UBO.ToUInt64(ref s, len - 24));
            WeakHashLen32WithSeeds(ref Unsafe.Add<byte>(ref s, (int)(len - 64)), len, z, out var vLow, out var vHigh);
            WeakHashLen32WithSeeds(ref Unsafe.Add<byte>(ref s, (int)(len - 32)), y + CS.K1, x, out var wLow, out var wHigh);
            x = x * CS.K1 + UBO.ToUInt64(ref s, 0);

            len = (uint)((len - 1) & ~63);
            var offset = 0;
            do
            {
                ref byte start = ref Unsafe.Add<byte>(ref s, offset);
                x = BitOps.RotateRight(x + y + vLow + UBO.ToUInt64(ref start, 8), 37) * CS.K1;
                y = BitOps.RotateRight(y + vHigh + UBO.ToUInt64(ref start, 48), 42) * CS.K1;
                x ^= wHigh;
                y += vLow + UBO.ToUInt64(ref start, 40);
                z = BitOps.RotateRight(z + wLow, 33) * CS.K1;
                WeakHashLen32WithSeeds(ref start, vHigh * CS.K1, x + wLow, out vLow, out vHigh);
                WeakHashLen32WithSeeds(ref Unsafe.Add<byte>(ref start, 32), z + wHigh, y + UBO.ToUInt64(ref start, 16), out wLow, out wHigh);
                CS.Swap(ref z, ref x);
                offset += 64;
                len -= 64;
            } while (len != 0);

            return Hash_Len16(
                Hash_Len16(vLow, wLow) + CS.ShiftMix(y) * CS.K1 + z,
                Hash_Len16(vHigh, wHigh) + x);
        }
    }
}
