using FitTrack2._0.Model;
using Fittrack2._0.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Fittrack2._0.View
{
    /// <summary>
    /// Interaction logic for RegisterUserWindow.xaml
    /// </summary>
    public partial class RegisterUserWindow : Window
    {
        public RegisterUserWindow()
        {
            InitializeComponent();
            // Använder ManageUser.Instance för att få Singleton-instansen
            ManageUser manageUser = ManageUser.Instance;
            // Skickar in instansen av RegisterUserWindow (this) till RegisterUserViewModel-konstruktorn
            this.DataContext = new RegisterUserViewModel(manageUser, this);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterUserViewModel viewModel)
            {
                // Uppdaterar lösenord i ViewModel när användaren skriver i PasswordBox
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterUserViewModel viewModel)
            {
                // Uppdaterar ConfirmPassword i ViewModel när användaren skriver i ConfirmPasswordBox
                viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
