using Dane;
namespace Tests
{
    [TestClass]
    public class DaneAPITest
    {
        [TestMethod]
        public void StworzAPITest()
        {
            int wysokosc = 200;
            int szerokosc =350;
            int srednica=20;
            DaneApi api = new DaneApi();
            Assert.AreEqual(wysokosc, api.WysokoscSceny);
            Assert.AreEqual(szerokosc, api.SzerokoscSceny);
            Assert.AreEqual(srednica, api.SrednicaKuli);
        }
    }
}