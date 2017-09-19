# Haschisch: A .NET Library for Non-Cryptographic Hashes

Collection of some non-cryptographic hash algorithm implementations in C#.



## Current State

Available algorithms: xx32, xx64, city32, city64, city64 w/seeds, hsip13, hsip24, sip13, sip24, seahash, marvin32, spookyv2.

Most algorithms are available for these interfaces:

* `IHashCodeCombiner`: Combine the hash-codes of .NET values into a new hash-code.  Currently the main optimization target.
* `IBlockHasher<int>`: Calculate the hash of a contiguous block of memory
* `IUnsafeBlockHasher<int>`: as above, but working of `ref byte` instead of an array, obviously unsafe
* `IStreamingHasher<int>`: Incremental hash calculation.  `city`-type hashes don't support this.

Performance remarks:

* Nothing is perfectly well optimized yet, but Murmur-3-32, City and xx seem doing well.
* For combining hash-codes, the `Combiner`-types of Murmur3x86-32 and City32 seem good.  xx32 is an option.
* Spooky-V2 and SeaHash seem unusually slow, the other hash-implementations seems "somewhat ok".
* For large-ish messages (larger than 2 kiB) the _Block_ implementation of xxHash64 seems acceptably fast.  City64 is likely not bad, but I've no exact numbers ready.
* Some _Stream_-type implementations support block updates with the unsafe API.  They should be good enough for file-checksumming etc.  Again, xxHash64 work acceptably well.
* Using _Stream_ to hash by byte or by int32 isn't anywhere near acceptable performance.



## Getting Started

0. .NET Core 2 SDK required
1. build solution (`dotnet publish -c Release`)
2. run benchmarks like `dotnet bin/Release/netcoreapp2.0/publish/Haschisch.Benchmarks.dll -j:core_x64 -- CombineHashCodes --allcategories=prime,per-ad-seed`

To use one of hashers:

```cs
int HashWithBlock<T>(byte[] data) where T : struct, IBlockHasher<int> =>
    default(T).Hash(data, 0, data.Length);
// ...
HashWithBlock<XXHash64Hasher.Block>(new byte[0]);
```



## Changelog

* **WIP**: introduce new hash-algorithms (sip13, sip24, city32, city64, city64-w-seeds, spookyv2), and optimize existing and new ones, especially for hash-code-combining; also extend the test-coverage
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
