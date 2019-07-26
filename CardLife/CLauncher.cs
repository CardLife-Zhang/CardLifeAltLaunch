using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardLifeAltLaunch
{
    public class CLauncher
    {
        public CLauncher(string aCardLifeLocation, Action aFinishedHandler)
        {
            CardLifeLocation = aCardLifeLocation;
            FinishedHandler = aFinishedHandler;
        }

        public string CardLifeLocation
        {
            get;
        }

        public Action FinishedHandler
        {
            get;
        }

        /// <summary>
        /// Launch cardlife with the public ID and auth token
        /// </summary>
        public void Launch(CAuthentificationData anAuthData)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = CardLifeLocation;
            psi.Arguments = anAuthData.PublicId + " " + anAuthData.Token;

            Process p = new Process();
            p.StartInfo = psi;
            p.EnableRaisingEvents = true;
            p.Exited += Terminated;

            p.Start();
        }

        /// <summary>
        ///  Handle the cardlife process terminating
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void Terminated(object o, EventArgs e)
        {
            FinishedHandler();
        }
    }
}
