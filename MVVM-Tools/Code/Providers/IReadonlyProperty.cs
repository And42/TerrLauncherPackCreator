using System.ComponentModel;

namespace MVVM_Tools.Code.Providers
{
    public interface IReadonlyProperty<TPropertyType> : INotifyPropertyChanged
    {
        TPropertyType Value { get; }
    }
}