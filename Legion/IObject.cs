using System;
using System.ComponentModel;

namespace Skogsaas.Legion
{
    public interface IObject : IId, INotifyPropertyChanged
    {
        string Owner { get; set; }
        bool IsPublished();
    }
}
