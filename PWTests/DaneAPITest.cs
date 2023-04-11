using Dane;
namespace Tests
{
    [TestClass]
    public class DaneAPITest
    {
        [TestMethod]
        public void StworzSceneTest()
        {
            DaneAPIAbstrakcyjne testAPI = DaneAPIAbstrakcyjne.StworzAPI();
            int szerokosc = 100;
            int wysokosc = 70;
            int licznoscKul = 5;
            int promien = 2;
            testAPI.StworzScene(szerokosc,wysokosc,licznoscKul,promien);
            Assert.AreEqual(testAPI.Scena.Wysokosc, 70);
            Assert.AreEqual(testAPI.Scena.Szerokosc, 100);
            Assert.AreEqual(testAPI.Scena.Wlaczony, true);
        }
    }
}