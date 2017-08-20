using System.Runtime.CompilerServices;

namespace Haschisch.Hashers
{
    public static class SeaHashSteps
    {
        public const int FullBlockSize = 4 * sizeof(ulong);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize(out ulong a, out ulong b, out ulong c, out ulong d)
        {
            a = 0x16f11fe89b0d677cUL;
            b = 0xb480a793d8e6c86cUL;
            c = 0x6fe2e5aaf078ebc9UL;
            d = 0x14f994a4c5259381UL;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MixStep(ref ulong a, ref ulong b, ref ulong c, ref ulong d, ulong block)
        {
            var next = Diffuse(ref a, ref b, ref c, ref d, a ^ block);
            a = b;
            b = c;
            c = d;
            d = next;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MixStep(ref ulong a, ref ulong b, ref ulong c, ref ulong d, ulong x0, ulong x1, ulong x2, ulong x3)
        {
            x0 ^= a;
            x1 ^= b;
            x2 ^= c;
            x3 ^= d;

            a = Diffuse(ref a, ref b, ref c, ref d, x0);
            b = Diffuse(ref a, ref b, ref c, ref d, x1);
            c = Diffuse(ref a, ref b, ref c, ref d, x2);
            d = Diffuse(ref a, ref b, ref c, ref d, x3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Finish(ref ulong a, ref ulong b, ref ulong c, ref ulong d, ulong length) =>
            Diffuse(ref a, ref b, ref c, ref d, a ^ b ^ c ^ d ^ length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Diffuse(ref ulong a, ref ulong b, ref ulong c, ref ulong d, ulong value)
        {
            value *= 0x6eed0e9da4d94a4fUL;
            value ^= (value >> 32) >> ((byte)(value >> 60));
            value *= 0x6eed0e9da4d94a4fUL;
            return value;
        }
    }
}
