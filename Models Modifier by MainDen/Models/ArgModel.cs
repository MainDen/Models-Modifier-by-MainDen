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

        private Grid GetArgView(int i, string name, string hint, string state, string[] states)
        {
            ToolTip ttHint = new ToolTip();
            ttHint.Content = hint;

            Grid result = new Grid();
            result.ColumnDefinitions.Add(new ColumnDefinition());
            result.ColumnDefinitions.Add(new ColumnDefinition());
            result.ToolTip = ttHint;
            
            Label lName = new Label();
            lName.Content = name;
            Grid.SetColumn(lName, 0);
            lName.ToolTip = ttHint;
            result.Children.Add(lName);
            
            TextBox tbState = new TextBox();
            tbState.Text = state;
            Grid.SetColumn(tbState, 1);
            tbState.ToolTip = ttHint;
            tbState.TextAlignment = TextAlignment.Left;
            tbState.TextChanged += (sender, e) =>
            {
                TextBox tb = (TextBox)sender;
                states[i] = tb.Text;
            };
            result.Children.Add(tbState);

            return result;
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
            string[] states = modifier.ArgStates;
            try
            {
                if (ContainsNull(names))
                    throw new NullReferenceException("ArgNames must not be null.");
                if (ContainsNull(hints))
                    throw new NullReferenceException("ArgHints must not be null.");
            }
            catch (Exception e)
            {
                throw new Exception("Invalid class implementation.", e);
            }
            if (ContainsNull(states))
                throw new MethodAccessException("ArgStates is not initialized.");
            int nlen = names.Length;
            int hlen = hints.Length;
            if (nlen != hlen)
                throw new Exception("Invalid class implementation.", new Exception($"Different count of argument properties: {nlen} ArgNames & {hlen} ArgHints."));
            int len = states.Length;
            if (len != nlen)
                throw new MethodAccessException("ArgStates is not initialized.");
            for (int i = 0; i < len; ++i)
                Panel.Children.Add(GetArgView(i, names[i], hints[i], states[i], states));
        }
    }
}
