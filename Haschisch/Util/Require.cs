using System;
using System.Runtime.CompilerServices;

namespace Haschisch.Util
{
    internal static class Require
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidRange(byte[] data, int offset, int length)
        {
            checked
            {
                if (data == null) { throw new ArgumentNullException(nameof(data)); }
                if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
                if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }
                if ((long)offset + (long)length > data.LongLength) { throw new ArgumentException("array is smaller than specified data-range"); }
            }
        }
    }
}
