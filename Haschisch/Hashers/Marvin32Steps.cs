// based upon:
//    https://github.com/dotnet/corert/blob/87e58839d6629b5f90777f886a2f52d7a99c076f/src/System.Private.CoreLib/src/System/Marvin.cs
//    Licensed to the .NET Foundation under one or more agreements.
//    The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    internal static class Marvin32Steps
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize(ulong seed, out uint p0, out uint p1)
        {
            p0 = (uint)seed;
            p1 = (uint)(seed >> 32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(ref uint rp0, ref uint rp1, ulong value)
        {
            var p0 = rp0;
            var p1 = rp1;

            p0 += (uint)value;
            MixBlock(ref p0, ref p1);

            p0 += (uint)(value >> 32);
            MixBlock(ref p0, ref p1);

            rp0 = p0;
            rp1 = p1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Finish(ref uint p0, ref uint p1, ulong value, int remainingBytes)
        {
            var byteOffset = 0;

            switch (remainingBytes)
            {
                case 4:
                    p0 += (uint)value;
                    MixBlock(ref p0, ref p1);
                    goto case 0;

                case 0:
                    p0 += 0x80u;
                    break;

                case 5:
                    p0 += (uint)value;
                    byteOffset += 4;
                    MixBlock(ref p0, ref p1);
                    goto case 1;

                case 1:
                    p0 += 0x8000u | (byte)(value >> (byteOffset * 8));
                    break;

                case 6:
                    p0 += (uint)value;
                    byteOffset += 4;
                    MixBlock(ref p0, ref p1);
                    goto case 2;

                case 2:
                    p0 += 0x800000u | (ushort)(value >> (byteOffset * 8));
                    break;

                case 7:
                    p0 += (uint)value;
                    byteOffset += 4;
                    MixBlock(ref p0, ref p1);
                    goto case 3;

                case 3:
                    p0 += 0x80000000u | (uint)((value >> (byteOffset * 8)) & 0x00FFFFFF);
                    break;

                default:
                    Debug.Fail("Should not get here.");
                    break;
            }

            MixBlock(ref p0, ref p1);
            MixBlock(ref p0, ref p1);

            return (((long)p1) << 32) | p0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void MixBlock(ref uint rp0, ref uint rp1)
        {
            uint p0 = rp0;
            uint p1 = rp1;

            p1 ^= p0;
            p0 = BitOps.RotateLeft(p0, 20);

            p0 += p1;
            p1 = BitOps.RotateLeft(p1, 9);

            p1 ^= p0;
            p0 = BitOps.RotateLeft(p0, 27);

            p0 += p1;
            p1 = BitOps.RotateLeft(p1, 19);

            rp0 = p0;
            rp1 = p1;
        }
    }
}
