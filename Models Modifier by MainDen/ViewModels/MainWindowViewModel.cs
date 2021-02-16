using Models_Modifier_by_MainDen.Commands;
using Modifiers_by_MainDen.Modifiers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Models_Modifier_by_MainDen.ViewModels
{
    public class MainWindowViewModel : WindowViewModel
    {
        static MainWindowViewModel()
        {
            InitializeModifiers();
        }

        public MainWindowViewModel(System.Windows.Window window) : base(window)
        {
            InitializeAppliedModifiers();
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
                        visibleModifiers.Add(modifier);
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
                OnPropertyChanged(nameof(SearchText));
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
                            AppliedModifiers.Add(SelectedModifier);
                            SelectedModifier = null;
                            SearchText = "";
                            OnPropertyChanged(nameof(Modifiers));
                        }
                    }));
            }
        }
    }
}
