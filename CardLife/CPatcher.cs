using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardLifeAltLaunch
{
    class CPatcher
    {
        public CPatcher(string aCardLifeLocation)
        {
            CardLifeLocation = aCardLifeLocation;
        }

        public string CardLifeLocation
        {
            get;
        }

        public Action FinishedHandler
        {
            get;
        }

    }
}
