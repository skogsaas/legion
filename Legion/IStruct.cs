using System;
using System.ComponentModel;

namespace Skogsaas.Legion
{
    public interface IStruct : INotifyPropertyChanged
    {
        Type GetInterface();
    }
}
