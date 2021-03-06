﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Haschisch.Tests;
using Haschisch.Util;

namespace Haschisch.Benchmarks
{
    public sealed class Mumur3A_TG_EqualityComparer : IEqualityComparer<Large>
    {
        public static readonly Mumur3A_TG_EqualityComparer Default = new Mumur3A_TG_EqualityComparer();

        public bool Equals(Large x, Large y) => x.Equals(y);

        public int GetHashCode(Large obj) =>
            Murmur3A_TG_Combiner.Combine(
                obj.M.S.Arg1,
                obj.M.S.Arg2,
                obj.M.S.Arg3,
                obj.M.Text,
                obj.List?.LongLength);
    }

    public sealed class ByCombinerEqualityComparer<T> : IEqualityComparer<Large>
        where T : IHashCodeCombiner
    {
        public static readonly ByCombinerEqualityComparer<T> Default = new ByCombinerEqualityComparer<T>();

        public bool Equals(Large x, Large y) => x.Equals(y);

        public int GetHashCode(Large obj) =>
            default(T).Combine(
                obj.M.S.Arg1,
                obj.M.S.Arg2,
                obj.M.S.Arg3,
                obj.M.Text,
                obj.List?.LongLength);
    }

    public sealed class ByBlockEqualityComparer<T> : IEqualityComparer<Large>
        where T : IUnsafeBlockHasher<int>
    {
        public static readonly ByBlockEqualityComparer<T> Default = new ByBlockEqualityComparer<T>();

        public bool Equals(Large x, Large y) => x.Equals(y);

        public int GetHashCode(Large obj)
        {
            var vec = new PackedList<int, int, int, int, int>(
                obj.M.S.Arg1,
                obj.M.S.Arg2,
                obj.M.S.Arg3,
                obj.M.Text?.GetHashCode() ?? 0,
                obj.List?.LongLength.GetHashCode() ?? 0);
            return default(T).Hash(
                ref Unsafe.As<PackedList<int, int, int, int, int>, byte>(ref vec),
                (int)BufferUtil.BufferSize(ref vec));
        }
    }

    public sealed class ByStreamEqualityComparer<T> : IEqualityComparer<Large>
        where T : IStreamingHasher<int>
    {
        public static readonly ByStreamEqualityComparer<T> Default = new ByStreamEqualityComparer<T>();

        public bool Equals(Large x, Large y) => x.Equals(y);

        public int GetHashCode(Large obj)
        {
            var hasher = default(T);
            hasher.Initialize();
            hasher.Write32(obj.M.S.Arg1);
            hasher.Write32(obj.M.S.Arg2);
            hasher.Write32(obj.M.S.Arg3);
            hasher.Write32(obj.M.Text?.GetHashCode() ?? 0);
            hasher.Write32(obj.List?.LongLength.GetHashCode() ?? 0);
            return hasher.Finish();
        }
    }
}
