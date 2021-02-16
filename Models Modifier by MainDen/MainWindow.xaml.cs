using Models_Modifier_by_MainDen.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Models_Modifier_by_MainDen
{
    public partial class MainWindow : Window
    {
        MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel(this);
            DataContext = ViewModel;
        }

        private void OnDragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
