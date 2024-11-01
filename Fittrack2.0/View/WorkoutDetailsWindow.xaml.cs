using Fittrack2._0.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using FitTrack2._0.Model;

namespace Fittrack2._0.View
{
    /// <summary>
    /// Interaction logic for WorkoutDetailsWindow.xaml
    /// </summary>
    public partial class WorkoutDetailsWindow : Window
    {
        public WorkoutDetailsWindow(Workout workout,ObservableCollection<Workout> workouts,ManageUser userManager)
        {
            InitializeComponent();
            this.DataContext = new WorkoutDetailsViewModel(workout,workouts,userManager);

            
        }
        
    }

       
       

    
}
