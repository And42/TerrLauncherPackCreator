using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM_Tools.Code.Providers
{
    public class DelegatedProperty<TPropertyType> : IProperty<TPropertyType>
    {
        private readonly Func<TPropertyType> _valueResolver;
        private readonly Action<TPropertyType> _valueApplier;

        private bool _isReadonly;

        public event PropertyChangedEventHandler PropertyChanged;

        public DelegatedProperty(Func<TPropertyType> valueResolver, Action<TPropertyType> valueApplier)
        {
            _valueResolver = valueResolver;
            _valueApplier = valueApplier;
        }

        public TPropertyType Value
        {
            get => _valueResolver();
            set
            {
                if (_isReadonly)
                    throw new NotSupportedException("Property is readonly");
                _valueApplier(value);
            }
        }

        public DelegatedProperty<TPropertyType> DependsOn(IReadonlyProperty<TPropertyType> otherProperty)
        {
            otherProperty.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Value));
            return this;
        }

        public DelegatedProperty<TPropertyType> DependsOn(INotifyPropertyChanged propertyProvider, string propertyName)
        {
            propertyProvider.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == propertyName)
                    OnPropertyChanged(nameof(Value));
            };
            return this;
        }

        public IReadonlyProperty<TPropertyType> AsReadonly()
        {
            _isReadonly = true;
            return this;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
