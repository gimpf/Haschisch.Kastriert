using System.Runtime.CompilerServices;

namespace Haschisch.Hashers
{
    internal static class CitySteps
    {
        public const ulong K0 = 0xc3a5c85c97cb3127UL;
        public const ulong K1 = 0xb492b66fbe98f273UL;
        public const ulong K2 = 0x9ae16a3b2f90404fUL;
        public const ulong KMul = 0x9ddfea08eb382d69UL;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void Permute3(ref uint a, ref uint b, ref uint c)
        {
            var t = c;
            c = b;
            b = a;
            a = t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref ulong left, ref ulong right)
        {
            ulong tmp = left;
            left = right;
            right = tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ShiftMix(ulong val) => val ^ (val >> 47);
    }
}
