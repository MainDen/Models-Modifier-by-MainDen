using MainDen.ModelsModifier.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MainDen.ModelsModifier
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
