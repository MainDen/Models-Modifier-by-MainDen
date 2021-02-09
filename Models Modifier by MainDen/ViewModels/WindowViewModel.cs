using Models_Modifier_by_MainDen.Commands;
using System.Windows;

namespace Models_Modifier_by_MainDen.ViewModels
{
    public class WindowViewModel : AbstractViewModel
    {
        protected Window Window { get; set; }

        public WindowViewModel(Window window)
        {
            Window = window;
        }

        private RelayCommand closeComand;
        public RelayCommand CloseCommand
        {
            get
            {
                return closeComand ??
                    (closeComand = new RelayCommand(obj =>
                    {
                        Window.Close();
                    }));
            }
        }

        private RelayCommand minimizeCommand;
        public RelayCommand MinimizeCommand
        {
            get
            {
                return minimizeCommand ??
                    (minimizeCommand = new RelayCommand(obj =>
                    {
                        Window.WindowState = WindowState.Minimized;
                    }));
            }
        }

        private RelayCommand maximizeCommand;
        public RelayCommand MaximizeCommand
        {
            get
            {
                return maximizeCommand ??
                    (maximizeCommand = new RelayCommand(obj =>
                    {
                        switch (Window.WindowState)
                        {
                            case WindowState.Normal:
                                Window.WindowState = WindowState.Maximized;
                                break;
                            case WindowState.Maximized:
                                Window.WindowState = WindowState.Normal;
                                break;
                        }
                        OnPropertyChanged(nameof(MaximizeSvgContent));
                    }));
            }
        }
        public string MaximizeSvgContent
        {
            get
            {
                if (Window.WindowState == WindowState.Maximized)
                    return "M 0 0 L 0 10 L 10 10 L 10 0 L 2 0 L 2 4 L 8 4 L 8 8 L 2 8 L 2 0 Z";
                return "M 0 0 L 0 10 L 10 10 L 10 0 L 2 0 L 2 2 L 8 2 L 8 8 L 2 8 L 2 0 Z";
            }
        }
    }
}
