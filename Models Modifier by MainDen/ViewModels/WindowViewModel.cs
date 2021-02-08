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

        private BaseCommand closeComand;
        public BaseCommand CloseCommand
        {
            get
            {
                return closeComand ??
                    (closeComand = new BaseCommand(obj =>
                    {
                        Window.Close();
                    }));
            }
        }

        private BaseCommand minimizeCommand;
        public BaseCommand MinimizeCommand
        {
            get
            {
                return minimizeCommand ??
                    (minimizeCommand = new BaseCommand(obj =>
                    {
                        Window.WindowState = WindowState.Minimized;
                    }));
            }
        }

        private BaseCommand maximizeCommand;
        public BaseCommand MaximizeCommand
        {
            get
            {
                return maximizeCommand ??
                    (maximizeCommand = new BaseCommand(obj =>
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
