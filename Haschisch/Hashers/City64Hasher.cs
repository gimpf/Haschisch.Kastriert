using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class City64Hasher
    {
        public struct Block :
            IBlockHasher<ulong>,
            IBlockHasher<int>,
            IUnsafeBlockHasher<ulong>,
            IUnsafeBlockHasher<int>
        {
            public ulong Hash(byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                return length == 0 ?
                    City64Steps.Hash(ref empty, 0) :
                    City64Steps.Hash(ref data[offset], (uint)length);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IBlockHasher<int>.Hash(byte[] data, int offset, int length) =>
                (int)this.Hash(data, offset, length);

            int IUnsafeBlockHasher<int>.Hash(ref byte data, int length) => (int)City64Steps.Hash(ref data, (uint)length);

            public ulong Hash(ref byte data, int length) => City64Steps.Hash(ref data, (uint)length);
        }

        public struct Combiner : IHashCodeCombiner
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1>(T1 value1) =>
                (int)CombineRaw(value1);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2>(T1 value1, T2 value2) =>
                (int)CombineRaw(value1, value2);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3) =>
                (int)CombineRaw(value1, value2, value3);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4) =>
                (int)CombineRaw(value1, value2, value3, value4);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) =>
                (int)CombineRaw(value1, value2, value3, value4, value5);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) =>
                (int)CombineRaw(value1, value2, value3, value4, value5, value6);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6, T7>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) =>
                (int)CombineRaw(value1, value2, value3, value4, value5, value6, value7);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) =>
                (int)CombineRaw(value1, value2, value3, value4, value5, value6, value7, value8);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ulong CombineRaw<T1>(T1 value1)
            {
                var x1 = (uint)(value1?.GetHashCode() ?? 0);

                return City64Steps.Hash_Len4to7(x1, x1, sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ulong CombineRaw<T1, T2>(T1 value1, T2 value2)
            {
                var x1 = (uint)(value1?.GetHashCode() ?? 0);
                var x2 = (uint)(value2?.GetHashCode() ?? 0);
                var a = (ulong)x2 << 32 | x1;

                return City64Steps.Hash_Len8to16(a, a, 2 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ulong CombineRaw<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
            {
                var x1 = (uint)(value1?.GetHashCode() ?? 0);
                var x2 = (uint)(value2?.GetHashCode() ?? 0);
                var x3 = (uint)(value3?.GetHashCode() ?? 0);

                var a = (ulong)x2 << 32 | x1;
                var b = (ulong)x3 << 32 | x2;

                return City64Steps.Hash_Len8to16(a, b, 3 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ulong CombineRaw<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var x1 = (uint)(value1?.GetHashCode() ?? 0);
                var x2 = (uint)(value2?.GetHashCode() ?? 0);
                var x3 = (uint)(value3?.GetHashCode() ?? 0);
                var x4 = (uint)(value4?.GetHashCode() ?? 0);
                var a = (ulong)x2 << 32 | x1;
                var b = (ulong)x4 << 32 | x3;

                return City64Steps.Hash_Len8to16(a, b, 4 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ulong CombineRaw<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var x1 = (uint)(value1?.GetHashCode() ?? 0);
                var x2 = (uint)(value2?.GetHashCode() ?? 0);
                var x3 = (uint)(value3?.GetHashCode() ?? 0);
                var x4 = (uint)(value4?.GetHashCode() ?? 0);
                var x5 = (uint)(value5?.GetHashCode() ?? 0);
                var a = (ulong)x2 << 32 | x1;
                var b = (ulong)x4 << 32 | x3;
                var c = (ulong)x5 << 32 | x4;
                var d = (ulong)x3 << 32 | x2;

                return City64Steps.Hash_Len17to32(a, b, c, d, 5 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ulong CombineRaw<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
            {
                var x1 = (uint)(value1?.GetHashCode() ?? 0);
                var x2 = (uint)(value2?.GetHashCode() ?? 0);
                var x3 = (uint)(value3?.GetHashCode() ?? 0);
                var x4 = (uint)(value4?.GetHashCode() ?? 0);
                var x5 = (uint)(value5?.GetHashCode() ?? 0);
                var x6 = (uint)(value6?.GetHashCode() ?? 0);
                var a = (ulong)x2 << 32 | x1;
                var b = (ulong)x4 << 32 | x3;
                var c = (ulong)x6 << 32 | x5;
                var d = (ulong)x4 << 32 | x3;

                return City64Steps.Hash_Len17to32(a, b, c, d, 6 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ulong CombineRaw<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
            {
                var x1 = (uint)(value1?.GetHashCode() ?? 0);
                var x2 = (uint)(value2?.GetHashCode() ?? 0);
                var x3 = (uint)(value3?.GetHashCode() ?? 0);
                var x4 = (uint)(value4?.GetHashCode() ?? 0);
                var x5 = (uint)(value5?.GetHashCode() ?? 0);
                var x6 = (uint)(value6?.GetHashCode() ?? 0);
                var x7 = (uint)(value7?.GetHashCode() ?? 0);
                var a = (ulong)x2 << 32 | x1;
                var b = (ulong)x4 << 32 | x3;
                var c = (ulong)x7 << 32 | x6;
                var d = (ulong)x5 << 32 | x4;

                return City64Steps.Hash_Len17to32(a, b, c, d, 7 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ulong CombineRaw<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
            {
                var x1 = (uint)(value1?.GetHashCode() ?? 0);
                var x2 = (uint)(value2?.GetHashCode() ?? 0);
                var x3 = (uint)(value3?.GetHashCode() ?? 0);
                var x4 = (uint)(value4?.GetHashCode() ?? 0);
                var x5 = (uint)(value5?.GetHashCode() ?? 0);
                var x6 = (uint)(value6?.GetHashCode() ?? 0);
                var x7 = (uint)(value7?.GetHashCode() ?? 0);
                var x8 = (uint)(value8?.GetHashCode() ?? 0);
                var a = (ulong)x2 << 32 | x1;
                var b = (ulong)x4 << 32 | x3;
                var c = (ulong)x8 << 32 | x7;
                var d = (ulong)x6 << 32 | x5;

                return City64Steps.Hash_Len17to32(a, b, c, d, 8 * sizeof(int));
            }
        }
    }
}
