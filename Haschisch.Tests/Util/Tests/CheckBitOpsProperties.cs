using System;
using Haschisch.Tests;
using NUnit.Framework;

namespace Haschisch.Util.Tests
{
    [TestFixture]
    public class CheckBitOpsProperties
    {
        [Test]
        public void BitOps_RotateLeft_With0_ChangesNothing()
        {
            Func<int, bool> unrotatedI32BeEqual = x => BitOps.RotateLeft(x, 0) == x;
            Func<uint, bool> unrotatedU32BeEqual = x => BitOps.RotateLeft(x, 0) == x;
            Func<long, bool> unrotatedI64BeEqual = x => BitOps.RotateLeft(x, 0) == x;
            Func<ulong, bool> unrotatedU64BeEqual = x => BitOps.RotateLeft(x, 0) == x;
            DoCheck.That(unrotatedI32BeEqual);
            DoCheck.That(unrotatedU32BeEqual);
            DoCheck.That(unrotatedI64BeEqual);
            DoCheck.That(unrotatedU64BeEqual);
        }

        [Test]
        public void BitOps_RotateLeft_WithAllBits_ChangesNothing()
        {
            Func<int, bool> unrotatedI32BeEqual = x => BitOps.RotateLeft(x, 8 * sizeof(int)) == x;
            Func<uint, bool> unrotatedU32BeEqual = x => BitOps.RotateLeft(x, 8 * sizeof(uint)) == x;
            Func<long, bool> unrotatedI64BeEqual = x => BitOps.RotateLeft(x, 8 * sizeof(long)) == x;
            Func<ulong, bool> unrotatedU64BeEqual = x => BitOps.RotateLeft(x, 8 * sizeof(ulong)) == x;
            DoCheck.That(unrotatedI32BeEqual);
            DoCheck.That(unrotatedU32BeEqual);
            DoCheck.That(unrotatedI64BeEqual);
            DoCheck.That(unrotatedU64BeEqual);
        }

        [Test]
        public void BitOps_RotateLeft_WithAllBitsInTwoSteps_ChangesNothing()
        {
            Func<int, bool> unrotatedI32BeEqual = x => BitOps.RotateLeft(BitOps.RotateLeft(x, 8 * sizeof(int) - 3), 3) == x;
            Func<uint, bool> unrotatedU32BeEqual = x => BitOps.RotateLeft(BitOps.RotateLeft(x, 8 * sizeof(uint) - 3), 3) == x;
            Func<long, bool> unrotatedI64BeEqual = x => BitOps.RotateLeft(BitOps.RotateLeft(x, 8 * sizeof(long) - 3), 3) == x;
            Func<ulong, bool> unrotatedU64BeEqual = x => BitOps.RotateLeft(BitOps.RotateLeft(x, 8 * sizeof(ulong) - 3), 3) == x;
            DoCheck.That(unrotatedI32BeEqual);
            DoCheck.That(unrotatedU32BeEqual);
            DoCheck.That(unrotatedI64BeEqual);
            DoCheck.That(unrotatedU64BeEqual);
        }
    }
}
