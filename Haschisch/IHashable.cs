namespace Haschisch
{
    public interface IHashable
    {
        void Hash<THasher>(ref THasher hasher)
            where THasher : struct, IStreamingHasherSink;
    }
}
