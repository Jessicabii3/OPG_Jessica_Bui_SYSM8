﻿using FitTrack2._0.Model;
using FitTrack2._0.ViewModel;
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
namespace FitTrack2._0.View
{
    /// <summary>
    /// Interaction logic for ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        public ResetPasswordWindow()
        {
            InitializeComponent();
            DataContext = new ResetPasswordViewModel(this);

        }
        

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ResetPasswordViewModel viewModel)
            {
                viewModel.NewPassword = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ResetPasswordViewModel viewModel)
            {
                viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
