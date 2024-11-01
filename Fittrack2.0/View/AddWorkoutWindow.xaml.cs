using System;
using System.Windows;
using Fittrack2._0.ViewModel;
using FitTrack2._0.Model;

namespace Fittrack2._0.View
{
    public partial class AddWorkoutWindow : Window
    {
        public AddWorkoutWindow()
        {
            InitializeComponent();

            AddWorkoutViewModel viewModel = new AddWorkoutViewModel();

            // Definierar en metod för att stänga fönstret när passet är sparat eller avbrutet
            viewModel.CloseWorkoutWindow = () => this.Close();

            // Sätter DataContext så att fönstret kan binda till ViewModel-egenskaper och kommandon
            DataContext = viewModel;

        }
    }
}
