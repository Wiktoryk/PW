namespace Dane
{
    public class Scena
    {
        private double m_szerokosc;
        private double m_wysokosc;
        private object width_lock = new();
        private object height_lock = new();

        public Scena(double szerokosc, double wysokosc)
        {
            this.m_szerokosc = szerokosc;
            this.m_wysokosc = wysokosc;
        }

        public void SetWidth(double szerokosc)
        {
            lock (width_lock)
            {
                this.m_szerokosc = szerokosc;
            }
        }

        public void SetHeight(double wysokosc)
        {
            lock (height_lock)
            {
                this.m_wysokosc = wysokosc;
            }
        }

        public double GetWidth()
        {
            lock (width_lock)
            {
                return m_szerokosc;
            }
        }

        public double GetHeight()
        {
            lock (height_lock)
            {
                return m_wysokosc;
            }
        }
    }
}
