using System;
using System.Security.Cryptography;

namespace Haschisch.Util
{
    internal static class Seeder
    {
        public static void GetNewSeed(out (ulong, ulong) result)
        {
            var key = GetBytes(sizeof(ulong) * 2);
            result = (BitConverter.ToUInt64(key, 0), BitConverter.ToUInt64(key, sizeof(ulong)));
        }

        public static void GetNewSeed(out (uint, uint) result)
        {
            var key = GetBytes(sizeof(uint) * 2);
            result = (BitConverter.ToUInt32(key, 0), BitConverter.ToUInt32(key, sizeof(uint)));
        }

        public static void GetNewSeed(out ulong result)
        {
            var key = GetBytes(sizeof(ulong));
            result = BitConverter.ToUInt64(key, 0);
        }

        public static void GetNewSeed(out uint result)
        {
            var key = GetBytes(sizeof(ulong));
            result = BitConverter.ToUInt32(key, 0);
        }

        private static byte[] GetBytes(int bytes)
        {
            var key = new byte[bytes];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            return key;
        }
    }
}
