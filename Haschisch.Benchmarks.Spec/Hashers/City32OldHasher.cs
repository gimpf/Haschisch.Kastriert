using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class City32OldHasher
    {
        public struct CombinerNonUnrolled : IHashCodeCombiner
        {
            public int Combine<T1>(T1 value1)
            {
                var x1 = value1?.GetHashCode() ?? 0;

                return (int)City32Steps.Hash_Len0to4(ref Unsafe.As<int, byte>(ref x1), sizeof(int));
            }

            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var x = new PackedList<int, int>(
                    value1?.GetHashCode() ?? 0,
                    value2?.GetHashCode() ?? 0);

                return (int)City32Steps.Hash_Len5to12(ref Unsafe.As<PackedList<int, int>, byte>(ref x), 2 * sizeof(int));
            }

            public int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
            {
                var x = new PackedList<int, int, int>(
                    value1?.GetHashCode() ?? 0,
                    value2?.GetHashCode() ?? 0,
                    value3?.GetHashCode() ?? 0);

                return (int)City32Steps.Hash_Len5to12(ref Unsafe.As<PackedList<int, int, int>, byte>(ref x), 3 * sizeof(int));
            }

            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var x = new PackedList<int, int, int, int>(
                    value1?.GetHashCode() ?? 0,
                    value2?.GetHashCode() ?? 0,
                    value3?.GetHashCode() ?? 0,
                    value4?.GetHashCode() ?? 0);

                return (int)City32Steps.Hash_Len13to24(
                    ref Unsafe.As<PackedList<int, int, int, int>, byte>(ref x), 4 * sizeof(int));
            }

            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var x = new PackedList<int, int, int, int, int>(
                    value1?.GetHashCode() ?? 0,
                    value2?.GetHashCode() ?? 0,
                    value3?.GetHashCode() ?? 0,
                    value4?.GetHashCode() ?? 0,
                    value5?.GetHashCode() ?? 0);

                return (int)City32Steps.Hash_Len13to24(
                    ref Unsafe.As<PackedList<int, int, int, int, int>, byte>(ref x), 5 * sizeof(int));
            }

            public int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
            {
                var x = new PackedList<int, int, int, int, int, int, int, int>(
                    value1?.GetHashCode() ?? 0,
                    value2?.GetHashCode() ?? 0,
                    value3?.GetHashCode() ?? 0,
                    value4?.GetHashCode() ?? 0,
                    value5?.GetHashCode() ?? 0,
                    value6?.GetHashCode() ?? 0,
                    value7?.GetHashCode() ?? 0,
                    value8?.GetHashCode() ?? 0);

                return (int)City32Steps.Hash_Gt24(
                    ref Unsafe.As<PackedList<int, int, int, int, int, int, int, int>, byte>(ref x), 8 * sizeof(int));
            }
        }

        public struct CombinerUnseeded : IHashCodeCombiner
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1>(T1 value1)
            {
                var x1 = (uint)(value1?.GetHashCode() ?? 0);

                return (int)City32Steps.Hash_Len4(x1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);

                return (int)City32Steps.Hash_Len5to12(v1, v2, v2, 2 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);

                return (int)City32Steps.Hash_Len5to12(v1, v3, v2, 3 * sizeof(int));
            }

            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);

                var a = v2;
                var b = v2;
                var c = v3;
                var d = v3;
                var e = v1;
                var f = v4;

                return (int)City32Steps.Hash_Len13to24(a, b, c, d, e, f, 4 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);
                var v5 = (uint)(value5?.GetHashCode() ?? 0);

                var a = (uint)((ushort)(v2 >> 16) | (ushort)v3);
                var b = v2;
                var c = v4;
                var d = (uint)((ushort)(v3 >> 16) | (ushort)v4);
                var e = v1;
                var f = v5;

                return (int)City32Steps.Hash_Len13to24(a, b, c, d, e, f, 5 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);
                var v5 = (uint)(value5?.GetHashCode() ?? 0);
                var v6 = (uint)(value6?.GetHashCode() ?? 0);
                var v7 = (uint)(value7?.GetHashCode() ?? 0);
                var v8 = (uint)(value8?.GetHashCode() ?? 0);

                City32Steps.Initialize_Gt24(
                    v8, v7, v5, v6, v4,
                    8 * sizeof(int),
                    out var h, out var g, out var f);
                City32Steps.Update_Gt24(
                    ref h, ref g, ref f,
                    v1, v2, v3, v4, v5);
                return (int)City32Steps.Finish(ref h, ref g, ref f);
            }
        }
    }
}
