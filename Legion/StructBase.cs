using System;
using System.ComponentModel;

namespace Skogsaas.Legion
{
    public class StructBase : IStruct
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string property)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        protected void ListenSubstructureChanged(INotifyPropertyChanged property, string name)
        {
            property.PropertyChanged += (object caller, PropertyChangedEventArgs args) => { NotifyPropertyChanged(name); };
        }
    }
}
