using System.ComponentModel;

namespace Legion
{
    public interface IObject : INotifyPropertyChanged
    {
        string Id { get; }
    }
}
