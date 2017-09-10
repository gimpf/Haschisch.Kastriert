using System;

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
    }
}
