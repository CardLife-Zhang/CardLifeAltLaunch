using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardLifeAltLaunch
{
    public enum EAuthentificationState
    {
        PreLogInPatch = 0, // Should try to patch before logging in
        NotLoggedIn = 1, // Pre-Patching done
        LoggingIn = 2, // Trying to log in
        LoggedIn = 3, // Have Auth Token
        PostLoginPatch = 4, // Should try to patch before playing
        Playing = 5, // Currently running
        Terminated = 6,  // We don't really need this state...
    }
}
