using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Haschisch.Tests
{
    public struct Small : IHashable, IEquatable<Small>
    {
        public readonly int Arg1;
        public readonly int Arg2;
        public readonly int Arg3;

        public Small(int a1, int a2, int a3)
        {
            this.Arg1 = a1;
            this.Arg2 = a2;
            this.Arg3 = a3;
        }

        public static bool operator ==(Small l, Small r) => l.Equals(r);

        public static bool operator !=(Small l, Small r) => !(l == r);

        public override string ToString() =>
            $"Small {{ {this.Arg1} {this.Arg2} {this.Arg3} }}";

        public override bool Equals(object obj) =>
            (obj as Small?)?.Equals(this) == true;

        public override int GetHashCode() => this.GetHaschisch();

        public bool Equals(Small o) =>
            this.Arg1 == o.Arg1 && this.Arg2 == o.Arg2 && this.Arg3 == o.Arg3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Hash<T>(ref T hasher)
            where T : struct, IStreamingHasherSink
        {
            Hasher.Write(ref hasher, this.Arg1);
            Hasher.Write(ref hasher, this.Arg2);
            Hasher.Write(ref hasher, this.Arg3);
        }
    }

    public struct Medium : IHashable, IEquatable<Medium>
    {
        public readonly Small S;
        public readonly string Text;

        public Medium(Small s, string t)
        {
            this.S = s;
            this.Text = t;
        }

        public static bool operator ==(Medium l, Medium r) => l.Equals(r);

        public static bool operator !=(Medium l, Medium r) => !(l == r);

        public override string ToString() =>
            $"Medium {{ {this.S} '{this.Text}' }}";

        public override bool Equals(object obj) =>
            (obj as Medium?)?.Equals(this) == true;

        public override int GetHashCode() => this.GetHaschisch();

        public bool Equals(Medium o) =>
            this.S == o.S && this.Text == o.Text;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Hash<T>(ref T hasher)
            where T : struct, IStreamingHasherSink
        {
            Hasher.Write(ref hasher, this.S);
            Hasher.Write(ref hasher, this.Text);
        }
    }

    public struct Large : IHashable, IEquatable<Large>
    {
        public readonly Medium M;
        public readonly int[] List;

        public Large(Medium m, int[] l)
        {
            this.M = m;
            this.List = l;
        }

        public static bool operator ==(Large l, Large r) => l.Equals(r);

        public static bool operator !=(Large l, Large r) => !(l == r);

        public override string ToString() =>
            $"Large {{ {this.M} {this.List} }}";

        public override bool Equals(object obj) =>
            (obj as Large?)?.Equals(this) == true;

        public override int GetHashCode() => this.GetHaschisch();

        public bool Equals(Large o) =>
            this.M == o.M && this.List?.SequenceEqual(o.List) == true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Hash<T>(ref T hasher)
            where T : struct, IStreamingHasherSink
        {
            Hasher.Write(ref hasher, this.M);
            Hasher.Write(ref hasher, this.List?.LongLength ?? 0);
        }
    }
}
