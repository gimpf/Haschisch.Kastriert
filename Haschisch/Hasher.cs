using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Haschisch
{
    public static class Hasher
    {
        private const int NullHashValue = int.MaxValue;

        public static Hashers.Murmur3x8632Hasher.Stream Default
        {
            get
            {
                var h = default(Hashers.Murmur3x8632Hasher.Stream);
                h.Initialize();
                return h;
            }
        }

        public static int GetHaschisch<THashable>(this THashable self)
            where THashable : IHashable
        {
            var h = Default;
            self.Hash(ref h);
            return h.Finish();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<THash>(ref THash hasher, byte value)
            where THash : struct, IStreamingHasherSink
        {
            hasher.Write8(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<THash>(ref THash hasher, sbyte value)
            where THash : struct, IStreamingHasherSink
        {
            unchecked { hasher.Write8((byte)value); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<THash>(ref THash hasher, short value)
            where THash : struct, IStreamingHasherSink
        {
            hasher.Write16(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<THash>(ref THash hasher, ushort value)
            where THash : struct, IStreamingHasherSink
        {
            unchecked { hasher.Write16((short)value); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<THash>(ref THash hasher, int value)
            where THash : struct, IStreamingHasherSink
        {
            hasher.Write32(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<THash>(ref THash hasher, uint value)
            where THash : struct, IStreamingHasherSink
        {
            unchecked { hasher.Write32((int)value); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<THash>(ref THash hasher, long value)
            where THash : struct, IStreamingHasherSink
        {
            hasher.Write64(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<THash>(ref THash hasher, ulong value)
            where THash : struct, IStreamingHasherSink
        {
            unchecked { hasher.Write64((long)value); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<THash, TValue>(ref THash hasher, TValue value)
            where THash : struct, IStreamingHasherSink
            where TValue : IHashable
        {
            if (value != null)
            {
                value.Hash(ref hasher);
            }
            else
            {
                hasher.Write32(NullHashValue);
            }
        }

        // 1st, always write the length of sequences of values, and
        // 2nd, at the end.
        // No. 1 ensures that for types with two arrays, the hash values of instances [1 1 1] and [1 1]
        //       are different from [1 1] and [1 1 1] (otherwise only five consecutive 1s would be hashed).
        //       This is obviously more important for small keys than for large and complex data.
        // No. 2 helps to keep the hash values of five items the same, whether they are listed via an array
        //       or any other collection.  This keeps hash-values the same when calculating the via a
        //       read-only wrapper.  If the type-information is relevant, mix the type-information explicitely.
        public static void Write<THash, TItem>(ref THash hasher, TItem[] items)
            where THash : struct, IStreamingHasherSink
            where TItem : IHashable
        {
            for (var i = 0L; i < items.LongLength; i++)
            {
                Write(ref hasher, items[i]);
            }

            hasher.Write64(items.LongLength);
        }

        public static void Write<THash, TItem>(ref THash hasher, IReadOnlyList<TItem> items)
            where THash : struct, IStreamingHasherSink
            where TItem : IHashable
        {
            for (var i = 0; i < items.Count; i++)
            {
                Write(ref hasher, items[i]);
            }

            hasher.Write64(items.Count);
        }

        public static void Write<THash, TItem>(ref THash hasher, IEnumerable<TItem> items)
            where THash : struct, IStreamingHasherSink
            where TItem : IHashable
        {
            var count = 0L;
            foreach (var item in items)
            {
                Write(ref hasher, item);
                count++;
            }

            hasher.Write64(count);
        }

        // TODO overloads for other array types, potentially improving performance
        public static void Write<THash>(ref THash hasher, byte[] data)
            where THash : struct, IStreamingHasherSink
        {
            var bytewiseStart = 0;
            if (data?.Length > 0)
            {
                if (hasher.AllowUnsafeWrite)
                {
                    bytewiseStart = hasher.UnsafeWrite(ref data[0], data.Length);
                }
                else
                {
                    bytewiseStart = data.Length - (data.Length % 8);
                    WriteBlocks(ref hasher, ref data[0], bytewiseStart);
                }
            }

            for (int i = bytewiseStart, len = data.Length; i < len; i++)
            {
                hasher.Write8(data[i]);
            }

            hasher.Write32(data?.Length ?? NullHashValue);
        }

        // TODO stringcomparison options etc.
        public static void Write<THash>(ref THash hasher, string value)
            where THash : struct, IStreamingHasherSink
        {
            if (value != null)
            {
                unsafe
                {
                    fixed (char* ptr = value)
                    {
                        short* ptrS = (short*)ptr; // short and char are both 16bit width
                        for (var i = 0; i < value.Length; i++)
                        {
                            Write(ref hasher, *(ptrS + i));
                        }
                    }

                    Write(ref hasher, value.Length);
                }
            }
            else
            {
                hasher.Write32(NullHashValue);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteBlocks<THash>(ref THash hasher, ref byte dataRaw, int fullBlockEnd)
            where THash : struct, IStreamingHasherSink
        {
            for (var i = 0; i < fullBlockEnd; i += sizeof(ulong))
            {
                hasher.Write64(Unsafe.As<byte, long>(ref Unsafe.Add(ref dataRaw, i)));
            }
        }
    }
}
