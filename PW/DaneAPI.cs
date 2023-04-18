using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dane
{
    public abstract class DaneAbstractApi
    {
        public abstract int WysokoscSceny { get; }
        public abstract int SzerokoscSceny { get; }
        public abstract int SrednicaKuli { get; }


        public static DaneAbstractApi StworzDaneApi()
        {
            return new DaneApi();
        }
    }
    public class DaneApi : DaneAbstractApi
    {
        //ustawiamy tutaj parametry
        public override int WysokoscSceny { get; } = 200;

        public override int SzerokoscSceny { get; } = 350;

        public override int SrednicaKuli { get; } = 20;
    }
}
