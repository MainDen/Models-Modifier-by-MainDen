using Modifiers_by_MainDen.Modifiers;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Models_Modifier_by_MainDen.ViewModels
{
    public class StatesViewModel
    {
        public ObservableCollection<FrameworkElement> StateViews { get; set; } = new ObservableCollection<FrameworkElement>();

        private FrameworkElement GetStateView(int i, string name, string hint, string format, string[] states)
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
            
            FrameworkElement eState = GetStateSetterView(i, format, states);
            Grid.SetColumn(eState, 1);
            eState.ToolTip = ttHint;
            stateView.Children.Add(eState);

            return stateView;
        }

        private FrameworkElement GetStateSetterView(int i, string format, string[] states)
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
                    textBlock.Text = states[i] == "" ? "Open File" : states[i];
                    button.Content = textBlock;
                    button.Click += (s, e) =>
                    {
                        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                        openFileDialog.Filter = formatTail;
                        if (openFileDialog.ShowDialog() == true)
                            states[i] = textBlock.Text = openFileDialog.FileName;
                    };
                    return button;
                case "text":
                default:
                    textBox = new TextBox();
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.Text = states[i];
                    textBox.TextChanged += (s, e) => states[i] = textBox.Text;
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

        public void ResetWithModifier(AbstractModifier modifier)
        {
            StateViews.Clear();
            if (modifier is null)
                return;
            string[] names = modifier.ArgNames;
            string[] hints = modifier.ArgHints;
            string[] formats = modifier.ArgFormats;
            string[] states = modifier.ArgStates;
            try
            {
                if (ContainsNull(names))
                    throw new NullReferenceException("ArgNames must not be null.");
                if (ContainsNull(hints))
                    throw new NullReferenceException("ArgHints must not be null.");
                if (ContainsNull(formats))
                    throw new NullReferenceException("ArgFormats must not be null.");
            }
            catch (Exception e)
            {
                throw new Exception("Invalid class implementation.", e);
            }
            if (ContainsNull(states))
                throw new MethodAccessException("ArgStates is not initialized.");
            int nlen = names.Length;
            int hlen = hints.Length;
            int flen = formats.Length;
            if (nlen != hlen)
                throw new Exception("Invalid class implementation.", new Exception($"Different count of argument properties: {nlen} ArgNames & {hlen} ArgHints."));
            int len = states.Length;
            if (len != nlen)
                throw new MethodAccessException("ArgStates is not initialized.");
            for (int i = 0; i < len; ++i)
                StateViews.Add(GetStateView(i, names[i], hints[i], i < flen ? formats[i] : "text", states));
        }
    }
}
