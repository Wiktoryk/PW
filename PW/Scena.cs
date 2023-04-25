namespace Dane
{
    public class Scena
    {
        private double m_szerokosc;
        private double m_wysokosc;

        public Scena(double szerokosc, double wysokosc)
        {
            this.m_szerokosc = szerokosc;
            this.m_wysokosc = wysokosc;
        }

        public void SetWidth(double szerokosc)
        {
            this.m_szerokosc = szerokosc;
        }

        public void SetHeight(double wysokosc)
        {
            this.m_wysokosc = wysokosc;
        }

        public double GetWidth()
        {
            return m_szerokosc;
        }

        public double GetHeight()
        {
            return m_wysokosc;
        }
    }
}
