﻿using FitTrack2._0.Model;
using FitTrack2._0.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FitTrack2._0.View
{
    /// <summary>
    /// Interaction logic for RegisterUserWindow.xaml
    /// </summary>
    public partial class RegisterUserWindow : Window
    {
        public RegisterUserWindow()
        {
            InitializeComponent();
            this.DataContext = new RegisterUserViewModel();
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
