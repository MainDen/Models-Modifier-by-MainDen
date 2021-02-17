using Models_Modifier_by_MainDen.Commands;
using Models_Modifier_by_MainDen.Models;
using Modifiers_by_MainDen.Modifiers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Models_Modifier_by_MainDen.ViewModels
{
    public class MainWindowViewModel : WindowViewModel
    {
        static MainWindowViewModel()
        {
            InitializeModifiers();
        }

        public MainWindowViewModel(MainWindow window) : base(window)
        {
            MainWindow = window;
            Arg = new ArgModel(MainWindow.properties);
            InitializeAppliedModifiers();
        }

        private MainWindow MainWindow { get; set; }
        private ArgModel Arg { get; set; }

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

        private static void InitializeModifiers()
        {
            try
            {
                modifiers = new ObservableCollection<AbstractModifier>();
                Type aType = typeof(AbstractModifier);
                IEnumerable<Type> types = Assembly.GetAssembly(aType).GetTypes().Where(type => type.IsSubclassOf(aType));
                foreach (var type in types)
                    modifiers.Add((AbstractModifier)type.GetConstructor(Type.EmptyTypes).Invoke(null));
            }
            catch { }
        }
        public static ObservableCollection<AbstractModifier> modifiers;
        public ObservableCollection<AbstractModifier> Modifiers
        {
            get
            {
                ObservableCollection<AbstractModifier> visibleModifiers = new ObservableCollection<AbstractModifier>();
                Type type = ResultType;
                foreach (var modifier in modifiers)
                    if (modifier.CanBeAppliedTo(type) && IsMatchesTheSearchQuery(SearchText, modifier.Name))
                        visibleModifiers.Add(modifier.Modifier);
                return visibleModifiers;
            }
        }
        private AbstractModifier selectedModifier;
        public AbstractModifier SelectedModifier
        {
            get
            {
                return selectedModifier;
            }
            set
            {
                selectedModifier = value;
                Arg.ResetPanelWithTemplate(selectedModifier);
                OnPropertyChanged(nameof(SelectedModifier));
            }
        }

        private void InitializeAppliedModifiers()
        {
            AppliedModifiers = new ObservableCollection<AbstractModifier>();
        }
        public ObservableCollection<AbstractModifier> AppliedModifiers { get; set; }
        private AbstractModifier selectedAppliedModifier;
        public AbstractModifier SelectedAppliedModifier
        {
            get
            {
                return selectedAppliedModifier;
            }
            set
            {
                selectedAppliedModifier = value;
                OnPropertyChanged(nameof(SelectedAppliedModifier));
            }
        }
        private Type ResultType
        {
            get
            {
                Type type = null;
                foreach (var modifier in AppliedModifiers)
                    type = modifier.ResultType(type);
                return type;
            }
        }
        private string searchText = "";
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Modifiers));
            }
        }
        private static bool IsMatchesTheSearchQuery(string query, string str)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));
            if (str is null)
                throw new ArgumentNullException(nameof(str));
            if (query == string.Empty)
                return true;
            string qStr = query.ToUpper();
            string sStr = str.ToUpper();
            int qi = 0;
            int si = 0;
            int qLength = qStr.Length;
            int sLength = sStr.Length;
            for (; qi < qLength && si < sLength; ++si)
                if (qStr[qi] == sStr[si])
                    ++qi;
            return qi == qLength;
        }

        private Bitmap resultBitmap = null;
        private Bitmap ResultBitmap
        {
            get => resultBitmap;
            set
            {
                resultBitmap = value;
                OnPropertyChanged(nameof(ResultImage));
            }
        }
        public BitmapImage ResultImage
        {
            get
            {
                return BitmapToImageSource(ResultBitmap);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            if (bitmap is null)
                return null;
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private RelayCommand applyCommand;
        public RelayCommand ApplyCommand
        {
            get
            {
                return applyCommand ??
                    (applyCommand = new RelayCommand(obj =>
                    {
                        if (selectedModifier != null)
                        {
                            AppliedModifiers.Add(SelectedModifier.Modifier);
                            SelectedModifier = null;
                            SearchText = "";
                            OnPropertyChanged(nameof(Modifiers));
                            ResultBitmap = null;
                        }
                    }));
            }
        }
    }
}
