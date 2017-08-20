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
}
