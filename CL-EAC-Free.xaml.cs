using RestSharp;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CardLifeAltLaunch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string email="";
        private string password="";
        private auth authdata;

        enum state
        {
            PreLogInPatch  = 0, // Should try to patch before logging in
            NotLoggedIn    = 1, // Pre-Patching done
            LoggedIn       = 2, // Have Auth Token
            PostLoginPatch = 3, // Should try to patch before playing
            Playing        = 4, // Currently running
            Terminated     = 5  // We don't really need this state...
        }

        private state curState;

        public MainWindow()
        {
            InitializeComponent();
            curState = state.NotLoggedIn;
            // Check to see if we've already got an email
            email = Properties.Settings.Default.emailAddress;
            // Do the opacity thing here
            if (!email.Equals(""))
            {
                username.Opacity = 1;
                username.Text = email;
            }
        }
        /// <summary>
        /// Launch cardlife with the public ID and auth token
        /// </summary>
        private void Launch()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "D:\\CL\\cardlife_mod.exe";
            psi.Arguments = authdata.PublicId + " " + authdata.Token;

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
            this.Dispatcher.Invoke(() => {
                button.Content = "Play";
                button.IsEnabled = true;
            });
            
            curState = state.LoggedIn;
        }

        /// <summary>
        /// Attempt to Authenticate against the auth server
        /// </summary>
        /// <returns></returns>
        private bool Authenticate()
        {
            // Check to see if they entered creds and abort if not
            if ((email.Length == 0) && (password.Length == 0))
            {
                return false;
            }

            var client = new RestSharp.RestClient("https://live-auth.cardlifegame.com/");
            var request = new RestSharp.RestRequest("api/auth/authenticate/", Method.POST);
            var body = new
            {
                emailAddress = email,
                Password = password
            };
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddJsonBody(body);
            
            
            var response = client.Execute<auth>(request);

            // Could we talk to the server?
            if (!response.IsSuccessful)
            {
                // Did the server accept our username
                if (response.Content.Equals("User not found"))  // Username enumeration... :(
                {

                    MessageBox.Show("Login failed. Please check\nyour email address.", "Couldn't log in", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Did the server accept our password
                if (response.Content.Equals("Failed to authenticate"))
                {

                    MessageBox.Show("Login failed. Please check\nyour password.", "Couldn't log in", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                // Couldn't contact server
                if (response.StatusCode == 0)
                {
                    MessageBox.Show("Couldn't contact the server. Are you online?\n" + response.ErrorMessage, "Couldn't log in", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                // All other failures!
                MessageBox.Show("Something went wrong talking to the server.\n" + response.ErrorMessage, "Couldn't log in", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Parse returned data
            try
            {
                authdata = response.Data;
                Properties.Settings.Default.emailAddress = authdata.EmailAddress;
            } catch (Exception e)
            {
                MessageBox.Show("Login failed. Server returned\ndata I couldn't understand!", "Couldn't log in", MessageBoxButton.OK, MessageBoxImage.Warning);
                Console.WriteLine("Exception " + e.Message);
            } 
            if (authdata != null)
            {
                Properties.Settings.Default.Save();
                return true;
            } else
            {
                return false;
            }

        }


        /// <summary>
        /// Handle statemachine behind button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (curState)
            {
                case state.NotLoggedIn:
                    if (Authenticate())
                    {
                        button.Content = "Play";
                        curState = state.LoggedIn;
                    }
                    break;
                case state.LoggedIn:
                    button.Content = "Playing";
                    button.IsEnabled = false;
                    Launch();
                    break;
                case state.Playing:
                    break;
                default:
                    Console.WriteLine("How did you get here?");
                    break;
            }
        }

        /// <summary>
        /// Store email from user entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbx_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Console.WriteLine(textBox.Name);

            if (textBox.Text.Equals(""))
            {
                if (textBox.Name.Equals("username"))
                {
                    textBox.Text = "Email address";
                    textBox.Opacity = 0.5;
                }
            } else
            {
                email = textBox.Text;
            }
                
        }
        /// <summary>
        /// Opacity stuff to make username box look nice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbx_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Opacity == 0.5)
            {
                textBox.Opacity = 1;
                textBox.Text = "";
            }
        }

        /// <summary>
        /// Opacity stuff to make password box look nice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void passwd_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.Opacity = 1;
        }

        /// <summary>
        /// Opacity stuff to make password box look nice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void passwd_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox.Password.Length == 0)
            {
                passwordBox.Opacity = 0;
            } else
            {
                password = passwordBox.Password;
            }
        }
    }
}
