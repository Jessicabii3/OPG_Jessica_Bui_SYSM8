using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FitTrack2._0.ViewModel;
using FitTrack2._0.Model;

namespace FitTrack2._0.View
{
    /// <summary>
    /// Interaction logic for UserDetailsWindow.xaml
    /// </summary>
    public partial class UserDetailsWindow : Window
    {

        public UserDetailsWindow()
        {
            InitializeComponent();
            DataContext = new UserDetailsViewModel();
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserDetailsViewModel viewModel)
            {
                var passwordBox = sender as PasswordBox;
                if (passwordBox != null)
                {
                    viewModel.NewPassword = passwordBox.Password;
                }
            }
        }

        private void ConfirmpasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserDetailsViewModel viewModel)
            {
                var confirmPasswordBox = sender as PasswordBox;
                if (confirmPasswordBox != null)
                {
                    viewModel.ConfirmPassword = confirmPasswordBox.Password;
                }
            }
        }
    }
}
