using RestSharp;
using System;
using System.ComponentModel;
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
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty =
             DependencyProperty.Register(
                 "CurrentTime", 
                 typeof(VMAuthentification),
                 typeof(MainWindow), 
                 new FrameworkPropertyMetadata(null)
             );

        public VMAuthentification ViewModel
        {
            get { return (VMAuthentification)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new VMAuthentification();
        }

        #region Password
        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.Password = passwordBox.SecurePassword;
        }
        #endregion
    }
}
