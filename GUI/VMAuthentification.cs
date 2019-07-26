using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CardLifeAltLaunch
{
    public class VMAuthentification : ViewModel
    {

        public VMAuthentification()
        {
            GUIDispatcher = Dispatcher.CurrentDispatcher;

            initCommands();
        }

        Dispatcher GUIDispatcher { get; }

        #region Commands
        void initCommands()
        {
            initCmdPasswordEnterHit();
            initCmdMainAction();
        }

        #region CmdPasswordEnterHit
        private void initCmdPasswordEnterHit()
        {
            CmdPasswordEnterHit = new VMCommand(
                (param) =>
                {
                    if (CurrentState == EAuthentificationState.NotLoggedIn)
                    {
                        ExecuteCommandForCurrentState();
                    }
                },
                (param) =>
                {
                    return true;
                }
            );
        }

        public VMCommand CmdPasswordEnterHit { get; private set; }
        #endregion

        #region CmdMainAction
        private void initCmdMainAction()
        {
            CmdMainAction = new VMCommand(
                (param) =>
                {
                    ExecuteCommandForCurrentState();
                },
                (param) =>
                {
                    return true;
                }
            );
        }

        public VMCommand CmdMainAction { get; private set; }
        #endregion

        #endregion

        #region Email
        public const string PROP_EMAIL = "Email";

        private string m_Email = Properties.Settings.Default.emailAddress;

        public string Email
        {
            get
            {
                return m_Email;
            }

            set
            {
                SetProperty(ref m_Email, value, PROP_EMAIL);
            }
        }
        #endregion

        #region Password
        public const string PROP_PASSWORD = "Password";

        private SecureString m_Password = new SecureString();

        public SecureString Password
        {
            get
            {
                return m_Password;
            }

            set
            {
                SetProperty(ref m_Password, value, PROP_PASSWORD);
            }
        }
        #endregion

        #region AuthenticationData
        public CAuthentificationData AuthenticationData
        {
            get; set;
        }
        #endregion

        #region CurrentState
        public const string PROP_CURRENTSTATE = "CurrentState";

        private EAuthentificationState m_CurrentState = EAuthentificationState.NotLoggedIn;

        public EAuthentificationState CurrentState
        {
            get
            {
                return m_CurrentState;
            }

            set
            {
                SetProperty(ref m_CurrentState, value, PROP_CURRENTSTATE, PROP_ACTIONBUTTONTEXT, PROP_ACTIONBUTTONISENABLED);
            }
        }
        #endregion

        #region ActionButtonText
        public const string PROP_ACTIONBUTTONTEXT = "ActionButtonText";
        public string ActionButtonText
        {
            get
            {
                switch(CurrentState)
                {
                    case EAuthentificationState.NotLoggedIn:
                    case EAuthentificationState.LoggingIn:
                        return Properties.Resources.MAINWIN_LOGIN;

                    case EAuthentificationState.LoggedIn:
                    case EAuthentificationState.PostLoginPatch:
                    case EAuthentificationState.PreLogInPatch:
                    case EAuthentificationState.Terminated:
                        return Properties.Resources.MAINWIN_PLAY;

                    case EAuthentificationState.Playing:
                        return Properties.Resources.MAINWIN_PLAYING;

                    default:
                        Debug.Assert(false, "Unknown CurrentState in VMAuthentification.ActionButtonText");
                        return "";
                }
            }
        }
        #endregion

        #region ActionButtonIsEnabled
        public const string PROP_ACTIONBUTTONISENABLED = "ActionButtonIsEnabled";
        public bool ActionButtonIsEnabled
        {
            get
            {
                switch (CurrentState)
                {
                    case EAuthentificationState.NotLoggedIn:
                    case EAuthentificationState.LoggedIn:
                    case EAuthentificationState.PostLoginPatch:
                    case EAuthentificationState.PreLogInPatch:
                    case EAuthentificationState.Terminated:
                    case EAuthentificationState.Playing:
                        return true;

                    case EAuthentificationState.LoggingIn:
                        return false;

                    default:
                        Debug.Assert(false, "Unknown CurrentState in VMAuthentification.ActionButtonIsEnabled");
                        return false;
                }
            }
        }
        #endregion

        private void ShowMessage(string aTitle, string aText, EMessageType aMessageType)
        {
            switch(aMessageType)
            {
                case EMessageType.Warning:
                    MessageBox.Show(aText, aTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                default:
                    Debug.Assert(false, "Missing EMessageType in VMAuthentifcation.ShowMessage");
                    MessageBox.Show(aText, aTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }

        private void ExecuteCommandForCurrentState()
        {
            switch (CurrentState)
            {
                case EAuthentificationState.NotLoggedIn:
                    CAuthenticator anAuthenticator = new CAuthenticator(ShowMessage);

                    CAuthentificationData anAuthData = null;
                    if (anAuthenticator.Authenticate(Email, Password, out anAuthData))
                    {
                        AuthenticationData = anAuthData;
                        CurrentState = EAuthentificationState.LoggedIn;
                    }
                    break;
                case EAuthentificationState.LoggedIn:
                    CurrentState = EAuthentificationState.LoggingIn;

                    string aCardLifeLoc = Properties.Settings.Default.CardLifeExeLocation;
                    CLauncher aLauncher = new CLauncher(aCardLifeLoc, () =>
                    {
                        GUIDispatcher.Invoke(() =>
                        {
                            CurrentState = EAuthentificationState.LoggedIn;
                        });                  
                    });

                    aLauncher.Launch(AuthenticationData);
                    break;
                case EAuthentificationState.Playing:
                    break;
                default:
                    Console.WriteLine("How did you get here?");
                    break;
            }
        }
    }
}
