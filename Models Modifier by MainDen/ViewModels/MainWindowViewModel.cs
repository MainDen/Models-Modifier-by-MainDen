using Models_Modifier_by_MainDen.Commands;
using Modifiers_by_MainDen.Modifiers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
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
            Applier = new StatesViewModel();
            Applier.AutoInitialize = true;
            Applier.AutoUpdate = true;
            Updater = new StatesViewModel();
            Result = new ModelViewModel();
            InitializeAppliedModifiers();
        }

        private MainWindow MainWindow { get; set; }
        public StatesViewModel Applier { get; set; }
        public StatesViewModel Updater { get; set; }
        public ModelViewModel Result { get; set; }

        private TimeSpan executionTime = new TimeSpan(0);
        public string ExecutionTime
        {
            get
            {
                return "Execution time: " + executionTime.TotalSeconds + "s";
            }
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

        private string status;
        public string Status
        {
            get
            {
                if (string.IsNullOrWhiteSpace(status))
                    status = "No events to show.";
                return "Status: " + status;
            }
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        private class DirectoryContext
        {
            private DirectoryContext(string path)
            {
                root = Directory.CreateDirectory(path);
            }

            private DirectoryInfo root;

            public IEnumerable<FileInfo> GetFiles(string searchPattern)
            {
                foreach (var file in root.GetFiles(searchPattern))
                    yield return file;
                foreach (var subdir in root.GetDirectories("*", SearchOption.AllDirectories))
                    foreach (var file in subdir.GetFiles(searchPattern))
                        yield return file;
            }

            public static DirectoryContext CreateDirectory(string path)
            {
                return new DirectoryContext(path);
            }
        }
        private static void InitializeModifiers()
        {
            modifiers = new ObservableCollection<AbstractModifier>();
            try
            {
                Type aType = typeof(AbstractModifier);
                IEnumerable<Type> types = Assembly.GetAssembly(aType).GetTypes().Where(type => type.IsSubclassOf(aType));
                foreach (var type in types)
                    modifiers.Add((AbstractModifier)type.GetConstructor(Type.EmptyTypes).Invoke(null));
                foreach (var dll in DirectoryContext.CreateDirectory(@"mods").GetFiles("*.dll"))
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(dll.FullName);
                        types = assembly.GetTypes().Where(type =>
                        {
                            Type bType = type.BaseType;
                            if (bType is null)
                                return false;
                            return bType.FullName == aType.FullName;
                        });
                        foreach (var type in types)
                            modifiers.Add(new InvokerModifier(type.GetConstructor(Type.EmptyTypes).Invoke(null)));
                    }
                    catch { }
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

        private void InitializeAppliedModifiers()
        {
            AppliedModifiers = new ObservableCollection<AbstractModifier>();
        }
        public ObservableCollection<AbstractModifier> AppliedModifiers { get; set; }
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

        private List<object> results = new List<object>();

        private RelayCommand applyCommand;
        public RelayCommand ApplyCommand
        {
            get
            {
                return applyCommand ??
                    (applyCommand = new RelayCommand(obj =>
                    {
                        AbstractModifier modifier = Applier.Modifier;
                        if (modifier != null)
                        {
                            try
                            {
                                executionTime = TimeSpan.Zero;
                                OnPropertyChanged(nameof(ExecutionTime));
                                Status = "Applying modifier...";
                                DateTime timeStart = DateTime.Now;
                                try
                                {
                                    object result = modifier.ApplyTo(Result.Model);
                                    results.Add(result);
                                    Result.Model = result;
                                    OnPropertyChanged(nameof(Modifiers));
                                }
                                catch
                                {
                                    Status = "Applying error.";
                                    throw;
                                }
                                finally
                                {
                                    executionTime = DateTime.Now - timeStart;
                                    OnPropertyChanged(nameof(ExecutionTime));
                                }
                                Status = "Applying successful.";
                                AppliedModifiers.Add(modifier);
                                Applier.Modifier = null;
                                SearchText = "";
                            }
                            catch (Exception e)
                            {
                                System.Windows.MessageBox.Show(e.Message ?? "Undefined exception.");
                            }
                        }
                        else
                        {
                            Status = "Modifier was not chosen.";
                            executionTime = TimeSpan.Zero;
                            OnPropertyChanged(nameof(ExecutionTime));
                        }
                    }));
            }
        }

        private RelayCommand updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return updateCommand ??
                    (updateCommand = new RelayCommand(obj =>
                    {
                        AbstractModifier modifier = Updater.Modifier;
                        if (modifier != null)
                        {
                            Updater.UpdateStates();
                            int i;
                            int count = AppliedModifiers.Count;
                            object result = null;
                            for (i = 0; i < count && AppliedModifiers[i] != modifier; ++i)
                                if ((result = results[i]) == null)
                                {
                                    executionTime = TimeSpan.Zero;
                                    OnPropertyChanged(nameof(ExecutionTime));
                                    Status = $"Error on {i + 1}";
                                    Result.Model = result;
                                    return;
                                }
                            for (int k = i; k < count; ++k)
                                results[k] = null;
                            int skipped = i;
                            try
                            {
                                DateTime timeStart = DateTime.Now;
                                try
                                {
                                for (; i < count; ++i)
                                    results[i] = result = AppliedModifiers[i].ApplyTo(result);
                                }
                                finally
                                {
                                    int updated = i - skipped;
                                    status = $"Skipped {skipped} | Updated {updated}";
                                    executionTime = DateTime.Now - timeStart;
                                    OnPropertyChanged(nameof(ExecutionTime));
                                }
                            }
                            catch (Exception e)
                            {
                                Status = status + $" | Error on {i + 1}";
                                Result.Model = result;
                                System.Windows.MessageBox.Show(e.Message ?? "Undefined exception.");
                            }
                            OnPropertyChanged(nameof(Status));
                            Result.Model = result;
                        }
                        else
                        {
                            Status = "Modifier was not chosen.";
                            executionTime = TimeSpan.Zero;
                            OnPropertyChanged(nameof(ExecutionTime));
                        }
                    }));
            }
        }

        private RelayCommand newCommand;
        public RelayCommand NewCommand
        {
            get
            {
                return newCommand ??
                    (newCommand = new RelayCommand(obj =>
                    {
                        if (MessageBox.Show("Are you sure?", "Create new project.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Updater.Modifier = null;
                            Applier.Modifier = null;
                            Result.Model = null;
                            AppliedModifiers.Clear();
                            results.Clear();
                            OnPropertyChanged(nameof(Modifiers));
                        }
                    }));
            }
        }
    }
}
