using Logika;

namespace Tests
{
    [TestClass]
    public class LogikaAPITest
    {
        [TestMethod]
        public void randGenKulTest()
        {
            int wysokosc = 200;
            int szerokosc = 350;
            int srednica = 20;
            SimMenager simMenager = new SimMenager(new Scena(szerokosc,wysokosc),srednica);
            IList<Kula> kule=simMenager.RandGenKul(5);
            Assert.AreEqual(kule.Count, 5);
        }
    }
}
