using System;
using System.ComponentModel;

namespace Skogsaas.Legion
{
    public class StructBase : IStruct
    {
        protected Type iface;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string property)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        protected void ListenSubstructureChanged(INotifyPropertyChanged property, string name)
        {
            if(property != null)
            {
                property.PropertyChanged += (object caller, PropertyChangedEventArgs args) => { NotifyPropertyChanged(name); };
            }
        }

        public Type GetInterface()
        {
            return iface;
        }
    }
}
