using System;
using System.Windows;
using FitTrack2._0.ViewModel;
using FitTrack2._0.Model;

namespace FitTrack2._0.View
{
    public partial class AddWorkoutWindow : Window
    {
        public AddWorkoutWindow()
        {
            InitializeComponent();

            AddWorkoutViewModel viewModel = new AddWorkoutViewModel(this);
            DataContext = viewModel;

        }
    }
}
