using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Haschisch.Util
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PackedList<T1, T2>
    {
        public T1 V1;
        public T2 V2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackedList(T1 v1, T2 v2)
        {
            this.V1 = v1;
            this.V2 = v2;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PackedList<T1, T2, T3>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackedList(T1 v1, T2 v2, T3 v3)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PackedList<T1, T2, T3, T4>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackedList(T1 v1, T2 v2, T3 v3, T4 v4)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
            this.V4 = v4;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PackedList<T1, T2, T3, T4, T5>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;
        public T5 V5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackedList(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
            this.V4 = v4;
            this.V5 = v5;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PackedList<T1, T2, T3, T4, T5, T6>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;
        public T5 V5;
        public T6 V6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackedList(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
            this.V4 = v4;
            this.V5 = v5;
            this.V6 = v6;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PackedList<T1, T2, T3, T4, T5, T6, T7>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;
        public T5 V5;
        public T6 V6;
        public T7 V7;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackedList(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
            this.V4 = v4;
            this.V5 = v5;
            this.V6 = v6;
            this.V7 = v7;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PackedList<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;
        public T5 V5;
        public T6 V6;
        public T7 V7;
        public T8 V8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackedList(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
            this.V4 = v4;
            this.V5 = v5;
            this.V6 = v6;
            this.V7 = v7;
            this.V8 = v8;
        }
    }
}
