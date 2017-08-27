# Haschisch: A .NET Library for Non-Cryptographic Hashes

Collection of some non-cryptographic hash algorithm implementations in C#.


## Current State

Available algorithms: xx32, xx64, seahash, marvin32, hsip13, hsip24.

All algorithms are available for three interfaces:

* `IBlockHasher<int>`: Calculate the hash of a contiguous block of memory
* `IStreamingHasher<int>`: Incremental hash calculation.
* `IHashCodeCombiner`: Combine the hash-codes of .NET values into a new hash-code.

Performance remarks:

* For large-ish messages (larger than 2 kiB) the _Block_ implementation of xxHash64 seems acceptably fast.
* Performance of the other algorithms' _Block_ implementations isn't bad per se, but it seems like more is possible.
* Some _Stream_-type implementations support block updates with the unsafe API.  They should be good enough for file-checksumming etc.  Again, xxHash64 work acceptably well.
* Using _Stream_ to hash by byte or by int32 isn't anywhere near acceptable performance.
* For combining hash-codes, the Murmur3x8432's `Combiner` type is the least worst.


## Getting Started

0. .NET Core 2 SDK required
1. build solution (`dotnet publish -c Release`)
2. run benchmarks like `dotnet path/to/Haschisch.Benchmarks.dll -j:core_x64 -j:clr_x86 -j:clr_x64 --quick -c:algo_block -c:algo_combiner -c:algo_hashset`

To use one of hashers:

```cs
int HashWithBlock<T>(byte[] data) where T : struct, IBlockHasher<int> =>
    default(T).Hash(data, 0, data.Length);
// ...
HashWithBlock<XXHash64Hasher.Block>(new byte[0]);
```


## Changelog

* **0.3.0**: introduce `IHashCodeCombiner`, extend benchmarks to compare performance for use-cases related to [dotnet/corefx issue 'Add System.HashCode'](https://github.com/dotnet/corefx/issues/14354).
* **0.2.0**: port to .NETStandard 2.0 and .NET Core 2.0
* **0.1.0**: first public version, having Block and Stream hashers for xx32, xx64, seahash, marvin32, hsip13, hsip24

## License

Parts of the Marvin32 implementation are available under the
MIT license (see [Marvin32Steps.cs](Haschisch/Hashers/Marvin32Steps.cs)).

Everything else is available under [CC0](LICENSE):

Haschisch by [Markus Grüneis](mailto:gimpf@gimpf.org)

To the extent possible under law, the person who associated CC0 with
Haschisch has waived all copyright and related or neighboring rights
to Haschisch.

You should have received a copy of the CC0 legalcode along with this
work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.
