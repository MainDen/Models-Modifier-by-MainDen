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
            Window.SizeChanged += (sender, e) => OnPropertyChanged(nameof(MaximizeSvgContent));
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
        public string CloseSvgContent
        {
            get
            {
                return "M 0 0 L 10 0 L 10 10 L 0 10 Z"
                    + "M 1 0 L 5 4 L 9 0 Z"
                    + "M 1 10 L 5 6 L 9 10 Z"
                    + "M 0 1 L 4 5 L 0 9 Z"
                    + "M 10 1 L 6 5 L 10 9 Z";
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
        public string MinimizeSvgContent
        {
            get
            {
                return "M 0 1 L 10 1 L 10 2 L 0 2 Z";
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
                    return "M 2 2 L 2 0 L 10 0 L 10 8 L 8 8 L 8 7 L 9 7 L 9 1 L 3 1 L 3 2 Z M 0 2 L 8 2 L 8 10 L 0 10 Z M 1 3 L 7 3 L 7 9 L 1 9 Z";
                return "M 0 0 L 10 0 L 10 10 L 0 10 Z M 1 1 L 9 1 L 9 9 L 1 9 Z";
            }
        }
    }
}
