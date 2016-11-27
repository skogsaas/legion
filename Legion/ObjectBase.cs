using System;
using System.ComponentModel;

namespace Legion
{
    public class ObjectBase : IObject, INotifyPropertyChanged
    {
        public string Id { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string property)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
