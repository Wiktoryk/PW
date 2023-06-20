namespace Dane
{
    public class Scena
    {
        private double m_width;
        private double m_height;

        private static object width_lock = new();
        private static object height_lock = new();

        public Scena(double width, double height)
        {
            this.m_width = width;
            this.m_height = height;
        }

        public void SetWidth(double width)
        {
            lock (width_lock)
            {
                this.m_width = width;
            }
        }

        public void SetHeight(double height)
        {
            lock (height_lock)
            {
                this.m_height = height;
            }
        }

        public double GetWidth()
        {
            lock (width_lock)
            {
                return m_width;
            }
        }

        public double GetHeight()
        {
            lock (height_lock)
            {
                return m_height;
            }
        }
    }
}
