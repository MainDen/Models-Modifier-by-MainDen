using Core_by_MainDen.Models;
using Core_by_MainDen.Modifiers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Models_Modifier_by_MainDen.ViewModels
{
    public abstract class AbstractViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
