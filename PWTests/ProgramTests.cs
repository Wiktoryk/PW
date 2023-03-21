using PW;
namespace PWTests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void add_works()
        {
            int x = 1;
            int y=2;
            int z = -1;
            Program program=new();
            Assert.AreEqual(program.add(x,y), 3);
            Assert.AreNotEqual(program.add(x, y), 2);
            Assert.AreEqual(program.add(x, z), 0);
        }
    }
}