using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Models_Modifier_by_MainDen.ViewModels
{
    public class MainWindowViewModel : WindowViewModel
    {
        public MainWindowViewModel(System.Windows.Window window) : base(window)
        {
        }

        private string filePath;
        public string FilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    filePath = @".\NewModel.xml";
                return "File: " + filePath;
            }
        }

        private TimeSpan executionTime = new TimeSpan(0);
        public string ExecutionTime
        {
            get
            {
                return "Execution time: " + executionTime.TotalSeconds + "s";
            }
        }
    }
}
