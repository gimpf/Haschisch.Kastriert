using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Haschisch.Benchmarks
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PackedVec<T1, T2>
    {
        public T1 V1;
        public T2 V2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackedVec(T1 v1, T2 v2)
        {
            this.V1 = v1;
            this.V2 = v2;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PackedVec<T1, T2, T3, T4, T5>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;
        public T5 V5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackedVec(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
            this.V4 = v4;
            this.V5 = v5;
        }
    }
}
