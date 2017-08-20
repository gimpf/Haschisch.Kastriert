using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Haschisch.Util
{
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
}
