using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Haschisch.Util
{
    internal static class UnsafeByteOps
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static byte ToByte<T>(ref T buffer, int startIndex) where T : struct =>
            Unsafe.Add<byte>(ref Unsafe.As<T, byte>(ref buffer), startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static ulong PartialToUInt64(ref byte data, int byteOffset, uint length)
        {
            if (length > 8) { length = 8; }
            switch (length)
            {
                case 8:
                    return Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, byteOffset));

                case 7:
                    return Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, byteOffset)) |
                        ((ulong)Unsafe.As<byte, ushort>(ref Unsafe.Add(ref data, byteOffset + 4)) << 32) |
                        ((ulong)Unsafe.Add(ref data, byteOffset + 6) << 48);

                case 6:
                    return Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, byteOffset)) |
                        ((ulong)Unsafe.As<byte, ushort>(ref Unsafe.Add(ref data, byteOffset + 4)) << 32);

                case 5:
                    return Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, byteOffset)) |
                        ((ulong)Unsafe.Add(ref data, byteOffset + 4) << 32);

                case 4:
                    return Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, byteOffset));

                case 3:
                    return Unsafe.As<byte, ushort>(ref Unsafe.Add(ref data, byteOffset)) |
                        ((ulong)Unsafe.Add(ref data, byteOffset + 2) << 16);

                case 2:
                    return Unsafe.As<byte, ushort>(ref Unsafe.Add(ref data, byteOffset));

                case 1:
                    return Unsafe.Add(ref data, byteOffset);

                case 0:
                    return 0;

                default:
                    Debug.Fail("Should not get here.");
                    break;
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static uint PartialToUInt32(ref byte data, int byteOffset, uint length)
        {
            if (length > 4) { length = 4; }
            switch (length)
            {
                case 4:
                    return Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, byteOffset));

                case 3:
                    return Unsafe.As<byte, ushort>(ref Unsafe.Add(ref data, byteOffset)) |
                        ((uint)Unsafe.Add(ref data, byteOffset + 2) << 16);

                case 2:
                    return Unsafe.As<byte, ushort>(ref Unsafe.Add(ref data, byteOffset));

                case 1:
                    return Unsafe.Add(ref data, byteOffset);

                case 0:
                    return 0;

                default:
                    Debug.Fail("Should not get here.");
                    break;
            }

            return 0;
        }
    }
}
