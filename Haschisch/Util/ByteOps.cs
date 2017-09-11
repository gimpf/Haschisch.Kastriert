using System;
using System.Runtime.CompilerServices;

namespace Haschisch.Util
{
    internal static class ByteOps
    {
        public static byte[] GetBytes((ulong, ulong) value)
        {
            var result = new byte[2 * sizeof(ulong)];
            Array.Copy(BitConverter.GetBytes(value.Item1), 0, result, 0, sizeof(ulong));
            Array.Copy(BitConverter.GetBytes(value.Item2), 0, result, sizeof(ulong), sizeof(ulong));
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SwapBytes(uint x)
        {
            // nice trick from https://stackoverflow.com/a/19560621/15529
            // swap adjacent 16-bit blocks
            x = (x >> 16) | (x << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SwapBytes(ulong x)
        {
            // nice trick from https://stackoverflow.com/a/19560621/15529
            // swap adjacent 32-bit blocks
            x = (x >> 32) | (x << 32);
            // swap adjacent 16-bit blocks
            x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8);
        }
    }
}
