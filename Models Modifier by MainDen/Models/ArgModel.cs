using Modifiers_by_MainDen.Modifiers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Models_Modifier_by_MainDen.Models
{
    public class ArgModel
    {
        Panel Panel;
        public ArgModel(Panel panel)
        {
            if (panel is null)
                throw new ArgumentNullException(nameof(panel));
            Panel = panel;
        }

        private Grid GetArgView(int i, string name, string hint, string format, string[] states)
        {
            ToolTip ttHint = new ToolTip();
            ttHint.Content = hint;

            Grid result = new Grid();
            ColumnDefinition cName = new ColumnDefinition();
            ColumnDefinition cState = new ColumnDefinition();
            cName.Width = GridLength.Auto;
            result.ColumnDefinitions.Add(cName);
            result.ColumnDefinitions.Add(cState);
            result.ToolTip = ttHint;
            
            Label lName = new Label();
            lName.Content = name + ":";
            Grid.SetColumn(lName, 0);
            lName.ToolTip = ttHint;
            result.Children.Add(lName);
            
            FrameworkElement eState = GetElementState(i, format, states);
            Grid.SetColumn(eState, 1);
            eState.ToolTip = ttHint;
            result.Children.Add(eState);

            return result;
        }

        private FrameworkElement GetElementState(int i, string format, string[] states)
        {
            switch (format)
            {
                case "path image":
                    Button button = new Button();
                    button.Content = states[i] == "" ? "Open Image File" : states[i];
                    button.Click += (s, e) =>
                    {
                        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                        openFileDialog.Filter = "Файлы изображений (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
                        if (openFileDialog.ShowDialog() == true)
                            states[i] = openFileDialog.FileName;
                        button.Content = states[i] == "" ? "Open Image File" : states[i];
                    };
                    return button;
                case "text":
                default:
                    TextBox textBox = new TextBox();
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

        public void ResetPanelWithTemplate(AbstractModifier modifier)
        {
            Panel.Children.Clear();
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
                Panel.Children.Add(GetArgView(i, names[i], hints[i], i < flen ? formats[i] : "text", states));
        }
    }
}
