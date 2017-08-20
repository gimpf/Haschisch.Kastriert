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
    }
}
