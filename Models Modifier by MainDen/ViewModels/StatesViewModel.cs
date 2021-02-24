using Modifiers_by_MainDen.Modifiers;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Models_Modifier_by_MainDen.ViewModels
{
    public class StatesViewModel : AbstractViewModel
    {
        private string[] statesBuffer = null;

        private AbstractModifier modifier = null;

        public AbstractModifier Modifier
        {
            get => modifier;
            set
            {
                statesBuffer = null;
                modifier = value;
                StateViews.Clear();
                if (!(modifier is null))
                    ResetStateViews();
                OnPropertyChanged();
                OnPropertyChanged(nameof(StateViews));
            }
        }

        public ObservableCollection<FrameworkElement> StateViews { get; set; } = new ObservableCollection<FrameworkElement>();
        
        public bool AutoUpdate { get; set; } = false;

        private void ResetStateViews()
        {
            if (modifier is null)
                return;
            
            string[] names = modifier.ArgNames;
            string[] hints = modifier.ArgHints;
            string[] formats = modifier.ArgFormats;
            string[] states = modifier.ArgStates;
            
            if (ContainsNull(states))
                throw new MethodAccessException("ArgStates is not initialized.");
            
            int nlen = names?.Length ?? 0;
            int hlen = hints?.Length ?? 0;
            int flen = formats?.Length ?? 0;
            int len = states.Length;
            
            statesBuffer = new string[len];
            for (int i = 0; i < len; ++i)
                statesBuffer[i] = states[i];

            const string hintDefault = "Has no hint.";
            const string formatDefault = "text";
            for (int i = 0; i < len; ++i)
                StateViews.Add(GetStateView(i,
                    i < nlen ? names[i] ?? $"Arg {i + 1}:" : $"Arg {i + 1}:",
                    i < hlen ? hints[i] ?? hintDefault : hintDefault,
                    i < flen ? formats[i] ?? formatDefault : formatDefault));
        }

        private FrameworkElement GetStateView(int i, string name, string hint, string format)
        {
            ToolTip ttHint = new ToolTip();
            ttHint.Content = hint;

            Grid stateView = new Grid();
            ColumnDefinition cName = new ColumnDefinition();
            ColumnDefinition cState = new ColumnDefinition();
            cName.Width = GridLength.Auto;
            stateView.ColumnDefinitions.Add(cName);
            stateView.ColumnDefinitions.Add(cState);
            stateView.ToolTip = ttHint;
            
            Label lName = new Label();
            lName.Content = name + ":";
            Grid.SetColumn(lName, 0);
            lName.ToolTip = ttHint;
            stateView.Children.Add(lName);
            
            FrameworkElement eState = GetStateSetterView(i, format);
            Grid.SetColumn(eState, 1);
            eState.ToolTip = ttHint;
            stateView.Children.Add(eState);

            return stateView;
        }

        private FrameworkElement GetStateSetterView(int i, string format)
        {
            string[] formatParts = format.Split("|");
            int fpLen = formatParts.Length;
            string formatType = formatParts[0];
            string formatTail;
            switch (fpLen)
            {
                case 0:
                case 1:
                    formatTail = "";
                    break;
                default:
                    formatTail = formatParts[1];
                    break;
            }
            for (int j = 2; j < fpLen; ++j)
                formatTail += '|' + formatParts[j];
            Button button;
            TextBlock textBlock;
            TextBox textBox;
            switch (formatType.ToLower())
            {
                case "openfilepath":
                    button = new Button();
                    textBlock = new TextBlock();
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    textBlock.Text = GetStateAt(i) == "" ? "Open File" : GetStateAt(i);
                    button.Content = textBlock;
                    button.Click += (s, e) =>
                    {
                        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                        openFileDialog.Filter = formatTail;
                        if (openFileDialog.ShowDialog() == true)
                            SetStateAt(i, textBlock.Text = openFileDialog.FileName);
                    };
                    return button;
                case "text":
                default:
                    textBox = new TextBox();
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.Text = GetStateAt(i);
                    textBox.TextChanged += (s, e) => SetStateAt(i, textBox.Text);
                    return textBox;
            }
        }

        private bool ContainsNull(string[] strArray)
        {
            if (strArray is null)
                return true;
            foreach (var str in strArray)
                if (str is null)
                    return true;
            return false;
        }

        private string GetStateAt(int i)
        {
            return statesBuffer[i];
        }

        private void SetStateAt(int i, string value)
        {
            statesBuffer[i] = value;
            if (AutoUpdate)
                modifier.ArgStates[i] = value;
        }

        public void UpdateStates()
        {
            if (statesBuffer is null)
                return;
            string[] states = modifier.ArgStates;
            int len = states?.Length ?? 0;
            for (int i = 0; i < len; ++i)
                states[i] = statesBuffer[i];
        }
    }
}
