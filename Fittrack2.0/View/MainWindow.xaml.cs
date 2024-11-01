﻿using FitTrack2._0.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using Fittrack2._0.ViewModel;

namespace Fittrack2._0.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       

        public MainWindow()
        {
            InitializeComponent();
            ManageUser manageUser = ManageUser.Instance;
            this.DataContext = new MainViewModel( manageUser);
        }
       



        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                // Sätt lösenordet i ViewModel när det ändras i PasswordBox
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
