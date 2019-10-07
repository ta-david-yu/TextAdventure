using Microsoft.VisualStudio.TestTools.UnitTesting;
using DYTA.Math;

namespace DYTATests
{
    [TestClass]
    public class MathTests
    {
        [TestMethod]
        public void Rect2IntTestMethod()
        {
            RectInt rect1 = new RectInt(0, 0, 3, 3);
            RectInt rect2 = new RectInt(new Vector2Int(0, 0), new Vector2Int(3, 3));

            Assert.AreEqual(rect1, rect2);
        }
    }
}
