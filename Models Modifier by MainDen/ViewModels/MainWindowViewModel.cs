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
using System.Windows;
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
            Applier = new ArgModel(MainWindow.NewModifierArgs);
            Updater = new ArgModel(MainWindow.AppliedModifierArgs);
            InitializeAppliedModifiers();
        }

        private MainWindow MainWindow { get; set; }
        private ArgModel Applier { get; set; }
        private ArgModel Updater { get; set; }

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

        private static void InitializeModifiers()
        {
            try
            {
                modifiers = new ObservableCollection<AbstractModifier>();
                Type aType = typeof(AbstractModifier);
                IEnumerable<Type> types = Assembly.GetAssembly(aType).GetTypes().Where(type => type.IsSubclassOf(aType));
                foreach (var type in types)
                    modifiers.Add((AbstractModifier)type.GetConstructor(Type.EmptyTypes).Invoke(null));
                DirectoryInfo mods = Directory.CreateDirectory(@"..\mods");
                foreach (var dll in mods.GetFiles("*.dll"))
                {
                    int loaded = 0;
                    int count = 0;
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
                        count = types.Count();
                        foreach (var type in types)
                        {
                            modifiers.Add(new InvokerModifier(type.GetConstructor(Type.EmptyTypes).Invoke(null)));
                            ++loaded;
                        }
                        MessageBox.Show($"Dll \"{dll.Name}\" was loaded. All {loaded} modifiers is ready to use.");
                    }
                    catch
                    {
                        MessageBox.Show($"Dll \"{dll.Name}\" was not loaded correct. Only {loaded} from {count} modifiers is ready to use.");
                    }
                }
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
                Applier.ResetPanelWithTemplate(selectedModifier);
                OnPropertyChanged(nameof(SelectedModifier));
            }
        }

        private void InitializeAppliedModifiers()
        {
            AppliedModifiers = new ObservableCollection<AbstractModifier>();
        }
        public ObservableCollection<AbstractModifier> AppliedModifiers { get; set; }
        private AbstractModifier selectedAppliedModifier;
        private AbstractModifier editedAppliedModifier;
        public AbstractModifier SelectedAppliedModifier
        {
            get
            {
                return selectedAppliedModifier;
            }
            set
            {
                selectedAppliedModifier = value;
                editedAppliedModifier = selectedAppliedModifier?.Modifier;
                Updater.ResetPanelWithTemplate(editedAppliedModifier);
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

        private List<object> results = new List<object>();
        private object Result
        {
            get
            {
                int count = results.Count;
                if (count != 0)
                    return results[count - 1];
                return null;
            }
        }
        public BitmapImage ResultImage
        {
            get
            {
                return BitmapToImageSource(Result as Bitmap);
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
                            AbstractModifier modifier = SelectedModifier;
                            try
                            {
                                executionTime = TimeSpan.Zero;
                                OnPropertyChanged(nameof(ExecutionTime));
                                Status = "Applying modifier...";
                                DateTime timeStart = DateTime.Now;
                                try
                                {
                                    results.Add(modifier.ApplyTo(Result));
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
                                OnPropertyChanged(nameof(ResultImage));
                                AppliedModifiers.Add(modifier);
                                SelectedModifier = null;
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
                        if (selectedAppliedModifier != null)
                        {
                            string[] appliedStates = SelectedAppliedModifier.ArgStates;
                            string[] updatedStates = editedAppliedModifier?.ArgStates;
                            int len = appliedStates.Length;
                            if (len == updatedStates?.Length)
                                for (int i = 0; i < len; ++i)
                                    appliedStates[i] = updatedStates[i];
                            int j;
                            int count = AppliedModifiers.Count;
                            object result = null;
                            for (j = 0; j < count && AppliedModifiers[j] != selectedAppliedModifier; ++j)
                                if ((result = results[j]) == null)
                                {
                                    executionTime = TimeSpan.Zero;
                                    OnPropertyChanged(nameof(ExecutionTime));
                                    Status = $"Error on {j + 1}";
                                    OnPropertyChanged(nameof(ResultImage));
                                    return;
                                }
                            for (int k = j; k < count; ++k)
                                results[k] = null;
                            int skipped = j;
                            try
                            {
                                DateTime timeStart = DateTime.Now;
                                try
                                {
                                for (; j < count; ++j)
                                    results[j] = result = AppliedModifiers[j].ApplyTo(result);
                                }
                                finally
                                {
                                    int updated = j - skipped;
                                    status = $"Skipped {skipped} | Updated {updated}";
                                    executionTime = DateTime.Now - timeStart;
                                    OnPropertyChanged(nameof(ExecutionTime));
                                }
                            }
                            catch (Exception e)
                            {
                                Status = status + $" | Error on {j + 1}";
                                OnPropertyChanged(nameof(ResultImage));
                                System.Windows.MessageBox.Show(e.Message ?? "Undefined exception.");
                            }
                            OnPropertyChanged(nameof(Status));
                            OnPropertyChanged(nameof(ResultImage));
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
                            SelectedAppliedModifier = null;
                            SelectedModifier = null;
                            AppliedModifiers.Clear();
                            results.Clear();
                            OnPropertyChanged(nameof(ResultImage));
                            OnPropertyChanged(nameof(Modifiers));
                        }
                    }));
            }
        }
    }
}
