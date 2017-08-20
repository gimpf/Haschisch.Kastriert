# Haschisch: A .NET Library for Non-Cryptographic Hashes

Collection of some non-cryptographic hash algorithm implementations in C#.

## Getting Started

0. .NET 4.7 SDK required
1. build solution
2. run benchmarks like `Haschisch.Benchmarks.exe -j:clr_x64 --quick-and-dirty -c:algo_block`

## License

Parts of the Marvin32 implementation are available under the
MIT license (see [Marvin32Steps.cs](Haschisch/Hashers/Marvin32Steps.cs)).

Everything else is available under [CC0](LICENSE.md):

Haschisch by [Markus Grüneis](mailto:gimpf@gimpf.org)

To the extent possible under law, the person who associated CC0 with
Haschisch has waived all copyright and related or neighboring rights
to Haschisch.

You should have received a copy of the CC0 legalcode along with this
work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.
