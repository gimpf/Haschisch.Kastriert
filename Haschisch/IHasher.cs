namespace Haschisch
{
    public interface IStreamingHasherSink
    {
        // interfaces and structs don't work well together ('as' operator boxes)
        // so instead of having a separate IUnsafeDingsBums, let's have an interface that lies
        bool AllowUnsafeWrite { get; }

        unsafe int UnsafeWrite(ref byte value, int maxLength);

        void Write8(byte value);

        void Write16(short value);

        void Write32(int value);

        void Write64(long value);
    }

    public interface IStreamingHasher<T> : IStreamingHasherSink
    {
        void Initialize();

        T Finish();
    }

    public interface IBlockHasher<T>
    {
        T Hash(byte[] data, int offset, int length);
    }

    public interface IUnsafeBlockHasher<T>
    {
        unsafe T Hash(ref byte data, int length);
    }

    internal interface ISeedableStreamingHasher<THashCode, TSeed> : IStreamingHasher<THashCode>
    {
        void Initialize(TSeed seed);
    }

    // only used for unit-tests
    internal interface ISeedableBlockHasher
    {
        byte[] GetZeroSeed();

        byte[] Hash(byte[] seed, byte[] data, int offset, int length);
    }

    internal interface ISeedableBlockHasher<THashCode, TSeed> : IBlockHasher<THashCode>
    {
        THashCode Hash(TSeed seed, byte[] data, int offset, int length);
    }

    public interface IHashCodeCombiner
    {
        int Combine<T1>(T1 x1);

        int Combine<T1, T2>(T1 x1, T2 x2);

        int Combine<T1, T2, T3, T4>(T1 x1, T2 x2, T3 x3, T4 x4);

        int Combine<T1, T2, T3, T4, T5>(T1 x1, T2 x2, T3 x3, T4 x4, T5 x5);

        int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, T7 x7, T8 x8);
    }
}
