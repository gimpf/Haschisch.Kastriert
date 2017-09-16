using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class SeaHashSteps
    {
        public const int FullBlockSize = 4 * sizeof(ulong);

        public const ulong TestVectorSeedA = 0x16f11fe89b0d677cUL;
        public const ulong TestVectorSeedB = 0xb480a793d8e6c86cUL;
        public const ulong TestVectorSeedC = 0x6fe2e5aaf078ebc9UL;
        public const ulong TestVectorSeedD = 0x14f994a4c5259381UL;

        public static readonly ulong DefaultSeedA = NewSeed();
        public static readonly ulong DefaultSeedB = NewSeed();
        public static readonly ulong DefaultSeedC = NewSeed();
        public static readonly ulong DefaultSeedD = NewSeed();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitializeForTestVectors(out ulong a, out ulong b, out ulong c, out ulong d)
        {
            a = TestVectorSeedA;
            b = TestVectorSeedB;
            c = TestVectorSeedC;
            d = TestVectorSeedD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize(out ulong a, out ulong b, out ulong c, out ulong d)
        {
            a = DefaultSeedA;
            b = DefaultSeedB;
            c = DefaultSeedC;
            d = DefaultSeedD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MixStep(ref ulong a, ref ulong b, ref ulong c, ref ulong d, ulong block)
        {
            var next = Diffuse(a ^ block);
            a = b;
            b = c;
            c = d;
            d = next;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MixFinalPartialStep(ref ulong a, ulong block)
        {
            a = Diffuse(a ^ block);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MixFinalPartialStep(ref ulong a, ref ulong b, ulong block1, ulong block2)
        {
            a = Diffuse(a ^ block1);
            b = Diffuse(b ^ block2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MixFinalPartialStep(ref ulong a, ref ulong b, ref ulong c, ulong block1, ulong block2, ulong block3)
        {
            a = Diffuse(a ^ block1);
            b = Diffuse(b ^ block2);
            c = Diffuse(c ^ block2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MixStep(ref ulong a, ref ulong b, ref ulong c, ref ulong d, ulong x0, ulong x1, ulong x2, ulong x3)
        {
            x0 ^= a;
            x1 ^= b;
            x2 ^= c;
            x3 ^= d;

            a = Diffuse(x0);
            b = Diffuse(x1);
            c = Diffuse(x2);
            d = Diffuse(x3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Finish(ref ulong a, ref ulong b, ref ulong c, ref ulong d, ulong length) =>
            Diffuse(a ^ b ^ c ^ d ^ length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Diffuse(ulong value)
        {
            value *= 0x6eed0e9da4d94a4fUL;
            value ^= (value >> 32) >> ((byte)(value >> 60));
            value *= 0x6eed0e9da4d94a4fUL;
            return value;
        }

        private static ulong NewSeed()
        {
            Seeder.GetNewSeed(out ulong seed);
            return seed;
        }
    }
}
