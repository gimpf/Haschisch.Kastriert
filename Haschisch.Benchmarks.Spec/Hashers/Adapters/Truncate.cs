using System.Runtime.CompilerServices;

namespace Haschisch.Hashers.Adapters
{
    public struct TruncateBlock<THasher> : IBlockHasher<int>
        where THasher : struct, IBlockHasher<long>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Hash(byte[] data, int offset, int length) { unchecked { return (int)default(THasher).Hash(data, offset, length); } }

        public override string ToString() => $"{default(THasher)}-trunc32b";
    }
}
