using FitTrack2._0.Model;
using Fittrack2._0.ViewModel;
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

namespace Fittrack2._0.View
{
    /// <summary>
    /// Interaction logic for WorkoutsWindow.xaml
    /// </summary>
    public partial class WorkoutsWindow : Window
    {
       
        public WorkoutsWindow() : this(new WorkoutsViewModel(ManageUser.Instance))
        {
        }
        public WorkoutsWindow(WorkoutsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

           
        }
        
    }
}
