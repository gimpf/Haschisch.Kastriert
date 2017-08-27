using System.Collections.Generic;

namespace Haschisch
{
    public sealed class HashableEqualityComparer<TValue, THasher> : IEqualityComparer<TValue>
        where TValue : IHashable
        where THasher : struct, IStreamingHasher<int>
    {
        public static readonly HashableEqualityComparer<TValue, THasher> Default = new HashableEqualityComparer<TValue, THasher>();

        public bool Equals(TValue x, TValue y) => EqualityComparer<TValue>.Default.Equals(x, y);

        public int GetHashCode(TValue obj)
        {
            var hasher = default(THasher);
            hasher.Initialize();
            obj.Hash(ref hasher);
            return hasher.Finish();
        }

        public override string ToString() =>
            $"HashableEqualityComparer-{default(THasher)}";
    }
}
