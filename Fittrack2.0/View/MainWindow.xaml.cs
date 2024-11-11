using FitTrack2._0.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using FitTrack2._0.ViewModel;

namespace FitTrack2._0.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 

       
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(this);
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
