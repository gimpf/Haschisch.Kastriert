using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class SpookyV2Steps
    {
        public static class Short
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Initialize((ulong seed0, ulong seed1) seed, out ulong s0, out ulong s1, out ulong s2, out ulong s3)
            {
                s0 = seed.seed0;
                s1 = seed.seed1;
                s2 = Long.Const;
                s3 = Long.Const;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Update(ref ulong s0, ref ulong s1, ref ulong s2, ref ulong s3, ulong v0, ulong v1, ulong v2, ulong v3)
            {
                s2 += v0;
                s3 += v1;
                Mix(ref s0, ref s1, ref s2, ref s3);
                s0 += v2;
                s1 += v3;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static (ulong, ulong) Finish(ref ulong s0, ref ulong s1, ref ulong s2, ref ulong s3, ulong v0, ulong v1, ulong v2, ulong v3, int length)
            {
                var remaining = length % (4 * sizeof(ulong));
                if (remaining >= 16)
                {
                    s2 += v0;
                    s3 += v1;
                    Mix(ref s0, ref s1, ref s2, ref s3);
                    remaining -= 16;
                    v0 = v2;
                    v1 = v3;
                }

                s3 += (ulong)length << 56;

                if (remaining > 0)
                {
                    if (remaining < sizeof(ulong)) { BufferUtil.ZeroUnusedBuffer(ref v0, (uint)remaining); }
                    if (remaining < 2 * sizeof(ulong))
                    {
                        var r = remaining - sizeof(ulong);
                        BufferUtil.ZeroUnusedBuffer(ref v1, (uint)(r < 0 ? 0 : r));
                    }

                    s2 += v0;
                    s3 += v1;
                }
                else
                {
                    s2 += Long.Const;
                    s3 += Long.Const;
                }

                End(ref s0, ref s1, ref s2, ref s3);
                return (s0, s1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void Mix(ref ulong s0, ref ulong s1, ref ulong s2, ref ulong s3)
            {
                s2 = BitOps.RotateLeft(s2, 50); s2 += s3; s0 ^= s2;
                s3 = BitOps.RotateLeft(s3, 52); s3 += s0; s1 ^= s3;
                s0 = BitOps.RotateLeft(s0, 30); s0 += s1; s2 ^= s0;
                s1 = BitOps.RotateLeft(s1, 41); s1 += s2; s3 ^= s1;
                s2 = BitOps.RotateLeft(s2, 54); s2 += s3; s0 ^= s2;
                s3 = BitOps.RotateLeft(s3, 48); s3 += s0; s1 ^= s3;
                s0 = BitOps.RotateLeft(s0, 38); s0 += s1; s2 ^= s0;
                s1 = BitOps.RotateLeft(s1, 37); s1 += s2; s3 ^= s1;
                s2 = BitOps.RotateLeft(s2, 62); s2 += s3; s0 ^= s2;
                s3 = BitOps.RotateLeft(s3, 34); s3 += s0; s1 ^= s3;
                s0 = BitOps.RotateLeft(s0, 5);  s0 += s1; s2 ^= s0;
                s1 = BitOps.RotateLeft(s1, 36); s1 += s2; s3 ^= s1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void End(ref ulong s0, ref ulong s1, ref ulong s2, ref ulong s3)
            {
                s3 ^= s2; s2 = BitOps.RotateLeft(s2, 15); s3 += s2;
                s0 ^= s3; s3 = BitOps.RotateLeft(s3, 52); s0 += s3;
                s1 ^= s0; s0 = BitOps.RotateLeft(s0, 26); s1 += s0;
                s2 ^= s1; s1 = BitOps.RotateLeft(s1, 51); s2 += s1;
                s3 ^= s2; s2 = BitOps.RotateLeft(s2, 28); s3 += s2;
                s0 ^= s3; s3 = BitOps.RotateLeft(s3, 9); s0 += s3;
                s1 ^= s0; s0 = BitOps.RotateLeft(s0, 47); s1 += s0;
                s2 ^= s1; s1 = BitOps.RotateLeft(s1, 54); s2 += s1;
                s3 ^= s2; s2 = BitOps.RotateLeft(s2, 32); s3 += s2;
                s0 ^= s3; s3 = BitOps.RotateLeft(s3, 25); s0 += s3;
                s1 ^= s0; s0 = BitOps.RotateLeft(s0, 63); s1 += s0;
            }
        }

        public static class Long
        {
            public const ulong Const = 0xdeadbeefdeadbeefUL;

            public const int NumVars = 12;
            public const int BlockSize = NumVars * sizeof(ulong);
            public const int MinLongSize = 2 * BlockSize;

            // internal (ulong, ulong) Seed => (this.s0, this.s1);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Initialize(
                (ulong seed0, ulong seed1) seed,
                out ulong s0, out ulong s1, out ulong s2, out ulong s3,
                out ulong s4, out ulong s5, out ulong s6, out ulong s7,
                out ulong s8, out ulong s9, out ulong sA, out ulong sB)
            {
                s0 = s3 = s6 = s9 = seed.seed0;
                s1 = s4 = s7 = sA = seed.seed1;
                s2 = s5 = s8 = sB = Const;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Update(
                ref ulong s0, ref ulong s1, ref ulong s2, ref ulong s3,
                ref ulong s4, ref ulong s5, ref ulong s6, ref ulong s7,
                ref ulong s8, ref ulong s9, ref ulong sA, ref ulong sB,
                ulong v0, ulong v1, ulong v2, ulong v3,
                ulong v4, ulong v5, ulong v6, ulong v7,
                ulong v8, ulong v9, ulong vA, ulong vB)
            {
                s0 += v0; s2 ^= sA; sB ^= s0; s0 = BitOps.RotateLeft(s0, 11); sB += s1;
                s1 += v1; s3 ^= sB; s0 ^= s1; s1 = BitOps.RotateLeft(s1, 32); s0 += s2;
                s2 += v2; s4 ^= s0; s1 ^= s2; s2 = BitOps.RotateLeft(s2, 43); s1 += s3;
                s3 += v3; s5 ^= s1; s2 ^= s3; s3 = BitOps.RotateLeft(s3, 31); s2 += s4;
                s4 += v4; s6 ^= s2; s3 ^= s4; s4 = BitOps.RotateLeft(s4, 17); s3 += s5;
                s5 += v5; s7 ^= s3; s4 ^= s5; s5 = BitOps.RotateLeft(s5, 28); s4 += s6;
                s6 += v6; s8 ^= s4; s5 ^= s6; s6 = BitOps.RotateLeft(s6, 39); s5 += s7;
                s7 += v7; s9 ^= s5; s6 ^= s7; s7 = BitOps.RotateLeft(s7, 57); s6 += s8;
                s8 += v8; sA ^= s6; s7 ^= s8; s8 = BitOps.RotateLeft(s8, 55); s7 += s9;
                s9 += v9; sB ^= s7; s8 ^= s9; s9 = BitOps.RotateLeft(s9, 54); s8 += sA;
                sA += vA; s0 ^= s8; s9 ^= sA; sA = BitOps.RotateLeft(sA, 22); s9 += sB;
                sB += vB; s1 ^= s9; sA ^= sB; sB = BitOps.RotateLeft(sB, 46); sA += s0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static (ulong, ulong) Finish(
                ref ulong s0, ref ulong s1, ref ulong s2, ref ulong s3,
                ref ulong s4, ref ulong s5, ref ulong s6, ref ulong s7,
                ref ulong s8, ref ulong s9, ref ulong sA, ref ulong sB,
                ulong v0, ulong v1, ulong v2, ulong v3,
                ulong v4, ulong v5, ulong v6, ulong v7,
                ulong v8, ulong v9, ulong vA, ulong vB,
                ulong length)
            {
                vB|= (length % BlockSize) << ((sizeof(ulong) - 1) * 8);

                End(
                    ref s0, ref s1, ref s2, ref s3,
                    ref s4, ref s5, ref s6, ref s7,
                    ref s8, ref s9, ref sA, ref sB,
                    v0, v1, v2, v3,
                    v4, v5, v6, v7,
                    v8, v9, vA, vB);

                return (s0, s1);
            }

            public static void End(
                ref ulong s0, ref ulong s1, ref ulong s2, ref ulong s3,
                ref ulong s4, ref ulong s5, ref ulong s6, ref ulong s7,
                ref ulong s8, ref ulong s9, ref ulong sA, ref ulong sB,
                ulong v0, ulong v1, ulong v2, ulong v3,
                ulong v4, ulong v5, ulong v6, ulong v7,
                ulong v8, ulong v9, ulong vA, ulong vB)
            {
                s0 += v0;
                s1 += v1;
                s2 += v2;
                s3 += v3;
                s4 += v4;
                s5 += v5;
                s6 += v6;
                s7 += v7;
                s8 += v8;
                s9 += v9;
                sA += vA;
                sB += vB;
                EndPartial(
                    ref s0, ref s1, ref s2, ref s3,
                    ref s4, ref s5, ref s6, ref s7,
                    ref s8, ref s9, ref sA, ref sB);
                EndPartial(
                    ref s0, ref s1, ref s2, ref s3,
                    ref s4, ref s5, ref s6, ref s7,
                    ref s8, ref s9, ref sA, ref sB);
                EndPartial(
                    ref s0, ref s1, ref s2, ref s3,
                    ref s4, ref s5, ref s6, ref s7,
                    ref s8, ref s9, ref sA, ref sB);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void EndPartial(
                ref ulong s0, ref ulong s1, ref ulong s2, ref ulong s3,
                ref ulong s4, ref ulong s5, ref ulong s6, ref ulong s7,
                ref ulong s8, ref ulong s9, ref ulong sA, ref ulong sB)
            {
                sB += s1; s2 ^= sB; s1 = BitOps.RotateLeft(s1, 44);
                s0 += s2; s3 ^= s0; s2 = BitOps.RotateLeft(s2, 15);
                s1 += s3; s4 ^= s1; s3 = BitOps.RotateLeft(s3, 34);
                s2 += s4; s5 ^= s2; s4 = BitOps.RotateLeft(s4, 21);
                s3 += s5; s6 ^= s3; s5 = BitOps.RotateLeft(s5, 38);
                s4 += s6; s7 ^= s4; s6 = BitOps.RotateLeft(s6, 33);
                s5 += s7; s8 ^= s5; s7 = BitOps.RotateLeft(s7, 10);
                s6 += s8; s9 ^= s6; s8 = BitOps.RotateLeft(s8, 13);
                s7 += s9; sA ^= s7; s9 = BitOps.RotateLeft(s9, 38);
                s8 += sA; sB ^= s8; sA = BitOps.RotateLeft(sA, 53);
                s9 += sB; s0 ^= s9; sB = BitOps.RotateLeft(sB, 42);
                sA += s0; s1 ^= sA; s0 = BitOps.RotateLeft(s0, 54);
            }
        }
    }
}
