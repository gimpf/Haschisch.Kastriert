# Haschisch: A .NET Library for Non-Cryptographic Hashes

_Haschisch_ provides several non-cryptographic hash algorithms for .NET, featuring a common API and high-performance implementations.


## Getting Started

0. .NET Core 2 SDK required
1. build solution:`dotnet publish -c Release`
2. run benchmarks: `dotnet bin/Release/netcoreapp2.0/publish/Haschisch.Benchmarks.dll -j:core_x64 -- CombineHashCodes --allcategories=prime,per-ad-seed`

To use one of hashers:

```cs
int HashWithBlock<T>(byte[] data) where T : struct, IBlockHasher<int> =>
    default(T).Hash(data, 0, data.Length);
// ...
HashWithBlock<XXHash64Hasher.Block>(new byte[0]);
```


## Current State

Available algorithms:

- XXHash: xx32, xx64
- CityHash: city32, city64, city64 w/seeds,
- HalfSip: hsip13, hsip24
- Sip: sip13, sip24
- _Ferner liefen_: seahash, marvin32, spookyv2.

Most algorithms are available for these interfaces:

* `IHashCodeCombiner`: Combine the hash-codes of .NET values into a new hash-code, that is, precursors and alternatives to [the new `System.HashCode` type](https://github.com/dotnet/corefx/issues/14354).
* `IStreamingHasher<int>`: Incremental/Streaming hash calculation, following the well-known init-mix-mix-finish interface.  `city`-type hashes don't support this.
* `IBlockHasher<int>`: Calculate the hash of a contiguous block of memory.
* `IUnsafeBlockHasher<int>`: as above, but working of `ref byte` instead of an array, obviously unsafe.

The size of the hash-codes depend on the algorithm, but hash-codes shortened to 32bit are available as a common API.

Performance remarks w.r.t. to algorithm selection:

* XXHash, Murmur-3-32, Sip and Half-Sip perform well, that is not too much slower than reference C implementations.
* For combining hash-codes, the `Combiner`-type for XXHash32 is the fastest accross both 64- and 32-bit .NET targets.  Sip-1-3 is doing well on 64 bit.
* For large-ish messages (larger than 2 kiB) the _Block_ implementation of xxHash64 seems acceptably fast (on 64bit targets).
* CityHash, SpookyHash and SeaHash are usable, but are slower than expected compared to XX etc.

Performance remarks for all algorithms:

* The native implementations are always faster than anything here.
* All implementations are non-allocating, that is there are no hidden penalties because of GC pressure.
* Only RyuJit leads to usable performance.  Running this on old runtimes will strongly disappoint you.
* Some _Stream_-type implementations support block updates with the unsafe API.  They should be good enough for file-checksumming etc.  Again, xxHash64 works acceptably well.
* Using _Stream_-type implementations with small updates leads to disastrous performance.



## Changelog

* **WIP**: introduce new hash-algorithms (sip13, sip24, city32, city64, city64-w-seeds, spookyv2), and optimize existing and new ones, especially for hash-code-combining; also extend the test-coverage, and simplify benchmarking
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
